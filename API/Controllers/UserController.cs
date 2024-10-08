using API.Hubs;
using Application.DTOs;
using Application.Services;
using Domain.Models;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers {
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase {
        private readonly UserService _userService;
        private readonly EmailNotificationHub _emailNotificationHub;
        public UserController(UserService userService, EmailNotificationHub emailNotificationHub)
        {
            _emailNotificationHub = emailNotificationHub;
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
        public ActionResult Create([FromBody] User user) {
            _userService.Insert(user);
            return Created();
        }

        [HttpGet("verify-email")]
        public async Task<OkResult> VerifyEmail([FromQuery] string verificationToken) {
            var id = _userService.VerifyEmail(verificationToken);
            //await _emailNotificationHub.NotifyRegistrationVerification(id.ToString());
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
        public async Task<OkResult> ConfirmPasswordChange([FromQuery] string verificationToken) {
            var id = _userService.ConfirmPasswordChange(verificationToken);
            //await _emailNotificationHub.NotifyPasswordReset(id.ToString());
            return Ok();
        }

        [HttpPatch("change-password")]
        public NoContentResult ChangePassword([FromBody] ChangePasswordDTO changePassword) {
            _userService.ChangePassword(changePassword);
            return NoContent();
        }

    }
}
