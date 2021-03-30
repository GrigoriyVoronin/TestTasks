using System;

namespace GeCoTest.Models.DB
{
    public sealed class DbOrder
    {
        public Guid Id { get; set; }
        public string SystemType { get; set; }
        public string OrderNumber { get; set; }
        public string SourceOrder { get; set; }
        public string ConvertedOrder { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsHandled { get; set; }
    }
}