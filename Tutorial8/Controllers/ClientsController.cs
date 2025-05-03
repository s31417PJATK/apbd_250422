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

        
        [HttpGet("{id}/trips")]
        public async Task<IActionResult> GetTrips(int id)
        {
            var res = await _clientsService.GetClientTrips(id);
            if (res is null) return BadRequest("Brak klienta o podanym id");
            var trips = (List<ClientTripDTO>)res;
            if (trips.Count == 0) return NotFound("Klient o podanym id nie jest zarejestrowany na żadne wycieczki");
            return Ok(trips);
        }

        [HttpPost]
        public async Task<IActionResult> PostClient([FromBody] ClientDTO client)
        {
            var res = await _clientsService.PostClient(client);
            if (res == "Invalid Email Address") return BadRequest("Błędny adres email");
            if (res == "Invalid Phone Number") return BadRequest("Błędny numer telefonu");
            if (res == "Invalid Pesel Number") return BadRequest("Błędny numer pesel");
            if (res != null) return Ok(res);
            return BadRequest("");
        }

        [HttpPut("{id}/trips/{tripId}")]
        public async Task<IActionResult> PutTrip(int id, int tripId)
        {
            int res = await _clientsService.PutClientTrip(id, tripId);
            if (res == 0) return Created();
            if (res == 1) return BadRequest("Brak klienta o podanym id");
            if (res == 2) return BadRequest("Brak wycieczki o podanym id");
            if (res == 3) return BadRequest("Na podaną wycieczkę jest już zarejestrowana maksymalna liczba osób");
            if (res == 4) return BadRequest("Ten klient jest już zarejestrowany na podaną wycieczkę");
            return BadRequest("");
        }

        [HttpDelete("{id}/trips/{tripId}")]
        public async Task<IActionResult> DeleteTrip(int id, int tripId)
        {
            int res = await _clientsService.DeleteClientTrip(id, tripId);
            if (res == 0) return Ok();
            if (res == 1) return BadRequest("Brak rejestracji o podanych id");
            return BadRequest("");
        }
    }
}
