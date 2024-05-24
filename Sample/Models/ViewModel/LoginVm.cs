using System.ComponentModel.DataAnnotations;

namespace Sample.Models.ViewModel
{
    public class LoginVm
    {
        [Required(ErrorMessage = "Username is required!")]
        [DataType(DataType.EmailAddress)]
        public string Username { get; set; }

        [Required(ErrorMessage = "Password is required!")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Display(Name = "Remember Me")]
        public bool RememberMe { get; set; }

    }
}
