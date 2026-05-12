using LibraryApp.Data;
using Microsoft.EntityFrameworkCore;
using System.Windows;

namespace LibraryApp;

public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        using var context = new LibraryContext();

        // Удаляем старую пустую БД и создаём заново со всеми таблицами и данными
        context.Database.EnsureDeleted();
        context.Database.EnsureCreated();
    }
}
