﻿using AutoMapper;
using BookWebAPI.Data;
using BookWebAPI.Dtos.Identity;
using BookWebAPI.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using static BookWebAPI.Common.CustomExceptions;

namespace BookWebAPI.Services
{
    public class IdentityService : IIdentityService
    {
        private readonly BookDbContext db;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IMapper mapper;
        private readonly RoleManager<ApplicationRole> roleManager;
        private readonly TokenValidationParameters tokenValidationParameters;
        private readonly IConfiguration configuration;

        public IdentityService(BookDbContext db,
            UserManager<ApplicationUser> userManager,
            IMapper mapper,
            RoleManager<ApplicationRole> roleManager,
            TokenValidationParameters tokenValidationParameters,
            IConfiguration configuration)
        {
            this.db = db;
            this.userManager = userManager;
            this.mapper = mapper;
            this.roleManager = roleManager;
            this.tokenValidationParameters = tokenValidationParameters;
            this.configuration = configuration;
        }

        public async Task<AuthenticationResponseModel> LoginAsync(LoginRequestModel model)
        {
            var user = await userManager.FindByEmailAsync(model.Email);
            if (user == null) throw new UnauthorizeException($"No user with this email: {model.Email}");

            var password = await userManager.CheckPasswordAsync(user, model.Password);
            if (!password) throw new UnauthorizeException($"Wrong password !!! ");

            if (model == null) throw new NullReferenceException(nameof(model));

            await this.GenerateJwtToken(user);
            var mappedUser = mapper.Map<AuthenticationResponseModel>(user);
            return mappedUser;
        }

        public async Task<AuthenticationResponseModel> RegisterAsync(RegisterRequestModel model)
        {
            var user = await userManager.FindByEmailAsync(model.Email);
            if (user != null) throw new ExistsException($"User with this email: {model.Email} already exists.");

            if (model == null) throw new NullReferenceException(nameof(model));

            var applicationUser = new ApplicationUser()
            {
                UserName = model.UserName,
                Email = model.Email,
                Country = model.Country
            };

            var result = await userManager.CreateAsync(applicationUser, model.Password);
            if (!result.Succeeded) throw new ArgumentException($"Failed to create user");


            var userRole = new ApplicationRole() { Name = "user", CreatedOn = DateTime.Now };
            var roleResult = await roleManager.CreateAsync(userRole);
            if (!roleResult.Succeeded) throw new ArgumentException($"{nameof(roleResult)}");
            await userManager.AddToRoleAsync(applicationUser, userRole.Name);
            await db.SaveChangesAsync();

            await this.GenerateJwtToken(applicationUser);
            var mappedUser = mapper.Map<AuthenticationResponseModel>(applicationUser);
            return mappedUser;
        }

        //public async Task<AuthenticationResponseModel> VerifyToken(TokenRequestModel model)
        //{
        //    var jwtHandler = new JwtSecurityTokenHandler();

        //    this.tokenValidationParameters.ValidateLifetime = false; //za debug

        //    var tokenInVerification = jwtHandler
        //        .ValidateToken(model.Token, tokenValidationParameters, out var validatedToken);

        //    if(validatedToken is JwtSecurityToken jwtSecureToken)
        //    {
        //        var result = jwtSecureToken.Header.Alg
        //            .Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase);

        //        if (!result) throw new ArgumentException($"Algorithm doesn't match.");
        //    }

        //    var utcExpirityDate = long.Parse(tokenInVerification.Claims
        //        .FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Exp).Value);

        //    var expiryDate = UnixTimeStampToDateTIme(utcExpirityDate);

        //    if (expiryDate > DateTime.Now) throw new ArgumentException($"Jwt still not expired.");


        //    var dbUser = await userManager.FindByIdAsync(storedToken.UserId);
        //    return await this.GenerateJwtToken(dbUser);
        //}


        private async Task<AuthenticationResponseModel> GenerateJwtToken(ApplicationUser user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(configuration["Jwt:Key"]);

            var roles = await userManager.GetRolesAsync(user);

            var tokenDescriptor = new SecurityTokenDescriptor()
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim("Id",user.Id),
                    new Claim(JwtRegisteredClaimNames.Sub,user.Email),
                    new Claim(JwtRegisteredClaimNames.Email,user.Email),
                    new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString()),
                    new Claim(JwtRegisteredClaimNames.Iat,DateTime.Now.ToUniversalTime().ToString())
                }),
                Expires = DateTime.UtcNow.Add(TimeSpan.Parse(this.configuration.GetSection("Jwt:ExpiryTimeFrame").Value)),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256),
            };

            //--tuka si !!!

            foreach (var role in roles)
            {
                tokenDescriptor.Claims.Add(ClaimTypes.Role, role);
            }

            var token = tokenHandler.CreateToken(tokenDescriptor);
            var jwtToken = tokenHandler.WriteToken(token);


            return new AuthenticationResponseModel()
            {
                UserId = user.Id,
                Email = user.Email,
                Token = jwtToken
            };

        }


        private DateTime UnixTimeStampToDateTIme(long unixTimeSpan)
        {
            var dateTImeVal = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            dateTImeVal = dateTImeVal.AddSeconds(unixTimeSpan).ToUniversalTime();

            return dateTImeVal;
        }


    }
}
