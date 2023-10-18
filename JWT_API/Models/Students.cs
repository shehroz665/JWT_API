using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JWT_API.Models
{
    public class Students
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity) ]
        public int StuId { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        [StringLength(7)]
        public string RollNumber { get; set; }

        [Required]
        public int Semester { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [MaxLength(11)]
        public string Phone { get; set; }

        public string? Address { get; set; }
    }
}
