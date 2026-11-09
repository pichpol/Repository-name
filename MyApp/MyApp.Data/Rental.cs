namespace MyApp.Data;

public class Rental
{
    public int Id { get; set; }
    public int ClientId { get; set; }
    public Client? Client { get; set; }

    public int ItemId { get; set; }
    public Item? Item { get; set; }

    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int TotalCost { get; set; }
    public bool IsReturned { get; set; }

    public override string ToString() => 
        $"Аренда: {Client?.FullName} взял {Item?.Title} с {StartDate:dd.MM} по {EndDate:dd.MM}. Сумма: {TotalCost}";
}