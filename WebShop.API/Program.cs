using WebShop.Application;
using WebShop.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Microsoft.Extensions.Configuration;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services
    .AddApplication()
    .AddInfrastructure(builder.Configuration);

//builder.Services.AddDbContext<ApplicationDbContext>(
//    dbContextOptionsBuilder =>
//    {
//        var connectionString = builder.Configuration.GetConnectionString("Default");

//        dbContextOptionsBuilder.UseSqlServer(connectionString);
//    });

builder.Host.UseSerilog((context, configuration) =>
    configuration.ReadFrom.Configuration(context.Configuration));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseProjectSqlServer();

app.UseSerilogRequestLogging();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
