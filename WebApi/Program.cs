using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using Serilog.Events;
using Serilog.Formatting.Compact;
using WebApi.Data;
using WebApi.Dtos.Account;
using WebApi.Permissions;
using WebApi.Seed;
using WebApi.Services;
using WebApi.Services.Permission;

var builder = WebApplication.CreateBuilder(args);

//register db context
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<DataContext>(opt => opt.UseNpgsql(connectionString));

//register permission based auth
builder.Services.AddSingleton<IAuthorizationPolicyProvider, PermissionPolicyProvider>();
builder.Services.AddScoped<IAuthorizationHandler, PermissionAuthorizationHandler>();


Log.Logger = new LoggerConfiguration()
    .WriteTo.Console(new RenderedCompactJsonFormatter())
    .MinimumLevel.Override("Microsoft.AspNetCore",LogEventLevel.Debug)
    .MinimumLevel.Override("Default",LogEventLevel.Debug)
    .WriteTo.File("diagnostics.txt",
        rollingInterval: RollingInterval.Day,
        fileSizeLimitBytes: 10 * 1024 * 1024,
        retainedFileCountLimit: null,
        rollOnFileSizeLimit: true).CreateLogger();

builder.Host.UseSerilog();

builder.Services.AddMemoryCache();

//register Identity 
builder.Services.AddIdentityCore<IdentityUser>(config =>
    {
        config.Password.RequiredLength = 4;
        config.Password.RequireDigit = false;
        config.Password.RequireNonAlphanumeric = false;
        config.Password.RequireUppercase = false;
        config.Password.RequireLowercase = false;
    })
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<DataContext>()
    .AddDefaultTokenProviders();


//register services
builder.Services.AddScoped<IQuoteService, QuoteService>();
builder.Services.AddScoped<IPermissionService, PermissionService>();
builder.Services.AddSingleton<IEmailService, EmailService>();
builder.Services.AddScoped<Seeder>();
builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.Configure<EmailConfiguration>(builder.Configuration.GetSection(EmailConfiguration.SectionName));

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        var key = builder.Configuration["JWT:Key"];
        options.TokenValidationParameters = new TokenValidationParameters()
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration ["JWT:Issuer"],
            ValidAudience = builder.Configuration["JWT:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(key)),
        };
    });

builder.Services.AddControllers();

//register swagger auth
builder.Services.AddSwaggerGen(opt =>
{
    opt.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Enter your access token",
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT"
    });

    opt.AddSecurityRequirement(new OpenApiSecurityRequirement {
        {
            new OpenApiSecurityScheme 
            {
                Reference = new OpenApiReference 
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});


var app = builder.Build();

//auto update database and seed
try
{
    //create database 
    var serviceProvider = app.Services.CreateScope().ServiceProvider; 
    var datacontext = serviceProvider.GetRequiredService<DataContext>();
    await datacontext.Database.MigrateAsync();
    
    //seed data
    var seeder = serviceProvider.GetRequiredService<Seeder>();
    await seeder.SeedRole();
    await seeder.SeedUser();
    await seeder.RestorePermissions();
}
catch (Exception e)
{
    // ignored
}


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
{
    app.UseSwagger();
    app.UseSwaggerUI();

}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();