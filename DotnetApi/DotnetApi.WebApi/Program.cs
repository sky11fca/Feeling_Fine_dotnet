using DotnetApi.Application.Abstractions;
using DotnetApi.Application.Authentication.Command;
using DotnetApi.Application.Businesses.Commands;
using DotnetApi.Application.Businesses.Queries;
using DotnetApi.Application.Reviews.Commands;
using DotnetApi.Application.Reviews.Validators;
using DotnetApi.Infrastructure.Authentication;
using DotnetApi.Infrastructure.Persistance;
using DotnetApi.Infrastructure.Repository;
using DotnetApi.WebApi.Controller;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi;

AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

var builder = WebApplication.CreateBuilder(args);


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
                }
            });
    });

builder.Services.AddOpenApi();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    {
        if (builder.Environment.IsEnvironment("Testing"))
        {
            options.UseInMemoryDatabase("FeelingFineDb");
        }
        else
        {
            options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")).UseSnakeCaseNamingConvention();
        }
    });

builder.Services.AddControllers();

builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));

builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(GetBusinessQuery).Assembly));

builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration["Redis:ConnectionString"] ?? "localhost:6379";
    options.InstanceName = "FeelingFine_";
});

builder.Services.AddScoped<HttpClient>();

// Dependency Injection

builder.Services.AddScoped<IBusinessRepository, BusinessRepository>();
builder.Services.AddScoped<IReviewRepository, ReviewRepository>();
builder.Services.AddScoped<IClientRepository, ClientRepository>();
builder.Services.AddScoped<IReplyRepository, ReplyRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();
builder.Services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();


builder.Services.AddValidatorsFromAssemblyContaining<AddReviewCommand>();
builder.Services.AddValidatorsFromAssemblyContaining<AddBusinessCommand>();
builder.Services.AddValidatorsFromAssemblyContaining<LoginCommand>();
builder.Services.AddValidatorsFromAssemblyContaining<RegisterCommand>();


builder.Services.AddCors(option =>
{
    option.AddPolicy("DefaultPolicy", c =>
    {
        if (builder.Environment.IsDevelopment() || builder.Environment.IsEnvironment("Testing"))
        {
            c.AllowAnyOrigin()
             .AllowAnyMethod()
             .AllowAnyHeader();
        }
        else
        {
            var allowedOrigins = builder.Configuration.GetSection("AllowedOrigins").Get<string[]>() ?? Array.Empty<string>();
            c.WithOrigins(allowedOrigins)
             .AllowAnyMethod()
             .AllowAnyHeader()
             .AllowCredentials();
        }
    });
});


var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    await context.Database.EnsureCreatedAsync();
}

if (app.Environment.IsDevelopment() || app.Environment.IsEnvironment("Testing"))
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


app.UseCors("DefaultPolicy");

app.MapControllers();

await app.RunAsync();

public partial class Program { }
