using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DbModels;
using Microsoft.EntityFrameworkCore;

namespace DbRepositories
{
    public class UsersRepository
    {
        private readonly UsersContext _usersContext;

        public UsersRepository(UsersContext usersContext)
        {
            _usersContext = usersContext;
        }

        public async Task<List<User>> GetUsersAsync()
        {
            return await _usersContext.Users.ToListAsync();
        }

        public async Task<User> AddUserAsync(User user)
        {
            await _usersContext.Users.AddAsync(user);
            await _usersContext.SaveChangesAsync();
            return user;
        }

        public async Task UpdateUsersAsync(IEnumerable<User> users)
        {
            _usersContext.Users.UpdateRange(users);
            await _usersContext.SaveChangesAsync();
        }
    }
}