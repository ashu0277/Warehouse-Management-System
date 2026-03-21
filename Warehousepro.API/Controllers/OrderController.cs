using Microsoft.AspNetCore.Authorization;

using Microsoft.AspNetCore.Mvc;

using WarehousePro.API.DTOs.Order;

using WarehousePro.API.Services.Interfaces;

namespace WarehousePro.API.Controllers

{

	[ApiController]

	[Route("api/[controller]")]

	[Authorize]

	public class OrderController : ControllerBase

	{

		private readonly IOrderService _orderService;

		public OrderController(IOrderService orderService)

		{

			_orderService = orderService;

		}

		// GET api/order

		[HttpGet]

		public async Task<IActionResult> GetAll()

		{

			var result = await _orderService.GetAllAsync();

			return Ok(result);

		}

		// GET api/order/5

		[HttpGet("{id}")]

		public async Task<IActionResult> GetById(int id)

		{

			var result = await _orderService.GetByIdAsync(id);

			if (result == null)

				return NotFound(new { message = $"Order with ID {id} not found." });

			return Ok(result);

		}

		// GET api/order/number/ORD-001

		[HttpGet("number/{orderNumber}")]

		public async Task<IActionResult> GetByOrderNumber(string orderNumber)

		{

			var result = await _orderService.GetByOrderNumberAsync(orderNumber);

			if (result == null)

				return NotFound(new { message = $"Order '{orderNumber}' not found." });

			return Ok(result);

		}

		// POST api/order

		[HttpPost]

		[Authorize(Roles = "Admin,Supervisor,Logistics")]

		public async Task<IActionResult> Create([FromBody] OrderCreateDto dto)

		{

			if (!ModelState.IsValid)

				return BadRequest(ModelState);

			var result = await _orderService.CreateAsync(dto);

			return CreatedAtAction(nameof(GetById), new { id = result.OrderID }, result);

		}

		// PUT api/order/5

		[HttpPut("{id}")]

		[Authorize(Roles = "Admin,Supervisor,Logistics")]

		public async Task<IActionResult> Update(int id, [FromBody] OrderUpdateDto dto)

		{

			if (!ModelState.IsValid)

				return BadRequest(ModelState);

			var result = await _orderService.UpdateAsync(id, dto);

			if (result == null)

				return NotFound(new { message = $"Order with ID {id} not found." });

			return Ok(result);

		}

		// DELETE api/order/5

		[HttpDelete("{id}")]

		[Authorize(Roles = "Admin")]

		public async Task<IActionResult> Delete(int id)

		{

			var result = await _orderService.DeleteAsync(id);

			if (!result)

				return NotFound(new { message = $"Order with ID {id} not found." });

			return Ok(new { message = "Order deleted successfully." });

		}

	}

}
