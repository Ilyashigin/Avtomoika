using Avtomoika.Application.Interfaces;
using Avtomoika.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace Avtomoika.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CarController : ControllerBase
    {
        private readonly IRepository<Car> _carRepository;

        public CarController(IRepository<Car> carRepository)
        {
            _carRepository = carRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll() =>
            Ok(await _carRepository.GetAllAsync());

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var car = await _carRepository.GetByIdAsync(id);
            return car == null ? NotFound() : Ok(car);
        }

        [HttpPost]
        public async Task<IActionResult> Create(Car car)
        {
            await _carRepository.AddAsync(car);
            await _carRepository.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = car.Id }, car);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, Car car)
        {
            if (id != car.Id) return BadRequest();
            await _carRepository.UpdateAsync(car);
            await _carRepository.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _carRepository.DeleteAsync(id);
            await _carRepository.SaveChangesAsync();
            return NoContent();
        }
    }
}