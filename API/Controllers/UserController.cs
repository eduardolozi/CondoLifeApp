using Application.DTOs;
using Application.Interfaces;
using Application.Services;
using Domain.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Photo = Domain.Models.Photo;
using User = Domain.Models.User;
using UserFilter = Domain.Models.Filters.UserFilter;

namespace API.Controllers {
    [Route("api/[controller]")]
    [ApiController]
    public class UserController(
        UserService userService,
        IAuthService authService)
        : ControllerBase
    {

        [HttpGet]
        public IActionResult GetAll([FromQuery] UserFilter? filter = null) {
            var users = userService.GetAll(filter);
            return users.HasValue() ? Ok(users) : NotFound();
        }
        
        [HttpGet("{id}")]
        public IActionResult GetById([FromRoute] int id) {
            var user = userService.GetById(id);
            return user.HasValue() ? Ok(user) : NotFound();
        }
        
        [HttpGet("{id}/photo")]
        public IActionResult GetUserPhoto([FromRoute] int id, [FromQuery] string? fileName = null)
        {
            var photo = new Photo
            {
                ContentBase64 = userService.GetUserPhoto(id, fileName)
            };
            Response.Headers.Append("Content-Disposition", "inline");
            return Ok(photo);
        }

        [HttpPost]
        public ActionResult Create([FromBody] User user) {
            userService.Insert(user);
            return Created();
        }

        [HttpGet("verify-email")]
        public OkResult VerifyEmail([FromQuery] string verificationToken) {
            var id = userService.VerifyEmail(verificationToken);
            return Ok();
        }

        [Authorize]
        [HttpPatch("{id}")]
        public IActionResult Update([FromRoute] int id, [FromBody] User user) { 
            userService.Update(id, user);
            
            var accessToken = authService.CreateAccessToken(user);
            var refreshToken = authService.CreateRefreshToken(user.Id).Token;
            return Ok(new LoginResponseDTO
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                IsSuccess = true,
                UserId = user.Id
            });
        }

        [Authorize]
        [HttpPatch("{id}/user-data")]
        public IActionResult UpdateUserData([FromRoute] int id, [FromBody] UpdateUserDataDTO userData)
        {
            var user = userService.UpdateUserData(id, userData);
            var accessToken = authService.CreateAccessToken(user);
            var refreshToken = authService.CreateRefreshToken(user.Id).Token;
            return Ok(new LoginResponseDTO
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                IsSuccess = true,
                UserId = user.Id
            });
        }
        
        [Authorize]
        [HttpPatch("{id}/user-notification-configs")]
        public IActionResult UpdateNotificationConfigs([FromRoute] int id, [FromBody] UpdateUserNotificationConfigsDTO configs)
        {
            var user = userService.UpdateNotificationConfigs(id, configs);
            var accessToken = authService.CreateAccessToken(user);
            var refreshToken = authService.CreateRefreshToken(user.Id).Token;
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
            await userService.SendRecoveryPasswordEmail(changePassword);
            return NoContent();
        }

        [HttpGet("confirm-password-change")]
        public OkResult ConfirmPasswordChange([FromQuery] string verificationToken) {
            var id = userService.ConfirmPasswordChange(verificationToken);
            return Ok();
        }

        [HttpPatch("change-password")]
        public NoContentResult ChangePassword([FromBody] ChangePasswordDTO changePassword) {
            userService.ChangePassword(changePassword);
            return NoContent();
        }

        [Authorize]
        [HttpDelete("{id}")]
        public NoContentResult Delete([FromRoute] int id) {
            userService.Delete(id);
            return NoContent();
        }

        [Authorize]
		[HttpDelete("all")]
		public NoContentResult DeleteAll() {
			userService.DeleteAll();
			return NoContent();
		}

        [Authorize(Policy = "AdminOrManager")]
        [HttpPatch("{id}/change-role")]
        public NoContentResult ChangeRole([FromRoute] int id, [FromBody] ChangeUserRoleDTO changeRole)
        {
            userService.ChangeUserRole(changeRole);
            return NoContent();
        }
    }
}
