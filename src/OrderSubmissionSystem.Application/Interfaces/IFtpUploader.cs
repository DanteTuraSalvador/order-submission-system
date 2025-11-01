using System.Threading.Tasks;
using OrderSubmissionSystem.Application.Models;

namespace OrderSubmissionSystem.Application.Interfaces
{
    public interface IFtpUploader
    {
        Task<bool> UploadAsync(OrderFilePayload payload);
    }
}
