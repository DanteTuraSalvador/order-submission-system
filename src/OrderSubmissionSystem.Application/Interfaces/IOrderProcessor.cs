using OrderSubmissionSystem.Domain.Entities;
using System.Threading.Tasks;

namespace OrderSubmissionSystem.Application.Interfaces
{
    public interface IOrderProcessor
    {
        Task<bool> ProcessOrderAsync(Order order);  // ← MUST return Task<bool>
    }
}