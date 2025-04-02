using System;
using System.IO;
using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using SalesAnalystApp.Data.Implementations;
using SalesAnalystApp.Data.Interfaces;
using SalesAnalystApp.Services;
using SalesAnalystApp.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<ISalesRepository, SalesRepository>();
builder.Services.AddScoped<ISalesService, SalesService>();

builder.Services.AddControllers();
builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true;
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Sales Analyst API",
        Version = "v1",
        Description = "REST API to fetch and filter sales records.",
        Contact = new Microsoft.OpenApi.Models.OpenApiContact
        {
            Name = "Sales Team",
            Email = "support@sundarsalesanalystapp.com",
            Url = new Uri("https://example.com/contact")
        }
    });

    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFilename);
    if (File.Exists(xmlPath))
    {
        options.IncludeXmlComments(xmlPath);
    }
});

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "Sales Analyst API v1");
    options.RoutePrefix = "apidocs";
});

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
