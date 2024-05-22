namespace AppleStore.Models.Entities
{
    public class ShoppingCart
    {
        public List<CartItem> Items { get; set; } = new List<CartItem>();
        public void AddItem(CartItem item)
        {
            var existingItem = Items.FirstOrDefault(i => i.ProductVariant?.ProductId == item.ProductVariant?.ProductId);

            if (existingItem != null)
            {
                existingItem.ProductVariant.Quantity += item.ProductVariant.Quantity;
            }
            else
            {
                Items.Add(item);
            }
        }
        public void RemoveItem(int productId)
        {
            Items.RemoveAll(i => i.ProductVariant.ProductId == productId);
        }
    }
}