using Application.DTOs;
using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers {
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase {
        private readonly IAuthService _authenticationService;
        public AuthController(IAuthService authenticationService)
        {
            _authenticationService = authenticationService;
        }

        [HttpGet("login")]
        public OkObjectResult Login(UserLoginDTO userLogin) {
            var loginResult = _authenticationService.Login(userLogin);
            return Ok(loginResult);
        }

        [HttpGet("refresh-login")]
        public ActionResult RefreshLogin([FromBody] RefreshRequestDTO refreshRequest) {
            try {
                var refreshResult = _authenticationService.RefreshAccessToken(refreshRequest);
                return Ok(refreshResult);
            }
            catch (Exception ex) {
                return BadRequest(ex.Message);
            }
        }
    }
}
