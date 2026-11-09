using Microsoft.EntityFrameworkCore;
using MyApp.Data;

Console.WriteLine($"Путь к базе данных: {AppDbContext.DbPath}");

using (var db = new AppDbContext())
{
    // Создает базу
    db.Database.EnsureCreated();
    
    // Заполняеь тестовыми данными
    DbSeed.Seed(db);

    Console.WriteLine("\n--- КЛИЕНТЫ ---");
    foreach (var client in db.Clients)
    {
        Console.WriteLine(client.ToString());
    }

    Console.WriteLine("\n--- ПРЕДМЕТЫ ПРОКАТА ---");
    foreach (var item in db.Items)
    {
        Console.WriteLine(item.ToString());
    }

    Console.WriteLine("\n--- АРЕНДЫ ---");
    // Использует Include, чтобы подтянуть имена клиентов и предметов
    var rentals = db.Rentals
        .Include(r => r.Client)
        .Include(r => r.Item)
        .ToList();

    foreach (var rental in rentals)
    {
        Console.WriteLine(rental.ToString());
    }
}