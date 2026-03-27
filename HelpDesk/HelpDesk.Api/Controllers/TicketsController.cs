using HelpDesk.Application.Services.Tickets;
using HelpDesk.Application.Services.Tickets.Requests;
using HelpDesk.Core.Models;
using HelpDesk.Shared.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HelpDesk.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("/api/v1/tickets")]
    public class TicketsController : ControllerBase
    {
        private readonly TicketsService _ticketsService;

        public TicketsController(TicketsService ticketsService)
        {
            _ticketsService = ticketsService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateTicket(CreateTicketRequest request)
        {
            Guid userId = Guid.Parse(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var createResult = await _ticketsService.CreateTicket(userId, request);
            if (!createResult.IsSuccess)
                return BadRequest(new { error = createResult.Error?.Description });

            return Ok(createResult.Value);
        }

        [Authorize(Policy = "admin-only")]
        [HttpGet]
        public async Task<IActionResult> GetAllTickets([FromQuery] PaginationQuery pagination)
        {
            var ticketsResult = await _ticketsService.GetAllTickets(pagination);
            if (!ticketsResult.IsSuccess)
                return BadRequest(new { error = ticketsResult.Error?.Description });

            return Ok(ticketsResult.Value);
        }

        [Authorize(Policy = "admin-only")]
        [HttpGet("support")]
        public async Task<IActionResult> GetAllTicketsAssignedToSupport([FromQuery] PaginationQuery pagination)
        {
            Guid userId = Guid.Parse(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var ticketsResult = await _ticketsService.GetAllTicketsAssignedToSupport(userId, pagination);
            if (!ticketsResult.IsSuccess)
                return BadRequest(new { error = ticketsResult.Error?.Description });

            return Ok(ticketsResult.Value);
        }

        [Authorize(Policy = "admin-only")]
        [HttpGet("pending")]
        public async Task<IActionResult> GetAllPendingTickets([FromQuery] PaginationQuery pagination)
        {
            var ticketsResult = await _ticketsService.GetAllPendingTickets(pagination);
            if (!ticketsResult.IsSuccess)
                return BadRequest(new { error = ticketsResult.Error?.Description });

            return Ok(ticketsResult.Value);
        }

        [HttpGet("user")]
        public async Task<IActionResult> GetAllTicketsCreatedByUser([FromQuery] PaginationQuery pagination)
        {
            Guid userId = Guid.Parse(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var ticketsResult = await _ticketsService.GetAllCreatedByUser(userId, pagination);
            if (!ticketsResult.IsSuccess)
                return BadRequest(new { error = ticketsResult.Error?.Description });

            return Ok(ticketsResult.Value);
        }

        [Authorize]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetTicket([FromRoute] Guid id)
        {
            Guid userId = Guid.Parse(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var ticketsResult = await _ticketsService.GetTicket(userId, id);
            if (!ticketsResult.IsSuccess)
                return BadRequest(new { error = ticketsResult.Error?.Description });

            return Ok(ticketsResult.Value);
        }

        [Authorize(Policy = "admin-only")]
        [HttpPut("{id}/assign")]
        public async Task<IActionResult> AssignTicket([FromRoute] Guid id)
        {
            Guid userId = Guid.Parse(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var assignResult = await _ticketsService.AssignTicket(userId, id);
            if (!assignResult.IsSuccess)
                return BadRequest(new { error = assignResult.Error?.Description });

            return Ok(assignResult.Value);
        }

        [Authorize(Policy = "admin-only")]
        [HttpPut("{id}/resolve")]
        public async Task<IActionResult> ResolveTicket([FromRoute] Guid id)
        {
            Guid userId = Guid.Parse(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var resolveResult = await _ticketsService.ResolveTicket(userId, id);
            if (!resolveResult.IsSuccess)
                return BadRequest(new { error = resolveResult.Error?.Description });

            return Ok(resolveResult.Value);
        }

        [HttpPost("{id}/comment")]
        public async Task<IActionResult> SendComment([FromRoute] Guid id, [FromBody] CreateCommentRequest request)
        {
            Guid userId = Guid.Parse(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var resolveResult = await _ticketsService.CreateComment(userId, id, request);
            if (!resolveResult.IsSuccess)
                return BadRequest(new { error = resolveResult.Error?.Description });

            return Ok(resolveResult.Value);
        }
    }
}
