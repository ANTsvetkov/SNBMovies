﻿using System.ComponentModel.DataAnnotations;

namespace SNBMovies.Models.ViewModels.ProducerVMs
{
    public class CreateProducerVM
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Profile Picture")]
        public IFormFile? ProfilePictureURL { get; set; }

        [Required]
        [Display(Name = "Full Name")]
        public string? FullName { get; set; }

        [Required]
        [Display(Name = "Biography")]
        public string? Biography { get; set; }
    }
}
