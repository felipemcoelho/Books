using System.Net;
using Amazon.S3;
using Amazon.S3.Model;
using Books.Core.Interfaces;
using Microsoft.Extensions.Configuration;

namespace Books.Core.Services;

public class CoverImageService : ICoverImageService
{
    private readonly IAmazonS3 _s3Client;
    private readonly string? _bucketName;

    public CoverImageService(IAmazonS3 s3Client, IConfiguration configuration)
    {
        _s3Client = s3Client;
        _bucketName = configuration["AWS:S3BucketName"];
    }

    public async Task<string> UploadCoverImage(Stream coverImageStream, string keyName)
    {
        var putRequest = new PutObjectRequest
        {
            BucketName = _bucketName,
            Key = keyName,
            InputStream = coverImageStream
        };

        var response = await _s3Client.PutObjectAsync(putRequest);
    
        if (response.HttpStatusCode != HttpStatusCode.OK)
            throw new Exception($"Failed to upload cover image to S3. Status code: {response.HttpStatusCode}");

        return $"https://{_bucketName}.s3.amazonaws.com/{keyName}";
    }

    public async Task DeleteCoverImage(string? keyName)
    {
        var deleteRequest = new DeleteObjectRequest
        {
            BucketName = _bucketName,
            Key = keyName
        };

        var response = await _s3Client.DeleteObjectAsync(deleteRequest);
        
        if (response.HttpStatusCode != HttpStatusCode.NoContent)
            throw new Exception($"Failed to delete cover image from S3. Status code: {response.HttpStatusCode}");
    }
}