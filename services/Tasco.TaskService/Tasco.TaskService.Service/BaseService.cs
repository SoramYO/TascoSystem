using AutoMapper;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Tasco.TaskService.Repository.Entities;
using Tasco.TaskService.Repository.UnitOfWork;


namespace Tasco.TaskService.Service
{
	public abstract class BaseService<T> where T : class
	{
		protected IUnitOfWork<TaskManagementDbContext> _unitOfWork;
		protected ILogger<T> _logger;
		protected IMapper _mapper;
		protected IHttpContextAccessor _httpContextAccessor;

		public BaseService(IUnitOfWork<TaskManagementDbContext> unitOfWork, ILogger<T> logger, IMapper mapper,
			IHttpContextAccessor httpContextAccessor)
		{
			_unitOfWork = unitOfWork;
			_logger = logger;
			_mapper = mapper;
			_httpContextAccessor = httpContextAccessor;
		}

		protected string GetUsernameFromJwt()
		{
			string username = _httpContextAccessor?.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
			return username;
		}

		protected string GetRoleFromJwt()
		{
			string role = _httpContextAccessor?.HttpContext?.User.FindFirstValue(ClaimTypes.Role);
			return role;
		}

		protected string GetBrandIdFromJwt()
		{
			return _httpContextAccessor?.HttpContext?.User?.FindFirstValue("userId");
		}
	}
}
