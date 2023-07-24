using Books.Core.Entities;
using Books.Infrastructure.Data;
using Books.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Books.UnitTests;

public class BookRepositoryTests
{
    private readonly AppDbContext _context;
    private readonly BookRepository _bookRepository;

    public BookRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDatabase")
            .Options;

        _context = new AppDbContext(options);

        _context.Books.RemoveRange(_context.Books);

        byte[] coverImage = Array.Empty<byte>();
        _context.Books.Add(new Book { Id = 1, Title = "Book 1", Author = "Author 1", ReleaseDate = new DateTime(2023, 1, 1), CoverImage = coverImage, CoverImageUrl = "https://test.com/Cover1.png" });
        _context.Books.Add(new Book { Id = 2, Title = "Book 2", Author = "Author 2", ReleaseDate = new DateTime(2023, 1, 1), CoverImage = coverImage, CoverImageUrl = "https://test.com/Cover2.png" });
        _context.SaveChanges();

        _bookRepository = new BookRepository(_context);
    }

    [Fact]
    public async Task GetAllBooks_ReturnsAllBooks()
    {
        // Act
        var result = await _bookRepository.GetAll();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
    }

    [Fact]
    public async Task GetBookById_ReturnsCorrectBook()
    {
        // Arrange
        var id = 1;

        // Act
        var result = await _bookRepository.GetById(id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(id, result.Id);
    }

    [Fact]
    public async Task AddBook_ValidBook_AddsBook()
    {
        // Arrange
        var book = new Book
        {
            Title = "Book 3",
            Author = "Author 3",
            ReleaseDate = new DateTime(2023, 7, 23),
            CoverImage = Array.Empty<byte>(),
            CoverImageUrl = "https://test.com/Cover3.png"
        };

        // Act
        await _bookRepository.Add(book);

        // Assert
        Assert.Contains(_context.Books, b => b.Title == "Book 3");
    }

    [Fact]
    public async Task UpdateBook_ValidBook_UpdatesBook()
    {
        // Arrange
        var id = 1;
        var bookToUpdate = await _bookRepository.GetById(id);
        bookToUpdate.Title = "Updated Title";

        // Act
        await _bookRepository.Update(bookToUpdate);

        // Assert
        var updatedBook = await _bookRepository.GetById(id);
        Assert.Equal("Updated Title", updatedBook.Title);
    }


    [Fact]
    public async Task DeleteBook_ValidBook_DeletesBook()
    {
        // Arrange
        var id = 1;
        var book = await _bookRepository.GetById(id);

        // Act
        await _bookRepository.Delete(book);

        // Assert
        var deletedBook = await _bookRepository.GetById(id);
        Assert.Null(deletedBook);
    }
}