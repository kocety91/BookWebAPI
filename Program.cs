using BookWebAPI.Data;
using BookWebAPI.Extensions;
using BookWebAPI.Filters;
using BookWebAPI.Models;
using BookWebAPI.Seeding;
using BookWebAPI.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var configuration = builder.Configuration;
builder.Services.AddDbContext<BookDbContext>
    (opt => opt.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));


builder.Services.AddDefaultIdentity<ApplicationUser>(opt =>
{
    opt.Password.RequireDigit = false;
    opt.Password.RequireLowercase = false;
    opt.Password.RequireUppercase = false;
    opt.Password.RequiredLength = 3;
    opt.Password.RequireNonAlphanumeric = false;
})
    .AddRoles<ApplicationRole>()
    .AddEntityFrameworkStores<BookDbContext>();


var key = Encoding.UTF8.GetBytes(configuration["Jwt:Key"]);
var tokenValidationParamaters = new TokenValidationParameters()
{
    ValidateActor = true,
    ValidateIssuer = false,
    ValidateAudience = false,
    ValidateIssuerSigningKey =true,
    ClockSkew = TimeSpan.Zero,
    IssuerSigningKey = new SymmetricSecurityKey(key)
};

builder.Services.AddAuthentication(opt =>
{
    opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    opt.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(opt =>
  {
      opt.SaveToken = true;
      opt.RequireHttpsMetadata = false;
      opt.TokenValidationParameters = tokenValidationParamaters;
  });


builder.Services.AddTransient<IBookService, BookService>();
builder.Services.AddTransient<IPublisherService, PublisherService>();
builder.Services.AddTransient<IGenreService, GenreService>();
builder.Services.AddTransient<IAuthorService, AuthorService>();
builder.Services.AddTransient<IIdentityService, IdentityService>();
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddControllers(opt => opt.Filters.Add(new BookActionFilter()));
builder.Services.AddTransient<ExceptionHandlingMiddleware>();
builder.Services.AddSingleton(tokenValidationParamaters);



var app = builder.Build();

//seed data
using(var serviceScope = app.Services.CreateScope())
{
    var dbContext = serviceScope.ServiceProvider.GetRequiredService<BookDbContext>();
    dbContext.Database.Migrate();
    new BookDbContextSeeder().SeedAsync(dbContext,serviceScope.ServiceProvider).GetAwaiter().GetResult();
}



// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.MapControllers();

app.Run();
