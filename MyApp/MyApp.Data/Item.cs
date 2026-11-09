namespace MyApp.Data;

public class Item
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public int PricePerDay { get; set; }

    public List<Rental> Rentals { get; set; } = new();

    public override string ToString() => $"{Title} [{Category}] - {PricePerDay} руб/день";
}