using _2380601309_NguyenHuuDieuLy_TH6.Models;
using System.Text.Json;

namespace _2380601309_NguyenHuuDieuLy_TH6.Helpers
{
    public static class ShoppingCartHelper
    {
        private const string CartSessionKey = "ShoppingCart";

        public static List<CartItem> GetCart(ISession session)
        {
            var cartJson = session.GetString(CartSessionKey);
            if (string.IsNullOrEmpty(cartJson))
                return new List<CartItem>();
            return JsonSerializer.Deserialize<List<CartItem>>(cartJson) ?? new List<CartItem>();
        }

        public static void SaveCart(ISession session, List<CartItem> cart)
        {
            var cartJson = JsonSerializer.Serialize(cart);
            session.SetString(CartSessionKey, cartJson);
        }

        public static void AddToCart(ISession session, CartItem item)
        {
            var cart = GetCart(session);
            var existingItem = cart.FirstOrDefault(c => c.ProductId == item.ProductId);
            if (existingItem != null)
            {
                existingItem.Quantity += item.Quantity;
            }
            else
            {
                cart.Add(item);
            }
            SaveCart(session, cart);
        }

        public static void RemoveFromCart(ISession session, int productId)
        {
            var cart = GetCart(session);
            cart.RemoveAll(c => c.ProductId == productId);
            SaveCart(session, cart);
        }

        public static void UpdateQuantity(ISession session, int productId, int quantity)
        {
            var cart = GetCart(session);
            var item = cart.FirstOrDefault(c => c.ProductId == productId);
            if (item != null)
            {
                if (quantity <= 0)
                    cart.Remove(item);
                else
                    item.Quantity = quantity;
            }
            SaveCart(session, cart);
        }

        public static void ClearCart(ISession session)
        {
            session.Remove(CartSessionKey);
        }

        public static int GetCartCount(ISession session)
        {
            var cart = GetCart(session);
            return cart.Sum(c => c.Quantity);
        }

        public static decimal GetCartTotal(ISession session)
        {
            var cart = GetCart(session);
            return cart.Sum(c => c.Total);
        }
    }
}
