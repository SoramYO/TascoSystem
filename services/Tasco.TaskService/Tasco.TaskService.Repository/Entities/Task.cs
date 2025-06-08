using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tasco.TaskService.Repository.Entities
{
	public class Task
	{
		[Key]
		public Guid Id { get; set; }
		public Guid ProjectId { get; set; }
		public string Title { get; set; }
		public string Description { get; set; }
		public Guid AssigneeId { get; set; }
		public string AssigneeName { get; set; }
		public Guid ReporterId { get; set; }
		public string ReporterName { get; set; }
		public string Status { get; set; }
		public DateTime? DueDate { get; set; }
		public DateTime CreatedAt { get; set; }
		public DateTime UpdatedAt { get; set; }
		public virtual ICollection<SubTask> SubTasks { get; set; } = new List<SubTask>();
		public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();
	}
}
