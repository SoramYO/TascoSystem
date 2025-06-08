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
            var userId = GetUserIdFromJwt();
            var userEmail = GetUserEmailFromJwt();
            if (userId == null)
            {
                throw new UnauthorizedAccessException("User is not authenticated.");
            }
            entity.UserId = Guid.Parse(userId);
            entity.UserName = userEmail;
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

        public async Task<IPaginate<TaskAction>> GetTaskActionsByTaskId(Guid taskId, int pageSize = 10, int pageIndex = 1)
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
