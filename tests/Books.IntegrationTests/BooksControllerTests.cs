using System.Globalization;
using System.Net.Http.Headers;
using Xunit;
using Microsoft.AspNetCore.Mvc.Testing;
using Books.API;
using Books.API.DTOs;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Books.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Books.Core.Entities;

namespace Books.IntegrationTests
{
    public class BooksControllerTests : IClassFixture<WebApplicationFactory<Startup>>, IDisposable
    {
        private readonly WebApplicationFactory<Startup> _factory;
        private readonly HttpClient _client;
        private AppDbContext _context;

        public BooksControllerTests(WebApplicationFactory<Startup> factory)
        {
            _factory = factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureTestServices(services =>
                {
                    var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<AppDbContext>));

                    services.Remove(descriptor);
                    services.AddDbContext<AppDbContext>(options => { options.UseInMemoryDatabase("InMemoryDbForTesting"); });

                    var sp = services.BuildServiceProvider();
                    _context = sp.GetRequiredService<AppDbContext>();
                });
            });

            _client = _factory.CreateClient();

            InitializeAsync().GetAwaiter().GetResult();
        }

        private async Task InitializeAsync()
        {
            _context.Books.RemoveRange(_context.Books);

            // Create test data
            var book = new Book
            {
                Id = 1,
                Title = "Initial Book",
                Author = "Initial Author",
                ReleaseDate = DateTime.Now.AddYears(-1), 
                CoverImage = "Initial image content"u8.ToArray() 
            };

            _context.Books.Add(book);

            await _context.SaveChangesAsync();
        }

        [Fact]
        public async Task GetAll_ShouldReturnOk()
        {
            // Arrange
            var url = "/api/books";

            // Act
            var response = await _client.GetAsync(url);

            // Assert
            response.EnsureSuccessStatusCode();
        }

        [Fact]
        public async Task GetById_WhenBookExists_ShouldReturnOk()
        {
            // Arrange
            var url = "/api/books/1";

            // Act
            var response = await _client.GetAsync(url);

            // Assert
            response.EnsureSuccessStatusCode();
        }

        [Fact]
        public async Task Post_ShouldAddBook()
        {
            // Arrange
            var url = "/api/books";
            
            var bookDto = new BookDto
            {
                Title = "Test Book",
                Author = "Test Author",
                ReleaseDate = DateTime.Now
            };

            var fileContent = new ByteArrayContent("Test image content"u8.ToArray());
            fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse("image/jpeg");

            var multipartContent = new MultipartFormDataContent
            {
                { new StringContent(bookDto.Title), "Title" },
                { new StringContent(bookDto.Author), "Author" },
                { new StringContent(bookDto.ReleaseDate.ToString(CultureInfo.InvariantCulture)), "ReleaseDate" },
                { fileContent, "CoverImage", "testImage.jpg" }
            };

            // Act
            var response = await _client.PostAsync(url, multipartContent);

            // Assert
            response.EnsureSuccessStatusCode();
        }

        [Fact]
        public async Task Put_ShouldUpdateBook()
        {
            // Arrange
            var url = "/api/books/1"; 
            var updatedBookDto = new BookDto
            {
                Title = "Updated Book",
                Author = "Updated Author",
                ReleaseDate = DateTime.Now
            };

            var fileContent = new ByteArrayContent("Updated image content"u8.ToArray());
            fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse("image/jpeg");

            var multipartContent = new MultipartFormDataContent
            {
                { new StringContent(updatedBookDto.Title), "Title" },
                { new StringContent(updatedBookDto.Author), "Author" },
                { new StringContent(updatedBookDto.ReleaseDate.ToString(CultureInfo.InvariantCulture)), "ReleaseDate" },
                { fileContent, "CoverImage", "updatedImage.jpg" }
            };

            // Act
            var response = await _client.PutAsync(url, multipartContent);

            // Assert
            response.EnsureSuccessStatusCode();
        }

        [Fact]
        public async Task Delete_ShouldRemoveBook()
        {
            // Arrange
            var url = "/api/books/1";

            // Act
            var response = await _client.DeleteAsync(url);

            // Assert
            response.EnsureSuccessStatusCode();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
