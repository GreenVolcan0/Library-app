using LibraryApp.Models;
using System.Windows;

namespace LibraryApp.Views;

public partial class GenreEditDialog : Window
{
    public Genre Genre { get; private set; }

    public GenreEditDialog(Genre? existing = null)
    {
        InitializeComponent();

        if (existing != null)
        {
            Genre = existing;
            TxtTitle.Text = "Редактирование жанра";
            TxtName.Text = existing.Name;
            TxtDescription.Text = existing.Description;
        }
        else
        {
            Genre = new Genre();
        }
    }

    private void BtnSave_Click(object sender, RoutedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(TxtName.Text))
        {
            MessageBox.Show("Введите название жанра.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
            TxtName.Focus();
            return;
        }

        Genre.Name = TxtName.Text.Trim();
        Genre.Description = TxtDescription.Text.Trim();
        DialogResult = true;
    }

    private void BtnCancel_Click(object sender, RoutedEventArgs e) => DialogResult = false;
}
