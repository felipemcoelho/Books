using AutoMapper;
using Books.API.DTOs;
using Books.Core.Entities;
using Books.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Books.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class BooksController : ControllerBase
{
    private readonly IBookService _bookService;
    private readonly ICoverImageService _coverImageService;
    private readonly IMapper _mapper;

    public BooksController(IBookService bookService, ICoverImageService coverImageService, IMapper mapper)
    {
        _bookService = bookService;
        _coverImageService = coverImageService;
        _mapper = mapper;
    }

    // GET: api/Books
    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var books = await _bookService.GetAllBooks();
        return Ok(books);
    }

    // GET api/Books/5
    [HttpGet("{id}")]
    public async Task<IActionResult> Get(int id)
    {
        var book = await _bookService.GetBookById(id);

        if (book == null)
            return NotFound();

        return Ok(book);
    }

    // POST api/Books
    [HttpPost]
    public async Task<IActionResult> Post([FromForm] BookDto bookDto)
    {
        if (bookDto.CoverImage != null && bookDto.CoverImage.Length > 0)
        {
            var extension = Path.GetExtension(bookDto.CoverImage.FileName).ToLower();
                
            if (extension is not ".jpg" and not ".jpeg" and not ".png")
                return BadRequest("Invalid file type. Only JPG, JPEG, PNG files are allowed.");
        
            var keyName = $"{Guid.NewGuid()}{extension}";
            var coverImageUrl = await _coverImageService.UploadCoverImage(bookDto.CoverImage.OpenReadStream(), keyName);
            bookDto.CoverImageUrl = coverImageUrl;
        }

        var book = _mapper.Map<Book>(bookDto);
        await _bookService.AddBook(book);

        return CreatedAtAction(nameof(Get), new { id = book.Id }, _mapper.Map<BookDto>(book));
    }

    // PUT api/Books/5
    [HttpPut("{id}")]
    public async Task<IActionResult> Put(int id, [FromForm] BookDto bookDto)
    {
        var book = await _bookService.GetBookById(id);

        if (book == null)
            return NotFound();

        await _coverImageService.DeleteCoverImage(book.CoverImageUrl);

        if (bookDto.CoverImage != null && bookDto.CoverImage.Length > 0)
        {
            var extension = Path.GetExtension(bookDto.CoverImage.FileName).ToLower();
            
            if (extension != ".jpg" && extension != ".jpeg" && extension != ".png")
                return BadRequest("Invalid file type. Only JPG, JPEG, PNG files are allowed.");

            var keyName = $"{Guid.NewGuid()}{extension}";
            var coverImageUrl = await _coverImageService.UploadCoverImage(bookDto.CoverImage.OpenReadStream(), keyName);
            bookDto.CoverImageUrl = coverImageUrl;

            using var memoryStream = new MemoryStream();
            await bookDto.CoverImage.CopyToAsync(memoryStream);
            
            book.CoverImage = memoryStream.ToArray();
        }

        _mapper.Map(bookDto, book);
        
        await _bookService.UpdateBook(book);

        return NoContent();
    }

    // DELETE api/Books/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var book = await _bookService.GetBookById(id);

        if (book == null)
            return NotFound();

        // Delete cover image
        await _coverImageService.DeleteCoverImage(book.CoverImageUrl);

        await _bookService.DeleteBook(book);

        return NoContent();
    }
}