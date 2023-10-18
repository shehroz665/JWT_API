using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JWT_API.Models
{
    public class Users
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required]
        [EmailAddress(ErrorMessage = "Invalid email address.")]
        public string Email { get; set; }
        [Required]
        [MinLength(6,ErrorMessage ="Password must be at least 6 character long.")]
        public string Password { get; set; }

        public string UserMessage { get; set; }

        public string UserToken { get; set; }
    }
}
