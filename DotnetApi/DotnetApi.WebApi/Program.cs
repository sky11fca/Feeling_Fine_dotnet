using DotnetApi.Application.Abstractions;
using DotnetApi.Application.Businesses.Commands;
using DotnetApi.Application.Businesses.Queries;
using DotnetApi.Application.Reviews.Commands;
using DotnetApi.Application.Reviews.Validators;
using DotnetApi.Infrastructure.Persistance;
using DotnetApi.Infrastructure.Repository;
using DotnetApi.WebApi.Controller;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi;

var builder = WebApplication.CreateBuilder(args);

//TODO: Hide secret

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
    {
        c.SwaggerDoc(
            "v1", 
            new OpenApiInfo
            {
                Title = "Feeling Fine API", 
                Version = "v1",
                Description = "API for Feeling Fine Service built in .NET",
                Contact = new OpenApiContact
                {
                    Name= "Dev: Bazon Bogdan (sky11fca)",
                    Email= "bogdan.bzn@FeelingFine.net",
                    Url= new Uri("https://github.com/sky11fca")
                }
            });
    });

builder.Services.AddOpenApi();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")).UseSnakeCaseNamingConvention());

builder.Services.AddControllers();

builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(GetBusinessQuery).Assembly));

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("http://localhost:8000/ai/review") });

// Dependency Injection

builder.Services.AddScoped<IBusinessRepository, BusinessRepository>();
builder.Services.AddScoped<IReviewRepository, ReviewRepository>();

builder.Services.AddValidatorsFromAssemblyContaining<AddReviewCommand>();
builder.Services.AddValidatorsFromAssemblyContaining<AddBusinessCommand>();

builder.Services.AddCors(option =>
{
    option.AddPolicy("AllowAll", c =>
    {
        c.AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});


var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    context.Database.EnsureCreated();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Feeling Fine API v1");
        c.RoutePrefix = string.Empty; // Serves Swagger UI at the root (http://localhost:<port>/)
        c.DisplayOperationId();
    });
    app.MapOpenApi();
}

// app.UseHttpsRedirection();

app.UseCors("AllowAll");

app.MapControllers();

app.Run();
