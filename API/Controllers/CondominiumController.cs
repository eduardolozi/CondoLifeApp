using Application.Services;
using Domain.Models;
using Domain.Utils;
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
            return condos.HasValue() ?  Ok(condos) : NoContent();
        }

        [HttpGet("{id}")]
        public IActionResult GetAll([FromRoute] int id) {
            var condo = _condominiumService.GetById(id);
            return condo.HasValue() ? Ok(condo) : NoContent();
        }

        [HttpPost]
        public CreatedResult Create([FromBody] Condominium condominium) {
            _condominiumService.Insert(condominium);
            return Created();
        }
    }
}
