using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
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
            entity.CreatedDate = DateTime.UtcNow;
            entity.IsActive = true;

            await _unitOfWork.GetRepository<WorkArea>().InsertAsync(entity);
            await _unitOfWork.CommitAsync();

            return entity;
        }

        public async Task DeleteWorkArea(Guid id)
        {
            var workArea = await _unitOfWork.GetRepository<WorkArea>()
                .SingleOrDefaultAsync(predicate: w => w.Id == id && !w.IsDeleted);

            if (workArea == null)
            {
                throw new KeyNotFoundException($"Work area with ID {id} not found.");
            }

            workArea.IsDeleted = true;
            _unitOfWork.GetRepository<WorkArea>().Update(workArea);
            await _unitOfWork.CommitAsync();
        }

        public async Task<IPaginate<WorkArea>> GetMyWorkAreasByProjectId(int pageSize, int pageIndex, Guid projectId)
        {
            var workArea = await _unitOfWork.GetRepository<WorkArea>().GetPagingListAsync(
                predicate: w => w.ProjectId == projectId && !w.IsDeleted,
                page: pageIndex,
                size: pageSize,
                include: query => query
                    .Include(w => w.WorkTasks)
                    .ThenInclude(wt => wt.TaskActions)
                    .Include(w => w.WorkTasks)
                    .ThenInclude(wt => wt.TaskMembers)
                    .Include(w => w.WorkTasks)
                    .ThenInclude(wt => wt.TaskObjectives)
            );

            return workArea;
        }


        public async Task<WorkArea> GetWorkAreaById(Guid id)
        {
            var workArea = await _unitOfWork.GetRepository<WorkArea>()
                .SingleOrDefaultAsync(
                    predicate: w => w.Id == id && !w.IsDeleted,
                    include: query => query
                        .Include(w => w.WorkTasks)
                        .ThenInclude(wt => wt.TaskActions)
                        .Include(w => w.WorkTasks)
                        .ThenInclude(wt => wt.TaskMembers)
                        .Include(w => w.WorkTasks)
                        .ThenInclude(wt => wt.TaskObjectives)
                    );

            if (workArea == null)
            {
                throw new KeyNotFoundException($"Work area with ID {id} not found.");
            }

            return workArea;
        }

        public async Task UpdateWorkArea(Guid id, WorkAreaBusinessModel workArea)
        {
            var existingWorkArea = await _unitOfWork.GetRepository<WorkArea>()
                .SingleOrDefaultAsync(predicate: w => w.Id == id && !w.IsDeleted);
            if (existingWorkArea == null)
            {
                throw new KeyNotFoundException($"Work area with ID {id} not found.");
            }
            
            _logger.LogInformation($"Updating work area with ID {id}");
            _logger.LogInformation($"Work area: {JsonConvert.SerializeObject(workArea)}");

            
            // Update the fields
            existingWorkArea.Name = workArea.Name;
            existingWorkArea.Description = workArea.Description;
            existingWorkArea.DisplayOrder = workArea.DisplayOrder;
            existingWorkArea.ProjectId = workArea.ProjectId;
            
            // Only update CreatedByUserId if it's not empty
            if (workArea.CreatedByUserId != Guid.Empty)
            {
                existingWorkArea.CreatedByUserId = workArea.CreatedByUserId;
            }
            
            _unitOfWork.GetRepository<WorkArea>().Update(existingWorkArea);
            await _unitOfWork.CommitAsync();
        }
    }
}