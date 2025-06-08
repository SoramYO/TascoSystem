using System;
using System.Collections.Generic;

namespace Tasco.TaskService.Service.BusinessModels
{
    public class TaskObjectiveBusinessModel
    {
        public Guid Id { get; set; }
        public Guid WorkTaskId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public bool IsCompleted { get; set; }
        public DateTime? CompletedDate { get; set; }
        public Guid? CompletedByUserId { get; set; }
        public string CompletedByUserName { get; set; }
        public int DisplayOrder { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public Guid CreatedByUserId { get; set; }
        public string CreatedByUserName { get; set; }
    }
} 