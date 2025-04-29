using Microsoft.Data.SqlClient;
using Tutorial8.Models.DTOs;

namespace Tutorial8.Services;

public class TripsService : ITripsService
{
    private readonly string _connectionString = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=APBD;Integrated Security=True;";
    
    public async Task<List<TripDTO>> GetTrips()
    {
        var trips = new List<TripDTO>();

        string command = "SELECT Trip.*, Country.Name FROM Trip join Country_Trip on Country_Trip.IdTrip = Trip.IdTrip join Country on Country_Trip.IdCountry = Country.IdCountry order by trip.IdTrip";
        
        using (SqlConnection conn = new SqlConnection(_connectionString))
        using (SqlCommand cmd = new SqlCommand(command, conn))
        {
            await conn.OpenAsync();

            using (var reader = await cmd.ExecuteReaderAsync())
            {
                var tmpTripDTO = new TripDTO();
                int? last_id = null;
                int curr_id;
                while (await reader.ReadAsync())
                {
                    int idOrdinal = reader.GetOrdinal("IdTrip");
                    curr_id = reader.GetInt32(idOrdinal);
                    if (last_id != curr_id)
                    {
                        trips.Add(tmpTripDTO);
                        tmpTripDTO = new TripDTO();
                        tmpTripDTO.Id = curr_id;
                        tmpTripDTO.Name = reader.GetString(1);
                        tmpTripDTO.Description = reader.GetString(2);
                        tmpTripDTO.DateFrom = reader.GetDateTime(3);
                        tmpTripDTO.DateTo = reader.GetDateTime(4);
                        tmpTripDTO.MaxPeople = reader.GetInt32(5);
                        tmpTripDTO.Countries = new List<CountryDTO>(){new CountryDTO()
                        {
                            Name = reader.GetString(6)
                        }};
                        last_id = curr_id;
                    }
                    else
                    {
                        tmpTripDTO.Countries.Add(new CountryDTO()
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

    public async Task<List<TripDTO>> GetTripsById(int Id)
    {
        var trips = new List<TripDTO>();

        string command = "SELECT * FROM Trip WHERE Id="+Id;
        
        using (SqlConnection conn = new SqlConnection(_connectionString))
        using (SqlCommand cmd = new SqlCommand(command, conn))
        {
            await conn.OpenAsync();

            using (var reader = await cmd.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    int idOrdinal = reader.GetOrdinal("IdTrip");
                    trips.Add(new TripDTO()
                    {
                        Id = reader.GetInt32(idOrdinal),
                        Name = reader.GetString(1),
                        Description = reader.GetString(2),
                        DateFrom = reader.GetDateTime(3),
                        DateTo = reader.GetDateTime(4),
                        MaxPeople = reader.GetInt32(5)
                    });
                }
            }
        }
        
        return trips;
    }
}