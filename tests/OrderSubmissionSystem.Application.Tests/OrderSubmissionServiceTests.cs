using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using OrderSubmissionSystem.Application.Interfaces;
using OrderSubmissionSystem.Application.Models;
using OrderSubmissionSystem.Application.Services;
using OrderSubmissionSystem.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OrderSubmissionSystem.Application.Tests
{
    [TestClass]
    public class OrderSubmissionServiceTests
    {
        [TestMethod]
        public async Task SubmitOrderAsync_InvalidOrder_ReturnsValidationFailure()
        {
            var processorMock = new Mock<IOrderProcessor>(MockBehavior.Strict);
            var service = new OrderSubmissionService(processorMock.Object);

            var invalidOrder = new Order
            {
                OrderId = "ORDER-001",
                CustomerId = null,
                TotalAmount = 0m
            };

            var result = await service.SubmitOrderAsync(invalidOrder);

            Assert.IsFalse(result.Success);
            Assert.AreEqual(OrderSubmissionStatus.ValidationFailed, result.Status);
            processorMock.Verify(p => p.ProcessOrderAsync(It.IsAny<Order>()), Times.Never);
        }

        [TestMethod]
        public async Task SubmitOrderAsync_ProcessingFailure_ReturnsProcessingFailure()
        {
            var processorMock = new Mock<IOrderProcessor>();
            processorMock
                .Setup(p => p.ProcessOrderAsync(It.IsAny<Order>()))
                .ReturnsAsync(false);

            var service = new OrderSubmissionService(processorMock.Object);
            var order = CreateValidOrder();

            var result = await service.SubmitOrderAsync(order);

            Assert.IsFalse(result.Success);
            Assert.AreEqual(OrderSubmissionStatus.ProcessingFailed, result.Status);
            processorMock.Verify(p => p.ProcessOrderAsync(order), Times.Once);
        }

        [TestMethod]
        public async Task SubmitOrderAsync_Success_ReturnsSuccessResult()
        {
            var processorMock = new Mock<IOrderProcessor>();
            processorMock
                .Setup(p => p.ProcessOrderAsync(It.IsAny<Order>()))
                .ReturnsAsync(true);

            var service = new OrderSubmissionService(processorMock.Object);
            var order = CreateValidOrder();

            var result = await service.SubmitOrderAsync(order);

            Assert.IsTrue(result.Success);
            Assert.AreEqual(OrderSubmissionStatus.Success, result.Status);
            Assert.AreEqual(order.OrderId, result.OrderId);
            processorMock.Verify(p => p.ProcessOrderAsync(order), Times.Once);
        }

        private static Order CreateValidOrder()
        {
            return new Order
            {
                OrderId = "ORDER-100",
                CustomerId = "CUSTOMER-100",
                TotalAmount = 100m,
                Items = new List<OrderItem>
                {
                    new OrderItem
                    {
                        ProductId = "SKU-1",
                        Quantity = 2,
                        UnitPrice = 50m
                    }
                }
            };
        }
    }
}
