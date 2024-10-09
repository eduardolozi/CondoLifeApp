using Application.Services;
using Domain.Models;
using Domain.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers {
    [Route("api/[controller]")]
    [ApiController]
    public class CondominiumController : ControllerBase {
        private readonly CondominiumService _condominiumService;
        public CondominiumController(CondominiumService condominiumService) {
            _condominiumService = condominiumService;
        }

        [HttpGet]
        public IActionResult GetAll([FromQuery] CondominiumFilter filter) {
            var condos = _condominiumService.GetAll(filter);
            return condos.HasValue() ?  Ok(condos) : NotFound();
        }

        [HttpGet("{id}")]
        public IActionResult GetById([FromRoute] int id) {
            var condo = _condominiumService.GetById(id);
            return condo.HasValue() ? Ok(condo) : NotFound();
        }

        [HttpPost]
        [Authorize(Policy = "AdminOnly")]
        public CreatedResult Create([FromBody] Condominium condominium) {
            _condominiumService.Insert(condominium);
            return Created();
        }

        [HttpPatch("{id}")]
        [Authorize(Policy = "AdminOnly")]
        public NoContentResult Update([FromRoute] int id, [FromBody] Condominium condominium) {
            _condominiumService.Update(id, condominium);
            return NoContent();
        }
        
        [HttpDelete("{id}")]
        [Authorize(Policy = "AdminOnly")]
        public NoContentResult Delete([FromRoute] int id) {
            _condominiumService.Delete(id);
            return NoContent();
        }
    }
}
