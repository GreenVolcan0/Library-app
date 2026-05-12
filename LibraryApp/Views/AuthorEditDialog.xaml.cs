using LibraryApp.Models;
using System.Windows;

namespace LibraryApp.Views;

public partial class AuthorEditDialog : Window
{
    public Author Author { get; private set; }

    public AuthorEditDialog(Author? existing = null)
    {
        InitializeComponent();

        if (existing != null)
        {
            Author = existing;
            TxtTitle.Text = "Редактирование автора";
            TxtFirstName.Text = existing.FirstName;
            TxtLastName.Text = existing.LastName;
            TxtCountry.Text = existing.Country;
            TxtBirthDate.Text = existing.BirthDate?.ToString("dd.MM.yyyy") ?? "";
        }
        else
        {
            Author = new Author();
        }
    }

    private void BtnSave_Click(object sender, RoutedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(TxtFirstName.Text))
        {
            MessageBox.Show("Введите имя автора.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
            TxtFirstName.Focus();
            return;
        }
        if (string.IsNullOrWhiteSpace(TxtLastName.Text))
        {
            MessageBox.Show("Введите фамилию автора.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
            TxtLastName.Focus();
            return;
        }

        // Парсинг даты (необязательно)
        DateTime? birthDate = null;
        if (!string.IsNullOrWhiteSpace(TxtBirthDate.Text))
        {
            if (!DateTime.TryParseExact(TxtBirthDate.Text, "dd.MM.yyyy",
                System.Globalization.CultureInfo.InvariantCulture,
                System.Globalization.DateTimeStyles.None, out var parsedDate))
            {
                MessageBox.Show("Неверный формат даты. Используйте ДД.ММ.ГГГГ", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            birthDate = parsedDate;
        }

        Author.FirstName = TxtFirstName.Text.Trim();
        Author.LastName = TxtLastName.Text.Trim();
        Author.Country = TxtCountry.Text.Trim();
        Author.BirthDate = birthDate;

        DialogResult = true;
    }

    private void BtnCancel_Click(object sender, RoutedEventArgs e) => DialogResult = false;
}
