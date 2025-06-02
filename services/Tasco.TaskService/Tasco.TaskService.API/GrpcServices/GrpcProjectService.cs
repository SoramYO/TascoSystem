using Microsoft.AspNetCore.Http;
using Tasco.TaskService.Service.Implementations;
using Tasco.TaskService.Service.Interfaces;
using Grpc.Core;
using AutoMapper;

namespace Tasco.TaskService.API.GrpcServices
{
	public class GrpcProjectService : ProjectService.ProjectServiceBase
	{
		private readonly IProjectService _projectService;
		private readonly IMapper _mapper;

		public GrpcProjectService(IProjectService projectService, IMapper mapper)
		{
			_projectService = projectService;
			_mapper = mapper;
		}

		public override async Task<ProjectResponseGRPC> GetProjectById(ProjectRequestById request, ServerCallContext context)
		{
			var project = await _projectService.GetProjectById(Guid.Parse(request.Id));
			if (project == null)
			{
				throw new RpcException(new Status(StatusCode.NotFound, $"Project with ID {request.Id} not found."));
			}
			return _mapper.Map<ProjectResponseGRPC>(project);
		}

		public override async Task<ProjectListResponse> GetAllProjects(ProjectListRequest request, ServerCallContext context)
		{
			var result = await _projectService.GetAllProjects(request.PageSize, request.PageIndex, request.Search);
			var response = new ProjectListResponse();
			foreach (var project in result.Items)
			{
				response.Projects.Add(
					_mapper.Map<ProjectResponseGRPC>(project)
				);
			};
			return response;
		}
	}
}
