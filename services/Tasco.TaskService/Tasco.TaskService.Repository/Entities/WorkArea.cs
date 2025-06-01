using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tasco.TaskService.Repository.Entities
{
	public class WorkArea
	{
		[Key]
		public Guid Id { get; set; }

		public Guid ProjectId { get; set; }

		[Required]
		[StringLength(200)]
		public string Name { get; set; }

		[StringLength(1000)]
		public string Description { get; set; }

		public int DisplayOrder { get; set; } = 0;

		public DateTime CreatedDate { get; set; } = DateTime.Now;

		[Required]
		public Guid CreatedByUserId { get; set; }

		public bool IsActive { get; set; } = true;

		// Foreign key
		[ForeignKey("ProjectId")]
		public virtual Project Project { get; set; }

		// Navigation properties
		public virtual ICollection<WorkTask> WorkTasks { get; set; } = new List<WorkTask>();
	}
}
