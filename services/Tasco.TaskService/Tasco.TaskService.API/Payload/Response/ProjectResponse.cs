namespace Tasco.TaskService.API.Payload.Response
{
	public class ProjectResponse
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
