#region using

using System.Globalization;

#endregion

namespace TestMVideo.Models
{
    public struct InputLine
    {
        public InputLine(string csvLine)
        {
            var parameters = csvLine.Split(',');
            ProductId = parameters[0];
            RecommendationId = parameters[1];
            Rank = float.Parse(parameters[2], NumberStyles.Any, CultureInfo.InvariantCulture);
        }

        public string ProductId { get; set; }
        public string RecommendationId { get; set; }
        public float Rank { get; set; }
    }
}