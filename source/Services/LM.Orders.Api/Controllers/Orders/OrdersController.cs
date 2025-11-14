using MediatR;
using Microsoft.AspNetCore.Mvc;
using LM.Orders.Contracts.Orders.Commands;
using LM.Orders.Contracts.Orders.Queries;
using LM.Orders.Contracts.Orders.Responses;
using System.Security.Claims;

namespace LM.Orders.Api.Controllers.Orders
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrdersController(IMediator mediator) : ControllerBase
    {
        private readonly IMediator _mediator = mediator;

        private Guid GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (Guid.TryParse(userIdClaim, out Guid userId))
            {
                return userId;
            }

            return new Guid("3fa85f64-5717-4562-b3fc-2c963f66afa6");
        }

        [HttpPost]
        [ProducesResponseType(typeof(CreateOrderResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateOrder([FromBody] CreateOrderCommand command)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = GetCurrentUserId();
            command.SetCreatedByUserId(userId);

            var response = await _mediator.Send(command);

            return Ok(response);
        }

        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(OrderResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetOrder([FromRoute] Guid id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = GetCurrentUserId();

            var query = new GetOrderQuery(id, userId);
            var response = await _mediator.Send(query);

            if (response == null)
            {
                return NotFound();
            }

            return Ok(response);
        }
    }
}