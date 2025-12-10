using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace test.Models
{
    public class UserConnections
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "User Id is required.")]
        public string UserId { get; set; }
        [ForeignKey("UserId")]
        public virtual ApplicationUser? User { get; set; }
        [Required(ErrorMessage = "Connection Id is required.")]
        [StringLength(255)]
        public string ConnectionId { get; set; }

    }
}
