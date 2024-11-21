using Application.Services;
using Domain.Models;
using Domain.Models.Filters;
using Domain.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class SpaceController(SpaceService spaceService) : ControllerBase
{
    [Authorize]
    [HttpGet]
    public IActionResult GetAll([FromQuery] SpaceFilter? filter = null)
    {
        var spaces = spaceService.GetAll(filter);
        return spaces.HasValue() ? Ok(spaces) : NotFound();
    }
    
    [Authorize]
    [HttpGet("{id}")]
    public IActionResult GetAll([FromRoute] int id)
    {
        var space = spaceService.GetById(id);
        return space.HasValue() ? Ok(space) : NotFound();
    }
    
    [Authorize]
    [HttpGet("{id}/photo")]
    public IActionResult GetSpacePhoto([FromRoute] int id)
    {
        var photo = spaceService.GetSpacePhoto(id);
        return Ok(photo);
    }
    
    [HttpPost]
    [Authorize(Policy = "Manager")]
    public NoContentResult Add([FromBody] Space space)
    {
        spaceService.Insert(space);
        return NoContent();
    }

    [HttpPatch("{id}")]
    [Authorize(Policy = "Manager")]
    public NoContentResult Update([FromRoute] int id, [FromBody] Space space)
    {
        spaceService.Update(id, space);
        return NoContent();
    }

    [HttpDelete("{id}")]
    [Authorize(Policy = "Manager")]
    public NoContentResult Delete([FromRoute] int id)
    {
        spaceService.Delete(id);
        return NoContent();
    }
}