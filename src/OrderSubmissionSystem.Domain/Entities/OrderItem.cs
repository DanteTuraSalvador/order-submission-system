namespace OrderSubmissionSystem.Domain.Entities
{
    public class OrderItem
    {
        public string ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }

        public decimal GetLineTotal()
        {
            return Quantity * UnitPrice;
        }
    }
}
