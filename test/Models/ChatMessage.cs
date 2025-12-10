using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;

using System.ComponentModel.DataAnnotations;
namespace test.Models
{
    public class ChatMessage
    {
        public int id { get; set; }
        [Required]
        public String? SenderId { get; set; }
        [ForeignKey("SenderId")]
        public ApplicationUser? Sender { get; set; }

     
        public string? ReceiverId { get; set; }
        [ForeignKey("ReceiverId")]
        public ApplicationUser? Receiver { get; set; }
        [Required]
        [MaxLength(2000)]
        public string Message { get; set; }
        public DateTime Time { get; set; } = DateTime.Now;
        public bool read { get; set; }
        
        public int? AnimalId { get; set; }
        [ForeignKey("AnimalId")]
        public Animal? Animal { get; set; }
    }
}
