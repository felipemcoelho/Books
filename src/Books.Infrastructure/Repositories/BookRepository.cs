using Books.Core.Entities;
using Books.Core.Interfaces;
using Books.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Books.Infrastructure.Repositories;

public class BookRepository : IBookRepository
{
    private readonly AppDbContext _context;

    public BookRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Book>> GetAll()
    {
        return await _context.Books.ToListAsync();
    }

    public async Task<Book> GetById(int id)
    {
        return await _context.Books.FindAsync(id);
    }

    public async Task Add(Book book)
    {
        _context.Books.Add(book);
        await _context.SaveChangesAsync();
    }

    public async Task Update(Book book)
    {
        _context.Entry(book).State = EntityState.Modified;
        await _context.SaveChangesAsync();
    }

    public async Task Delete(Book book)
    {
        _context.Books.Remove(book);
        await _context.SaveChangesAsync();
    }
}