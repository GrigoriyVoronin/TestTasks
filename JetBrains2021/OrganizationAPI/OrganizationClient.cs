using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OrganizationApi.Models;
using OrganizationApi.Models.Response;
using OrganizationApi.Models.Rules;

namespace OrganizationApi
{
    public class OrganizationClient
    {
        private readonly Dictionary<int, List<BypassSheet>> _bypassSheetSnapshots =
            new Dictionary<int, List<BypassSheet>>();

        private readonly Dictionary<int, (int CheckedCount, bool IsCycle)> _checkedBypassSheets =
            new Dictionary<int, (int, bool)>();

        private bool _isCycle;
        private bool _isSetupStarted;
        private Task _setupTask;

        public async Task SetupOrganization(Organization organization, Path path)
        {
            await ValidateInputAndCalculateRoute(organization, path);
        }

        public async Task SetupOrganization(Action<OrganizationBuilder> orgBuilderAction)
        {
            var orgBuilder = new OrganizationBuilder();
            orgBuilderAction(orgBuilder);
            await ValidateInputAndCalculateRoute(orgBuilder.Organization, orgBuilder.Path);
        }

        public async Task SetupOrganization(int stampsCount, int startDepartment, int endDepartment,
            Dictionary<int, IDepartmentRule> departments)
        {
            var path = new Path(startDepartment, endDepartment);
            var organization = new Organization(stampsCount, departments
                .Select(x => new Department(x.Key, x.Value))
                .ToList());
            await ValidateInputAndCalculateRoute(organization, path);
        }

        public async Task<BypassSheetInfoResponse> GetSheetsInDepartment(int departmentNumber)
        {
            await WaitOrganizationSetup();
            var visitedCount = _bypassSheetSnapshots.GetValueOrDefault(departmentNumber)?.Count ?? 0;
            var routeStatus = visitedCount == 0
                ? RouteStatus.Unvisited
                : _isCycle
                    ? RouteStatus.EndlessCycle
                    : RouteStatus.Attended;

            var bypassSheets = routeStatus switch
            {
                RouteStatus.Unvisited => new List<BypassSheet>(),
                _ => _bypassSheetSnapshots[departmentNumber]
            };
            return new BypassSheetInfoResponse(routeStatus, bypassSheets);
        }

        private async Task ValidateInputAndCalculateRoute(Organization organization, Path path)
        {
            CheckOrganizationAlreadySetup();
            CheckInputData(organization, path);
            _setupTask = CalculateRoute(organization, path);
            await _setupTask;
        }

        private async Task CalculateRoute(Organization organization, Path path)
        {
            await Task.Run(() =>
            {
                var sheet = new BypassSheet(organization, path);
                do
                {
                    var currentDepartment = sheet.CurrentDepartmentNumber;
                    ExecuteDepartmentRule(sheet, organization);
                    if (currentDepartment == path.End)
                    {
                        TakeStampsSnapshot(currentDepartment, sheet);
                        break;
                    }

                    if (IsInCycle(sheet, currentDepartment))
                        break;

                    TakeStampsSnapshot(currentDepartment, sheet);
                } while (true);
            });
        }

        private bool IsInCycle(BypassSheet sheet, int currentNumber)
        {
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
            if (differentStamps == 0 && sheetSnapshot.Stamps.Count == sheet.Stamps.Count)
            {
                _checkedBypassSheets[currentNumber] = (checkedCount + 1, true);
                _isCycle = true;
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

        private static void ExecuteDepartmentRule(BypassSheet sheet, Organization organization)
        {
            organization
                .GetDepartmentByNumber(sheet.CurrentDepartmentNumber)
                .Rule.Execute(sheet);
        }

        private async Task WaitOrganizationSetup()
        {
            if (_setupTask == null)
                throw new ArgumentException("You should start organization setup");

            await _setupTask;
        }

        private void CheckOrganizationAlreadySetup()
        {
            if (_isSetupStarted)
                throw new ArgumentException("Organization setup already started");
            _isSetupStarted = true;
        }

        private static void CheckInputData(Organization organization, Path path)
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