using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using UserService.Model;
using System.Net.Http;
using System.Net.Http.Formatting;
using UserService.DTO;
using UserService.Service;
using Newtonsoft.Json;

namespace UserService.Controllers
{
    // UserController.cs
    [ApiController]
    [Route("api/userservice")]
    public class UserController : ControllerBase
    {
        private readonly ILogger<UserController> _logger;
        private readonly HttpClient _productServiceClient;
        private readonly IUserService _userService;

        public UserController(ILogger<UserController> logger, HttpClient productServiceClient, IUserService userService
            )
        {
            _logger = logger;
            _productServiceClient = productServiceClient;
            _userService = userService;
        }

        [HttpGet("viewallproducts")]
        public async Task<ActionResult<List<ProductDto>>> ViewAllProducts()
        {
            try
            {
                // Make a request to the microservice containing the ProductList endpoint
                var productListResponse = await _productServiceClient.GetAsync("http://productservice:80/api/product/viewallproducts");
               // var productListResponse = await _productServiceClient.GetAsync("http://localhost:44343/api/product/viewallproducts");
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






        [HttpPost("addtocart")]
        // [Authorize(Roles = "User,Admin")]
        public async Task<IActionResult> AddToCart(CartItemDto cartItem)
        {
            try
            {
                if (cartItem.Quantity <= 0)
                {
                    return BadRequest("Quantity must be greater than zero");
                }

                // Make a request to the ProductService to check if the product exists
                var productServiceResponse = await _productServiceClient.GetAsync($"http://inventoryservice:80/api/inventory/checkproduct?productId={cartItem.ProductId}");

              //  var productServiceResponse = await _productServiceClient.GetAsync($"http://localhost:44309/api/inventory/checkproduct?productId={cartItem.ProductId}");

                if (productServiceResponse.IsSuccessStatusCode)
                {
                    var product = await productServiceResponse.Content.ReadFromJsonAsync<ProductDto>();

                    if (product != null)
                    {
                        // Calculate the total quantity if the cart item is added
                        int totalQuantity = cartItem.Quantity;

                        // Check if the product is already in the cart
                        if (_userService.CheckProductId(cartItem.ProductId))
                        {
                            // Get the existing quantity of the product in the cart
                            var existingCartItem = _userService.GetCartItem(cartItem.ProductId);
                            totalQuantity += existingCartItem.Quantity;
                        }

                        if (totalQuantity <= product.Quantity)
                        {
                            // Add logic to add the product to the cart
                            _userService.AddToCart(cartItem);
                            return Ok("Product added to cart");
                        }
                        else
                        {
                            return BadRequest($"Quantity exceeds available stock for product {product.Name}");
                        }
                    }
                    else
                    {
                        return NotFound($"Product {cartItem.ProductId} not found");
                    }
                }
                else
                {
                    _logger.LogError($"Failed to fetch product from Inventory Service: {productServiceResponse.StatusCode}");
                    return StatusCode((int)productServiceResponse.StatusCode, "Failed to fetch product from Inventory Service");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding product to cart");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPut("checkout")]
        //[Authorize(Roles = "User,Admin")]
        public async Task<IActionResult> Checkout()
        {
            try
            {
                // Call the function from the service layer to retrieve cart items
                List<CartItemDto> cartItems = _userService.ViewCart();

                if (cartItems == null || cartItems.Count == 0)
                {
                    return BadRequest("No items in the cart");
                }

                // Call the UpdateProductQuantity endpoint in the InventoryController to update product quantities
                var updateResponse = await _productServiceClient.PutAsJsonAsync("http://inventoryservice:80/api/inventory/updatequantity", cartItems);
               // var updateResponse = await _productServiceClient.PutAsJsonAsync("http://localhost:44309/api/inventory/updatequantity", cartItems);

                if (!updateResponse.IsSuccessStatusCode)
                {
                    _logger.LogError($"Failed to update product quantities: {updateResponse.StatusCode}");
                    return StatusCode((int)updateResponse.StatusCode, "Failed to update product quantities");
                }

                // Call the function from the service layer to clear the list of cart items
                _userService.Checkout();

                return Ok("Checkout successful");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during checkout");
                return StatusCode(500, "Internal server error");
            }
        }


        [HttpDelete("remove")]
        public IActionResult RemoveFromCart(int productId)
        {
            try
            {
                var removed = _userService.RemoveFromCart(productId);
                if (removed)
                {
                    _logger.LogInformation($"Product with ID {productId} successfully removed from the cart.");
                    return Ok("Product removed from the cart.");
                }
                else
                {
                    _logger.LogWarning($"Product with ID {productId} not found in the cart.");
                    return NotFound("Product not found in the cart.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while removing product from the cart.");
                return StatusCode(500, "Internal server error.");
            }
        }



        [HttpGet("viewcart")]
        public IActionResult ViewCart()
        {
            try
            {
                var cartItems = _userService.ViewCart();
                return Ok(cartItems);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving cart items");
                return StatusCode(500, "Internal server error");
            }
        }
    }

}

