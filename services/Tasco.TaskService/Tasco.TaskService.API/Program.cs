using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Tasco.TaskService.API.GrpcServices;
using Tasco.TaskService.API.Mapping;
using Tasco.TaskService.Repository.Entities;
using Tasco.TaskService.Repository.UnitOfWork;
using Tasco.TaskService.Service.Implementations;
using Tasco.TaskService.Service.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
//Add Swagger for API documentation
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
// Add gRPC services
builder.Services.AddGrpc();

// Add DbContext
builder.Services.AddDbContext<TaskManagementDbContext>(options =>
	options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add UnitOfWork
builder.Services.AddScoped<IUnitOfWork<TaskManagementDbContext>, UnitOfWork<TaskManagementDbContext>>();

// Add AutoMapper
builder.Services.AddAutoMapper(typeof(AutoMapperProfiles));

// Add Services
builder.Services.AddScoped<IWorkTaskService, WorkTaskService>();
builder.Services.AddScoped<ITaskActionService, TaskActionService>();
builder.Services.AddScoped<IWorkAreaService, WorkAreaService>();

// Add HttpContextAccessor
builder.Services.AddHttpContextAccessor();

// Add JWT Authentication

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Map gRPC services
app.MapGrpcService<WorkTaskGrpcService>();
app.MapGrpcService<TaskActionGrpcService>();
app.MapGrpcService<WorkAreaGrpcService>();

app.Run();
