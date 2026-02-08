using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VideoGameApi.Api.Dto.DigitalProducts;
using VideoGameApi.Api.Interfaces.DigitalProducts;
using VideoGameApi.Interfaces.DigitalProducts;

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
        public async Task<IActionResult> Create(CreateDigitalProductDto dto)
        {
            var productId = await _digitalProductService.CreateProductAsync(dto);
            return Ok(new
            {
                message = "Digital product created successfully",
                productId
            });
        }

        // add product key
        [HttpPost("keys")]
        public async Task<IActionResult> AddKey(AddDigitalProductKeyDto dto)
        {
            await _digitalProductService.AddProductKeyAsync(dto);
            return Ok(new { message = "Product key added successfully" });
        }

        // get all digital products
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var products = await _digitalProductService.GetAllProductsAsync();
            return Ok(products);
        }
    }
}
