using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProductDetailService.Model;
using System.Data;
using System.Net.Http;

namespace ProductDetailService.Controllers
{
    // ProductDetailController.cs
    [ApiController]
    [Route("api/productdetailsupdate")]
    public class ProductDetailController : ControllerBase
    {
        private readonly ILogger<ProductDetailController> _logger;
        private readonly HttpClient _httpClient;


        public ProductDetailController(ILogger<ProductDetailController> logger, HttpClient httpClient)
        {
            _logger = logger;
            _httpClient = httpClient;
        }

        [HttpPut("updateproduct")]
        // [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateProduct(ProductDto product)
        {
            try
            {
                // Make a request to the inventory microservice to update the product
                 var response = await _httpClient.PutAsJsonAsync("http://inventoryservice:80/api/inventory/updateproduct", product);

               // var response = await _httpClient.PutAsJsonAsync("http://localhost:44309/api/inventory/updateproduct", product);

                // Check if the request was successful
                if (response.IsSuccessStatusCode)
                {
                    return Ok("Product details updated successfully");
                }
                else
                {
                    // Log the error if the request failed
                    _logger.LogError($"Failed to update product details: {response.StatusCode}");
                    return StatusCode((int)response.StatusCode, "Failed to update product details");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating product details");
                return StatusCode(500, "Internal server error");
            }
        }

    }

}
