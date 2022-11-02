using BookWebAPI.Data;
using BookWebAPI.Filters;
using BookWebAPI.Models;
using BookWebAPI.Repositories;
using BookWebAPI.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace BookWebAPI.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection ConfigureDataBase(this IServiceCollection service, IConfiguration configuration)
        {
            service.AddDbContext<BookDbContext>
            (opt => opt.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

            service.AddDefaultIdentity<ApplicationUser>(opt =>
             {
                 opt.Password.RequireDigit = false;
                 opt.Password.RequireLowercase = false;
                 opt.Password.RequireUppercase = false;
                 opt.Password.RequiredLength = 3;
                 opt.Password.RequireNonAlphanumeric = false;
             })
                  .AddRoles<ApplicationRole>()
                  .AddEntityFrameworkStores<BookDbContext>();

            return service;
        }

        public static IServiceCollection ConfigureJwt(this IServiceCollection service, IConfiguration configuration)
        {
            var key = Encoding.UTF8.GetBytes(configuration["Jwt:Key"]);
            var tokenValidationParamaters = new TokenValidationParameters()
            {
                ValidateActor = true,
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateIssuerSigningKey = true,
                ClockSkew = TimeSpan.Zero,
                IssuerSigningKey = new SymmetricSecurityKey(key)
            };

            service.AddAuthentication(opt =>
            {
                opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                opt.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(opt =>
            {
                opt.SaveToken = true;
                opt.RequireHttpsMetadata = false;
                opt.TokenValidationParameters = tokenValidationParamaters;
                opt.Events = new JwtBearerEvents
                {
                    OnChallenge = async context =>
                    {
                        context.HandleResponse();

                        context.Response.StatusCode = 401;
                        context.Response.Headers.Append("Unauthorized", "User");
                        context.Response.ContentType = "application/json";

                        var errorResponse = new ErrorDetails
                        {
                            Message = "You are not authorized !",
                            Error = context.Response.StatusCode,
                        };

                        await context.Response.WriteAsync(errorResponse.ToString());
                    },
                };
            });

            service.AddSingleton(tokenValidationParamaters);
            return service;
        }

        public static IServiceCollection ConfigureDataRepositories(this IServiceCollection service)
        {
            service.AddScoped(typeof(IRepository<>), typeof(EfRepository<>));
            return service;
        }
        
        public static IServiceCollection ConfigureBusinesServices(this IServiceCollection service,IConfiguration configuration)
        {
            service.AddTransient<IBookService, BookService>();
            service.AddTransient<IPublisherService, PublisherService>();
            service.AddTransient<IGenreService, GenreService>();
            service.AddTransient<IAuthorService, AuthorService>();
            service.AddTransient<IIdentityService, IdentityService>();
            service.AddTransient<ExceptionHandlingMiddleware>();
            service.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
            return service;
        }

        public static IServiceCollection ConfigureFilters(this IServiceCollection service,IConfiguration configuration)
        {
            service.AddControllers(opt => opt.Filters.Add(new BookActionFilter()));
            return service;
        }

    }
}
