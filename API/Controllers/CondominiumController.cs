using Application.Services;
using Domain.Models;
using Domain.Models.Filters;
using Domain.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers {
    [Route("api/[controller]")]
    [ApiController]
    public class CondominiumController(CondominiumService condominiumService) : ControllerBase
    {
        [HttpGet]
        public IActionResult GetAll([FromQuery] CondominiumFilter? filter = null) {
            var condos = condominiumService.GetAll(filter);
            return condos.HasValue() ?  Ok(condos) : NotFound();
        }

        [HttpGet("{id}")]
        public IActionResult GetById([FromRoute] int id) {
            var condo = condominiumService.GetById(id);
            return condo.HasValue() ? Ok(condo) : NotFound();
        }

        [HttpPost]
        [Authorize(Policy = "AdminOnly")]
        public CreatedResult Create([FromBody] Condominium condominium) {
            condominiumService.Insert(condominium);
            return Created();
        }

        [HttpPatch("{id}")]
        [Authorize(Policy = "AdminOnly")]
        public NoContentResult Update([FromRoute] int id, [FromBody] Condominium condominium) {
            condominiumService.Update(id, condominium);
            return NoContent();
        }
        
        [HttpDelete("{id}")]
        [Authorize(Policy = "AdminOnly")]
        public NoContentResult Delete([FromRoute] int id) {
            condominiumService.Delete(id);
            return NoContent();
        }
    }
}
