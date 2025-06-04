using Microsoft.AspNetCore.Mvc;
using Tasco.UserAuthService.API.Models.ResponseModels;

namespace Tasco.UserAuthService.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class HealthController : ControllerBase
    {
        /// <summary>
        /// Health check endpoint
        /// </summary>
        /// <returns>API health status</returns>
        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<object>), 200)]
        public IActionResult GetHealth()
        {
            var response = new
            {
                Status = "Healthy",
                Timestamp = DateTime.UtcNow,
                Service = "Tasco User Authentication Service"
            };

            return Ok(ApiResponse<object>.SuccessResponse(response, "Service is healthy"));
        }
    }
} 