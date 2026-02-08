using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using VideoGameApi.Interfaces.DigitalProducts;

namespace VideoGameApi.Api.Controllers.DigitalProducts
{
    [ApiController]
    [Route("api/digital-products")]
    [Authorize]
    public class DigitalProductsController : ControllerBase
    {
        private readonly IDigitalProductService _digitalProductService;

        public DigitalProductsController(IDigitalProductService digitalProductsService)
        {
            _digitalProductService = digitalProductsService;
        }

        private int UserId =>
            int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        [HttpGet]
        //get all digital products
        public async Task<IActionResult> GetAll(int id)
        {
            var product = await _digitalProductService.GetProductAsync(id);
            if (product == null)
                return NotFound();

            return Ok(product);
        }

        [HttpPost("{id}/assign-key")]
        //assign product key
        public async Task<IActionResult> AssignKey(int id)
        {
            var key = await _digitalProductService.AssignProductKeyAsync(id, UserId);
            return Ok(new
            { 
                message = "Product Key assigned successfully",
                productKey = key
            });
        }

    }
}
