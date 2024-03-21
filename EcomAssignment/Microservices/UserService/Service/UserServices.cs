
using UserService.DTO;
using UserService.Model;

namespace UserService.Service
{
    public class UserServices : IUserService
    {
        private static readonly Dictionary<int, CartItemDto> ShoppingCart = new Dictionary<int, CartItemDto>();
        public void AddToCart(CartItemDto cartItemDto)
        {
           
            if (cartItemDto == null)
            {
                throw new ArgumentNullException(nameof(cartItemDto), "CartItemDto cannot be null.");
            }

           else if (ShoppingCart.ContainsKey(cartItemDto.ProductId))
            {
                // If the product ID already exists in the shopping cart, add the new quantity to the existing quantity
                ShoppingCart[cartItemDto.ProductId].Quantity += cartItemDto.Quantity;
            }
            else
            {
                ShoppingCart.Add(cartItemDto.ProductId, cartItemDto);
            }
        }

        public void Checkout()
        {
            ShoppingCart.Clear();
        }

        

        public List<CartItemDto> ViewCart()
        {   
            
            return ShoppingCart.Values.ToList();
        }
        public bool RemoveFromCart(int productId)
        {
            if(!ShoppingCart.ContainsKey(productId)) {  return false; }
            else
            return ShoppingCart.Remove(productId);
        }

        public bool CheckProductId(int productId)
        {
            return ShoppingCart.ContainsKey(productId);
        }

        public CartItemDto GetCartItem(int productId)
        {
            if (ShoppingCart.TryGetValue(productId, out var cartItem))
            {
                return cartItem;
            }
            else
            {
                return null; // Cart item not found
            }
        }
    }
}
