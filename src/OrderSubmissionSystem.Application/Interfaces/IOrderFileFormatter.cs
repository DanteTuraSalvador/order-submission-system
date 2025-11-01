using OrderSubmissionSystem.Application.Models;
using OrderSubmissionSystem.Domain.Entities;

namespace OrderSubmissionSystem.Application.Interfaces
{
    public interface IOrderFileFormatter
    {
        OrderFilePayload Format(Order order);
    }
}
