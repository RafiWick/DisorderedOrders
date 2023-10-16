using DisorderedOrdersMVC.DataAccess;
using Microsoft.EntityFrameworkCore;

namespace DisorderedOrdersMVC.Models
{
    public class Order
    {
        public int Id { get; set; }
        public Customer Customer { get; set; }
        public List<OrderItem> Items { get; set; } = new List<OrderItem>();


        public void VerifyStock()
        {
            foreach (var orderItem in Items)
            {
                if (!orderItem.Item.InStock(orderItem.Quantity))
                {
                    orderItem.Quantity = orderItem.Item.StockQuantity;
                }

                orderItem.Item.DecreaseStock(orderItem.Quantity);
            }
        }
        public int Total()
        {
            var total = 0;
            foreach (var orderItem in Items)
            {
                var itemPrice = orderItem.Item.Price * orderItem.Quantity;
                total += itemPrice;
            }
            return total;
        }
        public void PopulateOrderItems(IFormCollection collection, DisorderedOrdersContext context)
        {
            for (var i = 1; i < collection.Count - 1; i++)
            {
                var kvp = collection.ToList()[i];
                if (kvp.Value != "0")
                {
                    var product = context.Products.Where(p => p.Name == kvp.Key).First();
                    var orderItem = new OrderItem() { Item = product, Quantity = Convert.ToInt32(kvp.Value) };
                    Items.Add(orderItem);
                }
            }
        }
    }
}
