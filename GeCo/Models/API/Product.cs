namespace GeCoTest.Models.API
{
    public class Product
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Comment { get; set; }
        public decimal Quantity { get; set; }
        public decimal PaidPrice { get; set; }
        public decimal UnitPrice { get; set; }
        public string RemoteCode { get; set; }
        public string Description { get; set; }
        public decimal VatPercentage { get; set; }
        public decimal DiscountAmount { get; set; }
    }
}