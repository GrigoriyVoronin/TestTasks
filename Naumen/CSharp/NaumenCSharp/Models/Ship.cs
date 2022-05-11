using System.ComponentModel.DataAnnotations;

namespace NaumenCSharp.Models
{
    public class Ship
    {
        [Required] public int TimeOfArrival { get; set; }

        [Required] public int HandleTime { get; set; }
    }
}