using InventoryService.DTO;

namespace InventoryService.Service
{
    public interface IInventoryService
    {
        public List<ProductDto> GetProducts();
        void AddToInventory(ProductDto productDto);
        void RemoveFromInventory(int productId);
        Task<ProductDto> GetProductById(int productId);
        Task<bool> UpdateProductQuantity(int productId, int quantity);
        bool UpdateProduct(ProductDto product);
    }

}