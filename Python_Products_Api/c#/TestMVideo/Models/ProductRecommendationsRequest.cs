#region using

using System.ComponentModel.DataAnnotations;

#endregion

namespace TestMVideo.Models
{
    public class ProductRecommendationsRequest
    {
        [Required] public string ProductId { get; set; }
        public float MinRank { get; set; } = 0;
    }
}