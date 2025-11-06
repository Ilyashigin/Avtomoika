using Avtomoika.Application.Interfaces;
using Avtomoika.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace Avtomoika.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ClientController : ControllerBase
    {
        private readonly IRepository<Client> _clientRepository;

        public ClientController(IRepository<Client> clientRepository)
        {
            _clientRepository = clientRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll() =>
            Ok(await _clientRepository.GetAllAsync());

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var client = await _clientRepository.GetByIdAsync(id);
            return client == null ? NotFound() : Ok(client);
        }

        [HttpPost]
        public async Task<IActionResult> Create(Client client)
        {
            await _clientRepository.AddAsync(client);
            await _clientRepository.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = client.Id }, client);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, Client client)
        {
            if (id != client.Id) return BadRequest();
            await _clientRepository.UpdateAsync(client);
            await _clientRepository.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _clientRepository.DeleteAsync(id);
            await _clientRepository.SaveChangesAsync();
            return NoContent();
        }
    }
}