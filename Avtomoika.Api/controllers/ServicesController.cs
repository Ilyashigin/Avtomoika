using Avtomoika.Infrastructure.Persistence;
using Avtomoika.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace Avtomoika.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ServicesController : ControllerBase
    {
        private readonly ApplicationContext _db;

        public ServicesController(ApplicationContext db)
        {
            _db = db;
        }

        
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Service>>> GetAll()
        {
            var services = await _db.Services.AsNoTracking().ToListAsync();
            return Ok(services);
        }

        
        [HttpGet("{id}")]
        public async Task<ActionResult<Service>> GetById(int id)
        {
            var service = await _db.Services.AsNoTracking().FirstOrDefaultAsync(s => s.Id == id);
            if (service == null)
                return NotFound(new { message = "Услуга не найдена" });

            return Ok(service);
        }

        
        [HttpPost]
        public async Task<ActionResult<Service>> Create(Service service)
        {
            _db.Services.Add(service);
            await _db.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = service.Id }, service);
        }

        
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, Service updated)
        {
            if (id != updated.Id)
                return BadRequest(new { message = "Id в URL и теле запроса не совпадают" });

            var service = await _db.Services.FindAsync(id);
            if (service == null)
                return NotFound(new { message = "Услуга не найдена" });

            service.Name = updated.Name;
            service.Description = updated.Description;
            service.Price = updated.Price;

            await _db.SaveChangesAsync();
            return NoContent();
        }

        
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var service = await _db.Services.FindAsync(id);
            if (service == null)
                return NotFound(new { message = "Услуга не найдена" });

            _db.Services.Remove(service);
            await _db.SaveChangesAsync();

            return NoContent();
        }
    }
}
