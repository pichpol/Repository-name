namespace MyApp.Data;

public static class DbSeed
{
    public static void Seed(AppDbContext db)
    {
        
        if (db.Clients.Any() || db.Items.Any() || db.Rentals.Any())
            return;

        var clients = new List<Client>
        {
            new() { FullName = "Иванов Иван", PhoneNumber = "+79991112233", LoyaltyPoints = 150 },
            new() { FullName = "Петрова Анна", PhoneNumber = "+79994445566", LoyaltyPoints = 20 },
            new() { FullName = "Сидоров Олег", PhoneNumber = "+79997778899", LoyaltyPoints = 500 }
        };
        db.Clients.AddRange(clients);

        var items = new List<Item>
        {
            new() { Title = "Фотоаппарат Sony A7", Category = "Фото", PricePerDay = 1500 },
            new() { Title = "Настольная игра Catan", Category = "Игра", PricePerDay = 300 },
            new() { Title = "Объектив Canon 50mm", Category = "Фото", PricePerDay = 800 },
            new() { Title = "Настольная игра Манчкин", Category = "Игра", PricePerDay = 200 }
        };
        db.Items.AddRange(items);

        db.SaveChanges(); // Сохраняем клиентов и вещи, чтобы у них появились Id

        var rentals = new List<Rental>
        {
            new() { ClientId = clients[0].Id, ItemId = items[0].Id, StartDate = DateTime.Now.AddDays(-2), EndDate = DateTime.Now.AddDays(2), TotalCost = 6000, IsReturned = false },
            new() { ClientId = clients[1].Id, ItemId = items[1].Id, StartDate = DateTime.Now.AddDays(-5), EndDate = DateTime.Now.AddDays(-2), TotalCost = 900, IsReturned = true },
            new() { ClientId = clients[2].Id, ItemId = items[2].Id, StartDate = DateTime.Now, EndDate = DateTime.Now.AddDays(1), TotalCost = 800, IsReturned = false }
        };
        db.Rentals.AddRange(rentals);

        db.SaveChanges();
    }
}