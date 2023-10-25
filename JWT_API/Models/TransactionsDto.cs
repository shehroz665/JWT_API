using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace JWT_API.Models
{
    public class TransactionsDto
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

        public int Status { get; set; }

        public int UserId { get; set; }

        public string Title { get; set; }

        public string Email { get; set; }
    }
}
