using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tasco.TaskService.Repository.Entities;
using Tasco.TaskService.Repository.Paginate;
using Tasco.TaskService.Service.BusinessModels;

namespace Tasco.TaskService.Service.Interfaces
{
    public interface IProjectService
    {
		Task<ProjectBusinessModel> GetProjectById(Guid projectId);
		Task<IPaginate<ProjectBusinessModel>> GetAllProjects(int pageSize, int pageIndex, string search);
		Task<ProjectBusinessModel> CreateProject(ProjectBusinessModel request);
		Task<ProjectBusinessModel> UpdateProject(ProjectBusinessModel request);
    }
}
