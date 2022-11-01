using AutoMapper;
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
        private readonly RoleManager<ApplicationRole> roleManager;
        private readonly TokenValidationParameters tokenValidationParameters;
        private readonly IConfiguration configuration;

        public IdentityService(BookDbContext db,
            UserManager<ApplicationUser> userManager,
            RoleManager<ApplicationRole> roleManager,
            TokenValidationParameters tokenValidationParameters,
            IConfiguration configuration)
        {
            this.db = db;
            this.userManager = userManager;
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

            var mappedUser = await this.GenerateJwtTokenAsync(user);
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


            var roleExist = await roleManager.RoleExistsAsync("user");

            if (!roleExist)
            {
                var roleResult = await roleManager
                    .CreateAsync(new ApplicationRole() { Name = "user", CreatedOn = DateTime.Now });
                if (!roleResult.Succeeded) throw new ArgumentException($"{nameof(roleResult)}");
            }

            var roleToUserResult = await userManager.AddToRoleAsync(applicationUser, "user");
            if (!roleToUserResult.Succeeded) throw new ArgumentException($"{nameof(roleToUserResult)}");

            var mappedUser = await this.GenerateJwtTokenAsync(applicationUser);
            return mappedUser;
        }

        public async Task LogoutAsync(string userId)
        {
            var logoutUser = await userManager.FindByIdAsync(userId);
            if(logoutUser == null) throw new UnauthorizeException($"Can't logout user with id{userId}");

            var refreshTokensForUser = await db.RefreshTokens
                .Where(x => x.ApplicationUserId == userId).ToListAsync();

            db.RefreshTokens.RemoveRange(refreshTokensForUser);

            await db.SaveChangesAsync();
        }

        public async Task<AuthenticationResponseModel> VerifyTokenAsync(TokenRequestModel model)
        {
            var jwtHandler = new JwtSecurityTokenHandler(); 

            this.tokenValidationParameters.ValidateLifetime = false; //za debug

            var tokenInVerification = jwtHandler
                .ValidateToken(model.Token, tokenValidationParameters, out var validatedToken);

            if (validatedToken is JwtSecurityToken jwtSecureToken)
            {
                var result = jwtSecureToken.Header.Alg
                    .Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase);

                if (!result) throw new ArgumentException($"Algorithm doesn't match.");
            }

            var utcExpirityDate = long.Parse(tokenInVerification.
                Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Exp).Value);
            var expiryDate = UnixTimeStampToDateTIme(utcExpirityDate);

            if (expiryDate > DateTime.Now) throw new ArgumentException($"Jwt still not expired.");


            var storedToken = await db.RefreshTokens.FirstOrDefaultAsync(x => x.Token == model.RefreshToken);
            if (storedToken == null) throw new NullReferenceException(nameof(storedToken));

            if (storedToken.IsUsed) throw new ArgumentException($"Your refresh token is used!");

            if (storedToken.IsRevoked) throw new ArgumentException($"Your refresh token is revoked!");

            var jti = tokenInVerification.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Jti).Value;
            if (storedToken.JwtId != jti) throw new ArgumentException("Jti's doesnt match!");

            if(storedToken.ExpiryDate < DateTime.Now) throw new ArgumentException("Refresh token is expired!");

            storedToken.IsUsed = true;
            db.RefreshTokens.Update(storedToken);
            await db.SaveChangesAsync();

            var dbUser = await userManager.FindByIdAsync(storedToken.ApplicationUserId);
            return await this.GenerateJwtTokenAsync(dbUser);
        }

        private async Task<AuthenticationResponseModel> GenerateJwtTokenAsync(ApplicationUser user)
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

            foreach (var role in roles)
            {
                tokenDescriptor.Subject.AddClaim(new Claim(ClaimTypes.Role, role));
            }

            var token = tokenHandler.CreateToken(tokenDescriptor);
            var jwtToken = tokenHandler.WriteToken(token);


            var refreshToken = new RefreshToken()
            {
                JwtId = token.Id,
                Token = this.RandomStringGeneration(23),
                ApplicationUserId = user.Id,
                AddedDate = DateTime.UtcNow,
                ExpiryDate = DateTime.UtcNow.AddMonths(6),
                IsRevoked = false,
                IsUsed = false
            };

            await db.RefreshTokens.AddAsync(refreshToken);
            await db.SaveChangesAsync();


            return new AuthenticationResponseModel()
            {
                UserId = user.Id,
                Email = user.Email,
                Token = jwtToken,
                RefreshToken = refreshToken.Token
            };
        }

        private DateTime UnixTimeStampToDateTIme(long unixTimeSpan)
        {
            var dateTImeVal = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            dateTImeVal = dateTImeVal.AddSeconds(unixTimeSpan).ToUniversalTime();

            return dateTImeVal;
        }

        private string RandomStringGeneration(int length)
        {
            var random = new Random();
            var chars = "DSADSADJHSADJKHSADKQYSADSA3913123dasda";
            return new string(Enumerable.Repeat(chars, length).Select(s => s[random.Next(s.Length)]).ToArray());
        }

      
    }
}
