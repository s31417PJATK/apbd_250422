using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Tutorial8.Models.DTOs;
using Tutorial8.Services;

namespace Tutorial8.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClientsController : ControllerBase
    {
        private readonly IClientsService _clientsService;

        public ClientsController(IClientsService clientsService)
        {
            _clientsService = clientsService;
        }

        
        //nie działa z jakiegoś powodu
        [HttpGet("{id}/trips")]
        public async Task<IActionResult> GetTrips(int id)
        {
            var trips = await _clientsService.GetClientTrips(id);
            if (trips.Count == 0) return NotFound("");
            return Ok(trips);
        }
    }
}
