using UserService.DTO;
using UserService.Model;

namespace UserService.Service
{
    public interface IUserService
    {
        CartItemDto GetCartItem(int productId);
        bool CheckProductId(int productId);
        public bool RemoveFromCart(int productId);
        List<CartItemDto> ViewCart();
        void AddToCart(CartItemDto cartItemDto);
        void Checkout();
    }
}
