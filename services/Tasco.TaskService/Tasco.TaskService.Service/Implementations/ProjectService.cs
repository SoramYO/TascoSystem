using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tasco.TaskService.Repository.Entities;
using Tasco.TaskService.Repository.Paginate;
using Tasco.TaskService.Repository.UnitOfWork;
using Tasco.TaskService.Service.BusinessModels;
using Tasco.TaskService.Service.Interfaces;

namespace Tasco.TaskService.Service.Implementations
{
	public class ProjectService : BaseService<ProjectService>, IProjectService
	{
		public ProjectService(IUnitOfWork<TaskManagementDbContext> unitOfWork, ILogger<ProjectService> logger, IMapper mapper, IHttpContextAccessor httpContextAccessor) : base(unitOfWork, logger, mapper, httpContextAccessor)
		{
		}

		public async Task<ProjectBusinessModel> CreateProject(ProjectBusinessModel request)
		{
			var project = _mapper.Map<Project>(request);

			await _unitOfWork.GetRepository<Project>().InsertAsync(project);
			await _unitOfWork.CommitAsync();
			return _mapper.Map<ProjectBusinessModel>(project);
		}

		public async Task<IPaginate<ProjectBusinessModel>> GetAllProjects(int pageSize, int pageIndex, string search)
		{
			if (pageSize <= 0 || pageIndex < 0)
			{
				throw new ArgumentOutOfRangeException("Page size must be greater than zero and page index cannot be negative.");
			}

			var repository = await _unitOfWork.GetRepository<Project>().GetPagingListAsync
				(
				predicate: string.IsNullOrEmpty(search) ? null : p => p.Name.Contains(search) || p.Description.Contains(search),
				orderBy: p => p.OrderByDescending(x => x.CreatedDate),
				include: p => p.Include(x => x.ProjectMembers).Include(x => x.WorkAreas),
				page: pageIndex,
				size: pageSize
				);
			var mapped = _mapper.Map<IPaginate<ProjectBusinessModel>>(repository);
			return mapped;
		}

		public async Task<ProjectBusinessModel> GetProjectById(Guid projectId)
		{
			var project = await _unitOfWork.GetRepository<Project>().SingleOrDefaultAsync(
				predicate: p => p.Id == projectId,
				include: p => p.Include(x => x.ProjectMembers).Include(x => x.WorkAreas));

			// Map Entity to BusinessModel
			return _mapper.Map<ProjectBusinessModel>(project);
		}

		public async Task<ProjectBusinessModel> UpdateProject(ProjectBusinessModel request)
		{
			var project = _mapper.Map<Project>(request);

			_unitOfWork.GetRepository<Project>().Update(project);
			await _unitOfWork.CommitAsync();
			return _mapper.Map<ProjectBusinessModel>(project);
		}
	}
}
