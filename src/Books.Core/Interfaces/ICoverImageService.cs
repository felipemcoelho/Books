namespace Books.Core.Interfaces;

public interface ICoverImageService
{
    Task<string> UploadCoverImage(Stream coverImageStream, string keyName);
    Task DeleteCoverImage(string? keyName);
}