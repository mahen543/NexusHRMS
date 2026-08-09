using HRMS_Common;
using HRMS_Common.CommonProperties;
using HRMS_HikeFlow.NexusBAL;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using NexusHRMS.HikeFlowWeb.Authorization;
using NexusHRMS.HikeFlowWeb.Security;
using NexusHRMS.HikeFlowWeb.Services;
using Scalar.AspNetCore;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Addding configuration
var config = builder.Configuration;

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddSwaggerGen();


// Register dependencies
builder.Services.AddTransient(Connection => new AppDb(builder.Configuration["AppSettings:QRConnectionString"], int.Parse(builder.Configuration["AppSettings:CommandTimeOut"]!)));
builder.Services.Configure<RegularExpressions>(builder.Configuration.GetSection("RegularExpressions"));
builder.Services.AddScoped<General>();


//Adding JWT
builder.Services.AddScoped<ITokenGeneratorService, TokenGeneratorService>();
builder.Services.AddScoped<AuthBAL>();

// Register RBAC Authorization Middleware Handler
builder.Services.AddSingleton<IAuthorizationHandler, HierarchyAuthorizationHandler>();


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
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(builder.Configuration["Jwt:SecretKey"]!)
        ),
        ClockSkew = TimeSpan.Zero // Removes default 5-min grace period for expired tokens
    };
});

//Injecting Authorization policies through a seperate class
builder.Services.AddAppAuthorization();

//builder.Services.AddAuthorization(options =>
//{
//    // Level 1 Executives / System Admin (Global Access)
//    options.AddPolicy("Admin", policy =>
//        policy.Requirements.Add(new HierarchyRequirement(maxAllowedHierarchyLevel: 1)));

//    // HR Policy: Allows Hierarchy Level <= 2 AND Designation ID 2 (HR Manager)
//    options.AddPolicy("HRDirectorship", policy =>
//        policy.Requirements.Add(new HierarchyRequirement(maxAllowedHierarchyLevel: 2, 2L)));

//    // Finance/PM Policy: Allows Hierarchy Level <= 3 AND Designation IDs 3 (Project Manager) or 7 (Accountant)
//    options.AddPolicy("ProjectManager", policy =>
//        policy.Requirements.Add(new HierarchyRequirement(maxAllowedHierarchyLevel: 3, 3L, 7L)));
//});


//Adding CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngular",
        policy =>
        {
            policy.WithOrigins("http://localhost:4200")
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    // 2. Configure Scalar to consume Swashbuckle's OpenAPI endpoint
    app.MapScalarApiReference(options =>
    {
        options.WithOpenApiRoutePattern("/swagger/v1/swagger.json");
    });
}
app.UseCors("AllowAngular");

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
