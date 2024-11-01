using API.Hubs;
using Application.DTOs;
using Application.Interfaces;
using Application.Services;
using Domain.Models;
using Domain.Utils;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers {
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase {
        private readonly UserService _userService;
        private readonly EmailNotificationHub _emailNotificationHub;
        private readonly IAuthService _authService;
        public UserController(UserService userService,
            EmailNotificationHub emailNotificationHub,
            IAuthService authService)
        {
            _emailNotificationHub = emailNotificationHub;
            _userService = userService;
            _authService = authService;
        }

        [HttpGet]
        public IActionResult GetAll() {
            var users = _userService.GetAll();
            return users.HasValue() ? Ok(users) : NotFound();
        }
        
        [HttpGet("{id}")]
        public IActionResult GetById([FromRoute] int id) {
            var user = _userService.GetById(id);
            return user.HasValue() ? Ok(user) : NotFound();
        }
        
        [HttpGet("{id}/photo")]
        public IActionResult GetUserPhoto([FromRoute] int id, [FromQuery] string? fileName = null)
        {
            var photo = new Photo
            {
                ContentBase64 = _userService.GetUserPhoto(id, fileName)
            };
            Response.Headers.Append("Content-Disposition", "inline");
            return Ok(photo);
        }

        [HttpPost]
        public ActionResult Create([FromBody] User user) {
            _userService.Insert(user);
            return Created();
        }

        [HttpGet("verify-email")]
        public OkResult VerifyEmail([FromQuery] string verificationToken) {
            var id = _userService.VerifyEmail(verificationToken);
            //await _emailNotificationHub.NotifyRegistrationVerification(id.ToString());
            return Ok();
        }

        [HttpPatch("{id}")]
        public IActionResult Update([FromRoute] int id, [FromBody] User user) { 
            _userService.Update(id, user);
            
            var accessToken = _authService.CreateAccessToken(user);
            var refreshToken = _authService.CreateRefreshToken(user.Id).Token;
            return Ok(new LoginResponseDTO
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                IsSuccess = true,
                UserId = user.Id
            });
        }
        
        [HttpPatch("recovery-password-email")]
        public async Task<NoContentResult> SendRecoveryPasswordEmail([FromBody] ChangePasswordDTO changePassword) {
            await _userService.SendRecoveryPasswordEmail(changePassword);
            return NoContent();
        }

        [HttpGet("confirm-password-change")]
        public OkResult ConfirmPasswordChange([FromQuery] string verificationToken) {
            var id = _userService.ConfirmPasswordChange(verificationToken);
            //await _emailNotificationHub.NotifyPasswordReset(id.ToString());
            return Ok();
        }

        [HttpPatch("change-password")]
        public NoContentResult ChangePassword([FromBody] ChangePasswordDTO changePassword) {
            _userService.ChangePassword(changePassword);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public NoContentResult Delete([FromRoute] int id) {
            _userService.Delete(id);
            return NoContent();
        }

		[HttpDelete("all")]
		public NoContentResult DeleteAll() {
			_userService.DeleteAll();
			return NoContent();
		}
	}
}
