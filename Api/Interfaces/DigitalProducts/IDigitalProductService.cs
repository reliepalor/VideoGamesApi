using VideoGameApi.Api.Dto.DigitalProducts;

namespace VideoGameApi.Api.Interfaces.DigitalProducts
{
    public interface IDigitalProductService
    {
        Task<int> CreateProductAsync(CreateDigitalProductDto dto);
        Task<bool> UpdateProductAsync(int id, UpdateDigitalProductDto dto);
        Task AddProductKeyAsync(AddDigitalProductKeyDto dto);

        Task<IEnumerable<DigitalProductResponseDto>> GetAllProductsAsync();
        Task<DigitalProductResponseDto?> GetProductAsync(int id);
        Task<string> AssignProductKeyAsync(int digitalProductId, int userId);
        Task<bool> SoftDeleteProductAsync(int id);
        Task<bool> RestoreProductAsync(int id);
        Task<IEnumerable<DigitalProductKeyResponseDto>> GetProductKeysAsync(int productId);
        Task<IEnumerable<DigitalProductResponseDto>> GetAllProductsAdminAsync(bool includeInactive);
    }
}
