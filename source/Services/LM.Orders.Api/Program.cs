using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json.Serialization;
using LM.Orders.Infrastructure.Extensions;
using LM.Orders.Application.CommandHandlers;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;
var configuration = builder.Configuration;
var host = builder.Host;

services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(typeof(CreateOrderCommandHandler).Assembly);
});

services.AddInfrastructure(configuration);

services.AddControllers(a =>
{
    a.Filters.Add(new ResponseCacheAttribute { NoStore = true, Location = ResponseCacheLocation.None, Duration = -1 });
})
.AddJsonOptions(a =>
{
    a.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    a.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    a.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

services.AddEndpointsApiExplorer();
services.AddSwaggerGen();

services.AddCors(option => option.AddPolicy("OrdersPolicy", builderCors =>
{
    builderCors.AllowAnyOrigin()
                .AllowAnyHeader()
                .AllowAnyMethod();
}));


var app = builder.Build();

var env = app.Environment;

if (!env.IsDevelopment())
{
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseCors("OrdersPolicy");

app.UseAuthorization();

app.MapControllers();

if (env.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

    app.MapGet("/", context =>
    {
        context.Response.Redirect("/swagger");
        return Task.CompletedTask;
    });
}

app.Run();