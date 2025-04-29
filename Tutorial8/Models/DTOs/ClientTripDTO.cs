namespace Tutorial8.Models.DTOs;

public class ClientTripDTO
{
    public TripDTO Trip { get; set; }
    public int RegisteredAt { get; set; }
    public int? PaymentDate { get; set; }
}