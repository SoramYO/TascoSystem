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
    public class WorkTaskService : BaseService<WorkTaskService>, IWorkTaskService
    {
        private readonly ITaskActionService _taskActionService;

        public WorkTaskService(
            IUnitOfWork<TaskManagementDbContext> unitOfWork,
            ILogger<WorkTaskService> logger, IMapper mapper,
            IHttpContextAccessor httpContextAccessor,
            ITaskActionService taskActionService
        ) : base(unitOfWork, logger, mapper, httpContextAccessor)
        {
            _taskActionService = taskActionService;
        }

        public async Task<WorkTask> CreateWorkTask(WorkTaskBusinessModel workTask)
        {
            var entity = _mapper.Map<WorkTask>(workTask);
            

            // Set creation info
            entity.CreatedByUserId = workTask.CreatedByUserId;
            entity.CreatedByUserName = workTask.CreatedByUserName;
            entity.WorkAreaId = workTask.WorkAreaId;
            entity.CreatedDate = DateTime.UtcNow;
            entity.Progress = 0;

            entity.TaskMembers.Add(new TaskMember
            {
                UserId =  workTask.CreatedByUserId,
                UserName = workTask.CreatedByUserName,
                Role = "Assignee",
                AssignedByUserId =  workTask.CreatedByUserId,
                AssignedDate = DateTime.UtcNow,
                IsActive = true
            });

            await _unitOfWork.GetRepository<WorkTask>().InsertAsync(entity);
            await _unitOfWork.CommitAsync();

            // Log task creation
            await _taskActionService.CreateTaskAction(new TaskActionBusinessModel
            {
                WorkTaskId = entity.Id,
                ActionType = "Created",
                Description = $"Task '{entity.Title}' created",
                ActionDate = DateTime.UtcNow
            });

            return entity;
        }

        public async Task DeleteWorkTask(Guid id)
        {
            var task = await _unitOfWork.GetRepository<WorkTask>()
                .SingleOrDefaultAsync(predicate: t => t.Id == id && !t.IsDeleted);

            if (task == null)
            {
                throw new KeyNotFoundException($"Work task with ID {id} not found.");
            }

            task.IsDeleted = true;
            _unitOfWork.GetRepository<WorkTask>().Update(task);
            await _unitOfWork.CommitAsync();
        }

        public async Task<IPaginate<WorkTask>> GetAllWorkTasks(int pageSize, int pageIndex, string search = null)
        {
            var workTasks = await _unitOfWork.GetRepository<WorkTask>().GetPagingListAsync
            (predicate: t => !t.IsDeleted && (string.IsNullOrEmpty(search) ||
                                              t.Title.Contains(search) ||
                                              t.Description.Contains(search)),
                orderBy: q => q.OrderByDescending(t => t.CreatedDate),
                page: pageIndex,
                size: pageSize);
            return workTasks;
        }

        public async Task<IPaginate<WorkTask>> GetMyWorkTasks(int pageSize, int pageIndex, string search = null)
        {
            // Since authentication is disabled, return all tasks or implement alternative logic
            var query = await _unitOfWork.GetRepository<WorkTask>().GetPagingListAsync(
                predicate: t => !t.IsDeleted,
                orderBy: q => q.OrderByDescending(t => t.CreatedDate),
                page: pageIndex,
                size: pageSize);

            return query;
        }

        public async Task<WorkTask> GetWorkTaskById(Guid id)
        {
            var task = await _unitOfWork.GetRepository<WorkTask>()
                .SingleOrDefaultAsync(
                    predicate: t => t.Id == id && !t.IsDeleted,
                    include: t => t.Include(x => x.TaskMembers)
                        .Include(x => x.TaskObjectives)
                        .Include(x => x.TaskActions)
                        .Include(x => x.WorkArea));

            if (task == null)
            {
                throw new KeyNotFoundException($"Work task with ID {id} not found.");
            }

            return task;
        }

        public async Task UpdateWorkTask(Guid id, WorkTaskBusinessModel workTask)
        {
            if (workTask == null)
            {
                throw new ArgumentNullException(nameof(workTask), "WorkTask data cannot be null");
            }

            var existingTask = await _unitOfWork.GetRepository<WorkTask>()
                .SingleOrDefaultAsync(
                    predicate: t => t.Id == id,
                    include: t => t.Include(x => x.TaskMembers)
                        .Include(x => x.TaskObjectives));

            if (existingTask == null)
            {
                throw new KeyNotFoundException($"Work task with ID {id} not found.");
            }

            // Use user information from the request instead of JWT
            var userId = workTask.CreatedByUserId.ToString();
            var userEmail = workTask.CreatedByUserName;

            // Track changes for logging
            var changes = new List<string>();
            var oldValues = new List<string>();
            var newValues = new List<string>();

            if (existingTask.Title != workTask.Title)
            {
                changes.Add("Title");
                oldValues.Add(existingTask.Title ?? "");
                newValues.Add(workTask.Title ?? "");
            }

            if (existingTask.Description != workTask.Description)
            {
                changes.Add("Description");
                oldValues.Add(existingTask.Description ?? "");
                newValues.Add(workTask.Description ?? "");
            }

            if (existingTask.Status != workTask.Status)
            {
                changes.Add("Status");
                oldValues.Add(existingTask.Status ?? "");
                newValues.Add(workTask.Status ?? "");
            }

            if (existingTask.Priority != workTask.Priority)
            {
                changes.Add("Priority");
                oldValues.Add(existingTask.Priority ?? "");
                newValues.Add(workTask.Priority ?? "");
            }

            if (existingTask.StartDate != workTask.StartDate)
            {
                changes.Add("StartDate");
                oldValues.Add(existingTask.StartDate?.ToString("yyyy-MM-dd") ?? "");
                newValues.Add(workTask.StartDate?.ToString("yyyy-MM-dd") ?? "");
            }

            if (existingTask.EndDate != workTask.EndDate)
            {
                changes.Add("EndDate");
                oldValues.Add(existingTask.EndDate?.ToString("yyyy-MM-dd") ?? "");
                newValues.Add(workTask.EndDate?.ToString("yyyy-MM-dd") ?? "");
            }

            if (existingTask.DueDate != workTask.DueDate)
            {
                changes.Add("DueDate");
                oldValues.Add(existingTask.DueDate?.ToString("yyyy-MM-dd") ?? "");
                newValues.Add(workTask.DueDate?.ToString("yyyy-MM-dd") ?? "");
            }

            if (existingTask.Progress != workTask.Progress)
            {
                changes.Add("Progress");
                oldValues.Add(existingTask.Progress.ToString());
                newValues.Add(workTask.Progress.ToString());
            }

            // Preserve original creation data
            var createdByUserId = existingTask.CreatedByUserId;
            var createdByUserName = existingTask.CreatedByUserName;
            var createdDate = existingTask.CreatedDate;

            // Map new data
            _mapper.Map(workTask, existingTask);

            // Restore original creation data
            existingTask.CreatedByUserId = createdByUserId;
            existingTask.CreatedByUserName = createdByUserName;
            existingTask.CreatedDate = createdDate;

            // Update the task
            _unitOfWork.GetRepository<WorkTask>().Update(existingTask);
            await _unitOfWork.CommitAsync();

            // Log changes if any
            if (changes.Any())
            {
                for (int i = 0; i < changes.Count; i++)
                {
                    await _taskActionService.CreateTaskAction(new TaskActionBusinessModel
                    {
                        WorkTaskId = id,
                        ActionType = "Updated",
                        Description = $"Task '{existingTask.Title}' {changes[i].ToLower()} updated",
                        OldValue = oldValues[i],
                        NewValue = newValues[i],
                        ActionDate = DateTime.UtcNow
                    });
                }
            }
        }
    }
}