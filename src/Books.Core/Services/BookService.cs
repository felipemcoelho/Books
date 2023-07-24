using Books.Core.Entities;
using Books.Core.Interfaces;

namespace Books.Core.Services;

public class BookService : IBookService
{
    private readonly IBookRepository _bookRepository;

    public BookService(IBookRepository bookRepository)
    {
        _bookRepository = bookRepository;
    }

    public Task<List<Book>> GetAllBooks()
    {
        return _bookRepository.GetAll();
    }

    public Task<Book> GetBookById(int id)
    {
        return _bookRepository.GetById(id);
    }

    public async Task AddBook(Book book)
    {
        var existingBook = (await _bookRepository.GetAll()).FirstOrDefault(b => b.Title == book.Title);

        if (existingBook != null)
            throw new Exception("A book with the same title already exists.");

        await _bookRepository.Add(book);
    }

    public async Task UpdateBook(Book book)
    {
        var existingBook = (await _bookRepository.GetAll()).FirstOrDefault(b => b.Title == book.Title && b.Id != book.Id);

        if (existingBook != null)
            throw new Exception("A book with the same title already exists.");

        await _bookRepository.Update(book);
    }

    public Task DeleteBook(Book book)
    {
        return _bookRepository.Delete(book);
    }
}