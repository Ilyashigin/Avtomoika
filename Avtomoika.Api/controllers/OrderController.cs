using Avtomoika.Application.Interfaces;
using Avtomoika.Domain.Entities;
using Avtomoika.Api.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace Avtomoika.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrderController : ControllerBase
    {
        private readonly IRepository<Order> _orderRepository;
        private readonly IRepository<Service> _serviceRepository;

        public OrderController(IRepository<Order> orderRepository, IRepository<Service> serviceRepository)
        {
            _orderRepository = orderRepository;
            _serviceRepository = serviceRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var orders = await _orderRepository.GetAllAsync();
            return Ok(orders);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var order = await _orderRepository.GetByIdAsync(id);
            if (order == null) return NotFound();
            return Ok(order);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateOrderDto dto)
        {
            if (dto.ServiceIds == null || dto.ServiceIds.Count == 0)
                return BadRequest("Необходимо указать хотя бы одну услугу.");

            var services = new List<Service>();
            decimal total = 0;

            foreach (var id in dto.ServiceIds)
            {
                var service = await _serviceRepository.GetByIdAsync(id);
                if (service == null)
                    return BadRequest($"Услуга с ID {id} не найдена.");

                services.Add(service);
                total += service.Price;
            }

            var order = new Order
            {
                ClientId = dto.ClientId,
                CarId = dto.CarId,
                Services = services,
                TotalPrice = total,
                OrderDate = dto.OrderDate,
                Status = dto.Status
            };

            await _orderRepository.AddAsync(order);
            await _orderRepository.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = order.Id }, order);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] CreateOrderDto dto)
        {
            var order = await _orderRepository.GetByIdAsync(id);
            if (order == null) return NotFound();

            var services = new List<Service>();
            decimal total = 0;

            foreach (var sid in dto.ServiceIds)
            {
                var service = await _serviceRepository.GetByIdAsync(sid);
                if (service == null)
                    return BadRequest($"Услуга с ID {sid} не найдена.");

                services.Add(service);
                total += service.Price;
            }

            order.ClientId = dto.ClientId;
            order.CarId = dto.CarId;
            order.Services = services;
            order.TotalPrice = total;
            order.OrderDate = dto.OrderDate;
            order.Status = dto.Status;

            await _orderRepository.UpdateAsync(order);
            await _orderRepository.SaveChangesAsync();

            return Ok(order);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var order = await _orderRepository.GetByIdAsync(id);
            if (order == null) return NotFound();

            await _orderRepository.DeleteAsync(id);
            await _orderRepository.SaveChangesAsync();
            return NoContent();
        }
    }
}
