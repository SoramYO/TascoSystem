using System.ComponentModel.DataAnnotations;

namespace Tasco.TaskService.API.Payload.Request
{
	public class ProjectRequest
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
	}

	public class CreateProjectRequest
	{
		[Required(ErrorMessage = "Project name is required")]
		[StringLength(200, ErrorMessage = "Project name cannot exceed 200 characters")]
		public string Name { get; set; }

		[StringLength(1000, ErrorMessage = "Description cannot exceed 1000 characters")]
		public string Description { get; set; }

		public DateTime? StartDate { get; set; }

		public DateTime? EndDate { get; set; }

		[Required(ErrorMessage = "Created by user ID is required")]
		public Guid CreatedByUserId { get; set; }

		[Required(ErrorMessage = "Created by user name is required")]
		[StringLength(200, ErrorMessage = "User name cannot exceed 200 characters")]
		public string CreatedByUserName { get; set; }

		// Validation
		public bool IsValid()
		{
			if (StartDate.HasValue && EndDate.HasValue && StartDate > EndDate)
				return false;

			return true;
		}
	}
	public class UpdateProjectRequest
	{
		[Required(ErrorMessage = "Project ID is required")]
		public Guid Id { get; set; }

		[Required(ErrorMessage = "Project name is required")]
		[StringLength(200, ErrorMessage = "Project name cannot exceed 200 characters")]
		public string Name { get; set; }

		[StringLength(1000, ErrorMessage = "Description cannot exceed 1000 characters")]
		public string Description { get; set; }

		public DateTime? StartDate { get; set; }

		public DateTime? EndDate { get; set; }

		[StringLength(50, ErrorMessage = "Status cannot exceed 50 characters")]
		public string Status { get; set; }

		// Validation
		public bool IsValid()
		{
			if (StartDate.HasValue && EndDate.HasValue && StartDate > EndDate)
				return false;

			return true;
		}
	}

	public class ProjectSearchRequest
	{
		public string Search { get; set; }
		public int PageSize { get; set; } = 10;
		public int PageIndex { get; set; } = 1;
		public string Status { get; set; }
		public Guid? CreatedByUserId { get; set; }
		public DateTime? StartDateFrom { get; set; }
		public DateTime? StartDateTo { get; set; }
		public DateTime? EndDateFrom { get; set; }
		public DateTime? EndDateTo { get; set; }
		public string SortBy { get; set; } = "CreatedDate";
		public string SortDirection { get; set; } = "DESC"; // ASC or DESC

		// Validation
		public void Validate()
		{
			if (PageSize <= 0) PageSize = 10;
			if (PageSize > 100) PageSize = 100; // Limit max page size
			if (PageIndex <= 0) PageIndex = 1;
		}
	}
}