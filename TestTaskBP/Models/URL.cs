using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TestTaskBP.Models
{
    public class URL
    {
        public int ID { get; set; }

        [DisplayName("Полный адрес")]
        [DataType(DataType.Url)]
        [Required]
        public string FullUrl { get; set; }

        [DisplayName("Сокращенный адрес")]
        [DataType(DataType.Url)]
        public string ShortUrl { get; set; }

        [DisplayName("Дата создания")]
        [DataType(DataType.Date)]
        public DateTime CreateDate { get; set; } = DateTime.Now;

        [DisplayName("Количество переходов")]
        [Column(TypeName = "int")]
        public int NumberOfTransitions { get; set; } = 0;
    }
}