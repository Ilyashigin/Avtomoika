using Avtomoika.Infrastructure.Persistence;
using Avtomoika.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace Avtomoika.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ClientsController : ControllerBase
    {
        private readonly ApplicationContext _db;

        public ClientsController(ApplicationContext db)
        {
            _db = db;
        }

        
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Client>>> GetAll()
        {
            var clients = await _db.Clients
                .Include(c => c.Car)
                .Include(c => c.Order)
                .ToListAsync();

            return Ok(clients);
        }

        
        [HttpGet("{id}")]
        public async Task<ActionResult<Client>> GetById(int id)
        {
            var client = await _db.Clients
                .Include(c => c.Car)
                .Include(c => c.Order)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (client == null)
                return NotFound();

            return Ok(client);
        }

        
        [HttpPost]
        public async Task<ActionResult<Client>> Create(Client client)
        {
            _db.Clients.Add(client);
            await _db.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = client.Id }, client);
        }

        
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, Client client)
        {
            if (id != client.Id)
                return BadRequest();

            _db.Entry(client).State = EntityState.Modified;
            await _db.SaveChangesAsync();

            return NoContent();
        }

        
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var client = await _db.Clients.FindAsync(id);
            if (client == null)
                return NotFound();

            _db.Clients.Remove(client);
            await _db.SaveChangesAsync();

            return NoContent();
        }
    }
}
