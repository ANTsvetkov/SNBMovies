using System.ComponentModel.DataAnnotations;

namespace SNBMovies.Models.ViewModels
{
    public class EditProfileVM: ApplicationUser
    {
        [Display(Name = "Full name")]
        public string FullName { get; set; }

        [DataType(DataType.EmailAddress)]
        public string EmailAddress { get; set; }

        [DataType(DataType.PhoneNumber)]
        public string PhoneNumber { get; set; }
    }
}
