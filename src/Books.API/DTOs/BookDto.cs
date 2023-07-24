namespace Books.API.DTOs;

public class BookDto
{
    public string? Title { get; set; }
    public string? Author { get; set; }
    public DateTime ReleaseDate { get; set; }
    public string CoverImageUrl { get; set; }
    public IFormFile? CoverImage { get; set; }
}