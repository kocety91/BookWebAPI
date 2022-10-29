using BookWebAPI.Data;
using BookWebAPI.Extensions;
using BookWebAPI.Filters;
using BookWebAPI.Models;
using BookWebAPI.Seeding;
using BookWebAPI.Services;
using Microsoft.EntityFrameworkCore;

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

builder.Services.AddTransient<IBookService, BookService>();
builder.Services.AddTransient<IPublisherService, PublisherService>();
builder.Services.AddTransient<IGenreService, GenreService>();
builder.Services.AddTransient<IAuthorService, AuthorService>();
builder.Services.AddTransient<IIdentityService, IdentityService>();
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddControllers(opt => opt.Filters.Add(new BookActionFilter()));
builder.Services.AddTransient<ExceptionHandlingMiddleware>();



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

app.UseAuthorization();

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.MapControllers();

app.Run();
