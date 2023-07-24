using Books.Core.Entities;

namespace Books.Core.Interfaces;

public interface IBookService
{
    Task<List<Book>> GetAllBooks();
    Task<Book> GetBookById(int id);
    Task AddBook(Book book);
    Task UpdateBook(Book book);
    Task DeleteBook(Book book);
}