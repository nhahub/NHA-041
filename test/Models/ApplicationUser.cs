using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace test.Models
{
    public class ApplicationUser : IdentityUser
    {
        [MaxLength(255)]
        public string? PhotoUrl { get; set; }
        [MaxLength(100)]
        public string? FullName { get; set; }
        [MaxLength(600)]
        public string? location { get; set; }
        public string? Bio { get; set; }
    }
}

