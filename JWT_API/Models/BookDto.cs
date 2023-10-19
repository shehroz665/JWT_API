using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JWT_API.Models
{
    public class BookDto
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int BookId { get; set; }
        public string Title { get; set; }
        public int BookCatId { get; set; }
        public int BookAuthId { get; set; }
        public string Isbn { get; set; }
        public int ActualQuantity { get; set; }
        public int AvailableQuantity { get; set; }
        public int Price { get; set; }
        public int Status { get; set; }
        public int CatId { get; set; }
        public string CatName { get; set; }
        public int AuthId { get; set; }
        public string AuthName { get; set; }
    }
}
