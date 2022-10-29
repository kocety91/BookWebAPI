using AutoMapper;
using BookWebAPI.Data;
using BookWebAPI.Dtos.Identity;
using BookWebAPI.Models;
using Microsoft.AspNetCore.Identity;
using static BookWebAPI.Common.CustomExceptions;

namespace BookWebAPI.Services
{
    public class IdentityService : IIdentityService
    {
        private readonly BookDbContext db;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IMapper mapper;
        private readonly RoleManager<ApplicationRole> roleManager;

        public IdentityService(BookDbContext db,
            UserManager<ApplicationUser> userManager,
            IMapper mapper,
            RoleManager<ApplicationRole> roleManager)
        {
            this.db = db;
            this.userManager = userManager;
            this.mapper = mapper;
            this.roleManager = roleManager;
        }

        public async Task<AuthenticationResponseModel> LoginAsync(LoginRequestModel model)
        {
            var user = await userManager.FindByEmailAsync(model.Email);
            if (user == null) throw new UnauthorizeException($"No user with this email: {model.Email}");

            var password = await userManager.CheckPasswordAsync(user, model.Password);
            if (!password) throw new UnauthorizeException($"Wrong password !!! ");

            if (model == null) throw new NullReferenceException(nameof(model));

            var mappedUser = mapper.Map<AuthenticationResponseModel>(user);
            return mappedUser;
        }

        public async Task<AuthenticationResponseModel> RegisterAsync(RegisterRequestModel model)
        {
            var user = await userManager.FindByEmailAsync(model.Email);
            if (user != null) throw new ExistsException($"User with this email: {model.Email} already exists.");

            if (model == null) throw new NullReferenceException(nameof(model));

            var applicationUserRole = new ApplicationRole() { Name = "user" ,CreatedOn = DateTime.Now };

            var roleResult = await roleManager.CreateAsync(applicationUserRole);

            if (!roleResult.Succeeded) throw new ArgumentException($"dasjdasd");

            var applicationUser = new ApplicationUser()
            {
                UserName = model.UserName,
                Email = model.Email,
                ApplicationRoleId = applicationUserRole.Id,
                ApplicationRole = applicationUserRole,
                Country = model.Country,
            };

            var result = await userManager.CreateAsync(applicationUser, model.Password);

            if (!result.Succeeded) throw new ArgumentException($"dasdsadasd");

            await db.SaveChangesAsync();

            var mappedUser = mapper.Map<AuthenticationResponseModel>(applicationUser);
            return mappedUser;
        }
    }
}
