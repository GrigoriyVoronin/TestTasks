namespace Artsofte.Models
{
    public class Employee
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string LastName { get; set; }
        public int Age { get; set; }
        public bool Hide { get; set; }
        public Department Department { get; set; }
        public ProgrammingLanguage ProgrammingLanguage { get; set; }
    }
}