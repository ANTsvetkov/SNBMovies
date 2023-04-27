using SNBMovies.Data.Enums;
using System.ComponentModel.DataAnnotations;

namespace SNBMovies.Models.ViewModels.MovieVMs
{
    public class CreateMovieVM
    {
        public int Id { get; set; }

        [Display(Name = "Movie name")]
        [Required(ErrorMessage = "Name is required")]
        public string Title { get; set; }

        [Display(Name = "Movie description")]
        [Required(ErrorMessage = "Description is required")]
        public string Description { get; set; }

        [Display(Name = "Price")]
        [Required(ErrorMessage = "Price is required")]
        public double Price { get; set; }

        [Display(Name = "Movie poster URL")]
        [Required(ErrorMessage = "Movie poster URL is required")]
        public IFormFile? ImageURL { get; set; }

        [Display(Name = "Movie release date")]
        [Required(ErrorMessage = "Release date is required")]
        public DateTime ReleaseDate { get; set; }

        [Display(Name = "Select a genre")]
        [Required(ErrorMessage = "Movie genre is required")]
        public Genre Genre { get; set; }

        [Display(Name = "Select a category")]
        [Required(ErrorMessage = "Movie category is required")]
        public Category Category { get; set; }

        [Display(Name = "Movie file ")]
        [Required(ErrorMessage = "Movie file is required")]
        public IFormFile MovieFile { get; set; }


        //Relationships
        [Display(Name = "Select actor(s)")]
        [Required(ErrorMessage = "Movie actor(s) is required")]
        public List<int> ActorIds { get; set; }

        [Display(Name = "Select a producer")]
        [Required(ErrorMessage = "Movie producer is required")]
        public int ProducerId { get; set; }
    }
}
