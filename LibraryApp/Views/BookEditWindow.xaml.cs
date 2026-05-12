using LibraryApp.Data;
using LibraryApp.Models;
using System.Windows;

namespace LibraryApp.Views;

public partial class BookEditWindow : Window
{
    private readonly LibraryContext _context;
    private readonly int? _bookId; // null = новая книга

    public BookEditWindow(LibraryContext context, int? bookId = null)
    {
        InitializeComponent();
        _context = context;
        _bookId = bookId;

        LoadCombos();

        if (bookId.HasValue)
        {
            TxtWindowTitle.Text = "Редактирование книги";
            LoadBook(bookId.Value);
        }
        else
        {
            TxtYear.Text = DateTime.Now.Year.ToString();
            TxtQuantity.Text = "1";
        }
    }

    private void LoadCombos()
    {
        CmbAuthor.ItemsSource = _context.Authors.OrderBy(a => a.LastName).ToList();
        CmbGenre.ItemsSource = _context.Genres.OrderBy(g => g.Name).ToList();
    }

    private void LoadBook(int id)
    {
        var book = _context.Books.Find(id);
        if (book == null) return;

        TxtTitle.Text = book.Title;
        TxtYear.Text = book.PublishYear.ToString();
        TxtISBN.Text = book.ISBN;
        TxtQuantity.Text = book.QuantityInStock.ToString();

        CmbAuthor.SelectedValue = book.AuthorId;
        CmbAuthor.DisplayMemberPath = "FullName";
        CmbAuthor.SelectedItem = _context.Authors.Find(book.AuthorId);

        CmbGenre.SelectedItem = _context.Genres.Find(book.GenreId);
    }

    private void BtnSave_Click(object sender, RoutedEventArgs e)
    {
        // Валидация
        if (string.IsNullOrWhiteSpace(TxtTitle.Text))
        {
            MessageBox.Show("Введите название книги.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
            TxtTitle.Focus();
            return;
        }

        if (CmbAuthor.SelectedItem is not Author author)
        {
            MessageBox.Show("Выберите автора.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        if (CmbGenre.SelectedItem is not Genre genre)
        {
            MessageBox.Show("Выберите жанр.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        if (!int.TryParse(TxtYear.Text, out int year) || year < 1 || year > 2100)
        {
            MessageBox.Show("Введите корректный год издания.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
            TxtYear.Focus();
            return;
        }

        if (!int.TryParse(TxtQuantity.Text, out int qty) || qty < 0)
        {
            MessageBox.Show("Количество должно быть числом >= 0.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
            TxtQuantity.Focus();
            return;
        }

        try
        {
            if (_bookId.HasValue)
            {
                // Редактирование
                var book = _context.Books.Find(_bookId.Value)!;
                book.Title = TxtTitle.Text.Trim();
                book.AuthorId = author.Id;
                book.GenreId = genre.Id;
                book.PublishYear = year;
                book.ISBN = TxtISBN.Text.Trim();
                book.QuantityInStock = qty;
            }
            else
            {
                // Создание
                var book = new Book
                {
                    Title = TxtTitle.Text.Trim(),
                    AuthorId = author.Id,
                    GenreId = genre.Id,
                    PublishYear = year,
                    ISBN = TxtISBN.Text.Trim(),
                    QuantityInStock = qty
                };
                _context.Books.Add(book);
            }

            _context.SaveChanges();
            DialogResult = true;
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Ошибка сохранения:\n{ex.Message}", "Ошибка",
                MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void BtnCancel_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
    }
}
