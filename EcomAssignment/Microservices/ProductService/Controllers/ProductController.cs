using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using ProductService.Model;

namespace ProductService.Controllers
{
    // ProductController.cs
    [ApiController]
    [Route("api/product")]
    public class ProductController : ControllerBase
    {
        private readonly ILogger<ProductController> _logger;
        private readonly HttpClient _httpClient;

        public ProductController(ILogger<ProductController> logger, HttpClient httpClient)
        {
            _logger = logger;
            _httpClient = httpClient;
        }

        [HttpGet("viewallproducts")]
        public async Task<ActionResult<List<ProductDto>>> ViewAllProducts()
        {
            try
            {
                // Make a request to the microservice containing the ProductList endpoint
                 var productListResponse = await _httpClient.GetAsync("http://inventoryservice:80/api/inventory/allproducts");
               // var productListResponse = await _httpClient.GetAsync("http://localhost:44309/api/inventory/allproducts");
                // Check if the request was successful
                if (productListResponse.IsSuccessStatusCode)
                {
                    // Deserialize the response content to retrieve the product list
                    var products = await productListResponse.Content.ReadFromJsonAsync<List<ProductDto>>();
                    return products;
                }
                else
                {
                    // Log the error if the request failed
                    _logger.LogError($"Failed to fetch products: {productListResponse.StatusCode}");

                    return StatusCode((int)productListResponse.StatusCode, $"Failed to fetch products: {productListResponse.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching products");
                return StatusCode(500, "Internal server error");
            }
        }







    }

}
