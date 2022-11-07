using BookWebAPI.Extensions;

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
public partial class Program { }