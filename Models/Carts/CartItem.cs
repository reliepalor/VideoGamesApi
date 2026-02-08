using System.ComponentModel.DataAnnotations;
using VideoGameApi.Models.Games;

namespace VideoGameApi.Models.Carts
{
    public class CartItem
    {
        public int Id { get; set; }

        public int CartId { get; set; }
        public Cart Cart { get; set; } = null!;

        public int VideoGameId { get; set; }
        public VideoGame VideoGame { get; set; } = null!;

        public int Quantity { get; set; }
    }

}
