using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace wblg.models {
    public class User
    {
        [Key]
        public Guid Id { get; set; }
        [Required]
        public required string Username { get; set; }
        [Required, EmailAddress]
        public required string Email { get; set; }
        [Required, PasswordPropertyText]
        public required string Password { get; set; }
    }
}