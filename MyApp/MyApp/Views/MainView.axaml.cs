using System.Linq;
using Avalonia.Controls;
using Microsoft.EntityFrameworkCore;
using MyApp.Data;

namespace MyApp.Views;

public partial class MainView : UserControl
{
    public MainView()
    {
        InitializeComponent();
        LoadData();
    }

    private void LoadData()
    {
        // 1. Создаём базу и засеваем, если пустая
        using (var db = new AppDbContext())
        {
            db.Database.EnsureCreated();
            DbSeed.Seed(db);
        }

        // 2. Клиенты
        using (var db = new AppDbContext())
        {
            ClientsList.ItemsSource = db.Clients.ToList();
        }

        // 3. Предметы
        using (var db = new AppDbContext())
        {
            ItemsList.ItemsSource = db.Items.ToList();
        }

        // 4. Аренды с именами вместо Id
        using (var db = new AppDbContext())
        {
            RentalsList.ItemsSource = db.Rentals
                .Include(r => r.Client)
                .Include(r => r.Item)
                .ToList();
        }

        // 5. Клиент + его аренды
        using (var db = new AppDbContext())
        {
            ClientCardsList.ItemsSource = db.Clients
                .Include(c => c.Rentals)
                .ThenInclude(r => r.Item)
                .ToList();
        }
    }
}