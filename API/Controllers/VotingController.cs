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

    [HttpGet("{votingId}")]
    public IActionResult GetVotingById([FromRoute] int votingId, [FromQuery] int userId)
    {
        var voting = votingService.GetVotingById(votingId, userId);
        return voting is null ? NoContent() : Ok(voting);
    }
    
    [HttpGet("{id}/details")]
    public IActionResult GetVotingDetails([FromRoute] int id)
    {
        var voting = votingService.GetVotingDetails(id);
        return voting is null ? NoContent() : Ok(voting);
    }

    [HttpPost]
    [Authorize(Policy = "AdminOrManager")]
    public IActionResult CreateVoting([FromBody] Voting voting)
    {
        var authorizationHeader = HttpContext.Request.Headers.Authorization.ToString();
        var token = authorizationHeader.Substring("Bearer ".Length).Trim();
        votingService.CreateVoting(token, voting);
        return Ok();
    }

    [HttpPost("confirm-vote")]
    public IActionResult ConfirmVote([FromBody] Vote vote)
    {
        votingService.ConfirmVote(vote);
        return Ok();
    }
}