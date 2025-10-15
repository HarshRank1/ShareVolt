using Microsoft.AspNetCore.Identity;
using ShareVolt.Models;

namespace ShareVolt.Services
{
    public class AuthService
    {
        private readonly PasswordHasher<User> _passwordHasher = new PasswordHasher<User>();
        
        public string HashPassword(User user, string password)
        {
            return _passwordHasher.HashPassword(user, password);
        }

        public bool VerifyPassword(User user, string password)
        {
            var result = _passwordHasher.VerifyHashedPassword(user, user.Password, password);
            return result == PasswordVerificationResult.Success;
        }


    }
}
