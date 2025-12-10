using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace test.Models
{
    [Table("PaymentMethod")]
    public partial class PaymentMethods
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int PaymentMethodId { get; set; }
        [Required]
        public string UserId { get; set; }
        [ForeignKey("UserId")]
        public virtual ApplicationUser? User { get; set; }
        [Required]
        [StringLength(50)]
        public string MethodType { get; set; }

        [Required]
        [StringLength(4, MinimumLength = 4)]
        public string last4Digits { get; set; }
        [Required]
        [StringLength(2)]
        public string expiryMonth { get; set; }
        [Required]
   
        public string expiryYear { get; set; }
        [Required]
        [Column("GatewatyToken")]
        public string GatewayToken { get; set; }
    }
}
