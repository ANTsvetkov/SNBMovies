using System.ComponentModel.DataAnnotations.Schema;

namespace SNBMovies.Models.ViewModels.ShoppingCart
{
    public class ShoppingCartVM
    {
        public List<ShoppingCartItem> ShoppingCartItems { get; set; }

        public double TotalPrice { get; set; }

    }
}
