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
    }
}
