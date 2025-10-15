using Microsoft.EntityFrameworkCore;
using ShareVolt.Models;
using System.Collections.Generic;
using System.Linq;

namespace ShareVolt.Repositories
{
    public class SQLUserRepository : IUserRepository
    {
        private readonly ShareVoltDbContext _context;
        public SQLUserRepository(ShareVoltDbContext context)
        {
            _context = context;
        }

        public User AddUser(User user)
        {
            _context.Users.Add(user);
            _context.SaveChanges();
            return user;
        }

        public User DeleteUser(int id)
        {
            var user = _context.Users.Find(id);
            if (user != null)
            {
                _context.Users.Remove(user);
                _context.SaveChanges();
            }
            return user;
        }

        public IEnumerable<User> GetAllUsers()
        {
            //return _context.Students;
            // idealy we write above one but here we want to also load itt's connected information too
            return _context.Users
                .Include(u => u.Chargers)
                .Include(u => u.Bookings)
                .ToList();
        }

        public User GetUser(int id)
        {
            return _context.Users
                .Include(u => u.Chargers)
                .Include(u => u.Bookings)
                .FirstOrDefault(u => u.Id == id);
        }

        public User UpdateUser(User user)
        {
            _context.Users.Update(user);
            _context.SaveChanges();
            return user;
        }
    }
}
