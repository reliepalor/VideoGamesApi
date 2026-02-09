using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using VideoGameApi.Api.Interfaces.DigitalOrders;
using VideoGameApi.Api.Repositories.DigitalOrders;
using VideoGameApi.Api.Repositories.DigitalProducts;
using VideoGameApi.Api.Services;
using VideoGameApi.Api.Services.DigitalOrders;
using VideoGameApi.Api.Services.DigitalProducts;
using VideoGameApi.Auth.Interfaces;
using VideoGameApi.Auth.Services;
using VideoGameApi.Data;
using VideoGameApi.Hubs;
using VideoGameApi.Api.Interfaces.DigitalProducts;
using VideoGameApi.Repositories.DigitalProducts;

var builder = WebApplication.CreateBuilder(args);
var firebasePath = Path.Combine(
   builder.Environment.ContentRootPath,
   "Firebase",
   "firebase-admin.json"
);

// -------------------- SERVICES --------------------

builder.Services.AddScoped<IDigitalProductService, DigitalProductService>();
builder.Services.AddScoped<IDigitalProductRepository, DigitalProductRepository>();
builder.Services.AddScoped<IDigitalProductKeyRepository, DigitalProductKeyRepository>();
builder.Services.AddScoped<IDigitalOrderItemRepository, DigitalOrderItemRepository>();
builder.Services.AddScoped<IDigitalOrderRepository, DigitalOrderRepository>();
builder.Services.AddScoped<
    VideoGameApi.Api.Interfaces.DigitalOrders.IDigitalOrderService,
    VideoGameApi.Api.Services.DigitalOrders.DigitalOrderService
>();

var services = builder.Services;


// Add API controllers
builder.Services.AddControllers();

// Swagger / OpenAPI
builder.Services.AddEndpointsApiExplorer();

// 🔐 Swagger with JWT support
builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter JWT token like: Bearer {your token}"
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
// Product Key Service
builder.Services.AddScoped<IProductKeyService, ProductKeyService>();

// 🗄️ Database (SQL Server)
builder.Services.AddDbContext<VideoGameDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")
    )
);

// 🔑 JWT Authentication (HTTP + SignalR)
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
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
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!)
            )
        };

        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                var accessToken = context.Request.Query["access_token"];
                var path = context.HttpContext.Request.Path;

                if (!string.IsNullOrEmpty(accessToken) &&
                    path.StartsWithSegments("/hubs/conversations"))
                {
                    context.Token = accessToken;
                }

                return Task.CompletedTask;
            }
        };
    });


builder.Services.AddAuthorization();

builder.Services.AddSignalR();

builder.Services.AddScoped<IAuthService, AuthService>();

//CORS for Angular (localhost:4200)
builder.Services.AddCors(options =>
{
    options.AddPolicy("SignalRCors", policy =>
    {
        policy
            .WithOrigins("http://localhost:4200")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});



builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
    options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
});

FirebaseApp.Create(new AppOptions
{
    Credential = GoogleCredential.FromFile(firebasePath)
});



var app = builder.Build();

// -------------------- MIDDLEWARE --------------------

// Swagger only in development
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.UseCors("AllowAngular");
app.UseStaticFiles();
app.UseCors("SignalRCors");
app.UseAuthentication();
app.UseAuthorization();
app.MapHub<ConversationHub>("/hubs/conversations");
app.MapControllers();

app.Run();
