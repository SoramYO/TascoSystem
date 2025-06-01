using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tasco.TaskService.Service.BusinessModels
{
	public class WorkAreaBusinessModel
	{
		public Guid Id { get; set; }
		public string Name { get; set; }
		public string Description { get; set; }
		public Guid ProjectId { get; set; }
		public DateTime CreatedDate { get; set; }
		public bool IsActive { get; set; }
		public int TaskCount { get; set; }
	}
}
