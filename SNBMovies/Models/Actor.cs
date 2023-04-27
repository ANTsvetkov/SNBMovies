using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SNBMovies.Models
{
    public class Actor
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Display(Name = "Profile Picture")] 
        public string ProfilePictureURL { get; set; }

        [Required]
        [Display(Name = "Full Name")]
        public string? FullName { get; set; }

        [Required]
        [Display(Name = "Biography")]
        public string? Biography { get; set; }


        //Relationships
        public List<Actor_Movie>? Actor_Movie { get; set; }
    }
}
