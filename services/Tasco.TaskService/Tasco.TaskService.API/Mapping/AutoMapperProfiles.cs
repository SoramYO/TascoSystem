using AutoMapper;
using Tasco.TaskService.API.Models;
using Tasco.TaskService.API.Payload.Request;
using Tasco.TaskService.API.Protos;
using Tasco.TaskService.Repository.Entities;
using Tasco.TaskService.Repository.Paginate;
using Tasco.TaskService.Service.BusinessModels;

namespace Tasco.TaskService.API.Mapping
{
	public class AutoMapperProfiles : Profile
	{
		public AutoMapperProfiles()
		{
			// WorkArea mappings
			CreateMap<Payload.Request.WorkAreaRequest, WorkAreaBusinessModel>()
				.ForMember(dest => dest.ProjectId, opt => opt.MapFrom(src => src.ProjectId))
				.ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
				.ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
				.ForMember(dest => dest.DisplayOrder, opt => opt.MapFrom(src => src.DisplayOrder))
				.ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(src => DateTime.Now))
				.ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => true));

			CreateMap<WorkAreaBusinessModel, WorkArea>()
				.ForMember(dest => dest.ProjectId, opt => opt.MapFrom(src => src.ProjectId))
				.ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
				.ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
				.ForMember(dest => dest.DisplayOrder, opt => opt.MapFrom(src => src.DisplayOrder))
				.ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(src => DateTime.Now))
				.ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => true));

			CreateMap<WorkArea, WorkAreaResponse>();

			// WorkTask mappings
			CreateMap<Protos.WorkTaskRequest, WorkTaskBusinessModel>()
				.ForMember(dest => dest.WorkAreaId, opt => opt.MapFrom(src => Guid.Parse(src.WorkAreaId)))
				.ForMember(dest => dest.StartDate, opt => opt.MapFrom(src => DateTime.Parse(src.StartDate)))
				.ForMember(dest => dest.EndDate, opt => opt.MapFrom(src => DateTime.Parse(src.EndDate)))
				.ForMember(dest => dest.DueDate, opt => opt.MapFrom(src => DateTime.Parse(src.DueDate)));

			CreateMap<WorkTaskBusinessModel, WorkTask>()
				.ForMember(dest => dest.WorkAreaId, opt => opt.MapFrom(src => src.WorkAreaId))
				.ForMember(dest => dest.StartDate, opt => opt.MapFrom(src => src.StartDate))
				.ForMember(dest => dest.EndDate, opt => opt.MapFrom(src => src.EndDate))
				.ForMember(dest => dest.DueDate, opt => opt.MapFrom(src => src.DueDate));

			CreateMap<WorkTask, Protos.WorkTaskResponse>();

			// TaskAction mappings
			CreateMap<Protos.TaskActionRequest, TaskActionBusinessModel>()
				.ForMember(dest => dest.WorkTaskId, opt => opt.MapFrom(src => Guid.Parse(src.WorkTaskId)));

			CreateMap<TaskActionBusinessModel, TaskAction>()
				.ForMember(dest => dest.WorkTaskId, opt => opt.MapFrom(src => src.WorkTaskId));

			CreateMap<TaskAction, Protos.TaskActionResponse>();

			// TaskObjective mappings
			CreateMap<TaskObjectiveBusinessModel, TaskObjective>();
			CreateMap<TaskObjective, TaskObjectiveBusinessModel>();

			// TaskMember mappings
			CreateMap<TaskMemberBusinessModel, TaskMember>();
			CreateMap<TaskMember, TaskMemberBusinessModel>();
		}
	}
}
