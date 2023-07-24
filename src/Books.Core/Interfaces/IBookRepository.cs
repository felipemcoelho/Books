using Books.Core.Entities;

namespace Books.Core.Interfaces;

public interface IBookRepository
{
    Task Add(Book book);
    Task Update(Book book);
    Task<List<Book>> GetAll();
    Task<Book> GetById(int id);
    Task Delete(Book book);
}