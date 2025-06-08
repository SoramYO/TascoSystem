using System;

namespace Tasco.TaskService.API.Models
{
    public class TaskActionRequest
    {
        public string WorkTaskId { get; set; }
        public string ActionType { get; set; }
        public string Description { get; set; }
        public string OldValue { get; set; }
        public string NewValue { get; set; }
    }

    public class TaskActionRequestById
    {
        public string Id { get; set; }
    }

    public class TaskActionListRequest
    {
        public string WorkTaskId { get; set; }
        public int PageSize { get; set; }
        public int PageIndex { get; set; }
    }

    public class TaskActionResponse
    {
        public string Id { get; set; }
        public string WorkTaskId { get; set; }
        public string ActionType { get; set; }
        public string Description { get; set; }
        public string OldValue { get; set; }
        public string NewValue { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string ActionDate { get; set; }
    }

    public class TaskActionListResponse
    {
        public int TotalCount { get; set; }
        public int PageCount { get; set; }
        public int CurrentPage { get; set; }
        public List<TaskActionResponse> TaskActions { get; set; } = new List<TaskActionResponse>();
    }
} 