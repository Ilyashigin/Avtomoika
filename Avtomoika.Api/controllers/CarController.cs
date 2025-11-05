using Avtomoika.Domain.Entities;
using Avtomoika.Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace Avtomoika.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CarsController : ControllerBase
    {
        private readonly ApplicationContext _db;

        public CarsController(ApplicationContext db)
        {
            _db = db;
        }
        
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Car>>> GetAll()
        {
            var cars = await _db.Cars
                .AsNoTracking()
                .Include(c => c.Client)
                .ToListAsync();

            return Ok(cars);
        }
        
        [HttpGet("{id}")]
        public async Task<ActionResult<Car>> GetById(int id)
        {
            var car = await _db.Cars
                .AsNoTracking()
                .Include(c => c.Client)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (car == null)
                return NotFound(new { message = "Машина не найдена" });

            return Ok(car);
        }

        
        [HttpPost]
        public async Task<ActionResult<Car>> Create(Car car)
        {
            var clientExists = await _db.Clients.AnyAsync(c => c.Id == car.ClientId);
            if (!clientExists)
                return BadRequest(new { message = $"Клиент с Id={car.ClientId} не найден" });

            _db.Cars.Add(car);
            await _db.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = car.Id }, car);
        }

        
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, Car updated)
        {
            if (id != updated.Id)
                return BadRequest(new { message = "Id в URL и теле запроса не совпадают" });

            var car = await _db.Cars.FindAsync(id);
            if (car == null)
                return NotFound(new { message = "Машина не найдена" });

            car.Marka = updated.Marka;
            car.Model = updated.Model;
            car.Number = updated.Number;
            car.ClientId = updated.ClientId;

            await _db.SaveChangesAsync();
            return NoContent();
        }
        
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var car = await _db.Cars.FindAsync(id);
            if (car == null)
                return NotFound(new { message = "Машина не найдена" });

            _db.Cars.Remove(car);
            await _db.SaveChangesAsync();

            return NoContent();
        }
    }
}
