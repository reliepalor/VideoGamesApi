using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VideoGameApi.Models.Carts
{
    public class Cart
    {
        public int Id { get; set; }

        public int UserId { get; set; }


        public List<CartItem> Items { get; set; } = new();
    }
}
