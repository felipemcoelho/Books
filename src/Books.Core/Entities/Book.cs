namespace Books.Core.Entities;

public class Book
{
    public int Id { get; set; }
    public string? Title { get; set; }
    public string? Author { get; set; }
    public DateTime ReleaseDate { get; set; }
    public string? CoverImageUrl { get; set; }
    public byte[]? CoverImage { get; set; }
}