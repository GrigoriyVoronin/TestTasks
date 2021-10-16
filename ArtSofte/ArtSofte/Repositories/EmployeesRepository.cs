using System.Threading.Tasks;
using Artsofte.Db;
using Artsofte.Models;

namespace Artsofte.Repositories
{
    public class EmployeesRepository : AbstractRepository<Employee>
    {
        public EmployeesRepository(StaffContext staffContext)
            : base(staffContext)
        {
        }

        public override async Task DeleteAsync(Employee model)
        {
            model.Hide = true;
            await UpdateAsync(model);
        }
    }
}