using LibraryApp.Data;
using LibraryApp.Models;
using System.Windows;
using System.Windows.Input;

namespace LibraryApp.Views;

public partial class GenresWindow : Window
{
    private readonly LibraryContext _context;

    public GenresWindow(LibraryContext context)
    {
        InitializeComponent();
        _context = context;
        LoadGenres();
    }

    private void LoadGenres()
    {
        GenresGrid.ItemsSource = _context.Genres.OrderBy(g => g.Name).ToList();
    }

    private void BtnAdd_Click(object sender, RoutedEventArgs e)
    {
        var dlg = new GenreEditDialog();
        if (dlg.ShowDialog() == true)
        {
            _context.Genres.Add(dlg.Genre);
            _context.SaveChanges();
            LoadGenres();
        }
    }

    private void BtnEdit_Click(object sender, RoutedEventArgs e)
    {
        if (GenresGrid.SelectedItem is not Genre selected)
        {
            MessageBox.Show("Выберите жанр.", "Внимание", MessageBoxButton.OK, MessageBoxImage.Information);
            return;
        }
        var dlg = new GenreEditDialog(selected);
        if (dlg.ShowDialog() == true)
        {
            _context.SaveChanges();
            LoadGenres();
        }
    }

    private void BtnDelete_Click(object sender, RoutedEventArgs e)
    {
        if (GenresGrid.SelectedItem is not Genre selected)
        {
            MessageBox.Show("Выберите жанр.", "Внимание", MessageBoxButton.OK, MessageBoxImage.Information);
            return;
        }

        var result = MessageBox.Show(
            $"Удалить жанр «{selected.Name}»?\nВсе книги этого жанра тоже будут удалены!",
            "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Warning);

        if (result == MessageBoxResult.Yes)
        {
            _context.Genres.Remove(selected);
            _context.SaveChanges();
            LoadGenres();
        }
    }

    private void GenresGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e) => BtnEdit_Click(sender, e);
    private void BtnClose_Click(object sender, RoutedEventArgs e) => Close();
}
