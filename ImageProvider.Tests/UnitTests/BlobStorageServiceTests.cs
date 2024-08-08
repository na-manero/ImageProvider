using Azure.Storage.Blobs;
using ImageProvider.Api.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Moq;

namespace ImageProvider.Tests.UnitTests;

public class BlobStorageServiceTests
{
    private readonly Mock<BlobServiceClient> _blobServiceClientMock;
    private readonly Mock<IConfiguration> _configurationMock;
    private readonly BlobStorageService _blobStorageService;

    public BlobStorageServiceTests()
    {
        _blobServiceClientMock = new Mock<BlobServiceClient>();
        _configurationMock = new Mock<IConfiguration>();
        _blobStorageService = new BlobStorageService(_blobServiceClientMock.Object, _configurationMock.Object);
    }

    [Fact]
    public async Task UploadImageAsync_Success()
    {
        // Arrange
        var containerName = "test-container";
        var fileName = "test-image.jpg";
        var fileMock = new Mock<IFormFile>();
        fileMock.Setup(f => f.FileName).Returns(fileName);
        fileMock.Setup(f => f.ContentType).Returns("image/jpeg");
        fileMock.Setup(f => f.OpenReadStream()).Returns(new MemoryStream());

        var blobContainerClientMock = new Mock<BlobContainerClient>();
        var blobClientMock = new Mock<BlobClient>();

        _blobServiceClientMock.Setup(c => c.GetBlobContainerClient(containerName)).Returns(blobContainerClientMock.Object);

        blobContainerClientMock.Setup(c => c.GetBlobClient(fileName)).Returns(blobClientMock.Object);

        blobClientMock.Setup(b => b.Uri).Returns(new Uri($"https://fakeuri.com/{containerName}/{fileName}"));

        // Act
        var result = await _blobStorageService.UploadImageAsync(fileMock.Object, containerName);

        // Assert
        Assert.Equal($"https://fakeuri.com/{containerName}/{fileName}", result);
    }

    [Fact]
    public async Task UploadImageAsync_Failure()
    {
        // Arrange
        var containerName = "test-container";
        var fileName = "test-image.jpg";
        var fileMock = new Mock<IFormFile>();
        fileMock.Setup(f => f.FileName).Returns(fileName);
        fileMock.Setup(f => f.ContentType).Returns("image/jpeg");
        fileMock.Setup(f => f.OpenReadStream()).Returns(new MemoryStream());

        var blobContainerClientMock = new Mock<BlobContainerClient>();

        _blobServiceClientMock.Setup(c => c.GetBlobContainerClient(containerName)).Returns(blobContainerClientMock.Object);

        blobContainerClientMock.Setup(c => c.GetBlobClient(fileName)).Throws(new Exception("Upload failed"));

        // Act
        var result = await _blobStorageService.UploadImageAsync(fileMock.Object, containerName);

        // Assert
        Assert.Equal(string.Empty, result);
    }
}
