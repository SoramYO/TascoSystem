using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tasco.TaskService.Repository.Entities
{
	public class SubTask
	{
		[Key]
		public Guid Id { get; set; }
		public Guid ParentTaskId { get; set; }
		public string Title { get; set; }
		public string Description { get; set; }
		public Guid AssigneeId { get; set; }
		public string AssigneeName { get; set; }
		public string Status { get; set; }
		[ForeignKey("ParentTaskId")]
		public virtual Task Task { get; set; }

	}
}
