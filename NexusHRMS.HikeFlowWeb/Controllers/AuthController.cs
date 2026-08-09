using HRMS_Common;
using HRMS_HikeFlow.NexusBAL;
using HRMS_HikeFlow.NexusProperties;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NexusHRMS.HikeFlowWeb.Services;

namespace NexusHRMS.HikeFlowWeb.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AuthBAL _authBAL;
        private readonly ITokenGeneratorService _tokenGeneratorService;

        public AuthController(AuthBAL authBAL, ITokenGeneratorService tokenGeneratorService)
        {
            _authBAL = authBAL;
            _tokenGeneratorService = tokenGeneratorService;
        }


        /// <summary>
        /// Authenticates user credentials and generates a JWT token containing user details upon successful login.
        /// </summary>
        /// <param name="request">Payload containing the user's login credentials.</param>
        /// <returns>An IActionResult containing the authentication result and JWT payload, or an error status code on failure.</returns>
        [HttpPost("login")]
        [AllowAnonymous] // Allows unauthenticated users to access login
        public IActionResult Login([FromBody] LoginRequestDto request)
        {
            // 1. Authenticate user credentials and retrieve user model via AuthBAL
            var authResponse = _authBAL.AuthenticateUser(request);

            if (authResponse == null || authResponse.ReturnCode != 0 || authResponse.Data == null)
            {
                return Unauthorized(new { message = authResponse?.ReturnMessage ?? "Invalid username or password" });
            }

            // 2. Generate JWT Token carrying DesignationId & HierarchyLevel claims
            string token = _tokenGeneratorService.GenerateToken(authResponse.Data);

            return Ok(new
            {
                token,
                returnCode = authResponse.ReturnCode,
                returnMessage = authResponse.ReturnMessage
            });
        }
    }
}