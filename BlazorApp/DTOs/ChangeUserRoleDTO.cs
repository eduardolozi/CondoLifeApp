using BlazorApp.Enums;

namespace BlazorApp.DTOs;

public class ChangeUserRoleDTO
{
    public int? UserId { get; set; }
    public UserRoleEnum Role { get; set; }
}