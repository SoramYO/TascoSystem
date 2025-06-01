using System;
using System.Collections.Generic;

namespace Tasco.TaskService.Service.BusinessModels
{
	public class ProjectBusinessModel
	{
		public Guid Id { get; set; }
		public string Name { get; set; }
		public string Description { get; set; }
		public DateTime CreatedDate { get; set; }
		public DateTime? StartDate { get; set; }
		public DateTime? EndDate { get; set; }
		public string Status { get; set; }
		public Guid CreatedByUserId { get; set; }
		public string CreatedByUserName { get; set; }
	}
}
