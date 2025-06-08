using DotNetEnv;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Tasco.UserAuthService.API.Mapping;
using Tasco.UserAuthService.API.Middlewares;
using Tasco.UserAuthService.Repository;
using Tasco.UserAuthService.Repository.UnitOfWork;

var builder = WebApplication.CreateBuilder(args);

var currentDir = Directory.GetCurrentDirectory();
var relativePath = Path.Combine(currentDir, "../../../.env");
var envPath = Path.GetFullPath(relativePath);
if (!File.Exists(envPath))
{
    throw new FileNotFoundException($"Environment file not found at path: {envPath}");
}
Env.Load(envPath);
builder.Configuration.AddEnvironmentVariables();
var connectionString = builder.Configuration.GetConnectionString("TascoAuth") ??
                       throw new InvalidOperationException("Connection string 'TascoAuth' not found.");
// Add services to the container.

builder.Services.AddControllers();

//-------------------- Swagger --------------------
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "Tasco Auth API", Version = "v1" });
    options.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, new OpenApiSecurityScheme
    {
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = JwtBearerDefaults.AuthenticationScheme,
        BearerFormat = "JWT",
        Description = "Example: \"Bearer (Token here)\"",
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = JwtBearerDefaults.AuthenticationScheme
                },
                Scheme = "Oauth2",
                Name = JwtBearerDefaults.AuthenticationScheme,
                In = ParameterLocation.Header
            },
            new List<string>()
        }
    });
});

//-------------------- Database --------------------
builder.Services.AddDbContext<TascoAuthDBContext>(options =>
               options.UseSqlServer(builder.Configuration.GetConnectionString("TascoAuth")));

//-------------------- AutoMapper --------------------
builder.Services.AddAutoMapper(typeof(AutoMapperProfiles));

//-------------------- UnitOfWork --------------------
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

//-------------------- Service --------------------

//-------------------- Cors --------------------
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

//-------------------- Authentication --------------------
builder.Services.AddIdentity<IdentityUser, IdentityRole>()
                .AddRoles<IdentityRole>()
                .AddTokenProvider<DataProtectorTokenProvider<IdentityUser>>("Tasco")
                .AddEntityFrameworkStores<TascoAuthDBContext>()
                .AddDefaultTokenProviders();

builder.Services.Configure<IdentityOptions>(options =>
{
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequiredLength = 8;
    options.Password.RequiredUniqueChars = 1;
    options.SignIn.RequireConfirmedEmail = true;
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
        ValidIssuer = builder.Configuration["JWT:Issuer"],
        ValidAudience = builder.Configuration["JWT:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(builder.Configuration["JWT:Key"]))
    };
});

var app = builder.Build();


app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.UseMiddleware<ExceptionMiddleware>();
app.MapControllers();

app.Run();
