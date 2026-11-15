using System;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Interactivity;
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

    //  Загрузка данных
    private void LoadData()
    {
        using (var db = new AppDbContext())
        {
            db.Database.EnsureCreated();
            DbSeed.Seed(db);
        }

        RefreshAll();
    }

    // Обновляем ВСЕ списки и оба выпадающих списка.
    // Вызываем после каждого добавления / изменения / удаления.
    private void RefreshAll()
    {
        using (var db = new AppDbContext())
        {
            var clients = db.Clients.ToList();
            ClientsList.ItemsSource = clients;
            ClientCombo.ItemsSource = clients;
        }

        using (var db = new AppDbContext())
        {
            var items = db.Items.ToList();
            ItemsList.ItemsSource = items;
            ItemCombo.ItemsSource = items;
        }

        using (var db = new AppDbContext())
        {
            RentalsList.ItemsSource = db.Rentals
                .Include(r => r.Client)
                .Include(r => r.Item)
                .ToList();
        }

        using (var db = new AppDbContext())
        {
            ClientCardsList.ItemsSource = db.Clients
                .Include(c => c.Rentals)
                .ThenInclude(r => r.Item)
                .ToList();
        }
    }

    //  КЛИЕНТЫ: добавить / изменить / удалить
    private void AddClient_Click(object? sender, RoutedEventArgs e)
    {
        if (!TryReadClientFields(out var fullName, out var phone, out var points))
            return;

        try
        {
            using (var db = new AppDbContext())
            {
                db.Clients.Add(new Client
                {
                    FullName = fullName,
                    PhoneNumber = phone,
                    LoyaltyPoints = points
                });
                db.SaveChanges();
            }

            ClientsMessage.Text = "Клиент добавлен";
            FullNameBox.Text = "";
            PhoneBox.Text = "";
            PointsBox.Text = "";
            ClientsList.SelectedItem = null;
            RefreshAll();
        }
        catch (Exception ex)
        {
            ClientsMessage.Text = "Ошибка: " + ex.Message;
        }
    }

    private void SaveClient_Click(object? sender, RoutedEventArgs e)
    {
        if (ClientsList.SelectedItem is not Client selected)
        {
            ClientsMessage.Text = "Выберите клиента в списке";
            return;
        }

        if (!TryReadClientFields(out var fullName, out var phone, out var points))
            return;

        try
        {
            using (var db = new AppDbContext())
            {
                var client = db.Clients.FirstOrDefault(c => c.Id == selected.Id);
                if (client is null)
                {
                    ClientsMessage.Text = "Запись уже удалена";
                    return;
                }

                client.FullName = fullName;
                client.PhoneNumber = phone;
                client.LoyaltyPoints = points;
                db.SaveChanges();
            }

            ClientsMessage.Text = "Изменения сохранены";
            RefreshAll();
        }
        catch (Exception ex)
        {
            ClientsMessage.Text = "Ошибка: " + ex.Message;
        }
    }

    private void DeleteClient_Click(object? sender, RoutedEventArgs e)
    {
        if (ClientsList.SelectedItem is not Client selected)
        {
            ClientsMessage.Text = "Выберите клиента в списке";
            return;
        }

        try
        {
            using (var db = new AppDbContext())
            {
                var client = db.Clients.FirstOrDefault(c => c.Id == selected.Id);
                if (client is null)
                {
                    ClientsMessage.Text = "Запись уже удалена";
                    return;
                }

                db.Clients.Remove(client);
                db.SaveChanges();
            }

            ClientsMessage.Text = "Клиент удалён вместе со своими арендами";
            FullNameBox.Text = "";
            PhoneBox.Text = "";
            PointsBox.Text = "";
            RefreshAll();
        }
        catch (Exception ex)
        {
            ClientsMessage.Text = "Ошибка: " + ex.Message;
        }
    }

    private void ClientsList_SelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        // Когда список перезагружается, выбор сбрасывается и здесь будет null
        if (ClientsList.SelectedItem is not Client client)
            return;

        FullNameBox.Text = client.FullName;
        PhoneBox.Text = client.PhoneNumber;
        PointsBox.Text = client.LoyaltyPoints.ToString();
        ClientsMessage.Text = "";
    }

    private bool TryReadClientFields(out string fullName, out string phone, out int points)
    {
        fullName = FullNameBox.Text?.Trim() ?? "";
        phone = PhoneBox.Text?.Trim() ?? "";
        points = 0;

        if (string.IsNullOrWhiteSpace(fullName))
        {
            ClientsMessage.Text = "Введите ФИО клиента";
            return false;
        }

        if (string.IsNullOrWhiteSpace(phone))
        {
            ClientsMessage.Text = "Введите номер телефона";
            return false;
        }

        if (!int.TryParse(PointsBox.Text, out points) || points < 0)
        {
            ClientsMessage.Text = "Поле «Бонусные баллы»: введите целое число";
            return false;
        }

        return true;
    }

    //  ПРОКАТ: добавить / удалить
    private void AddItem_Click(object? sender, RoutedEventArgs e)
    {
        var title = TitleBox.Text?.Trim() ?? "";
        var category = CategoryBox.Text?.Trim() ?? "";

        if (string.IsNullOrWhiteSpace(title))
        {
            ItemsMessage.Text = "Введите название предмета";
            return;
        }

        if (string.IsNullOrWhiteSpace(category))
        {
            ItemsMessage.Text = "Введите категорию";
            return;
        }

        if (!int.TryParse(PriceBox.Text, out var price) || price < 0)
        {
            ItemsMessage.Text = "Поле «Цена за сутки»: введите целое число";
            return;
        }

        try
        {
            using (var db = new AppDbContext())
            {
                db.Items.Add(new Item
                {
                    Title = title,
                    Category = category,
                    PricePerDay = price
                });
                db.SaveChanges();
            }

            ItemsMessage.Text = "Предмет добавлен";
            TitleBox.Text = "";
            CategoryBox.Text = "";
            PriceBox.Text = "";
            RefreshAll();
        }
        catch (Exception ex)
        {
            ItemsMessage.Text = "Ошибка: " + ex.Message;
        }
    }

    private void DeleteItem_Click(object? sender, RoutedEventArgs e)
    {
        if (ItemsList.SelectedItem is not Item selected)
        {
            ItemsMessage.Text = "Выберите предмет в списке";
            return;
        }

        try
        {
            using (var db = new AppDbContext())
            {
                var item = db.Items.FirstOrDefault(i => i.Id == selected.Id);
                if (item is null)
                {
                    ItemsMessage.Text = "Запись уже удалена";
                    return;
                }

                db.Items.Remove(item);
                db.SaveChanges();
            }

            ItemsMessage.Text = "Предмет удалён вместе со связанными арендами";
            RefreshAll();
        }
        catch (Exception ex)
        {
            ItemsMessage.Text = "Ошибка: " + ex.Message;
        }
    }

    //  АРЕНДЫ: добавить / удалить
    private void AddRental_Click(object? sender, RoutedEventArgs e)
    {
        if (ClientCombo.SelectedItem is not Client client)
        {
            RentalsMessage.Text = "Выберите клиента";
            return;
        }

        if (ItemCombo.SelectedItem is not Item item)
        {
            RentalsMessage.Text = "Выберите предмет";
            return;
        }

        if (StartPicker.SelectedDate is not DateTime start)
        {
            RentalsMessage.Text = "Выберите дату выдачи";
            return;
        }

        if (EndPicker.SelectedDate is not DateTime end)
        {
            RentalsMessage.Text = "Выберите дату возврата";
            return;
        }

        if (end < start)
        {
            RentalsMessage.Text = "Дата возврата не может быть раньше даты выдачи";
            return;
        }

        if (!int.TryParse(CostBox.Text, out var cost) || cost < 0)
        {
            RentalsMessage.Text = "Поле «Стоимость»: введите целое число";
            return;
        }

        try
        {
            using (var db = new AppDbContext())
            {
                db.Rentals.Add(new Rental
                {
                    // Записываем Id, а НЕ сами объекты
                    ClientId = client.Id,
                    ItemId = item.Id,
                    StartDate = start,
                    EndDate = end,
                    TotalCost = cost,
                    IsReturned = ReturnedBox.IsChecked == true
                });
                db.SaveChanges();
            }

            RentalsMessage.Text = "Аренда добавлена";
            CostBox.Text = "";
            ReturnedBox.IsChecked = false;
            StartPicker.SelectedDate = null;
            EndPicker.SelectedDate = null;
            RefreshAll();
        }
        catch (Exception ex)
        {
            RentalsMessage.Text = "Ошибка: " + ex.Message;
        }
    }

    private void DeleteRental_Click(object? sender, RoutedEventArgs e)
    {
        if (RentalsList.SelectedItem is not Rental selected)
        {
            RentalsMessage.Text = "Выберите аренду в списке";
            return;
        }

        try
        {
            using (var db = new AppDbContext())
            {
                var rental = db.Rentals.FirstOrDefault(r => r.Id == selected.Id);
                if (rental is null)
                {
                    RentalsMessage.Text = "Запись уже удалена";
                    return;
                }

                db.Rentals.Remove(rental);
                db.SaveChanges();
            }

            RentalsMessage.Text = "Аренда удалена";
            RefreshAll();
        }
        catch (Exception ex)
        {
            RentalsMessage.Text = "Ошибка: " + ex.Message;
        }
    }
}