using System.ComponentModel.DataAnnotations;

namespace NZWalks.API.Models.DTO
{
    public class UpdateRegionRequestDto
    {
        [Required]
        [MinLength(2, ErrorMessage = "Code too short")]
        [MaxLength(7, ErrorMessage = "Code too long")]
        public string Code { get; set; }
        [Required]
        [MaxLength(100, ErrorMessage = "Name too long")]
        public string Name { get; set; }
        public string? RegionImageURL { get; set; }
    }
}
