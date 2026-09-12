using DailyTaskPlaner.Api.Config;
using DailyTaskPlaner.Business.Services;
using DailyTaskPlaner.Business.Services.Interfaces;
using DailyTaskPlaner.Data;
using DailyTaskPlaner.Data.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.OpenApi.Models;
using System.Text.Json.Serialization;
using Microsoft.Extensions.AI;
using OllamaSharp;
var builder = WebApplication.CreateBuilder(args);

builder.LoadDeveloperConfiguration();
builder.LoadDotEnv();


builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

// Add services
builder.Services.AddScoped<IUsersService, UsersService>(); // Dependency Injection
builder.Services.AddScoped<IDailyTaskService, DailyTaskService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<ILLMService, LLMService>();

// The Ollama client comes from the HTTP client factory, so the underlying connection
// is reused between requests. The timeout is set explicitly: the default of 100
// seconds is shorter than inference takes under load, so requests would be cancelled
// before the model finished.
var ollamaBaseUrl = builder.Configuration["Ollama:BaseUrl"] ?? "http://localhost:11434/";
var ollamaModel = builder.Configuration["Ollama:Model"] ?? "qwen2.5:3b";

builder.Services.AddHttpClient("ollama", client =>
{
    client.BaseAddress = new Uri(ollamaBaseUrl);
    client.Timeout = TimeSpan.FromMinutes(10);
});

builder.Services.AddScoped<IChatClient>(sp =>
{
    var http = sp.GetRequiredService<IHttpClientFactory>().CreateClient("ollama");
    return new OllamaApiClient(http, ollamaModel);
});

builder.Services.AddScoped<PasswordHasher<User>>();


// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Daily Task Planner API",
        Version = "v1",
        Description = "API for managing daily tasks with user authentication",
    });

    // Explicit OpenAPI 3.0 support for JWT
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Enter your JWT token like: Bearer {token}",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});



// Conect to database
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
//builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
//    .AddJwtBearer(options =>
//    {
//        options.TokenValidationParameters = new TokenValidationParameters
//        {
//            ValidateIssuer = true,
//            ValidIssuer = builder.Configuration["AppSettings:Issuer"],
//            ValidateAudience = true,
//            ValidAudience = builder.Configuration["AppSettings:Audience"],
//            ValidateLifetime = true,
//            IssuerSigningKey = new SymmetricSecurityKey(
//                Encoding.UTF8.GetBytes(builder.Configuration["AppSettings:Token"]!)),
//            ValidateIssuerSigningKey = true
//        };
//    });


builder.Services.AddScoped<IAuthService, AuthService>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("ReactFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:5173")
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials(); // Ako ti treba slanje kola?i?a ili autentifikacija preko fronta
    });
});

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["AppSettings:Issuer"],
        ValidAudience = builder.Configuration["AppSettings:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(builder.Configuration["AppSettings:Token"]!))
    };
});




var app = builder.Build();
app.UseCors("ReactFrontend");


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication(); // mora pre Authorization
app.UseAuthorization();

app.MapControllers();


app.Run();