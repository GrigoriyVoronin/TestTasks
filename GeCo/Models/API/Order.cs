using System;

namespace GeCoTest.Models.API
{
    public class Order
    {
        public Guid Id { get; set; }
        public string OrderNumber { get; set; }
        public Product[] Products { get; set; }
        public DateTime CreatedAt { get; set; }
        public string SystemType { get; set; }
    }
}