using VideoGameApi.Api.Dto.DigitalProducts;

namespace VideoGameApi.Interfaces.DigitalProducts
{
    public interface IDigitalProductService
    {
        Task<int> CreateProductAsync(CreateDigitalProductDto dto);
        Task<IEnumerable<DigitalProductResponseDto>> GetAllProductsAsync();
        Task<DigitalProductResponseDto?> GetProductAsync(int id);

        Task AddProductKeyAsync(AddDigitalProductKeyDto dto);
        Task<string> AssignProductKeyAsync(int productId, int userId);
    }
}
