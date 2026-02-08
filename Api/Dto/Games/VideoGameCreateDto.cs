using Microsoft.AspNetCore.Http;

namespace VideoGameApi.Api.Dto.Games
{
    public class VideoGameCreateDto
    {
        public string Title { get; set; } = null!;
        public string? Platform { get; set; }
        public string? Developer { get; set; }
        public string? Publisher { get; set; }

        public decimal Price { get; set; }
        public IFormFile? Image { get; set; }
    }
}
