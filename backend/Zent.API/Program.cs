using System.Text;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;
using Zent.API;
using Zent.API.Extensions;
using Zent.Application.Messaging.DependencyInjection;
using Zent.Infrastructure.DependencyInjection;
using Zent.Postgresql.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApiWithJwtAuth();
builder.Services.AddJsonStringEnumConversion();
builder.Services.AddInfrastructure();
builder.Services.AddPostgresql(builder.Configuration);
builder.Services.AddCqrs();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(o =>
    {
        o.RequireHttpsMetadata = false;
        o.TokenValidationParameters = new TokenValidationParameters
        {
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Secret"]!)),
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddAuthorization();

builder.Services.AddCors(options =>
{
    options.AddPolicy("FrontendPolicy", policy =>
    {
        policy
            .WithOrigins("http://localhost:5173")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

builder.Services.AddValidatorsFromAssemblyContaining<Program>();
builder.Services.AddEndpoints(typeof(Program).Assembly);

var app = builder.Build();

app.UsePathBase(builder.Configuration["PathBase"]);

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseMiddleware<GlobalExceptionHandler>();

app.UseCors("FrontendPolicy");

app.UseAuthentication();
app.UseAuthorization();

app.MapEndpoints();

app.Run();