using Domain.Models;

namespace Application.DTOs;

public class UpdateUserDataDTO
{
    public int Id { get; set; }
    public string Username { get; set; }
    public string Email { get; set; }
    public string Block { get; set; }
    public int Apartment { get; set; }
    public Photo? Photo { get; set; }
}