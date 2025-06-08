using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tasco.TaskService.Repository.Entities;
using Tasco.TaskService.Repository.Paginate;
using Tasco.TaskService.Service.BusinessModels;

namespace Tasco.TaskService.Service.Interfaces
{
    public interface IWorkAreaService
    {
        Task<WorkArea> GetWorkAreaById(Guid id);
        Task<IPaginate<WorkArea>> GetAllWorkAreas(int pageSize, int pageIndex, string search = null);
        Task<IPaginate<WorkArea>> GetMyWorkAreas(int pageSize, int pageIndex, string search = null);
        Task<WorkArea> CreateWorkArea(WorkAreaBusinessModel workArea);
        Task UpdateWorkArea(Guid id, WorkAreaBusinessModel workArea);
        Task DeleteWorkArea(Guid id);
    }
}
