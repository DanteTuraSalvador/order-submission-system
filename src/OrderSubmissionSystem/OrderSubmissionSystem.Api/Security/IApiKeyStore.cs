namespace OrderSubmissionSystem.Api.Security
{
    public interface IApiKeyStore
    {
        bool IsValid(string apiKey);
    }
}
