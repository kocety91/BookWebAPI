using BookWebAPI.Data;
using BookWebAPI.Extensions;
using BookWebAPI.Seeding;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
var configuration = builder.Configuration;

builder.Services
    .ConfigureDataBase(configuration)
    .ConfigureJwt(configuration)
    .ConfigureDataRepositories()
    .ConfigureBusinesServices(configuration)
    .ConfigureFilters(configuration)
    .AddAuthorization()
    .AddEndpointsApiExplorer()
    .AddSwaggerGen();


var app = builder.Build();


using (var serviceScope = app.Services.CreateScope())
{
    var dbContext = serviceScope.ServiceProvider.GetRequiredService<BookDbContext>();
    dbContext.Database.Migrate();
    new BookDbContextSeeder().SeedAsync(dbContext, serviceScope.ServiceProvider).GetAwaiter().GetResult();
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
