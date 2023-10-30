using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace JWT_API.Models
{
        public class AuthorDto
        {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int AuthId { get; set; }
        public string AuthName { get; set; }
        public int Status { get; set; }
        public string Titles { get; set; }
    }

}
