using System.ComponentModel.DataAnnotations;

namespace NaumenCSharp.Models
{
    public class SetupNumberOfPlacesRequest
    {
        [Required] public int NumberOfPlaces { get; set; }
    }
}