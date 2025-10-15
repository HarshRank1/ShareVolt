using ShareVolt.Models;
using System.Collections;
using System.Collections.Generic;

namespace ShareVolt.Repositories
{
    public interface IUserRepository
    {
        User GetUser(int id);
        IEnumerable<User> GetAllUsers();
        User AddUser(User user);
        User UpdateUser(User user);
        User DeleteUser(int id);

    }
}
