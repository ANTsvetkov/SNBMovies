using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SNBMovies.Models
{
    public class OrderHistory
    {
        [Key]
        public int Id { get; set; }

        public double Price { get; set; }
        public Movie Movie { get; set; }

        public string UserId { get; set; }
        [ForeignKey("UserId")]
        public ApplicationUser User { get; set; }

        public bool IsPurchased { get; set; }
        public DateTime PurchaseDate { get; set; }
        public string OrderId { get; set; }
    }
}
