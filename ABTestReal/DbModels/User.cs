using System;
using System.ComponentModel.DataAnnotations;

namespace DbModels
{
    public class User
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public DateTime RegistrationDate { get; set; }
        [Required]
        public DateTime LastActivityDate { get; set; }
    }
}