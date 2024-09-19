using Application.Services;
using Domain.Models;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers {
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase {
        private readonly UserService _userService;
        public UserController(UserService userService)
        {
            _userService = userService;
        }

        [HttpPost]
        public ActionResult Create([FromBody] User user) {
            _userService.Insert(user);
            return Created();
        }

        [HttpGet("verify-email")]
        public OkResult VerifyEmail([FromQuery] string verificationToken) {
            _userService.VerifyEmail(verificationToken);
            return Ok();
        }
    }
}
