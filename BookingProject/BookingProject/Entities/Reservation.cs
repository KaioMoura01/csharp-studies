using System.Text;
using BookingProject.Domain;

namespace BookingProject.Entities;

public class Reservation
{
    public string Room { get; set; }
    public DateTime CheckIn { get; set; }
    public DateTime CheckOut { get; set; }

    public Reservation(string room, DateTime checkIn, DateTime checkOut)
    {
        if (checkOut <= checkIn)
        {
            throw new DomainException("Booking error: check-out date must be after check-in date");
        }
        if (checkIn <= DateTime.Today)
        {
            throw new DomainException("Booking error: check-in date must be in the future");
        }

        Room = room;
        CheckIn = checkIn;
        CheckOut = checkOut;
    }

    public int Duration()
    {
        return (int)(CheckOut - CheckIn).TotalDays;
    }

    public override string ToString()
    {
        var sb = new StringBuilder();

        sb.Append("Room: ");
        sb.AppendLine(Room);
        sb.Append("Check-in: ");
        sb.AppendLine(CheckIn.ToString("dd/MM/yyyy"));
        sb.Append("Check-out: ");
        sb.AppendLine(CheckOut.ToString("dd/MM/yyyy"));
        
        return sb.ToString();
    }
}