namespace OrderSubmissionSystem.Application.Models
{
    public class OrderFilePayload
    {
        public string FileName { get; set; }
        public string ContentType { get; set; }
        public byte[] Content { get; set; }
    }
}
