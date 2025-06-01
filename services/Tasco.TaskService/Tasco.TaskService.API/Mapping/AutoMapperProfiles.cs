using AutoMapper;
using Tasco.TaskService.API.Payload.Request;
using Tasco.TaskService.Repository.Entities;

namespace Tasco.TaskService.API.Mapping
{
	public class AutoMapperProfiles : Profile
	{
		public AutoMapperProfiles()
		{
			CreateMap<CreateProjectRequest, Project>()
				.ForMember(dest => dest.Id, opt => opt.Ignore())
				.ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(src => DateTime.Now))
				.ForMember(dest => dest.Status, opt => opt.MapFrom(src => "Active"));

			CreateMap<UpdateProjectRequest, Project>()
				.ForMember(dest => dest.CreatedDate, opt => opt.Ignore())
				.ForMember(dest => dest.CreatedByUserId, opt => opt.Ignore())
				.ForMember(dest => dest.CreatedByUserName, opt => opt.Ignore());
		}
	}
}
