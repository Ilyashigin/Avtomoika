using Avtomoika.Application.Interfaces;
using Avtomoika.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace Avtomoika.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ServiceController : ControllerBase
    {
        private readonly IRepository<Service> _serviceRepository;

        public ServiceController(IRepository<Service> serviceRepository)
        {
            _serviceRepository = serviceRepository;
        }
        
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var services = await _serviceRepository.GetAllAsync();
            return Ok(services);
        }
        
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var service = await _serviceRepository.GetByIdAsync(id);
            if (service == null)
                return NotFound();

            return Ok(service);
        }
        
        [HttpPost]
        public async Task<IActionResult> Create(Service service)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await _serviceRepository.AddAsync(service);
            await _serviceRepository.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = service.Id }, service);
        }
        
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, Service updatedService)
        {
            if (id != updatedService.Id)
                return BadRequest("ID услуги не совпадает.");

            var existingService = await _serviceRepository.GetByIdAsync(id);
            if (existingService == null)
                return NotFound();

            existingService.Name = updatedService.Name;
            existingService.Description = updatedService.Description;
            existingService.Price = updatedService.Price;

            await _serviceRepository.UpdateAsync(existingService);
            await _serviceRepository.SaveChangesAsync();

            return NoContent();
        }
        
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var service = await _serviceRepository.GetByIdAsync(id);
            if (service == null)
            {
                return NotFound();
            }

            await _serviceRepository.DeleteAsync(id);
            await _serviceRepository.SaveChangesAsync();
            return NoContent();
        }
    }
}
