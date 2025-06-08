using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
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
        public WorkAreaService(
            IUnitOfWork<TaskManagementDbContext> unitOfWork,
            ILogger<WorkAreaService> logger,
            IMapper mapper,
            IHttpContextAccessor httpContextAccessor
        ) : base(unitOfWork, logger, mapper, httpContextAccessor)
        {
        }

        public async Task<WorkArea> CreateWorkArea(WorkAreaBusinessModel workArea)
        {
            var entity = _mapper.Map<WorkArea>(workArea);
            var userId = GetUserIdFromJwt();
            var userEmail = GetUserEmailFromJwt();
            if (userId == null)
            {
                throw new UnauthorizedAccessException("User is not authenticated.");
            }

            entity.CreatedByUserId = Guid.Parse(userId);
            entity.CreatedDate = DateTime.UtcNow;
            entity.IsActive = true;

            await _unitOfWork.GetRepository<WorkArea>().InsertAsync(entity);
            await _unitOfWork.CommitAsync();

            return entity;
        }

        public async Task DeleteWorkArea(Guid id)
        {
            var workArea = await _unitOfWork.GetRepository<WorkArea>()
                .SingleOrDefaultAsync(predicate: w => w.Id == id);

            if (workArea == null)
            {
                throw new KeyNotFoundException($"Work area with ID {id} not found.");
            }

            _unitOfWork.GetRepository<WorkArea>().Delete(workArea);
            await _unitOfWork.CommitAsync();
        }

        public async Task<IPaginate<WorkArea>> GetAllWorkAreas(int pageSize, int pageIndex, string search = null)
        {
            var workAreas = await _unitOfWork.GetRepository<WorkArea>().GetPagingListAsync(
                predicate: w => string.IsNullOrEmpty(search) ||
                               w.Name.Contains(search) ||
                               w.Description.Contains(search),
                orderBy: q => q.OrderByDescending(w => w.CreatedDate),
                page: pageIndex,
                size: pageSize);

            return workAreas;
        }

        public async Task<IPaginate<WorkArea>> GetMyWorkAreas(int pageSize, int pageIndex, string search = null)
        {
            var userId = GetUserIdFromJwt();
            if (userId == null)
            {
                throw new UnauthorizedAccessException("User is not authenticated.");
            }

            var workAreas = await _unitOfWork.GetRepository<WorkArea>().GetPagingListAsync(
                predicate: w => w.CreatedByUserId == Guid.Parse(userId) &&
                               (string.IsNullOrEmpty(search) ||
                                w.Name.Contains(search) ||
                                w.Description.Contains(search)),
                orderBy: q => q.OrderByDescending(w => w.CreatedDate),
                page: pageIndex,
                size: pageSize);

            return workAreas;
        }

        public async Task<WorkArea> GetWorkAreaById(Guid id)
        {
            var workArea = await _unitOfWork.GetRepository<WorkArea>()
                .SingleOrDefaultAsync(predicate: w => w.Id == id);

            if (workArea == null)
            {
                throw new KeyNotFoundException($"Work area with ID {id} not found.");
            }

            return workArea;
        }

        public async Task UpdateWorkArea(Guid id, WorkAreaBusinessModel workArea)
        {
            var existingWorkArea = await _unitOfWork.GetRepository<WorkArea>()
                .SingleOrDefaultAsync(predicate: w => w.Id == id);

            if (existingWorkArea == null)
            {
                throw new KeyNotFoundException($"Work area with ID {id} not found.");
            }

            var userId = GetUserIdFromJwt();
            var userEmail = GetUserEmailFromJwt();
            if (userId == null)
            {
                throw new UnauthorizedAccessException("User is not authenticated.");
            }

            // Preserve original creation data
            var createdByUserId = existingWorkArea.CreatedByUserId;
            var createdDate = existingWorkArea.CreatedDate;

            // Map new data
            _mapper.Map(workArea, existingWorkArea);

            // Restore original creation data
            existingWorkArea.CreatedByUserId = createdByUserId;
            existingWorkArea.CreatedDate = createdDate;

            _unitOfWork.GetRepository<WorkArea>().Update(existingWorkArea);
            await _unitOfWork.CommitAsync();
        }
    }
}
