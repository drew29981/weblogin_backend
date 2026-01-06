using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace wblg.models {
    public class LoginDTO
    {
        [Required]
        public required string UsernameOrEmail { get; set; }
        [Required, PasswordPropertyText]
        public required string Password { get; set; }
    }
}