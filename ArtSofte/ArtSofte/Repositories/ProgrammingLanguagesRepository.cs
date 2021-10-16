using Artsofte.Db;
using Artsofte.Models;

namespace Artsofte.Repositories
{
    public class ProgrammingLanguagesRepository : AbstractRepository<ProgrammingLanguage>
    {
        public ProgrammingLanguagesRepository(StaffContext staffContext)
            : base(staffContext)
        {
        }
    }
}