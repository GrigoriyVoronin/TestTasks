using System;

namespace GeCoTest.Models.API.Requests
{
    public sealed class AddOrderRequest
    {
        public string OrderNumber { get; set; }
        public ProductFromRequest[] Products { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}