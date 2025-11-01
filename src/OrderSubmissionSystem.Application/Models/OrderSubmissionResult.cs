namespace OrderSubmissionSystem.Application.Models
{
    public class OrderSubmissionResult
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public string OrderId { get; set; }
        public OrderSubmissionStatus Status { get; set; }

        public static OrderSubmissionResult SuccessResult(string orderId)
        {
            return new OrderSubmissionResult
            {
                Success = true,
                Message = "Order submitted successfully",
                OrderId = orderId,
                Status = OrderSubmissionStatus.Success
            };
        }

        public static OrderSubmissionResult ValidationFailure(string message)
        {
            return new OrderSubmissionResult
            {
                Success = false,
                Message = message,
                Status = OrderSubmissionStatus.ValidationFailed
            };
        }

        public static OrderSubmissionResult ProcessingFailure(string message, string orderId = null)
        {
            return new OrderSubmissionResult
            {
                Success = false,
                Message = message,
                OrderId = orderId,
                Status = OrderSubmissionStatus.ProcessingFailed
            };
        }

        public static OrderSubmissionResult FailureResult(string message)
        {
            return ProcessingFailure(message);
        }
    }
}
