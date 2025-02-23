using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace MagicVilla_VillaAPI.Models
{
    public class ApplicationUser : IdentityUser
    {
        [Required, MaxLength(50)]
        public string Name { get; set; }
    }
}
