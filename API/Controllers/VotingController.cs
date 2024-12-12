using Application.DTOs;
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
    public IActionResult CreateVoting([FromBody] CreateVotingDTO voting)
    {
        votingService.CreateVoting(voting);
        return Ok();
    }
}