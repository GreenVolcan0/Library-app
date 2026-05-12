using LibraryApp.Models;
using Microsoft.EntityFrameworkCore;

namespace LibraryApp.Data;

public class LibraryContext : DbContext
{
    public DbSet<Book> Books { get; set; }
    public DbSet<Author> Authors { get; set; }
    public DbSet<Genre> Genres { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        // SQLite база данных — файл создаётся рядом с .exe
        optionsBuilder.UseSqlite("Data Source=library.db");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // ===== Конфигурация Author =====
        modelBuilder.Entity<Author>(entity =>
        {
            entity.HasKey(a => a.Id);

            entity.Property(a => a.FirstName)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(a => a.LastName)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(a => a.Country)
                .HasMaxLength(100);

            entity.Property(a => a.BirthDate)
                .IsRequired(false);
        });

        // ===== Конфигурация Genre =====
        modelBuilder.Entity<Genre>(entity =>
        {
            entity.HasKey(g => g.Id);

            entity.Property(g => g.Name)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(g => g.Description)
                .HasMaxLength(500);
        });

        // ===== Конфигурация Book =====
        modelBuilder.Entity<Book>(entity =>
        {
            entity.HasKey(b => b.Id);

            entity.Property(b => b.Title)
                .IsRequired()
                .HasMaxLength(300);

            entity.Property(b => b.ISBN)
                .HasMaxLength(20);

            entity.Property(b => b.PublishYear)
                .IsRequired();

            entity.Property(b => b.QuantityInStock)
                .IsRequired()
                .HasDefaultValue(0);

            // Связь Book -> Author (один автор — много книг)
            entity.HasOne(b => b.Author)
                .WithMany(a => a.Books)
                .HasForeignKey(b => b.AuthorId)
                .OnDelete(DeleteBehavior.Cascade); // каскадное удаление

            // Связь Book -> Genre (один жанр — много книг)
            entity.HasOne(b => b.Genre)
                .WithMany(g => g.Books)
                .HasForeignKey(b => b.GenreId)
                .OnDelete(DeleteBehavior.Cascade); // каскадное удаление
        });

        // ===== Начальные данные (seed data) =====
        modelBuilder.Entity<Author>().HasData(
            new Author { Id = 1, FirstName = "Лев", LastName = "Толстой", BirthDate = new DateTime(1828, 9, 9), Country = "Россия" },
            new Author { Id = 2, FirstName = "Фёдор", LastName = "Достоевский", BirthDate = new DateTime(1821, 11, 11), Country = "Россия" },
            new Author { Id = 3, FirstName = "Михаил", LastName = "Булгаков", BirthDate = new DateTime(1891, 5, 15), Country = "Россия" }
        );

        modelBuilder.Entity<Genre>().HasData(
            new Genre { Id = 1, Name = "Роман", Description = "Крупное прозаическое произведение" },
            new Genre { Id = 2, Name = "Классика", Description = "Произведения классической литературы" },
            new Genre { Id = 3, Name = "Фантастика", Description = "Научная фантастика и фэнтези" }
        );

        modelBuilder.Entity<Book>().HasData(
            new Book { Id = 1, Title = "Война и мир", AuthorId = 1, GenreId = 1, PublishYear = 1869, ISBN = "978-5-17-090000-1", QuantityInStock = 5 },
            new Book { Id = 2, Title = "Анна Каренина", AuthorId = 1, GenreId = 1, PublishYear = 1877, ISBN = "978-5-17-090000-2", QuantityInStock = 3 },
            new Book { Id = 3, Title = "Преступление и наказание", AuthorId = 2, GenreId = 2, PublishYear = 1866, ISBN = "978-5-17-090000-3", QuantityInStock = 7 },
            new Book { Id = 4, Title = "Мастер и Маргарита", AuthorId = 3, GenreId = 2, PublishYear = 1967, ISBN = "978-5-17-090000-4", QuantityInStock = 4 }
        );
    }
}
