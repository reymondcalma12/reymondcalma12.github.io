using Microsoft.AspNetCore.Identity;

namespace Sample.Models
{
    public class AppUser : IdentityUser
    {
        public string Name { get; set; }

        public bool Online { get; set; }

        public string? Profile {  get; set; }

    }
}
