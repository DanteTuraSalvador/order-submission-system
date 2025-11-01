namespace OrderSubmissionSystem.Application.Models
{
    public class OrderSubmissionResult
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public string OrderId { get; set; }

        public static OrderSubmissionResult SuccessResult(string orderId)
        {
            return new OrderSubmissionResult
            {
                Success = true,
                Message = "Order submitted successfully",
                OrderId = orderId
            };
        }

        public static OrderSubmissionResult FailureResult(string message)
        {
            return new OrderSubmissionResult
            {
                Success = false,
                Message = message
            };
        }
    }
}