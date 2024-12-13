using Application.Services;
using Domain.Models;
using Domain.Models.Filters;
using Domain.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[Authorize]
[Route("api/[controller]")]
public class PostController(PostService postService) : ControllerBase
{
    [HttpPost]
    public IActionResult Create(Post post)
    {
        postService.Create(post);
        return Created();
    }

    [HttpGet]
    public IActionResult Get(PostFilter? filter = null)
    {
        var posts = postService.GetAll(filter);
        return posts.Count > 0 ? Ok(posts) : NotFound();
    }
}