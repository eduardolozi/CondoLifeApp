using Application.Services;
using Domain.Models;
using Domain.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class SpaceController : ControllerBase {
    private readonly SpaceService _spaceService;
    public SpaceController(SpaceService spaceService)
    {
        _spaceService = spaceService;
    }

    [HttpGet]
    public IActionResult GetAll([FromQuery] SpaceFilter? filter = null)
    {
        var spaces = _spaceService.GetAll(filter);
        return spaces.HasValue() ? Ok(spaces) : NotFound();
    }
    
    [HttpGet("{id}")]
    public IActionResult GetAll([FromRoute] int id)
    {
        var space = _spaceService.GetById(id);
        return space.HasValue() ? Ok(space) : NotFound();
    }
    
    [HttpGet("{id}/photo")]
    public IActionResult GetSpacePhoto([FromRoute] int id)
    {
        var photo = new Photo
        {
            ContentBase64 = _spaceService.GetSpacePhoto(id)
        };
        Response.Headers.Append("Content-Disposition", "inline");
        return Ok(photo);
    }

    [HttpPost]
    [Authorize(Policy = "ManagerOnly")]
    public NoContentResult Add([FromBody] Space space)
    {
        _spaceService.Insert(space);
        return NoContent();
    }

    [HttpPatch("{id}")]
    [Authorize(Policy = "ManagerOnly")]
    public NoContentResult Update([FromRoute] int id, [FromBody] Space space)
    {
        _spaceService.Update(id, space);
        return NoContent();
    }

    [HttpDelete("{id}")]
    [Authorize(Policy = "ManagerOnly")]
    public NoContentResult Delete([FromRoute] int id)
    {
        _spaceService.Delete(id);
        return NoContent();
    }
}