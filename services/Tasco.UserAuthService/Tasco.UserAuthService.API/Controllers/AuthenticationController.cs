using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using Tasco.UserAuthService.Service.Services.Interface;
using Tasco.UserAuthService.Service.BusinessModels;
using Tasco.UserAuthService.API.Models.RequestModels;
using Tasco.UserAuthService.API.Models.ResponseModels;

namespace Tasco.UserAuthService.API.Controllers
{
    [ApiController]
    [Route("api/authentications")]
    [Produces("application/json")]
    public class AuthenticationController : ControllerBase
    {
        private readonly IAuthenticationService _authenticationService;
        private readonly IMapper _mapper;
        private readonly ILogger<AuthenticationController> _logger;

        public AuthenticationController(
            IAuthenticationService authenticationService,
            IMapper mapper,
            ILogger<AuthenticationController> logger)
        {
            _authenticationService = authenticationService;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// User login endpoint
        /// </summary>
        /// <param name="request">Login credentials</param>
        /// <returns>Login response with user information and access token</returns>
        [HttpPost("login")]
        [ProducesResponseType(typeof(ApiResponse<LoginResponse>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 400)]
        [ProducesResponseType(typeof(ApiResponse<object>), 500)]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();

                return BadRequest(ApiResponse<object>.ErrorResponse("Validation failed", errors));
            }

            _logger.LogInformation("Login attempt for email: {Email}", request.Email);

            var loginBusiness = _mapper.Map<LoginAccountBusiness>(request);
            var account = await _authenticationService.LoginAsync(loginBusiness);

            var response = _mapper.Map<LoginResponse>(account);
            
            _logger.LogInformation("User logged in successfully: {UserId}", account.UserId);
            
            return Ok(ApiResponse<LoginResponse>.SuccessResponse(response, "Login successful"));
        }

        /// <summary>
        /// User registration endpoint
        /// </summary>
        /// <param name="request">Registration information</param>
        /// <returns>Registration confirmation</returns>
        [HttpPost("register")]
        [ProducesResponseType(typeof(ApiResponse<RegisterResponse>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 400)]
        [ProducesResponseType(typeof(ApiResponse<object>), 422)]
        [ProducesResponseType(typeof(ApiResponse<object>), 500)]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();

                return BadRequest(ApiResponse<object>.ErrorResponse("Validation failed", errors));
            }

            _logger.LogInformation("Registration attempt for email: {Email}", request.Email);

            var registerBusiness = _mapper.Map<RegisterAccountBusiness>(request);
            var baseUrl = $"{Request.Scheme}://{Request.Host}/api/authentication/confirm-email";
            
            await _authenticationService.RegisterAsync(baseUrl, registerBusiness);

            var response = new RegisterResponse();
            
            _logger.LogInformation("User registered successfully: {Email}", request.Email);
            
            return Ok(ApiResponse<RegisterResponse>.SuccessResponse(response, "Registration successful"));
        }

        /// <summary>
        /// Email confirmation endpoint
        /// </summary>
        /// <param name="userId">User ID from email confirmation link</param>
        /// <param name="token">Confirmation token from email</param>
        /// <returns>Email confirmation result</returns>
        [HttpGet("confirm-email")]
        [ProducesResponseType(typeof(ApiResponse<ConfirmEmailResponse>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 400)]
        [ProducesResponseType(typeof(ApiResponse<object>), 422)]
        [ProducesResponseType(typeof(ApiResponse<object>), 500)]
        public async Task<IActionResult> ConfirmEmail([FromQuery] string userId, [FromQuery] string token)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(token))
            {
                return BadRequest(ApiResponse<object>.ErrorResponse("User ID and token are required"));
            }

            _logger.LogInformation("Email confirmation attempt for user: {UserId}", userId);

            await _authenticationService.ConfirmEmailAsync(userId, token);

            var response = new ConfirmEmailResponse();
            
            _logger.LogInformation("Email confirmed successfully for user: {UserId}", userId);
            
            return Ok(ApiResponse<ConfirmEmailResponse>.SuccessResponse(response, "Email confirmed successfully"));
        }

        /// <summary>
        /// Alternative POST endpoint for email confirmation (for frontend forms)
        /// </summary>
        /// <param name="request">Confirmation request with userId and token</param>
        /// <returns>Email confirmation result</returns>
        [HttpPost("confirm-email")]
        [ProducesResponseType(typeof(ApiResponse<ConfirmEmailResponse>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 400)]
        [ProducesResponseType(typeof(ApiResponse<object>), 422)]
        [ProducesResponseType(typeof(ApiResponse<object>), 500)]
        public async Task<IActionResult> ConfirmEmailPost([FromBody] ConfirmEmailRequest request)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();

                return BadRequest(ApiResponse<object>.ErrorResponse("Validation failed", errors));
            }

            _logger.LogInformation("Email confirmation attempt for user: {UserId}", request.UserId);

            await _authenticationService.ConfirmEmailAsync(request.UserId, request.Token);

            var response = new ConfirmEmailResponse();
            
            _logger.LogInformation("Email confirmed successfully for user: {UserId}", request.UserId);
            
            return Ok(ApiResponse<ConfirmEmailResponse>.SuccessResponse(response, "Email confirmed successfully"));
        }
    }
} 