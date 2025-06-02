using AutoMapper;
using Tasco.TaskService.API.GrpcServices;
using Tasco.TaskService.API.Payload.Request;
using Tasco.TaskService.Repository.Entities;
using Tasco.TaskService.Repository.Paginate;
using Tasco.TaskService.Service.BusinessModels;

namespace Tasco.TaskService.API.Mapping
{
	public class AutoMapperProfiles : Profile
	{
		public AutoMapperProfiles()
		{
			CreateMap<CreateProjectRequest, ProjectBusinessModel>();

			CreateMap<UpdateProjectRequest, ProjectBusinessModel>();

			CreateMap<ProjectBusinessModel, Project>();
			CreateMap<Project, ProjectBusinessModel>();
			CreateMap<Project, ProjectBusinessModel>();


			CreateMap<ProjectBusinessModel, ProjectResponseGRPC>();

			CreateMap<ProjectMember, ProjectMemberBusinessModel>().ReverseMap();
			CreateMap<WorkArea, WorkAreaBusinessModel>().ReverseMap();

			// Paginated results
			CreateMap<IPaginate<Project>, IPaginate<ProjectBusinessModel>>()
				.ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.Items));

		}
	}
}
