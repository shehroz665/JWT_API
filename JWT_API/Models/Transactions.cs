using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JWT_API.Models
{
    public class Transactions
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int TransID { get; set; }

        [Required]
        public int TransStuId { get; set; }

        [Required]
        public int TransBookId { get; set; }

        [Required]
        public DateTime BorrowedDate { get; set; }

        [Required]
        public DateTime DueDate { get; set; }

        public DateTime? ReturnedDate { get; set; }

        [Required]
        [MaxLength(1)]
        public int Status { get; set; }
    }
}
