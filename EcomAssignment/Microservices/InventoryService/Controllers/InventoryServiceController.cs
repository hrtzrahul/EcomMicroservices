using InventoryService.DTO;
using InventoryService.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Text.Json;
using System.Text;
using System.Net.Http;
using InventoryService.Model;
using Microsoft.Extensions.Http;
//using Newtonsoft.Json;

namespace InventoryService.Controllers
{
    // InventoryController.cs
    [ApiController]
    [Route("api/inventory")]
    public class InventoryController : ControllerBase
    {
        private readonly ILogger<InventoryController> _logger;
        private readonly HttpClient _httpClient; // HTTP client for making requests to other microservices
        private readonly IInventoryService _inventoryService;

        public InventoryController(ILogger<InventoryController> logger, HttpClient httpClient, IInventoryService inventoryService)
        {
            _logger = logger;
            _httpClient = httpClient;
            _inventoryService = inventoryService;
        }

        [HttpGet("checkproduct")]
        public async Task<ActionResult<ProductDto>> CheckProduct(int productId)
        {
            try
            {
                // Call the method in the service layer to check if the product exists
                var product = await _inventoryService.GetProductById(productId);

                // If product exists, send it to the consuming microservice (UserService)
                if (product != null)
                {
                    return product;
                }
                else
                {
                    _logger.LogError($"Product with ID {productId} not found");
                    return NotFound($"Product with ID {productId} not found");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking product or sending to UserService");
                return StatusCode(500, "Internal server error");
            }
        }





        [HttpPut("updateproduct")]
        public async Task<IActionResult> UpdateProduct(ProductDto product)
        {
            try
            {
                // Call the appropriate function in the service layer to update the product information based on the product ID
                var updated = _inventoryService.UpdateProduct(product);

                if (updated)
                {
                    return Ok("Product details updated successfully");
                }
                else
                {
                    return NotFound($"Product with ID {product.ProductId} not found");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating product details");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("allproducts")]
        public ActionResult<List<ProductDto>> ProductList()
        {
            try
            {
                var products = _inventoryService.GetProducts();

                if (products != null)
                {
                    return products;
                }
                else
                {
                    _logger.LogError($"Failed to send product list");
                    return StatusCode(500, "Failed to send product list");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving or sending product list");
                return StatusCode(500, "Internal server error");
            }
        }



        [HttpPut("updatequantity")]
        public async Task<IActionResult> UpdateProductQuantity(List<ProductQuantityUpdateModel> productQuantities)
        {
            try
            {
                if (productQuantities == null || productQuantities.Count == 0)
                {
                    return BadRequest("Invalid input");
                }

                foreach (var productQuantity in productQuantities)
                {
                    int productId = productQuantity.ProductId;
                    int quantity = productQuantity.Quantity;

                    // Call the function from the service layer to update the quantity of the product
                    var updated = _inventoryService.UpdateProductQuantity(productId, quantity);

                    // Handle the update logic if needed
                }

                return Ok("Product quantities updated successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating product quantities");
                return StatusCode(500, "Internal server error");
            }
        }


        [HttpPost("addproduct")]
      // [Authorize(Roles = "Admin")]
        public IActionResult AddProduct([FromBody] ProductDto productDto)
        {
            try
            {
                _inventoryService.AddToInventory(productDto);
                return Ok("Product added to inventory");
            }
            catch (ArgumentNullException ex)
            {
                // Handle ArgumentNullException
                return BadRequest(ex.Message);
            }
            catch (ArgumentException ex)
            {
                // Handle ArgumentException
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding product to inventory");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpDelete("removeproduct")]
       // [Authorize(Roles = "Admin")]
        public IActionResult RemoveProduct(int productId)
        {
            try
            {
                // Logic to remove product from inventory
                _inventoryService.RemoveFromInventory(productId);
                // Return success message
                return Ok("Product removed from inventory");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing product from inventory");
                return StatusCode(500, "Internal server error");
            }
        }
    }

}
