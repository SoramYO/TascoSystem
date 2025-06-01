using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tasco.TaskService.Service.BusinessModels
{
	public class ProjectMemberBusinessModel
	{
		public Guid Id { get; set; }
		public Guid ProjectId { get; set; }
		public Guid UserId { get; set; }
		public string UserName { get; set; }
		public string Role { get; set; }
		public DateTime JoinedDate { get; set; }
		public bool IsActive { get; set; }
	}
}
