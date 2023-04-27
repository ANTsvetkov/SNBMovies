using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using SNBMovies.Data.Enums;

namespace SNBMovies.Models
{
    public class Movie
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string? Title { get; set; }

        [Required]
        public string? Description { get; set; }
        [Required]
        public double Price { get; set; }
        [Required]
        public string ImageURL { get; set; }
        [Required]
        [DataType(DataType.DateTime)]
        public DateTime ReleaseDate { get; set; }
        [Required]
        public Genre Genre { get; set; }
        [Required]
        public Category Category { get; set; }
        public string? MovieFile { get; set; }

        //Relationships
        public List<Actor_Movie>? Actor_Movie { get; set; }

        //Producer
        public int ProducerId { get; set; }
        [ForeignKey("ProducerId")]
        public Producer? Producer { get; set; }
    }
}
