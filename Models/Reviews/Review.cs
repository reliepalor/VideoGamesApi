using System.ComponentModel.DataAnnotations;
using VideoGameApi.Models.Games;
using VideoGameApi.Models.Orders;
using VideoGameApi.Models.Users;

namespace VideoGameApi.Models.Reviews
{
    public class Review
    {
        [Key]
        public int Id { get; set; }

        public int VideoGameId { get; set; }
        public VideoGame VideoGame { get; set; } = null!;

        public int UserId { get; set; }
        public User User { get; set; } = null!;

        [Range(1, 5)]
        public int Rating { get; set; }

        [MaxLength(500)]
        public string? Comment { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public int OrderId { get; set; }
        public Order Order { get; set; } = null!;

    }
}
