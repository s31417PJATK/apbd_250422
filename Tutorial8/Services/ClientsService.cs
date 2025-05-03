using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Tutorial8.Models.DTOs;

namespace Tutorial8.Services;

public class ClientsService : IClientsService
{
    private readonly string _connectionString = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=APBD;Integrated Security=True;";


    public async Task<object> GetClientTrips(int Id)
    {
        var trips = new List<ClientTripDTO>();

        string command = "SELECT Trip.*, Country.Name, Client_Trip.RegisteredAt, Client_Trip.PaymentDate FROM Trip join Country_Trip on Country_Trip.IdTrip = Trip.IdTrip join Country on Country_Trip.IdCountry = Country.IdCountry join Client_Trip on Client_Trip.IdTrip = Trip.IdTrip WHERE Client_Trip.IdClient = "+Id+" order by trip.IdTrip";
        string command2 = "Select count(*) from Client where IdClient = @clientId";

        using (SqlConnection conn = new SqlConnection(_connectionString))
        {
            await conn.OpenAsync();
            using (SqlCommand cmd2 = new SqlCommand(command2, conn))
            {
                cmd2.Parameters.AddWithValue("@clientId", Id);
                int r_c1 = (int)await cmd2.ExecuteScalarAsync();
                if (r_c1 == 0) return null;
                
            }
            using (SqlCommand cmd = new SqlCommand(command, conn))
            {
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
                            tmpClientTripDto.RegisteredAt = reader.GetInt32(7);
                            tmpClientTripDto.PaymentDate = (!reader.IsDBNull(8)) ? reader.GetInt32(8) : null;
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

                    trips.Add(tmpClientTripDto);
                }
            }
        }

        trips.RemoveAt(0);
        return trips;
    }

    public async Task<object> PostClient(ClientDTO client)
    {
        int? id;
        string command = "Insert into Client values (@FirstName,@LastName,@Email,@Phone,@Pesel); Select SCOPE_IDENTITY();";
        
        if (!Regex.IsMatch(client.Email, "^\\S*[@]\\S*[.].\\S*$")) return "Invalid Email Address";
        if (!Regex.IsMatch(client.Phone,"^[+]\\d{11}$")) return "Invalid Phone Number";
        if (!Regex.IsMatch(client.Pesel,"^\\d{11}$")) return "Invalid Pesel Number";
            
        using (SqlConnection conn = new SqlConnection(_connectionString))
        using (SqlCommand cmd = new SqlCommand(command,conn))
        {
            cmd.Parameters.AddWithValue("@FirstName", client.FirstName);
            cmd.Parameters.AddWithValue("@LastName", client.LastName);
            cmd.Parameters.AddWithValue("@Email", client.Email);
            cmd.Parameters.AddWithValue("@Phone", client.Phone);
            cmd.Parameters.AddWithValue("@Pesel", client.Pesel);

            await conn.OpenAsync();
            
            var res = await cmd.ExecuteScalarAsync();
            
            id = Convert.ToInt32(res);

        }
        
        return id;
    }

    public async Task<int> PutClientTrip(int clientId, int tripId)
    {
        string command1 = "Select count(*) from Client where IdClient = @clientId";
        string command2 = "Select MaxPeople from Trip where IdTrip = @tripId";
        int? r_c2;
        string command3 = "Select count(*) from Client_Trip where IdTrip = @tripId";
        string command4 = "Insert into Client_Trip values (@IdClient,@IdTrip,@RegisteredAt,null)";

        using (SqlConnection conn = new SqlConnection(_connectionString))
        {
            await conn.OpenAsync();
            using (SqlCommand cmd = new SqlCommand(command1, conn))
            {
                cmd.Parameters.AddWithValue("@clientId", clientId);
                int r_c1 = (int)await cmd.ExecuteScalarAsync();
                if (r_c1 == 0) return 1;
            }

            using (SqlCommand cmd2 = new SqlCommand(command2, conn))
            {
                cmd2.Parameters.AddWithValue("@tripId", tripId);
                r_c2 = (int?) await cmd2.ExecuteScalarAsync();
                if (r_c2 == null) return 2;
            }

            using (SqlCommand cmd3 = new SqlCommand(command3, conn))
            {
                cmd3.Parameters.AddWithValue("@tripId", tripId);
                int r_c3 = (int)await cmd3.ExecuteScalarAsync();
                if (r_c3 >= r_c2) return 3;
            }

            using (SqlCommand cmd4 = new SqlCommand(command4, conn))
            {
                cmd4.Parameters.AddWithValue("@IdClient", clientId);
                cmd4.Parameters.AddWithValue("@IdTrip", tripId);

                string formattedDate = DateTime.Now.ToString("yyyyMMdd");
                int intDate = Convert.ToInt32(formattedDate);

                cmd4.Parameters.AddWithValue("@RegisteredAt", intDate);
                try
                {
                    await cmd4.ExecuteScalarAsync();
                }
                catch
                {
                    return 4;
                }
            }
        }
        return 0;
    }

    public async Task<int> DeleteClientTrip(int clientId, int tripId)
    {
        string command = "Delete from Client_Trip where IdClient = @clientId and IdTrip = @tripId";

        using (SqlConnection conn = new SqlConnection(_connectionString))
        {
            await conn.OpenAsync();
            using (SqlCommand cmd = new SqlCommand(command, conn))
            {
                cmd.Parameters.AddWithValue("@clientId", clientId);
                cmd.Parameters.AddWithValue("@tripId", tripId);

                int res = await cmd.ExecuteNonQueryAsync();
                
                if (res == 0) return 1;
                
            }
        }
        return 0;
    }
}