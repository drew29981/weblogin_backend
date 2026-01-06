using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace wblg.models {
    public class UserData
    {
        [Key]
        public Guid Id { get; set; }
        [Required]
        public required string Username { get; set; }
        [Required, EmailAddress]
        public required string Email { get; set; }
    }
}