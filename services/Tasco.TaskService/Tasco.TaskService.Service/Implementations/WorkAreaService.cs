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
    public class WorkAreaService : BaseService<WorkAreaService>, IWorkAreaService
    {
        public WorkAreaService(IUnitOfWork<TaskManagementDbContext> unitOfWork, ILogger<WorkAreaService> logger, IMapper mapper, IHttpContextAccessor httpContextAccessor) : base(unitOfWork, logger, mapper, httpContextAccessor)
        {
        }

        public async Task<WorkArea> CreateWorkArea(WorkAreaBusinessModel workArea)
        {
            var userId = GetUserIdFromJwt();
            var userEmail = GetUserEmailFromJwt();
            if (userId == null)
            {
                throw new UnauthorizedAccessException("User is not authenticated.");
            }
            var mappedWorkArea = _mapper.Map<WorkArea>(workArea);
            mappedWorkArea.CreatedByUserId = Guid.Parse(userId); 

            await _unitOfWork.GetRepository<WorkArea>().InsertAsync(mappedWorkArea); 
            await _unitOfWork.CommitAsync();
            return mappedWorkArea;
        }

        public async Task DeleteWorkArea(Guid id)
        {
            var existWorkArea = await GetWorkAreaById(id);
            _unitOfWork.GetRepository<WorkArea>().Delete(existWorkArea);
            await _unitOfWork.CommitAsync();
        }

        public async Task<IPaginate<WorkArea>> GetAllWorkAreas(int pageSize, int pageIndex, string search = null)
        {
            var workAreas = await _unitOfWork.GetRepository<WorkArea>()
                .GetPagingListAsync(predicate: w => string.IsNullOrEmpty(search) || w.Name.Contains(search),
                                    orderBy: q => q.OrderBy(w => w.Name),
                                    page: pageIndex,
                                    size: pageSize);
            return workAreas;
        }

        public Task<IPaginate<WorkArea>> GetMyWorkAreas(int pageSize, int pageIndex, string search = null)
        {
            var userId = GetUserIdFromJwt();
            if (userId == null)
            {
                throw new UnauthorizedAccessException("User is not authenticated.");
            }
            var workAreas = _unitOfWork.GetRepository<WorkArea>()
                .GetPagingListAsync(predicate: w => w.CreatedByUserId == Guid.Parse(userId) && (string.IsNullOrEmpty(search) || w.Name.Contains(search)),
                                    orderBy: q => q.OrderBy(w => w.Name),
                                    page: pageIndex,
                                    size: pageSize);
            return workAreas;
        }

        public async Task<WorkArea> GetWorkAreaById(Guid id)
        {
            var workarea = await _unitOfWork.GetRepository<WorkArea>().SingleOrDefaultAsync(predicate: w => w.Id == id);
            return workarea;
        }

        public async Task UpdateWorkArea(Guid id, WorkAreaBusinessModel workArea)
        {
            var existingWorkArea = await GetWorkAreaById(id);
            if (existingWorkArea == null)
            {
                throw new KeyNotFoundException($"WorkArea with ID {id} not found.");
            }
            var mappedWorkArea = _mapper.Map<WorkArea>(workArea);

            _unitOfWork.GetRepository<WorkArea>().Update(mappedWorkArea);
            await _unitOfWork.CommitAsync();
        }
    }
}
