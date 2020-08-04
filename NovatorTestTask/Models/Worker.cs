using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace NovatorTestTask.Models
{
    public class Worker
    {
        public int Id { get; set; }

        [DisplayName("Фамилия")]
        [Required]
        public string SecondName { get; set; }

        [DisplayName("Имя")]
        [Required]
        public string FirstName { get; set; }

        [DisplayName("Отчество")]
        [Required]
        public string Patronymic { get; set; }

        public static Worker GetWorker(string firstName, string secondName, string patrinymic) =>
            new Worker
            {
                SecondName = secondName,
                FirstName = firstName,
                Patronymic = patrinymic
            };
    }
}