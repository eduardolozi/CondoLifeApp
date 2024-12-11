using Application.Services;
using Domain.Models;
using Domain.Models.Filters;
using Domain.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class VotingController(VotingService votingService) : ControllerBase
{
    [HttpGet]
    public IActionResult GetAllVotings([FromQuery] VotingFilter filter)
    {
        var votings = votingService.GetAllVotings(filter);
        return votings.Any() ? Ok(votings) : NoContent();
    }

    [HttpPost]
    [Authorize(Policy = "AdminOrManager")]
    public IActionResult CreateVoting([FromBody] Voting voting)
    {
        votingService.CreateVoting(voting);
        return Ok();
    }
}