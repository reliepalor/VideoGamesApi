using System.ComponentModel.DataAnnotations;
using VideoGameApi.Models.Conversations;
using VideoGameApi.Models.Orders;
using VideoGameApi.Models.DigitalOrders;

namespace VideoGameApi.Models.Users
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Username { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;

        public string? Token { get; set; }
        public string? VerifiedToken { get; set; }

        //google auth
        public string? AuthProvider { get; set; }
        public string? FirebaseUid { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime LastLoginAt { get; set; }


        public bool IsAdmin { get; set; } = false;
        public bool IsExternalAuth { get; set; } = false;

        public ICollection<Order> Orders { get; set; } = new List<Order>();
        public ICollection<Conversation> Conversations { get; set; } = new List<Conversation>();
        public ICollection<ConversationMessage> ConversationMessages { get; set; } = new List<ConversationMessage>();

        public ICollection<DigitalOrder> Digitalorders { get; set; } = new List<DigitalOrder>();
    }
}
