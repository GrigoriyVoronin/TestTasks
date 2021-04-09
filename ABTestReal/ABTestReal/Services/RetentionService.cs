using System;
using System.Linq;
using System.Threading.Tasks;
using ABTestReal.ApiModels;
using DbRepositories;

namespace ABTestReal.Services
{
    public class RetentionService
    {
        private readonly UsersRepository _usersRepository;

        public RetentionService(UsersRepository usersRepository)
        {
            _usersRepository = usersRepository;
        }

        public async Task<RetentionInfo> CalculateRetentionInfo(int days)
        {
            var users = await _usersRepository.GetUsersAsync();
            var usersRegistrationsCount = users.Count(x => x.RegistrationDate <= DateTime.Now.AddDays(-days));
            var usersReturnedCount = users.Count(x => x.LastActivityDate >= x.RegistrationDate.AddDays(7));
            var rollingRetention = usersReturnedCount
                                   /  (usersRegistrationsCount == 0 ? 1 : usersRegistrationsCount)
                                   * 100;
            var usersLifeSpan = users
                .Select(x => new UserLifeSpan
                    {LifeSpan = (int) (x.LastActivityDate - x.RegistrationDate).TotalDays, UserId = x.Id})
                .ToList();

            return new RetentionInfo{RollingRetention = rollingRetention, UserLifeSpans = usersLifeSpan};
        }
    }
}