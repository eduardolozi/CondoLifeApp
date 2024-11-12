using Domain.Enums;

namespace Application.DTOs;

public class ChangeUserRoleDTO
{
    public int UserId { get; set; }
    public UserRoleEnum Role { get; set; }
}