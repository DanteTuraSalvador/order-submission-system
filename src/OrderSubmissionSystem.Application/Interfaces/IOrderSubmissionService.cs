using OrderSubmissionSystem.Application.Models;
using OrderSubmissionSystem.Domain.Entities;
using System.Threading.Tasks;

namespace OrderSubmissionSystem.Application.Interfaces
{
    public interface IOrderSubmissionService
    {
        Task<OrderSubmissionResult> SubmitOrderAsync(Order order);

        bool ValidateOrder(Order order);
    }
}
