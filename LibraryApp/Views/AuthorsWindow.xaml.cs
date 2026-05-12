using LibraryApp.Data;
using LibraryApp.Models;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace LibraryApp.Views;

public partial class AuthorsWindow : Window
{
    private readonly LibraryContext _context;

    public AuthorsWindow(LibraryContext context)
    {
        InitializeComponent();
        _context = context;
        LoadAuthors();
    }

    private void LoadAuthors()
    {
        AuthorsGrid.ItemsSource = _context.Authors.OrderBy(a => a.LastName).ToList();
    }

    private void BtnAdd_Click(object sender, RoutedEventArgs e)
    {
        var dlg = new AuthorEditDialog();
        if (dlg.ShowDialog() == true)
        {
            _context.Authors.Add(dlg.Author);
            _context.SaveChanges();
            LoadAuthors();
        }
    }

    private void BtnEdit_Click(object sender, RoutedEventArgs e)
    {
        if (AuthorsGrid.SelectedItem is not Author selected)
        {
            MessageBox.Show("Выберите автора.", "Внимание", MessageBoxButton.OK, MessageBoxImage.Information);
            return;
        }
        var dlg = new AuthorEditDialog(selected);
        if (dlg.ShowDialog() == true)
        {
            _context.SaveChanges();
            LoadAuthors();
        }
    }

    private void BtnDelete_Click(object sender, RoutedEventArgs e)
    {
        if (AuthorsGrid.SelectedItem is not Author selected)
        {
            MessageBox.Show("Выберите автора.", "Внимание", MessageBoxButton.OK, MessageBoxImage.Information);
            return;
        }

        var result = MessageBox.Show(
            $"Удалить автора «{selected.FullName}»?\nВсе книги этого автора тоже будут удалены!",
            "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Warning);

        if (result == MessageBoxResult.Yes)
        {
            _context.Authors.Remove(selected);
            _context.SaveChanges();
            LoadAuthors();
        }
    }

    private void AuthorsGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e) => BtnEdit_Click(sender, e);
    private void BtnClose_Click(object sender, RoutedEventArgs e) => Close();
}
