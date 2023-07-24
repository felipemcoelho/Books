using Books.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace Books.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public virtual DbSet<Book> Books { get; set; }
}