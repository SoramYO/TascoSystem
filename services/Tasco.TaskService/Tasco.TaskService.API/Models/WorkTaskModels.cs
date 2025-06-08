using System;

namespace Tasco.TaskService.API.Models
{
    public class WorkTaskRequest
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string WorkAreaId { get; set; }
        public string Status { get; set; }
        public string Priority { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string DueDate { get; set; }
        public int Progress { get; set; }
    }

    public class WorkTaskRequestById
    {
        public string Id { get; set; }
    }

    public class WorkTaskListRequest
    {
        public int PageSize { get; set; }
        public int PageIndex { get; set; }
        public string Search { get; set; }
    }

    public class UpdateWorkTaskRequest
    {
        public string Id { get; set; }
        public WorkTaskRequest Task { get; set; }
    }

    public class WorkTaskResponse
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string WorkAreaId { get; set; }
        public string Status { get; set; }
        public string Priority { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string DueDate { get; set; }
        public int Progress { get; set; }
        public string CreatedByUserId { get; set; }
        public string CreatedByUserName { get; set; }
        public string CreatedDate { get; set; }
        public string ModifiedDate { get; set; }
    }

    public class WorkTaskListResponse
    {
        public int TotalCount { get; set; }
        public int PageCount { get; set; }
        public int CurrentPage { get; set; }
        public List<WorkTaskResponse> WorkTasks { get; set; } = new List<WorkTaskResponse>();
    }
} 