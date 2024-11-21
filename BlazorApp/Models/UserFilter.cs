using BlazorApp.Enums;

namespace BlazorApp.Models;

public class UserFilter
{
    public int? CondominiumId { get; set; }
    public string? Username { get; set; }
    public UserRoleEnum? Role { get; set; }
}
