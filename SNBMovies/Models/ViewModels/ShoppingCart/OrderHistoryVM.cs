namespace SNBMovies.Models.ViewModels.ShoppingCart
{
    public class OrderHistoryVM
    {
        public List<OrderHistory> OrderHistoryItems { get; set; }

        public double TotalPrice { get; set; }
        public string OrderId { get; set; }
    }
}
