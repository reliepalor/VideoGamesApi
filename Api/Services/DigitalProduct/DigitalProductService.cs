using Microsoft.EntityFrameworkCore;
using VideoGameApi.Api.Dto.DigitalProducts;
using VideoGameApi.Api.Interfaces.DigitalProducts;
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

        // create digital product
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

            if (dto.Image != null)
            {
                var uploadsPath = Path.Combine(
                    Directory.GetCurrentDirectory(),
                     "wwwroot",
                    "uploads",
                    "digital-products"
                );
            
                Directory.CreateDirectory( uploadsPath );

                var filename = $"{Guid.NewGuid}{Path.GetExtension(dto.Image.FileName)}";

                var filePath = Path.Combine ( uploadsPath, filename );

                await using var stream = new FileStream(filePath, FileMode.Create);
                await dto.Image.CopyToAsync( stream );

                product.ImagePath = $"/uploads/digital-products/{filename}";
            }

            await _productRepo.AddAsync(product);
            await _productRepo.SaveChangesAsync();

            return product.Id;
        }

        //
        public async Task<bool> UpdateProductAsync(int id, UpdateDigitalProductDto dto)
        { 
            var product = await _productRepo.GetByIdAsync( id );
            if (product == null) return false;

            product.Name = dto.Name;
            product.Brand = dto.Brand;
            product.Platform = dto.Platform;
            product.ProductType = dto.ProductType;
            product.LicenseDuration = dto.LicenseDuration;
            product.Description = dto.Description;
            product.Price = dto.Price;
            product.IsActive = dto.IsActive;
            product.UpdatedAt = DateTime.UtcNow;

            if (dto.Image != null)
            {
                var uploadsPath = Path.Combine(
                    Directory.GetCurrentDirectory(),
                    "wwwroot",
                    "uploads",
                    "digital-products"
                );

                Directory.CreateDirectory( uploadsPath );

                var fileName = $"{Guid.NewGuid()}{Path.GetExtension(dto.Image.FileName)}";
                var filePath = Path.Combine( uploadsPath, fileName );

                await using var stream = new FileStream(filePath, FileMode.Create);
                await dto.Image.CopyToAsync( stream );

                product.ImagePath = $"/uploads/digital-products/{fileName}";
            }

            _productRepo.UpdateAsync( product );
            await _productRepo.SaveChangesAsync();

            return true;
        }


        // assigned or add product key
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

        // get all digital products
        public async Task<IEnumerable<DigitalProductResponseDto>> GetAllProductsAsync()
        {
            var products = await _productRepo.GetAllAsync();

            return products
                .Where(p => p.IsActive)
                .Select(p => new DigitalProductResponseDto
            {
                Id = p.Id,
                Name = p.Name,
                Brand = p.Brand,
                Platform = p.Platform,
                ProductType = p.ProductType,
                LicenseDuration = p.LicenseDuration,
                Description = p.Description,
                Price = p.Price,
                ImagePath = p.ImagePath,

                Stock = p.DigitalProductKeys.Count(),
                AvailableKeys = p.DigitalProductKeys.Count(k => !k.IsUsed),
                IsActive = p.IsActive,
               
            });
        }

        // get products that is active only
        public async Task<IEnumerable<DigitalProductResponseDto>> GetAllProductsAdminAsync(bool includeInactive)
        {
            var products = await _productRepo.GetAllAsync();

            if (!includeInactive)
                products = products.Where(p => p.IsActive);

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
                ImagePath = p.ImagePath,

                Stock = p.DigitalProductKeys.Count(),
                AvailableKeys = p.DigitalProductKeys.Count(k => !k.IsUsed),
                IsActive = p.IsActive
            });
        }


        //
        public async Task<DigitalProductResponseDto?> GetProductAsync(int id)
        {
            var product = await _productRepo.GetByIdAsync(id);
            if (product == null || !product.IsActive) return null;

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
                ImagePath = product.ImagePath,
                Stock = product.DigitalProductKeys.Count(),
                AvailableKeys = product.DigitalProductKeys.Count(k => !k.IsUsed),
                IsActive = product.IsActive,
            };
        }

        // assigned product key
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

        // get all product keys
        public async Task<IEnumerable<DigitalProductKeyResponseDto>> GetProductKeysAsync(int productId)
        { 
            var keys = await _keyRepo.GetByProductIdAsync(productId);

            return keys.Select(k => new DigitalProductKeyResponseDto
            { 
                Id = k.Id,
                ProductKey = k.ProductKey,
                IsUsed = k.IsUsed,
                AssignedToUserId = k.AssignedToUserId,
                UsedAt = k.UsedAt,
                CreatedAt = k.CreatedAt
            });
        }

        // soft delete a digital products
        public async Task<bool> SoftDeleteProductAsync(int id)
        {
            var product = await _productRepo.GetByIdAsync(id);
            if (product == null) return false;

            product.IsActive = false;
            product.UpdatedAt = DateTime.UtcNow;

            _productRepo.UpdateAsync(product);
            await _productRepo.SaveChangesAsync();

            return true;
        }

        // restore soft deleted products
        public async Task<bool> RestoreProductAsync(int id)
        {
            var product = await _productRepo.GetByIdAsync(id);
            if (product == null) return false;

            product.IsActive = true;
            product.UpdatedAt = DateTime.UtcNow;

            _productRepo.UpdateAsync(product);
            await _productRepo.SaveChangesAsync();

            return true;
        }
    }
}
