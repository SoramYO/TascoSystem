using AutoMapper;
using Microsoft.AspNetCore.Http;
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
    public class TaskActionService : BaseService<TaskActionService>, ITaskActionService
    {
        public TaskActionService(
            IUnitOfWork<TaskManagementDbContext> unitOfWork,
            ILogger<TaskActionService> logger,
            IMapper mapper,
            IHttpContextAccessor httpContextAccessor
        ) : base(unitOfWork, logger, mapper, httpContextAccessor)
        {
        }

        public async Task CreateTaskAction(TaskActionBusinessModel taskAction)
        {
            var entity = _mapper.Map<TaskAction>(taskAction);
            
            // Use user information from the business model instead of JWT
            // Since authentication is disabled, we'll use default values or values from the model
            if (entity.UserId == Guid.Empty)
            {
                entity.UserId = Guid.NewGuid(); // Use a default user ID
            }
            if (string.IsNullOrEmpty(entity.UserName))
            {
                entity.UserName = "System User"; // Use a default user name
            }
            
            await _unitOfWork.GetRepository<TaskAction>().InsertAsync(entity);
            await _unitOfWork.CommitAsync();
        }

        public async Task<TaskAction> GetTaskActionById(Guid id)
        {
            var taskAction = await _unitOfWork.GetRepository<TaskAction>()
                .SingleOrDefaultAsync(predicate: t => t.Id == id);

            if (taskAction == null)
            {
                throw new KeyNotFoundException($"Task action with ID {id} not found.");
            }

            return taskAction;
        }

        public async Task<IPaginate<TaskAction>> GetTaskActionsByTaskId(Guid taskId, int pageSize = 10,
            int pageIndex = 1)
        {
            var taskActions = await _unitOfWork.GetRepository<TaskAction>().GetPagingListAsync(
                predicate: t => t.WorkTaskId == taskId,
                orderBy: q => q.OrderByDescending(t => t.ActionDate),
                page: pageIndex,
                size: pageSize
            );
            return taskActions;
        }
    }
}