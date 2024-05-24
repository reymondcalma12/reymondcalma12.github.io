using System.ComponentModel.DataAnnotations;

namespace Sample.Models.ViewModel
{
    public class RegisterVm
    {
        [Required]
        public string? Name { get; set; }

        [Required]
        [DataType(DataType.EmailAddress)]
        public string? Email{ get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string? Password { get; set; }

        [Compare("Password", ErrorMessage = "Password Don't Match!")]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm Password")]
        public string? ConfirmPassword { get; set; }
    }
}
