using Microsoft.EntityFrameworkCore;
using Tasco.ProjectService.API.Mapping;
using Tasco.ProjectService.Repository.Entities;
using Tasco.ProjectService.Repository.Repositories;
using Tasco.ProjectService.Repository.UnitOfWork;
using Tasco.ProjectService.Service.BussinessModel.ProjectBussinessModel;
using Tasco.ProjectService.Service.Helper;
using Tasco.ProjectService.Service.Services.Implemention;
using Tasco.ProjectService.Service.Services.Interface;
using Tasco.ProjectService.API.Middlewares;
using Tasco.ProjectService.Service.Services.GRpcService;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// --- Add gRPC services ---
builder.Services.AddGrpc();

// --- Db Context Configuration ---
builder.Services.AddDbContext<ProjectManagementDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
// --- Mapper Configuration ---
builder.Services.AddAutoMapper(typeof(AutoMapperProfiles));
// --- Unit of Work Configuration ---
builder.Services.AddTransient<IUnitOfWork, UnitOfWork<ProjectManagementDbContext>>();
// --- Repository Configuration ---
builder.Services.AddTransient(typeof(IGenericRepository<>), typeof(GenericRepository<>));
// --- Service Configuration ---
builder.Services.AddTransient<IProjectService, ProjectService>();
builder.Services.AddTransient<IProjectMemberService, ProjectMemberService>();

// --- CORS Configuration ---
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// --- Swagger Configuration ---
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "Project Service API", Version = "v1" });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Thêm middleware CORS
app.UseCors("AllowAll");

// Thêm middleware đọc claims từ header từ Gateway
app.UseClaimsFromHeaders();

app.UseAuthorization();

app.MapControllers();


// 👉 Map gRPC service
app.MapGrpcService<GrpcProjectService>();
app.MapGrpcService<GrpcProjectMenberService>();

app.Run();