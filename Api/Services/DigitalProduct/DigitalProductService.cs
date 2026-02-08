using Microsoft.EntityFrameworkCore;
using VideoGameApi.Api.Dto.DigitalProducts;
using VideoGameApi.Api.Interfaces.DigitalProducts;
using VideoGameApi.Interfaces.DigitalProducts;
using VideoGameApi.Models.DigitalProducts;
using VideoGameApi.Models.DigitalProducts.Enums;

namespace VideoGameApi.Api.Services.DigitalProducts
{
    public class DigitalProductService : IDigitalProductService
    {
        private readonly IDigitalProductRepository _productRepo;
        private readonly IDigitalProductKeyRepository _keyRepo;

        public DigitalProductService(
            IDigitalProductRepository productRepo,
            IDigitalProductKeyRepository keyRepo)
        {
            _productRepo = productRepo;
            _keyRepo = keyRepo;
        }

        public async Task<int> CreateProductAsync(CreateDigitalProductDto dto)
        {
            var product = new DigitalProduct
            {
                Name = dto.Name,
                Brand = dto.Brand,
                Platform = dto.Platform,
                ProductType = dto.ProductType,
                LicenseDuration = dto.LicenseDuration,
                Description = dto.Description,
                Price = dto.Price,
                Stock = 0,
                IsActive = true
            };

            await _productRepo.AddAsync(product);
            await _productRepo.SaveChangesAsync();

            return product.Id;
        }

        public async Task AddProductKeyAsync(AddDigitalProductKeyDto dto)
        {

            var key = new DigitalProductKey
            {
                DigitalProductId = dto.DigitalProductId,
                ProductKey = dto.ProductKey
            };

            await _keyRepo.AddAsync(key);
            await _keyRepo.SaveChangesAsync();
        }

        public async Task<IEnumerable<DigitalProductResponseDto>> GetAllProductsAsync()
        {
            var products = await _productRepo.GetAllAsync();

            return products.Select(p => new DigitalProductResponseDto
            {
                Id = p.Id,
                Name = p.Name,
                Brand = p.Brand,
                Platform = p.Platform,
                ProductType = p.ProductType,
                LicenseDuration = p.LicenseDuration,
                Description = p.Description,
                Price = p.Price,
                Stock = p.ProductKeys.Count(k => !k.IsUsed),
                IsActive = p.IsActive,
            });
        }

        public async Task<DigitalProductResponseDto?> GetProductAsync(int id)
        {
            var product = await _productRepo.GetByIdAsync(id);
            if (product == null) return null;

            return new DigitalProductResponseDto
            {
                Id = product.Id,
                Name = product.Name,
                Brand = product.Brand,
                Platform = product.Platform,
                ProductType = product.ProductType,
                LicenseDuration = product.LicenseDuration,
                Description = product.Description,
                Price = product.Price,
                Stock = product.ProductKeys.Count(k => !k.IsUsed),
                IsActive = product.IsActive,
            };
        }



        public async Task<string> AssignProductKeyAsync(int digitalProductId, int userId)
        {
            var key = await _keyRepo.GetUnusedKeyAsync(digitalProductId);
            if (key == null)
                throw new InvalidOperationException("No available product keys.");

            key.IsUsed = true;
            key.UsedAt = DateTime.UtcNow;
            key.AssignedToUserId = userId;
            
            

            await _keyRepo.MarkAsUsedAsync(key);
            await _keyRepo.SaveChangesAsync();

            return key.ProductKey;
        }
    }
}
