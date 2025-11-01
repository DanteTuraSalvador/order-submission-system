using OrderSubmissionSystem.Domain.Entities;
using System.Threading.Tasks;

namespace OrderSubmissionSystem.Application.Interfaces
{
    public interface IOrderSubmissionService
    {
        Task<bool> SubmitOrderAsync(Order order);  // ← Must return Task<bool>
        bool ValidateOrder(Order order);
    }
}