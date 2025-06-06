using ITB2203Application.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ITB2203Application.Controllers;

[Route("api/[controller]")]
[ApiController]
public class TicketsController : ControllerBase
{
    private readonly DataContext _context;

    public TicketsController(DataContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetTickets()
    {
        var tickets = await _context.Tickets.ToListAsync();
        return Ok(tickets);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Ticket>> GetTicket(int id)
    {
        var ticket = await _context.Tickets.FindAsync(id);
        if (ticket == null)
            return NotFound();

        return Ok(ticket);
    }

    [HttpPost]
    public async Task<IActionResult> CreateTicket([FromBody] Ticket ticket)
    {
        var sessionExists = await _context.Sessions.AnyAsync(s => s.Id == ticket.SessionId);
        if (!sessionExists)
            return BadRequest("Session does not exist.");

        if (ticket.Price <= 0)
            return BadRequest("Ticket price must be a positive number.");

        var seatTaken = await _context.Tickets.AnyAsync(t =>
            t.SessionId == ticket.SessionId && t.SeatNo == ticket.SeatNo);

        if (seatTaken)
            return BadRequest("Seat number must be unique within a session.");

        _context.Tickets.Add(ticket);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetTicket), new { id = ticket.Id }, ticket);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateTicket(int id, [FromBody] Ticket updatedTicket)
    {
        var ticket = await _context.Tickets.FindAsync(id);
        if (ticket == null)
            return NotFound();

        if (updatedTicket.Price <= 0)
            return BadRequest("Ticket price must be a positive number.");

        var seatTaken = await _context.Tickets.AnyAsync(t =>
            t.SessionId == updatedTicket.SessionId &&
            t.SeatNo == updatedTicket.SeatNo &&
            t.Id != id);

        if (seatTaken)
            return BadRequest("Seat number must be unique within a session.");

        ticket.SeatNo = updatedTicket.SeatNo;
        ticket.Price = updatedTicket.Price;
        ticket.SessionId = updatedTicket.SessionId;

        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTicket(int id)
    {
        var ticket = await _context.Tickets.FindAsync(id);
        if (ticket == null)
            return NotFound();

        _context.Tickets.Remove(ticket);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
