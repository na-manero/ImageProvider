using ImageProvider.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace ImageProvider.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ImageController(IBlobStorageService blobStorageService) : ControllerBase
{
    private readonly IBlobStorageService _blobService = blobStorageService;
    private readonly string _containerName = "images";

    [HttpPost("upload")]
    public async Task<IActionResult> UploadImage(IFormFile file)
    {
        if (string.IsNullOrWhiteSpace(file.FileName))
            return BadRequest("File must be provided.");

        var result = await _blobService.UploadImageAsync(file, _containerName);

        if (!string.IsNullOrEmpty(result))
        {
            return Ok(new { Url = result });
        }

        return BadRequest("Failed to upload image");
    }

    [HttpDelete("delete/{imageName}")]
    public async Task<IActionResult> DeleteImage(string imageName)
    {
        if (string.IsNullOrWhiteSpace(imageName))
            return BadRequest("Image name must be provided.");

        var result = await _blobService.DeleteImageAsync(imageName, _containerName);

        return result ? Ok() : BadRequest("Failed to delete image");
    }
}
