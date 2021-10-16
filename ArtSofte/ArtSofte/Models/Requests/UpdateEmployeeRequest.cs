namespace Artsofte.Models.Requests
{
    public class UpdateEmployeeRequest
    {
        public string Name { get; set; }
        public string LastName { get; set; }
        public int Age { get; set; }
        public int DepartmentId { get; set; }
        public int ProgrammingLanguageId { get; set; }
    }
}