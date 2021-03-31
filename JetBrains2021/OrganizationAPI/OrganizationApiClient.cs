using System;
using System.Collections.Generic;
using System.Linq;
using OrganizationApi.Models;
using OrganizationApi.Models.Response;
using OrganizationApi.Models.Rules;

namespace OrganizationApi
{
    public class OrganizationApiClient
    {
        private readonly Dictionary<int, List<BypassSheet>> _bypassSheetSnapshots =
            new Dictionary<int, List<BypassSheet>>();

        private readonly Dictionary<int, (int CheckedCount, bool IsCycle)> _checkedBypassSheets =
            new Dictionary<int, (int, bool)>();

        private bool _isSetup;

        public void SetupOrganization(Organization organization, Path path)
        {
            ValidateInputAndCalculateRoute(organization, path);
        }

        public void SetupOrganization(Action<OrganizationBuilder> orgBuilderAction)
        {
            var orgBuilder = new OrganizationBuilder();
            orgBuilderAction(orgBuilder);
            ValidateInputAndCalculateRoute(orgBuilder.Organization, orgBuilder.Path);
        }

        public void SetupOrganization(int stampsCount, int startDepartment, int endDepartment,
            Dictionary<int, IDepartmentRule> departments)
        {
            var path = new Path(startDepartment, endDepartment);
            var organization = new Organization(stampsCount, departments
                .Select(x => new Department(x.Key, x.Value))
                .ToList());
            ValidateInputAndCalculateRoute(organization, path);
        }

        public BypassSheetInfoResponse GetSheetsInDepartment(int departmentNumber)
        {
            var visitedCount = _bypassSheetSnapshots.GetValueOrDefault(departmentNumber)?.Count ?? 0;
            var routeStatus = visitedCount == 0
                ? RouteStatus.Unvisited
                : visitedCount > 1
                    ? _checkedBypassSheets[departmentNumber].IsCycle
                        ? RouteStatus.EndlessCycle
                        : RouteStatus.Attended
                    : RouteStatus.Attended;

            var bypassSheets = routeStatus switch
            {
                RouteStatus.Unvisited => new List<BypassSheet>(),
                _ => _bypassSheetSnapshots[departmentNumber]
            };
            return new BypassSheetInfoResponse(routeStatus, bypassSheets);
        }

        private void ValidateInputAndCalculateRoute(Organization organization, Path path)
        {
            CheckOrganizationSetup();
            CheckInputData(organization, path);
            CalculateRoute(organization, path);
        }

        private void CalculateRoute(Organization organization, Path path)
        {
            var sheet = new BypassSheet(organization, path);
            do
            {
                if (IsInCycle(sheet))
                    break;

                var currentDepartment = sheet.CurrentDepartmentNumber;
                ExecuteDepartmentRule(sheet, organization);
                TakeStampsSnapshot(currentDepartment, sheet);
                if (currentDepartment == path.End)
                    break;
            } while (true);
        }

        private bool IsInCycle(BypassSheet sheet)
        {
            var currentNumber = sheet.CurrentDepartmentNumber;
            if (!_bypassSheetSnapshots.ContainsKey(currentNumber))
                return false;

            var sheetSnapshots = _bypassSheetSnapshots[currentNumber];
            var (checkedCount, isCycle) = _checkedBypassSheets.GetValueOrDefault(currentNumber);
            if (checkedCount == sheetSnapshots.Count)
                return isCycle;

            var sheetSnapshot = sheetSnapshots[checkedCount];
            var differentStamps = sheetSnapshot.Stamps
                .Except(sheet.Stamps)
                .Count();
            if (differentStamps == 0)
            {
                _checkedBypassSheets[currentNumber] = (checkedCount + 1, true);
                return true;
            }

            _checkedBypassSheets[currentNumber] = (checkedCount + 1, false);
            return false;
        }

        private void TakeStampsSnapshot(int currentDepartment, BypassSheet sheet)
        {
            if (!_bypassSheetSnapshots.ContainsKey(currentDepartment))
                _bypassSheetSnapshots[currentDepartment] = new List<BypassSheet>();
            _bypassSheetSnapshots[currentDepartment].Add(sheet.GetSnapshot());
        }

        private void ExecuteDepartmentRule(BypassSheet sheet, Organization organization)
        {
            organization
                .GetDepartmentByNumber(sheet.CurrentDepartmentNumber)
                .Rule.Execute(sheet);
        }

        private void CheckOrganizationSetup()
        {
            if (_isSetup)
                throw new ArgumentException("Organization already Setup");
            _isSetup = true;
        }

        private void CheckInputData(Organization organization, Path path)
        {
            if (path == null)
                throw new ArgumentNullException(nameof(Path), "You should setup Path");
            if (organization == null)
                throw new ArgumentNullException(nameof(Organization), "You should setup Organization");
            if (organization.StampsCount < 2)
                throw new ArgumentException("You should setup Stamps Count, value should be larger than 1");
            if (organization.Departments.Count < 2)
                throw new ArgumentException("You should setup Departments, count should be larger than 1");
            var departmentsUniqueNumbers = organization.Departments
                .Select(x => x.Number)
                .ToHashSet();
            if (departmentsUniqueNumbers.Count != organization.Departments.Count)
                throw new ArgumentException("Departments numbers should be unique");
            var maxNumber = departmentsUniqueNumbers.Max();
            if (maxNumber != departmentsUniqueNumbers.Count)
                throw new ArgumentException("Departments Numbers should go one by one");
            if (!departmentsUniqueNumbers.Contains(path.End) || !departmentsUniqueNumbers.Contains(path.Start))
                throw new ArgumentException("Path should go through existing departments");
        }
    }
}