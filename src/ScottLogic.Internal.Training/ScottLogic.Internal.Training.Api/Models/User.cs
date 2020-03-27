using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ScottLogic.Internal.Training.Api
{
    public enum UserRole
    {
        None,
        Admin,
        User = 0
    }
    public class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int AccountNumber { get; set; }
        [Required]
        public string Username { get; set; }
        [Required]
        public string Password { get; set; }
        public string Salt { get; set; }
        public UserRole Role { get; set; }
    }
}
