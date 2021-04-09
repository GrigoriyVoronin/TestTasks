using System.Collections.Generic;
using System.Threading.Tasks;
using DbModels;
using DbRepositories;

namespace ABTestReal.Services
{
    public class UsersService
    {
        private readonly UsersRepository _usersRepository;

        public UsersService(UsersRepository usersRepository)
        {
            _usersRepository = usersRepository;
        }

        public async Task<List<User>> GetUsersAsync()
        {
            return await _usersRepository.GetUsersAsync();
        }

        public async Task<List<User>> UpdateUsersAsync(List<User> users)
        {
            await _usersRepository.UpdateUsersAsync(users);
            return users;
        }
    }
}