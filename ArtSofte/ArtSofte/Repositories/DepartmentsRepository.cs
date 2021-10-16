using Artsofte.Db;
using Artsofte.Models;

namespace Artsofte.Repositories
{
    public class DepartmentsRepository : AbstractRepository<Department>
    {
        public DepartmentsRepository(StaffContext staffContext)
            : base(staffContext)
        {
        }
    }
}