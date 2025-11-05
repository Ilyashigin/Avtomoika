using Avtomoika.Aplication.Orders.Dto;
using Avtomoika.Infrastructure.Persistence;
using Avtomoika.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace Avtomoika.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrdersController : ControllerBase
    {
        private readonly ApplicationContext _db;

        public OrdersController(ApplicationContext db)
        {
            _db = db;
        }
        
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Order>>> GetAll()
        {
            var orders = await _db.Orders
                .AsNoTracking()
                .Include(o => o.Client)
                .Include(o => o.Car)
                .Include(o => o.Services)
                .ToListAsync();

            return Ok(orders);
        }
        
        [HttpGet("{id}")]
        public async Task<ActionResult<Order>> GetById(int id)
        {
            var order = await _db.Orders
                .AsNoTracking()
                .Include(o => o.Client)
                .Include(o => o.Car)
                .Include(o => o.Services)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null)
                return NotFound(new { message = "Заказ не найден" });

            return Ok(order);
        }
        
        [HttpPost]
        public async Task<ActionResult<Order>> Create(OrderCreateDto dto)
        {
            var services = await _db.Services
                .Where(s => dto.ServiceIds.Contains(s.Id))
                .ToListAsync();

            if (services.Count != dto.ServiceIds.Count)
                return BadRequest(new { message = "Некоторые услуги не найдены" });

            var order = new Order
            {
                ClientId = dto.CustomerId,
                CarId = dto.CarId,
                Services = services,
                OrderDate = dto.OrderDate,
                TotalPrice = services.Sum(s => (decimal)s.Price),
                Status = dto.Status
            };

            _db.Orders.Add(order);
            await _db.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = order.Id }, order);
        }


        
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, Order updated)
        {
            if (id != updated.Id)
                return BadRequest(new { message = "Id в URL и теле запроса не совпадают" });

            var order = await _db.Orders.FindAsync(id);
            if (order == null)
                return NotFound(new { message = "Заказ не найден" });

            order.ClientId = updated.ClientId;
            order.CarId = updated.CarId;
            order.Services = updated.Services;
            order.OrderDate = updated.OrderDate;
            order.TotalPrice = updated.TotalPrice;
            order.Status = updated.Status;

            await _db.SaveChangesAsync();
            return NoContent();
        }
        
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var order = await _db.Orders.FindAsync(id);
            if (order == null)
                return NotFound(new { message = "Заказ не найден" });

            _db.Orders.Remove(order);
            await _db.SaveChangesAsync();

            return NoContent();
        }
    }
}
