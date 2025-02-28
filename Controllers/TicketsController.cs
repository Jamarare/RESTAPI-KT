using ITB2203Application.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ITB2203Application.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TicketsController : ControllerBase
{
    private readonly DataContext _context;

    public TicketsController(DataContext context)
    {
        _context = context;
    }

    [HttpGet]
    public ActionResult<IEnumerable<Ticket>> GetTickets(string? nam = null)
    {
        var query = _context.Tickets!.AsQueryable();

        if (nam != null)
            query = query.Where(x => x.SeatNo != null && x.SeatNo.ToUpper().Contains(nam.ToUpper()));

        return query.ToList();
    }

    [HttpGet("{id}")]
    public ActionResult<TextReader> GetTicket(int id)
    {
        var ticket = _context.Tickets!.Find(id);

        if (ticket == null)
        {
            return NotFound();
        }

        return Ok(ticket);
    }

    [HttpPut("{id}")]
    public IActionResult PutTicket(int id, Ticket Ti)
    {
        var dbTicket = _context.Tickets!.AsNoTracking().FirstOrDefault(x => x.Id == Ti.Id);
        if (id != Ti.Id || dbTicket == null)
        {
            return NotFound();
        }

        _context.Update(Ti);
        _context.SaveChanges();

        return NoContent();
    }

    [HttpPost]
    public ActionResult<Ticket> PostTicket(Ticket Ti)
    {
        var dbExercise = _context.Tickets!.Find(Ti.Id);
        if (dbExercise == null)
        {
            _context.Add(Ti);
            _context.SaveChanges();

            return CreatedAtAction(nameof(GetTicket), new { Id = Ti.Id }, Ti);
        }
        else
        {
            return Conflict();
        }
    }

    [HttpDelete("{id}")]
    public IActionResult DeleteTicket(int id)
    {
        var ticket = _context.Tickets!.Find(id);
        if (ticket == null)
        {
            return NotFound();
        }

        _context.Remove(ticket);
        _context.SaveChanges();

        return NoContent();
    }
}
