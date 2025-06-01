using Microsoft.AspNetCore.Http;
using Tasco.TaskService.Service.Implementations;
using Tasco.TaskService.Service.Interfaces;
using Grpc.Core;

namespace Tasco.TaskService.API.GrpcServices
{
	public class GrpcProjectService : ProjectService.ProjectServiceBase
	{
		private readonly IProjectService _projectService;

		public GrpcProjectService(IProjectService projectService)
		{
			_projectService = projectService;
		}

		public override async Task<ProjectResponse> GetProjectById(ProjectRequest request, ServerCallContext context)
		{
			var project = await _projectService.GetProjectById(Guid.Parse(request.Id));
			if (project == null)
				throw new RpcException(new Status(StatusCode.NotFound, "Project not found"));

			return new ProjectResponse
			{
				Id = project.Id.ToString(),
				Name = project.Name,
				Description = project.Description
				// Map các trường khác nếu cần
			};
		}

		public override async Task<ProjectListResponse> GetAllProjects(ProjectListRequest request, ServerCallContext context)
		{
			var result = await _projectService.GetAllProjects(request.PageSize, request.PageIndex, request.Search);
			var response = new ProjectListResponse();
			foreach (var project in result.Items)
			{
				response.Projects.Add(new ProjectResponse
				{
					Id = project.Id.ToString(),
					Name = project.Name,
					Description = project.Description
					// Map các trường khác nếu cần
				});
			}
			return response;
		}
	}
}
