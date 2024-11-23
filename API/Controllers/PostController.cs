using Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[Authorize]
[Route("api/[controller]")]
public class PostController : ControllerBase
{
    // public IActionResult Create(Post post)
    // {
    //     
    // }
}