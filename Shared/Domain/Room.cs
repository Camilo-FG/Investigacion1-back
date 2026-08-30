namespace Investigacion1_back.Shared.Domain;

public class Room
{
    public Guid Id { get; set; }
    public string Number { get; set; } = string.Empty;
    public RoomType Type { get; set; }
    public int Floor { get; set; }
    public int Capacity { get; set; }
    public decimal BasePricePerNight { get; set; }

    public ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
}