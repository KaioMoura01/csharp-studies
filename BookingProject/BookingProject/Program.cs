using BookingProject.Domain;
using BookingProject.Entities;
using BookingProject.Misc;

var reservations = new List<Reservation>();

Console.WriteLine("=== Welcome to BookingProject ===");

var running = true;
while (running)
{
    Console.WriteLine();
    Console.WriteLine("1 - Add reservation");
    Console.WriteLine("2 - List reservations");
    Console.WriteLine("0 - Exit");

    var option = Utils.ReadInt("Choose an option: ");
    Console.WriteLine();

    try
    {
        switch (option)
        {
            case 1:
            {
                var room = Utils.ReadString("Room: ");
                var checkIn = Utils.ReadDate("Check-in date (dd/MM/yyyy): ");
                var checkOut = Utils.ReadDate("Check-out date (dd/MM/yyyy): ");

                var reservation = new Reservation(room, checkIn, checkOut);
                
                var existing = reservations.Any(
                    x => 
                        x.Room == room 
                        && reservation.CheckIn <  x.CheckOut 
                        && x.CheckIn < reservation.CheckOut);

                if (existing)
                {
                    throw new DomainException("Reservation already exists");
                }
                
                reservations.Add(reservation);
                Console.WriteLine($"Reservation added successfully! ({reservation.Duration()} night(s))");
                break;
            }
            case 2:
                if (reservations.Count == 0)
                {
                    Console.WriteLine("No reservations registered yet.");
                    break;
                }

                foreach (var reservation in reservations)
                {
                    Console.WriteLine(reservation);
                }
                break;
            case 0:
                running = false;
                Console.WriteLine("Goodbye!");
                break;
            default:
                Console.WriteLine("Invalid option, please try again.");
                break;
        }
    }
    catch (DomainException e)
    {
        Console.WriteLine($"Validation error: {e.Message}");
    }
    catch (Exception e)
    {
        Console.WriteLine($"Unexpected error: {e.Message}");
    }
}
