using OrderSubmissionSystem.Application.Interfaces;
using OrderSubmissionSystem.Application.Models;
using OrderSubmissionSystem.Domain.Entities;
using System;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;

namespace OrderSubmissionSystem.Api.Controllers
{
    [RoutePrefix("api/orders")]
    public class OrdersController : ApiController
    {
        private readonly IOrderSubmissionService _orderSubmissionService;

        public OrdersController(IOrderSubmissionService orderSubmissionService)
        {
            _orderSubmissionService = orderSubmissionService;
        }

        [HttpPost]
        [Route("")]
        public async Task<IHttpActionResult> SubmitOrder(Order order)
        {
            if (order == null)
                return BadRequest("Order cannot be null");

            var submissionResult = await _orderSubmissionService.SubmitOrderAsync(order);

            if (submissionResult.Success)
                return Ok(submissionResult);

            if (submissionResult.Status == OrderSubmissionStatus.ValidationFailed)
                return Content(HttpStatusCode.BadRequest, submissionResult);

            return Content(HttpStatusCode.InternalServerError, submissionResult);
        }

        [HttpGet]
        [Route("health")]
        public IHttpActionResult Health()
        {
            return Ok(new { status = "healthy", timestamp = DateTime.UtcNow });
        }
    }
}
