using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Tasco.TaskService.Repository.Entities;
using Tasco.TaskService.Repository.Paginate;
using Tasco.TaskService.Repository.UnitOfWork;
using Tasco.TaskService.Service.BusinessModels;
using Tasco.TaskService.Service.Interfaces;

namespace Tasco.TaskService.Service.Implementations
{
    public class TaskObjectiveService : BaseService<TaskObjectiveService>, ITaskObjectiveService
    {
        public TaskObjectiveService(
            IUnitOfWork<TaskManagementDbContext> unitOfWork,
            ILogger<TaskObjectiveService> logger,
            IMapper mapper,
            IHttpContextAccessor httpContextAccessor
        ) : base(unitOfWork, logger, mapper, httpContextAccessor)
        {
        }

        public async Task<TaskObjective> CreateTaskObjectiveAsync(TaskObjectiveBusinessModel taskObjective)
        {
            // Validate that the WorkTask exists
            var workTask = await _unitOfWork.GetRepository<WorkTask>()
                .SingleOrDefaultAsync(wt => wt.Id == taskObjective.WorkTaskId && !wt.IsDeleted);
            
            if (workTask == null)
            {
                throw new ArgumentException($"WorkTask with ID {taskObjective.WorkTaskId} does not exist or has been deleted");
            }

            var entity = _mapper.Map<TaskObjective>(taskObjective);
            
            // Since authentication is disabled, use a default user ID
            var userId = Guid.NewGuid(); // Use a default user ID

            entity.Id = Guid.NewGuid();
            entity.CreatedByUserId = userId;
            entity.CreatedDate = DateTime.UtcNow;
            entity.IsCompleted = false;
            entity.IsDeleted = false;

            await _unitOfWork.GetRepository<TaskObjective>().InsertAsync(entity);
            await _unitOfWork.CommitAsync();
            return entity;
        }

        public async Task<TaskObjective> GetTaskObjectiveByIdAsync(Guid id)
        {
            var taskObjective = await _unitOfWork.GetRepository<TaskObjective>()
                .SingleOrDefaultAsync(
                    predicate: to => to.Id == id && !to.IsDeleted
                );

            if (taskObjective == null)
            {
                throw new KeyNotFoundException($"Task objective with ID {id} not found.");
            }

            return taskObjective;
        }

        public async Task<IPaginate<TaskObjective>> GetTaskObjectivesByWorkTaskIdAsync(Guid workTaskId, int pageIndex, int pageSize)
        {
            return await _unitOfWork.GetRepository<TaskObjective>()
                .GetPagingListAsync(
                    predicate: to => to.WorkTaskId == workTaskId && !to.IsDeleted,
                    orderBy: q => q.OrderBy(to => to.DisplayOrder),
                    page: pageIndex,
                    size: pageSize 
                );
        }

        public async Task<TaskObjective> UpdateTaskObjectiveAsync(Guid id, TaskObjectiveBusinessModel taskObjective)
        {
            var existingTaskObjective = await _unitOfWork.GetRepository<TaskObjective>()
                .SingleOrDefaultAsync(predicate: to => to.Id == id && !to.IsDeleted);

            if (existingTaskObjective == null)
            {
                throw new KeyNotFoundException($"Task objective with ID {id} not found.");
            }

            // Validate that the WorkTask exists
            var workTask = await _unitOfWork.GetRepository<WorkTask>()
                .SingleOrDefaultAsync(wt => wt.Id == taskObjective.WorkTaskId && !wt.IsDeleted);
            
            if (workTask == null)
            {
                throw new ArgumentException($"WorkTask with ID {taskObjective.WorkTaskId} does not exist or has been deleted");
            }

            // Preserve original creation data
            var createdByUserId = existingTaskObjective.CreatedByUserId;
            var createdDate = existingTaskObjective.CreatedDate;

            // Map new data
            _mapper.Map(taskObjective, existingTaskObjective);

            // Restore original creation data
            existingTaskObjective.CreatedByUserId = createdByUserId;
            existingTaskObjective.CreatedDate = createdDate;

            _unitOfWork.GetRepository<TaskObjective>().Update(existingTaskObjective);
            await _unitOfWork.CommitAsync();
            return existingTaskObjective;
        }

        public async Task DeleteTaskObjectiveAsync(Guid id)
        {
            var taskObjective = await _unitOfWork.GetRepository<TaskObjective>()
                .SingleOrDefaultAsync(predicate: to => to.Id == id && !to.IsDeleted);

            if (taskObjective == null)
            {
                throw new KeyNotFoundException($"Task objective with ID {id} not found.");
            }

            taskObjective.IsDeleted = true;
            _unitOfWork.GetRepository<TaskObjective>().Update(taskObjective);
            await _unitOfWork.CommitAsync();
        }

        public async Task<TaskObjective> CompleteTaskObjectiveAsync(Guid id, bool isCompleted, Guid userId)
        {
            var taskObjective = await _unitOfWork.GetRepository<TaskObjective>()
                .SingleOrDefaultAsync(predicate: to => to.Id == id && !to.IsDeleted);

            if (taskObjective == null)
            {
                throw new KeyNotFoundException($"Task objective with ID {id} not found.");
            }

            taskObjective.IsCompleted = isCompleted;
            taskObjective.CompletedDate = isCompleted ? DateTime.UtcNow : null;
            taskObjective.CompletedByUserId = isCompleted ? userId : null;

            _unitOfWork.GetRepository<TaskObjective>().Update(taskObjective);
            await _unitOfWork.CommitAsync();
            return taskObjective;
        }
    }
}