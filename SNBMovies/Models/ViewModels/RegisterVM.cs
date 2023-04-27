using System.ComponentModel.DataAnnotations;

namespace SNBMovies.Models.ViewModels
{
    public class RegisterVM
    {
        [Required(ErrorMessage = "Полето 'Име' е задължително!")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "Полето 'Имейл' е задължително!")]
        [DataType(DataType.EmailAddress)]
        public string EmailAddress { get; set; }

        [Required(ErrorMessage = "Полето 'Парола' е задължително!")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*\W).{6,}$",
    ErrorMessage = "Паролата трябва да има поне 6 символа, една главна буква, една малка буква, една цифра и един специален символ!")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Потвърждаването на паролата е задължително!")]
        [Compare("Password", ErrorMessage = "Паролите се различават!")]
        public string ConfirmPassword { get; set; }
    }
}
