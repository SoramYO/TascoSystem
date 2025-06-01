using AutoMapper;
using Tasco.TaskService.Repository.Entities;
using Tasco.TaskService.Service.BusinessModels;
using Tasco.TaskService.Repository.Paginate;

namespace Tasco.TaskService.Service.Mapping
{
	public class ProjectMappingProfile : Profile
	{
		public ProjectMappingProfile()
		{
			// Supporting entities
			CreateMap<ProjectMember, ProjectMemberBusinessModel>().ReverseMap();
			CreateMap<WorkArea, WorkAreaBusinessModel>().ReverseMap();

			// Paginated results
			CreateMap<IPaginate<Project>, IPaginate<ProjectBusinessModel>>()
				.ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.Items));

		}
	}
}
