﻿using ITB2203Application.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ITB2203Application.Controllers;

[Route("api/[controller]")]
[ApiController]
public class TicketsController : ControllerBase
{
    private readonly DataContext _context;

    private static List<Ticket> Tickets = new List<Ticket>();
    private static int ticketIdCounter = 1;

    public TicketsController(DataContext context)
    {
        _context = context;
    }

    [HttpGet]
    public IActionResult GetTickets() => Ok(Tickets);

    [HttpGet("{id}")]
    public ActionResult<Ticket> GetTicket(int id)
    {
        var Tic = _context.Tickets!.Find(id);

        if (Tic == null)
        {
            return NotFound();
        }

        return Ok(Tic);
    }

    [HttpPost]
    public IActionResult CreateTicket([FromBody] Ticket ticket)
    {
        if (ticket.Price <= 0)
            return BadRequest("Ticket price must be a positive number.");

        if (Tickets.Any(t => t.SessionId == ticket.SessionId && t.SeatNo == ticket.SeatNo))
            return BadRequest("Seat number must be unique within a session.");

        ticket.Id = ticketIdCounter++;
        Tickets.Add(ticket);
        return CreatedAtAction(nameof(GetTickets), new { id = ticket.Id }, ticket);
    }

    [HttpPut("{id}")]
    public IActionResult UpdateTicket(int id, [FromBody] Ticket updatedTicket)
    {
        var ticket = Tickets.FirstOrDefault(t => t.Id == id);
        if (ticket == null) return NotFound();

        if (updatedTicket.Price <= 0)
            return BadRequest("Ticket price must be a positive number.");

        ticket.SeatNo = updatedTicket.SeatNo;
        ticket.Price = updatedTicket.Price;
        return NoContent();
    }

    [HttpDelete("{id}")]
    public IActionResult DeleteTicket(int id)
    {
        var ticket = Tickets.FirstOrDefault(t => t.Id == id);
        if (ticket == null) return NotFound();

        Tickets.Remove(ticket);
        return NoContent();
    }
}
