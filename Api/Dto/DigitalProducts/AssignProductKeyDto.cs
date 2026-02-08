
using System.ComponentModel.DataAnnotations;
namespace VideoGameApi.Api.Dto.DigitalProducts
{
    public class AssignProductKeyDto
    {
        [Required]
        public int DigitalProductId { get; set; }
        [Required]
        public int UserID { get; set; }
    }
}
