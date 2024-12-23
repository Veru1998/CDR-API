using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CDR.Models
{
    public class CallDetailRecord
    {
        [Key]
        public string Reference { get; set; }
        [Required]
        public long CallerId { get; set; }
        [Required]
        public long Recipient { get; set; }
        [Required]
        public DateTime CallDate { get; set; }
        [Required]
        public TimeSpan EndTime { get; set; }
        [Required]
        public int Duration { get; set; }
        [Required]
        [Column(TypeName = "decimal(18, 2)")]
        public decimal Cost { get; set; }
        [Required]
        [MaxLength(3)]
        public string Currency { get; set; }
    }
}
