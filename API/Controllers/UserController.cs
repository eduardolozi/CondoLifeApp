using Application.DTOs;
using Application.Services;
using Domain.Models;
using Microsoft.AspNetCore.Http.HttpResults;
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

        [HttpGet]
        public OkObjectResult GetAll() {
            var users = _userService.GetAll();
            return Ok(users);
        }
        
        [HttpGet("{id}")]
        public OkObjectResult GetById([FromRoute] int id) {
            var user = _userService.GetById(id);
            return Ok(user);
        }

        [HttpPost]
        public async Task<ActionResult> Create([FromBody] User user) {
            await _userService.Insert(user);
            return Created();
        }

        [HttpGet("verify-email")]
        public OkResult VerifyEmail([FromQuery] string verificationToken) {
            _userService.VerifyEmail(verificationToken);
            return Ok();
        }

        [HttpPatch("{id}")]
        public NoContentResult Update([FromRoute] int id, [FromBody] User user) { 
            _userService.Update(id, user);
            return NoContent();
        }
        
        [HttpPatch("recovery-password-email")]
        public async Task<NoContentResult> SendRecoveryPasswordEmail([FromBody] ChangePasswordDTO changePassword) {
            await _userService.SendRecoveryPasswordEmail(changePassword);
            return NoContent();
        }

        [HttpGet("confirm-password-change")]
        public OkResult ConfirmPasswordChange([FromQuery] string verificationToken) {
            _userService.ConfirmPasswordChange(verificationToken);
            return Ok();
        }

        [HttpPatch("change-password")]
        public NoContentResult ChangePassword([FromBody] ChangePasswordDTO changePassword) {
            _userService.ChangePassword(changePassword);
            return NoContent();
        }

    }
}
