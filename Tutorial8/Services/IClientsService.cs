using Microsoft.AspNetCore.Mvc;
using Tutorial8.Models.DTOs;

namespace Tutorial8.Services;

public interface IClientsService
{
    Task<object> GetClientTrips(int Id);
    Task<object> PostClient(ClientDTO client);
    
    Task<int> PutClientTrip(int clientId,int tripId);
    
    Task<int> DeleteClientTrip(int clientId, int tripId);
}