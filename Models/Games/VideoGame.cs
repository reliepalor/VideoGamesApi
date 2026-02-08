using System.ComponentModel.DataAnnotations;
using VideoGameApi.Models.Reviews;

namespace VideoGameApi.Models.Games
{
    public class VideoGame
    {
        public int Id { get; set; }

        [Required]
        public string Title { get; set; } = null!;

        public string? Platform { get; set; }
        public string? Developer { get; set; }
        public string? Publisher { get; set; }

        [Range(0, 999999)]
        public decimal Price { get; set; }

        public string? ImagePath { get; set; }

        public List<Review> Reviews { get; set; } = new();

        public decimal AverageRating =>
            Reviews.Any() ? Math.Round((decimal)Reviews.Average(r => r.Rating), 1) : 0;
    }
}
