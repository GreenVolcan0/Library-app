using LibraryApp.Data;
using LibraryApp.Models;
using Microsoft.EntityFrameworkCore;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace LibraryApp.Views;

public partial class MainWindow : Window
{
    private LibraryContext _context = new LibraryContext();
    private List<Book> _allBooks = new();

    public MainWindow()
    {
        InitializeComponent();
        LoadData();
    }

    // =============================
    //   Загрузка и фильтрация данных
    // =============================

    private void LoadData()
    {
        try
        {
            // Загружаем все книги с зависимостями
            _allBooks = _context.Books
                .Include(b => b.Author)
                .Include(b => b.Genre)
                .OrderBy(b => b.Title)
                .ToList();

            // Заполняем фильтры (с пустым пунктом "Все")
            var allAuthors = _context.Authors.OrderBy(a => a.LastName).ToList();
            var allGenres = _context.Genres.OrderBy(g => g.Name).ToList();

            CmbAuthorFilter.ItemsSource = new[] { new Author { Id = 0, FirstName = "Все", LastName = "авторы" } }
                .Concat(allAuthors).ToList();
            CmbAuthorFilter.SelectedIndex = 0;

            CmbGenreFilter.ItemsSource = new[] { new Genre { Id = 0, Name = "Все жанры" } }
                .Concat(allGenres).ToList();
            CmbGenreFilter.SelectedIndex = 0;

            ApplyFilters();
            UpdateHeader();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Ошибка загрузки данных:\n{ex.Message}", "Ошибка",
                MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void ApplyFilters()
    {
        var filtered = _allBooks.AsEnumerable();

        // Фильтр по поиску
        string search = TxtSearch.Text.Trim().ToLower();
        if (!string.IsNullOrEmpty(search))
            filtered = filtered.Where(b => b.Title.ToLower().Contains(search));

        // Фильтр по жанру
        if (CmbGenreFilter.SelectedItem is Genre g && g.Id != 0)
            filtered = filtered.Where(b => b.GenreId == g.Id);

        // Фильтр по автору
        if (CmbAuthorFilter.SelectedItem is Author a && a.Id != 0)
            filtered = filtered.Where(b => b.AuthorId == a.Id);

        BooksGrid.ItemsSource = filtered.ToList();
        UpdateStatus(filtered.Count());
    }

    private void UpdateHeader()
    {
        TxtBookCount.Text = $"Всего книг: {_allBooks.Count} | Авторов: {_context.Authors.Count()} | Жанров: {_context.Genres.Count()}";
    }

    private void UpdateStatus(int count)
    {
        TxtStatus.Text = $"Найдено книг: {count}";
    }

    // =============================
    //   Обработчики событий
    // =============================

    private void TxtSearch_TextChanged(object sender, TextChangedEventArgs e) => ApplyFilters();
    private void Filter_Changed(object sender, SelectionChangedEventArgs e) => ApplyFilters();

    private void BtnClearFilters_Click(object sender, RoutedEventArgs e)
    {
        TxtSearch.Clear();
        CmbGenreFilter.SelectedIndex = 0;
        CmbAuthorFilter.SelectedIndex = 0;
    }

    private void BtnAddBook_Click(object sender, RoutedEventArgs e)
    {
        var dlg = new BookEditWindow(_context);
        if (dlg.ShowDialog() == true)
            LoadData();
    }

    private void BtnEditBook_Click(object sender, RoutedEventArgs e)
    {
        if (BooksGrid.SelectedItem is not Book book)
        {
            MessageBox.Show("Выберите книгу для редактирования.", "Внимание",
                MessageBoxButton.OK, MessageBoxImage.Information);
            return;
        }
        var dlg = new BookEditWindow(_context, book.Id);
        if (dlg.ShowDialog() == true)
            LoadData();
    }

    private void BtnDeleteBook_Click(object sender, RoutedEventArgs e)
    {
        if (BooksGrid.SelectedItem is not Book book)
        {
            MessageBox.Show("Выберите книгу для удаления.", "Внимание",
                MessageBoxButton.OK, MessageBoxImage.Information);
            return;
        }

        var result = MessageBox.Show(
            $"Удалить книгу «{book.Title}»?\nЭто действие нельзя отменить.",
            "Подтверждение удаления",
            MessageBoxButton.YesNo, MessageBoxImage.Warning);

        if (result == MessageBoxResult.Yes)
        {
            _context.Books.Remove(book);
            _context.SaveChanges();
            LoadData();
        }
    }

    private void BtnManageAuthors_Click(object sender, RoutedEventArgs e)
    {
        var dlg = new AuthorsWindow(_context);
        dlg.ShowDialog();
        LoadData(); // перезагружаем, т.к. авторы могли измениться
    }

    private void BtnManageGenres_Click(object sender, RoutedEventArgs e)
    {
        var dlg = new GenresWindow(_context);
        dlg.ShowDialog();
        LoadData();
    }

    // Двойной клик — открыть редактирование
    private void BooksGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
        if (BooksGrid.SelectedItem is Book)
            BtnEditBook_Click(sender, e);
    }

    protected override void OnClosed(EventArgs e)
    {
        _context.Dispose();
        base.OnClosed(e);
    }
}
