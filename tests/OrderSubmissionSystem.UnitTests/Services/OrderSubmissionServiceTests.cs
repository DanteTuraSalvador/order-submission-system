using System.Threading.Tasks;
using Moq;
using OrderSubmissionSystem.Application.Interfaces;
using OrderSubmissionSystem.Application.Models;
using OrderSubmissionSystem.Application.Services;
using OrderSubmissionSystem.UnitTests.Helpers;
using Xunit;

namespace OrderSubmissionSystem.UnitTests.Services
{
    public class OrderSubmissionServiceTests
    {
        [Fact]
        public async Task SubmitOrderAsync_InvalidOrder_ReturnsValidationFailure()
        {
            var processor = new Mock<IOrderProcessor>(MockBehavior.Strict);
            var service = new OrderSubmissionService(processor.Object);

            var order = TestOrderFactory.CreateOrder(o => o.CustomerId = null);

            var result = await service.SubmitOrderAsync(order);

            Assert.False(result.Success);
            Assert.Equal(OrderSubmissionStatus.ValidationFailed, result.Status);
            processor.Verify(p => p.ProcessOrderAsync(It.IsAny<OrderSubmissionSystem.Domain.Entities.Order>()), Times.Never);
        }

        [Fact]
        public async Task SubmitOrderAsync_ProcessorFailure_ReturnsProcessingFailure()
        {
            var processor = new Mock<IOrderProcessor>();
            processor.Setup(p => p.ProcessOrderAsync(It.IsAny<OrderSubmissionSystem.Domain.Entities.Order>()))
                     .ReturnsAsync(false);

            var service = new OrderSubmissionService(processor.Object);
            var order = TestOrderFactory.CreateOrder();

            var result = await service.SubmitOrderAsync(order);

            Assert.False(result.Success);
            Assert.Equal(OrderSubmissionStatus.ProcessingFailed, result.Status);
            processor.Verify(p => p.ProcessOrderAsync(order), Times.Once);
        }

        [Fact]
        public async Task SubmitOrderAsync_Success_ReturnsSuccessResult()
        {
            var processor = new Mock<IOrderProcessor>();
            processor.Setup(p => p.ProcessOrderAsync(It.IsAny<OrderSubmissionSystem.Domain.Entities.Order>()))
                     .ReturnsAsync(true);

            var service = new OrderSubmissionService(processor.Object);
            var order = TestOrderFactory.CreateOrder();

            var result = await service.SubmitOrderAsync(order);

            Assert.True(result.Success);
            Assert.Equal(OrderSubmissionStatus.Success, result.Status);
            Assert.Equal(order.OrderId, result.OrderId);
            processor.Verify(p => p.ProcessOrderAsync(order), Times.Once);
        }
    }
}
