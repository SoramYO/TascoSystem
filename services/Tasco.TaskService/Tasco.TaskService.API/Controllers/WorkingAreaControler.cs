
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Tasco.TaskService.API.Payload.Request;
using Tasco.TaskService.Service.BusinessModels;
using Tasco.TaskService.Service.Interfaces;

namespace Tasco.TaskService.API.Controllers
{
    [Route("api/v1/workareas")]
    [ApiController]
    public class WorkingAreaControler : BaseController<WorkingAreaControler>
    {
        private readonly IWorkAreaService _workAreaService;

        public WorkingAreaControler(ILogger<WorkingAreaControler> logger, IMapper mapper, IWorkAreaService workAreaService) : base(logger, mapper)
        {
            _workAreaService = workAreaService;
        }

        [HttpGet]
        public async Task<IActionResult> GetWorkingAreas(int pageSize = 10, int pageIndex = 1, string search = null)
        {
            var result = await _workAreaService.GetAllWorkAreas(pageSize, pageIndex, search);
            return Ok(result);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetWorkingAreaById(Guid id)
        {
            var result = await _workAreaService.GetWorkAreaById(id);
            if (result == null)
            {
                return NotFound($"Work area with ID {id} not found.");
            }
            return Ok(result);
        }
        [HttpGet("me")]
        public async Task<IActionResult> GetMyWorkingAreas(int pageSize = 10, int pageIndex = 1, string search = null)
        {
            var result = await _workAreaService.GetMyWorkAreas(pageSize, pageIndex, search);
            return Ok(result);
        }
        [HttpPost]
        public async Task<IActionResult> CreateWorkingArea([FromBody] WorkAreaRequest workArea)
        {
            if (workArea == null)
            {
                return BadRequest("Work area data is required.");
            }
            var mappedWorkArea = _mapper.Map<WorkAreaBusinessModel>(workArea);
            var createdWorkArea = await _workAreaService.CreateWorkArea(mappedWorkArea);
            return CreatedAtAction(nameof(GetWorkingAreaById), new { id = createdWorkArea.Id }, createdWorkArea);
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateWorkingArea(Guid id, [FromBody] WorkAreaRequest workArea)
        {
            if (workArea == null)
            {
                return BadRequest("Work area data is required.");
            }
            var mappedWorkArea = _mapper.Map<WorkAreaBusinessModel>(workArea);
            await _workAreaService.UpdateWorkArea(id, mappedWorkArea);
            return NoContent();
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteWorkingArea(Guid id)
        {
            var workArea = await _workAreaService.GetWorkAreaById(id);
            if (workArea == null)
            {
                return NotFound($"Work area with ID {id} not found.");
            }
            await _workAreaService.DeleteWorkArea(id);
            return NoContent();
        }
    }
}
