using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NZWalks.API.Models.Domain;
using NZWalks.API.Models.DTO;
using NZWalks.API.Repositories;

namespace NZWalks.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImagesController : ControllerBase
    {
        private readonly IImageRepository imageRepository;

        public ImagesController(IImageRepository imageRepository)
        {
            this.imageRepository = imageRepository;
        }

        [HttpPost]
        [Route("Upload")]
        public async Task<IActionResult> Upload([FromForm] ImageUploadRequestDto requestDto)
        {
            ValidateFileUpload(requestDto);
            if(!ModelState.IsValid) return BadRequest(ModelState);
            var imageDomainModel = new Image
            {
                File = requestDto.File,
                FileExtention = Path.GetExtension(requestDto.File.FileName),
                FileSizeInBytes = requestDto.File.Length,
                FileName = requestDto.File.FileName,
                FileDescription = requestDto.FileDescription,
            };
            await imageRepository.Upload(imageDomainModel);
            return Ok(imageDomainModel);
        }

        private void ValidateFileUpload(ImageUploadRequestDto requestDto)
        {
            var allowedExtentions = new string[]
            {
                ".jpg",
                ".jpeg",
                ".png"
            };
            if (!allowedExtentions.Contains(Path.GetExtension(requestDto.FileName)))
            {
                ModelState.AddModelError("file", "Unsupported file extention");

            }
            if (requestDto.File.Length > 10485760)
            {
                ModelState.AddModelError("file", "File size more than 10MB, please upload a smaller file");
            }
        }
    }
}
