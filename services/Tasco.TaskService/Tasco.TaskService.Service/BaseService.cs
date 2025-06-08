using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Security.Claims;
using Tasco.TaskService.Repository.Entities;
using Tasco.TaskService.Repository.UnitOfWork;

namespace Tasco.TaskService.Service
{
	public abstract class BaseService<T> where T : class
	{
		protected readonly IUnitOfWork<TaskManagementDbContext> _unitOfWork;
		protected readonly ILogger<T> _logger;
		protected readonly IMapper _mapper;
		protected readonly IHttpContextAccessor _httpContextAccessor;

		protected BaseService(
			IUnitOfWork<TaskManagementDbContext> unitOfWork,
			ILogger<T> logger,
			IMapper mapper,
			IHttpContextAccessor httpContextAccessor)
		{
			_unitOfWork = unitOfWork;
			_logger = logger;
			_mapper = mapper;
			_httpContextAccessor = httpContextAccessor;
		}

		protected string GetUserIdFromJwt()
		{
			var userId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
			return userId;
		}

		protected string GetUserEmailFromJwt()
		{
			var email = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Email)?.Value;
			return email;
		}

		protected string GetRoleFromJwt()
		{
			string role = _httpContextAccessor?.HttpContext?.User.FindFirstValue(ClaimTypes.Role);
			return role;
		}
	}
}
