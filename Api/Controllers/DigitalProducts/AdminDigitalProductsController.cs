using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VideoGameApi.Api.Dto.DigitalProducts;
using VideoGameApi.Api.Interfaces.DigitalProducts;

namespace VideoGameApi.Controllers.DigitalProducts
{
    [ApiController]
    [Route("api/admin/digital-products")]
    [Authorize(Roles = "Admin")]
    public class AdminDigitalProductsController : ControllerBase
    {
        private readonly IDigitalProductService _digitalProductService;

        public AdminDigitalProductsController(IDigitalProductService digitalProductService)
        {
            _digitalProductService = digitalProductService;
        }

        // create digital product
        [HttpPost]
        public async Task<IActionResult> Create([FromForm] CreateDigitalProductDto dto)
        {
            var productId = await _digitalProductService.CreateProductAsync(dto);
            return Ok(new
            {
                message = "Digital product created successfully",
                productId
            });
        }

        //update digital product
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(
            int id,
            [FromForm] UpdateDigitalProductDto dto)
        {
            var updated = await _digitalProductService.UpdateProductAsync(id, dto);
            if (!updated)
                return NotFound(new { message = "Digital product not found." });

            return Ok(new { message = "Digital product updated successfully." });
        }

        // get all digital products
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] bool includeInactive = false)
        {
            Console.WriteLine($"includeInactive = {includeInactive}");

            var products = await _digitalProductService
                .GetAllProductsAdminAsync(includeInactive);

            return Ok(products);
        }

        // add product key
        [HttpPost("keys")]
        public async Task<IActionResult> AddKey(AddDigitalProductKeyDto dto)
        {
            await _digitalProductService.AddProductKeyAsync(dto);
            return Ok(new { message = "Product key added successfully" });
        }



        //get all product keys
        [HttpGet("{id}/keys")]
        public async Task<IActionResult> GetKeys(int id)
        { 
            var keys = await _digitalProductService.GetProductKeysAsync(id);
            return Ok(keys);
        }

        // soft delete a product
        [HttpDelete("{id}")]
        public async Task<IActionResult> SoftDelete(int id)
        {
            var deleted = await _digitalProductService.SoftDeleteProductAsync(id);
            if (!deleted)
                return NotFound(new { message = "Digital product not found." });

            return Ok(new { message = "Digital product deleted successfully." });
        }

        // restore deleted product
        [HttpPost("{id}/restore")]
        public async Task<IActionResult> Restore(int id)
        {
            var restore = await _digitalProductService.RestoreProductAsync(id);
            if (!restore)
                return NotFound(new { message = "Digital product not found" });

            return Ok(new { message = "Digital product restored successfullyy" });
        }

    }
}
