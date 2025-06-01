using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tasco.TaskService.Repository.Entities
{
	public class ProjectMember
	{
		[Key]
		public Guid Id { get; set; }

		public Guid ProjectId { get; set; }

		[Required]
		public Guid UserId { get; set; } // Id từ Identity service

		[StringLength(200)]
		public string UserName { get; set; } // Cache để hiển thị

		[StringLength(200)]
		public string UserEmail { get; set; } // Cache để hiển thị

		[StringLength(50)]
		public string Role { get; set; } = "Member"; // Admin, Manager, Member

		public DateTime JoinedDate { get; set; } = DateTime.Now;

		public bool IsActive { get; set; } = true;

		// Foreign key
		[ForeignKey("ProjectId")]
		public virtual Project Project { get; set; }
	}
}
