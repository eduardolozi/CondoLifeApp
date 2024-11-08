using Application.DTOs;
using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers {
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController(IAuthService authenticationService) : ControllerBase
    {
        [HttpPost("login")]
        public OkObjectResult Login(UserLoginDTO userLogin) {
            var loginResult = authenticationService.Login(userLogin);
            return Ok(loginResult);
        }

        [HttpPost("refresh-login")]
        public ActionResult RefreshLogin([FromBody] RefreshRequestDTO refreshRequest) {
            try {
                var refreshResult = authenticationService.RefreshAccessToken(refreshRequest);
                return Ok(refreshResult);
            }
            catch (Exception ex) {
                return BadRequest(ex.Message);
            }
        }
    }
}
