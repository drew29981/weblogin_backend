using Microsoft.EntityFrameworkCore;
using wblg.models;

namespace wblg.contexts {
    public class UserContext : DbContext
    {
        public UserContext(DbContextOptions<UserContext> options) : base(options) {}
        public async Task<bool> ContainsIdAsync(Guid Id)
        {
            return await Users.AnyAsync(u => u.Id == Id);
        }
        public async Task<bool> ContainsUsernameAsync(string Username)
        {
            return await Users.AnyAsync(u => u.Username.Equals(Username, StringComparison.Ordinal));
        }
        public async Task<User?> FindByUsernameAsync(string Username)
        {
            var user = Users.FirstOrDefaultAsync(u => u.Username.Equals(Username, StringComparison.Ordinal));
            return await user;
        }
        public async Task<User?> FindByEmailAsync(string Email)
        {
            var user = Users.FirstOrDefaultAsync(u => u.Email.Equals(Email, StringComparison.Ordinal));
            return await user;
        }
        public async Task<User?> FindByUser(string UsernameOrEmail)
        {
            var user = Users.FirstOrDefaultAsync(u => 
                u.Username == UsernameOrEmail ||
                u.Email == UsernameOrEmail);
            return await user;
        }
        public DbSet<User> Users { get; set; }
    }
}