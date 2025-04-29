using Microsoft.Data.SqlClient;
using Tutorial8.Models.DTOs;

namespace Tutorial8.Services;

public class ClientsService : IClientsService
{
    private readonly string _connectionString = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=APBD;Integrated Security=True;";


    public async Task<List<ClientTripDTO>> GetClientTrips(int Id)
    {
        var trips = new List<ClientTripDTO>();

        string command = "SELECT Trip.*, Country.Name, Client_Trip.RegisteredAt, Client_Trip.PaymentDate FROM Trip join Country_Trip on Country_Trip.IdTrip = Trip.IdTrip join Country on Country_Trip.IdCountry = Country.IdCountry join Client_Trip on Client_Trip.IdTrip = Trip.IdTrip WHERE Client_Trip.IdClient = "+Id+" order by trip.IdTrip";
        
        using (SqlConnection conn = new SqlConnection(_connectionString))
        using (SqlCommand cmd = new SqlCommand(command, conn))
        {
            await conn.OpenAsync();

            using (var reader = await cmd.ExecuteReaderAsync())
            {
                var tmpClientTripDto = new ClientTripDTO();
                int? lastid = null;
                int currid;
                while (await reader.ReadAsync())
                {
                    int idOrdinal = reader.GetOrdinal("IdTrip");
                    currid = reader.GetInt32(idOrdinal);
                    if (lastid != currid)
                    {
                        trips.Add(tmpClientTripDto);
                        tmpClientTripDto = new ClientTripDTO();
                        tmpClientTripDto.Trip = new TripDTO()
                        {
                            Id = currid,
                            Name = reader.GetString(1),
                            Description = reader.GetString(2),
                            DateFrom = reader.GetDateTime(3),
                            DateTo = reader.GetDateTime(4),
                            MaxPeople = reader.GetInt32(5),
                            Countries = new List<CountryDTO>() 
                            { 
                                new CountryDTO()
                                {
                                    Name = reader.GetString(6),
                                } 
                            }
                        };
                        lastid = currid;
                    }
                    else
                    {
                        tmpClientTripDto.Trip.Countries.Add(new CountryDTO()
                        {
                            Name = reader.GetString(6)
                        });
                    }
                }
            }
        }
        
        trips.RemoveAt(0);
        return trips;
    }

    
}