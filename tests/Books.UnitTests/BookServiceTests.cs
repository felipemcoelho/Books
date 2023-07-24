using Moq;
using Xunit;
using Books.Core.Services;
using Books.Core.Entities;
using Books.Core.Interfaces;

namespace Books.UnitTests;

public class BookServiceTests
{
    private readonly BookService _bookService;
    private readonly Mock<IBookRepository> _bookRepositoryMock = new();

    public BookServiceTests()
    {
        _bookService = new BookService(_bookRepositoryMock.Object);
    }

    [Fact]
    public async Task GetAllBooks_ShouldReturnAllBooks()
    {
        // Arrange
        var books = new List<Book> { new(), new() };
        _bookRepositoryMock.Setup(x => x.GetAll()).ReturnsAsync(books);

        // Act
        var result = await _bookService.GetAllBooks();

        // Assert
        Assert.Equal(books, result);
        _bookRepositoryMock.Verify(x => x.GetAll(), Times.Once);
    }

    [Fact]
    public async Task GetBookById_ShouldReturnBook()
    {
        // Arrange
        var book = new Book();
        _bookRepositoryMock.Setup(x => x.GetById(It.IsAny<int>())).ReturnsAsync(book);

        // Act
        var result = await _bookService.GetBookById(1);

        // Assert
        Assert.Equal(book, result);
        _bookRepositoryMock.Verify(x => x.GetById(It.IsAny<int>()), Times.Once);
    }

    [Fact]
    public async Task AddBook_ShouldAddBook()
    {
        // Arrange
        var book = new Book();

        // Act
        await _bookService.AddBook(book);

        // Assert
        _bookRepositoryMock.Verify(x => x.Add(It.IsAny<Book>()), Times.Once);
    }

    [Fact]
    public async Task UpdateBook_ShouldUpdateBook()
    {
        // Arrange
        var book = new Book();

        // Act
        await _bookService.UpdateBook(book);

        // Assert
        _bookRepositoryMock.Verify(x => x.Update(It.IsAny<Book>()), Times.Once);
    }

    [Fact]
    public async Task DeleteBook_ShouldDeleteBook()
    {
        // Arrange
        var book = new Book();

        // Act
        await _bookService.DeleteBook(book);

        // Assert
        _bookRepositoryMock.Verify(x => x.Delete(It.IsAny<Book>()), Times.Once);
    }
}