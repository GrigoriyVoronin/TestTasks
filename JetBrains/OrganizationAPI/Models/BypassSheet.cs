using System.Collections.Generic;

namespace OrganizationApi.Models
{
    public class BypassSheet
    {
        private readonly Organization _organization;
        private readonly Path _path;

        internal BypassSheet(Organization organization, Path path)
        {
            _organization = organization;
            _path = path;
            CurrentDepartmentNumber = path.Start;
            Stamps = new HashSet<int>();
        }

        private BypassSheet()
        {
        }

        public HashSet<int> Stamps { get; private set; }
        public int CurrentDepartmentNumber { get; private set; }

        internal BypassSheet GetSnapshot()
        {
            return new BypassSheet
            {
                CurrentDepartmentNumber = CurrentDepartmentNumber,
                Stamps = new HashSet<int>(Stamps)
            };
        }

        internal bool AddStamp(int stamp)
        {
            return Stamps.Add(stamp);
        }

        internal bool RemoveStamp(int stamp)
        {
            return Stamps.Remove(stamp);
        }

        internal bool Contains(int stamp)
        {
            return Stamps.Contains(stamp);
        }

        internal void MoveTo(int departmentNumber)
        {
            CurrentDepartmentNumber = departmentNumber;
        }
    }
}