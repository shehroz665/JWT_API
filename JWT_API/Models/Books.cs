using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace JWT_API.Models
{
    public class Books
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int BookId { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public int  BookCatId { get; set; }

        [Required]
        public int BookAuthId { get; set; }

        [Required]

        [MaxLength(13)]
        public string  Isbn { get; set; }

        [Required]
        public int ActualQuantity { get; set; }

        [Required]
        public int AvailableQuantity { get; set; }

        [Required]
        public int Price { get; set; }

        [Required]
        public int Status { get; set; }
    }
}
