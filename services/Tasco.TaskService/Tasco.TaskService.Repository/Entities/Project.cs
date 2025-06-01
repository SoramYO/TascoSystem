using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tasco.TaskService.Repository.Entities
{
	public class Project
	{
		[Key]
		public Guid Id { get; set; }

		[Required]
		[StringLength(200)]
		public string Name { get; set; }

		[StringLength(1000)]
		public string Description { get; set; }

		public DateTime CreatedDate { get; set; } = DateTime.Now;

		public DateTime? StartDate { get; set; }

		public DateTime? EndDate { get; set; }

		[StringLength(50)]
		public string Status { get; set; } = "Active";

		[Required]
		public Guid CreatedByUserId { get; set; }

		[StringLength(200)]
		public string CreatedByUserName { get; set; }

		public virtual ICollection<ProjectMember> ProjectMembers { get; set; } = new List<ProjectMember>();
		public virtual ICollection<WorkArea> WorkAreas { get; set; } = new List<WorkArea>();
	}
}
