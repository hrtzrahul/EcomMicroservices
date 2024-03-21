using InventoryService.DTO;

namespace InventoryService.Service
{
    public class InventoryServices : IInventoryService
    {
        private static readonly Dictionary<int, ProductDto> Products = new Dictionary<int, ProductDto>();

        public Task<ProductDto> GetProductById(int productId)
        {
           
                return Task.FromResult(Products.GetValueOrDefault(productId));
           
        }

        void IInventoryService.AddToInventory(ProductDto productDto)
        {
            if (productDto == null)
            {
                throw new ArgumentNullException(nameof(productDto), "Product cannot be null.");
            }

            else if (productDto.ProductId <= 0 || productDto.Quantity <= 0)
            {
                throw new ArgumentException("ProductId and quantity must be greater than 0.");
            }
            else if (Products.ContainsKey(productDto.ProductId))
            {
                throw new ArgumentException("ProductId already exists.");
            }

           else if (!Products.ContainsKey(productDto.ProductId))
            {
                Products.Add(productDto.ProductId, productDto);

            }
            

        }

        List<ProductDto> IInventoryService.GetProducts()
        {
            return Products.Values.ToList();
        }

        void IInventoryService.RemoveFromInventory(int productId)
        {
            var productToRemove = Products.GetValueOrDefault(productId);
            if (productToRemove != null)
            {
                Products.Remove(productId);
            }
            
        }
        public async Task<bool> UpdateProductQuantity(int productId, int quantity)
        {
            try
            {
                if (quantity <= 0)
                {
                    throw new ArgumentException("Quantity must be greater than 0.");
                }

                if (Products.TryGetValue(productId, out var product))
                {
                    if (quantity > product.Quantity)
                    {
                        // Log the message if the input quantity exceeds the available stock
                        Console.WriteLine($"Quantity exceeds available stock which is: {product.Quantity}");
                        return false;
                    }

                    product.Quantity -= quantity;
                    return true;
                }
                else
                {
                    return false; // Product not found
                }
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"Error updating product quantity: {ex.Message}");
                return false;
            }
        }
        public bool UpdateProduct(ProductDto product)
        {
            try
            {
                if (product == null)
                {
                    throw new ArgumentNullException(nameof(product), "Product cannot be null.");
                }

                if (product.ProductId <= 0 || product.Quantity <= 0)
                {
                    throw new ArgumentException("ProductId and quantity must be greater than 0.");
                }

                if (Products.ContainsKey(product.ProductId))
                {
                    Products[product.ProductId] = product;
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"Error updating product quantity: {ex.Message}");
                return false;
            }
        }

    
    }
}
