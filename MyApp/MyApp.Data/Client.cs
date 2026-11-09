namespace MyApp.Data;

public class Client
{
    public int Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public int LoyaltyPoints { get; set; }

    public List<Rental> Rentals { get; set; } = new();

    public override string ToString() => $"{FullName} ({PhoneNumber})";
}