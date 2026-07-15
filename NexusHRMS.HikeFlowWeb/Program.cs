using HRMS_Common;
using HRMS_Common.CommonProperties;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Addding configuration
var config = builder.Configuration;

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen();

// Register dependencies
builder.Services.AddTransient(Connection => new AppDb(builder.Configuration["AppSettings:QRConnectionString"], int.Parse(builder.Configuration["AppSettings:CommandTimeOut"])));

builder.Services.Configure<RegularExpressions>(builder.Configuration.GetSection("RegularExpressions"));

builder.Services.AddScoped<General>();


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
    app.MapScalarApiReference();
    app.MapOpenApi();
}
app.UseCors("AllowAngular");

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
