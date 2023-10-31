using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace JWT_API.Models
{
    public class CategoriesDto
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CatId { get; set; }

        [Required]
        public string CatName { get; set; }
        [Required]
        public int Status { get; set; }

        public string Titles { get; set; }
    }
}
