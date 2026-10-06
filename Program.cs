
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using SmartSupport.API.Data;
using SmartSupport.API.Interfaces;
using SmartSupport.API.Middleware;
using SmartSupport.API.Services;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// ===============================
// Controllers
// ===============================

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler =
            System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
    });

// ===============================
// Database
// ===============================

var connectionString =
    builder.Configuration.GetConnectionString("DefaultConnection");

if (string.IsNullOrWhiteSpace(connectionString))
{
    throw new InvalidOperationException(
        "Database connection string is not configured."
    );
}

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySQL(connectionString));

// ===============================
// JWT Authentication
// ===============================

var jwtKey = builder.Configuration["Jwt:Key"];

if (string.IsNullOrWhiteSpace(jwtKey))
{
    throw new InvalidOperationException(
        "JWT key is not configured."
    );
}

builder.Services.AddAuthentication(
    JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters =
            new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,

                ValidIssuer =
                    builder.Configuration["Jwt:Issuer"],

                ValidAudience =
                    builder.Configuration["Jwt:Audience"],

                IssuerSigningKey =
                    new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(jwtKey)
                    )
            };
    });

// ===============================
// Authorization
// ===============================

builder.Services.AddAuthorization();

// ===============================
// CORS
// ===============================

var allowedOrigins =
    builder.Configuration
        .GetSection("Cors:AllowedOrigins")
        .Get<string[]>();

if (allowedOrigins == null ||
    allowedOrigins.Length == 0)
{
    throw new InvalidOperationException(
        "CORS allowed origins are not configured."
    );
}

builder.Services.AddCors(options =>
{
    options.AddPolicy("Frontend", policy =>
    {
        policy
            .WithOrigins(allowedOrigins)
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

// ===============================
// Gemini AI Service
// ===============================

builder.Services.AddScoped<IGeminiService, GeminiService>();

// ===============================
// Swagger
// ===============================

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition(
        "Bearer",
        new OpenApiSecurityScheme
        {
            Name = "Authorization",
            Type = SecuritySchemeType.Http,
            Scheme = "Bearer",
            BearerFormat = "JWT",
            In = ParameterLocation.Header,
            Description =
                "Enter your JWT token. Example: Bearer eyJhbGciOiJIUzI1NiIs..."
        });

    options.AddSecurityRequirement(document =>
        new OpenApiSecurityRequirement
        {
            [new OpenApiSecuritySchemeReference(
                "Bearer",
                document)] =
                new List<string>()
        });
});

// ===============================
// Build Application
// ===============================

var app = builder.Build();

// ===============================
// Global Exception Middleware
// ===============================

app.UseMiddleware<ExceptionMiddleware>();

// ===============================
// Swagger
// ===============================

//app.UseSwagger();
//app.UseSwaggerUI();



if (app.Environment.IsDevelopment())
{
app.UseSwagger();
app.UseSwaggerUI();
}

// ===============================
// HTTPS
// ===============================

//app.UseHttpsRedirection();

if (app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

// ===============================
// CORS
// IMPORTANT: Before Authentication
// ===============================

app.UseCors("Frontend");

// ===============================
// Authentication
// ===============================

app.UseAuthentication();

// ===============================
// Authorization
// ===============================

app.UseAuthorization();

// ===============================
// Controllers
// ===============================

app.MapControllers();

// ===============================
// Run
// ===============================

app.Run();
















//using Microsoft.AspNetCore.Authentication.JwtBearer;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.IdentityModel.Tokens;
//using Microsoft.OpenApi;
//using SmartSupport.API.Data;
//using SmartSupport.API.Interfaces;
//using SmartSupport.API.Services;
//using System.Text;

//var builder = WebApplication.CreateBuilder(args);

//// ===============================
//// Controllers
//// ===============================

//builder.Services.AddControllers()
//    .AddJsonOptions(options =>
//    {
//        options.JsonSerializerOptions.ReferenceHandler =
//            System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
//    });

//// ===============================
//// Database
//// ===============================

//var connectionString =
//    builder.Configuration.GetConnectionString("DefaultConnection");

//if (string.IsNullOrWhiteSpace(connectionString))
//{
//    throw new InvalidOperationException(
//        "Database connection string is not configured."
//    );
//}

//builder.Services.AddDbContext<AppDbContext>(options =>
//    options.UseMySQL(connectionString));

//// ===============================
//// JWT Authentication
//// ===============================

//var jwtKey = builder.Configuration["Jwt:Key"];

//if (string.IsNullOrWhiteSpace(jwtKey))
//{
//    throw new InvalidOperationException(
//        "JWT key is not configured."
//    );
//}

//builder.Services.AddAuthentication(
//    JwtBearerDefaults.AuthenticationScheme)
//    .AddJwtBearer(options =>
//    {
//        options.TokenValidationParameters =
//            new TokenValidationParameters
//            {
//                ValidateIssuer = true,
//                ValidateAudience = true,
//                ValidateLifetime = true,
//                ValidateIssuerSigningKey = true,

//                ValidIssuer =
//                    builder.Configuration["Jwt:Issuer"],

//                ValidAudience =
//                    builder.Configuration["Jwt:Audience"],

//                IssuerSigningKey =
//                    new SymmetricSecurityKey(
//                        Encoding.UTF8.GetBytes(jwtKey)
//                    )
//            };
//    });

//// ===============================
//// Authorization
//// ===============================

//builder.Services.AddAuthorization();

//// ===============================
//// CORS
//// ===============================

//var allowedOrigins =
//    builder.Configuration
//        .GetSection("Cors:AllowedOrigins")
//        .Get<string[]>();

//if (allowedOrigins == null ||
//    allowedOrigins.Length == 0)
//{
//    throw new InvalidOperationException(
//        "CORS allowed origins are not configured."
//    );
//}

//builder.Services.AddCors(options =>
//{
//    options.AddPolicy("Frontend", policy =>
//    {
//        policy
//            .WithOrigins(allowedOrigins)
//            .AllowAnyHeader()
//            .AllowAnyMethod();
//    });
//});

//// ===============================
//// Gemini AI Service
//// ===============================

//builder.Services.AddScoped<IGeminiService, GeminiService>();

//// ===============================
//// Swagger
//// ===============================

//builder.Services.AddEndpointsApiExplorer();

//builder.Services.AddSwaggerGen(options =>
//{
//    options.AddSecurityDefinition(
//        "Bearer",
//        new OpenApiSecurityScheme
//        {
//            Name = "Authorization",
//            Type = SecuritySchemeType.Http,
//            Scheme = "Bearer",
//            BearerFormat = "JWT",
//            In = ParameterLocation.Header,
//            Description =
//                "Enter your JWT token. Example: Bearer eyJhbGciOiJIUzI1NiIs..."
//        });

//    options.AddSecurityRequirement(document =>
//        new OpenApiSecurityRequirement
//        {
//            [new OpenApiSecuritySchemeReference(
//                "Bearer",
//                document)] =
//                new List<string>()
//        });
//});

//// ===============================
//// Build Application
//// ===============================

//var app = builder.Build();

//// ===============================
//// Swagger
//// ===============================

//app.UseSwagger();
//app.UseSwaggerUI();

//// ===============================
//// HTTPS
//// ===============================

//app.UseHttpsRedirection();

//// ===============================
//// CORS
//// IMPORTANT: Before Authentication
//// ===============================

//app.UseCors("Frontend");

//// ===============================
//// Authentication
//// ===============================

//app.UseAuthentication();

//// ===============================
//// Authorization
//// ===============================

//app.UseAuthorization();

//// ===============================
//// Controllers
//// ===============================

//app.MapControllers();

//// ===============================
//// Run
//// ===============================

//app.Run();




















//using Microsoft.AspNetCore.Authentication.JwtBearer;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.IdentityModel.Tokens;
//using Microsoft.OpenApi;
//using SmartSupport.API.Data;
//using SmartSupport.API.Interfaces;
//using SmartSupport.API.Services;
//using System.Text;

//var builder = WebApplication.CreateBuilder(args);

//// ===============================
//// Controllers
//// ===============================

//builder.Services.AddControllers()
//    .AddJsonOptions(options =>
//    {
//        options.JsonSerializerOptions.ReferenceHandler =
//            System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
//    });

//// ===============================
//// Database
//// ===============================

//var connectionString =
//    builder.Configuration.GetConnectionString("DefaultConnection");

//builder.Services.AddDbContext<AppDbContext>(options =>
//    options.UseMySQL(connectionString));

//// ===============================
//// JWT Authentication
//// ===============================

//var jwtKey = builder.Configuration["Jwt:Key"];

//builder.Services.AddAuthentication(
//    JwtBearerDefaults.AuthenticationScheme)
//    .AddJwtBearer(options =>
//    {
//        options.TokenValidationParameters =
//            new TokenValidationParameters
//            {
//                ValidateIssuer = true,
//                ValidateAudience = true,
//                ValidateLifetime = true,
//                ValidateIssuerSigningKey = true,

//                ValidIssuer =
//                    builder.Configuration["Jwt:Issuer"],

//                ValidAudience =
//                    builder.Configuration["Jwt:Audience"],

//                IssuerSigningKey =
//                    new SymmetricSecurityKey(
//                        Encoding.UTF8.GetBytes(jwtKey!)
//                    )
//            };
//    });

//// ===============================
//// Authorization
//// ===============================

//builder.Services.AddAuthorization();

//// ===============================
//// CORS
//// React Frontend:
//// http://localhost:5173
//// ===============================

//builder.Services.AddCors(options =>
//{
//    options.AddPolicy("Frontend", policy =>
//    {
//        policy
//            .WithOrigins("http://localhost:5173")
//            .AllowAnyHeader()
//            .AllowAnyMethod();
//    });
//});

//// ===============================
//// Gemini AI Service
//// ===============================

//builder.Services.AddScoped<IGeminiService, GeminiService>();

//// ===============================
//// Swagger
//// ===============================

//builder.Services.AddEndpointsApiExplorer();

//builder.Services.AddSwaggerGen(options =>
//{
//    options.AddSecurityDefinition(
//        "Bearer",
//        new OpenApiSecurityScheme
//        {
//            Name = "Authorization",
//            Type = SecuritySchemeType.Http,
//            Scheme = "Bearer",
//            BearerFormat = "JWT",
//            In = ParameterLocation.Header,
//            Description =
//                "Enter your JWT token. Example: Bearer eyJhbGciOiJIUzI1NiIs..."
//        });

//    options.AddSecurityRequirement(document =>
//        new OpenApiSecurityRequirement
//        {
//            [new OpenApiSecuritySchemeReference(
//                "Bearer",
//                document)] =
//                new List<string>()
//        });
//});

//// ===============================
//// Build Application
//// ===============================

//var app = builder.Build();

//// ===============================
//// Swagger
//// ===============================

//app.UseSwagger();
//app.UseSwaggerUI();

//// ===============================
//// HTTPS
//// ===============================

//app.UseHttpsRedirection();

//// ===============================
//// CORS
//// IMPORTANT: Before Authentication
//// ===============================

//app.UseCors("Frontend");

//// ===============================
//// Authentication
//// ===============================

//app.UseAuthentication();

//// ===============================
//// Authorization
//// ===============================

//app.UseAuthorization();

//// ===============================
//// Controllers
//// ===============================

//app.MapControllers();

//// ===============================
//// Run
//// ===============================

//app.Run();












//using Microsoft.AspNetCore.Authentication.JwtBearer;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.IdentityModel.Tokens;
//using Microsoft.OpenApi;
//using SmartSupport.API.Data;
//using SmartSupport.API.Interfaces;
//using SmartSupport.API.Services;
//using System.Text;

//var builder = WebApplication.CreateBuilder(args);

//builder.Services.AddControllers()
//    .AddJsonOptions(options =>
//    {
//        options.JsonSerializerOptions.ReferenceHandler =
//            System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
//    });


//// ========================================
//// MySQL Database Connection
//// ========================================

//var connectionString =
//    builder.Configuration.GetConnectionString("DefaultConnection");

//builder.Services.AddDbContext<AppDbContext>(options =>
//    options.UseMySQL(connectionString));


//// ========================================
//// JWT Authentication
//// ========================================

//var jwtKey = builder.Configuration["Jwt:Key"];

//builder.Services.AddAuthentication(
//    JwtBearerDefaults.AuthenticationScheme)
//    .AddJwtBearer(options =>
//    {
//        options.TokenValidationParameters =
//            new TokenValidationParameters
//            {
//                ValidateIssuer = true,
//                ValidateAudience = true,
//                ValidateLifetime = true,
//                ValidateIssuerSigningKey = true,

//                ValidIssuer =
//                    builder.Configuration["Jwt:Issuer"],

//                ValidAudience =
//                    builder.Configuration["Jwt:Audience"],

//                IssuerSigningKey =
//                    new SymmetricSecurityKey(
//                        Encoding.UTF8.GetBytes(jwtKey!)
//                    )
//            };
//    });

//builder.Services.AddAuthorization();
//builder.Services.AddScoped<IGeminiService, GeminiService>();

//// ========================================
//// Swagger + JWT Authorization
//// ========================================

//builder.Services.AddEndpointsApiExplorer();

//builder.Services.AddSwaggerGen(options =>
//{
//    options.AddSecurityDefinition(
//        "Bearer",
//        new OpenApiSecurityScheme
//        {
//            Name = "Authorization",
//            Type = SecuritySchemeType.Http,
//            Scheme = "Bearer",
//            BearerFormat = "JWT",
//            In = ParameterLocation.Header,
//            Description =
//                "Enter your JWT token. Example: Bearer eyJhbGciOiJIUzI1NiIs..."
//        });

//    options.AddSecurityRequirement(document =>
//        new OpenApiSecurityRequirement
//        {
//            [new OpenApiSecuritySchemeReference("Bearer", document)] =
//                new List<string>()
//        });
//});


//// ========================================
//// Build Application
//// ========================================

//var app = builder.Build();


//// ========================================
//// Middleware
//// ========================================

//app.UseSwagger();
//app.UseSwaggerUI();

//app.UseHttpsRedirection();

//app.UseAuthentication();

//app.UseAuthorization();

//app.MapControllers();

//app.Run();