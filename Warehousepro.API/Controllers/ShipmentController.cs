using Microsoft.AspNetCore.Authorization;

using Microsoft.AspNetCore.Mvc;

using WarehousePro.API.DTOs.Outbound;

using WarehousePro.API.Services.Interfaces;

namespace WarehousePro.API.Controllers

{

	[ApiController]

	[Route("api/[controller]")]

	[Authorize]

	public class ShipmentController : ControllerBase

	{

		private readonly IShipmentService _shipmentService;

		public ShipmentController(IShipmentService shipmentService)

		{

			_shipmentService = shipmentService;

		}

		// GET api/shipment

		[HttpGet]

		public async Task<IActionResult> GetAll()

		{

			var result = await _shipmentService.GetAllAsync();

			return Ok(result);

		}

		// GET api/shipment/by-order/5

		[HttpGet("by-order/{orderId}")]

		public async Task<IActionResult> GetByOrder(int orderId)

		{

			var result = await _shipmentService.GetByOrderIdAsync(orderId);

			return Ok(result);

		}

		// GET api/shipment/5

		[HttpGet("{id}")]

		public async Task<IActionResult> GetById(int id)

		{

			var result = await _shipmentService.GetByIdAsync(id);

			if (result == null)

				return NotFound(new { message = $"Shipment with ID {id} not found." });

			return Ok(result);

		}

		// POST api/shipment

		[HttpPost]

		[Authorize(Roles = "Admin,Supervisor,Logistics")]

		public async Task<IActionResult> Create([FromBody] ShipmentCreateDto dto)

		{

			if (!ModelState.IsValid)

				return BadRequest(ModelState);

			var result = await _shipmentService.CreateAsync(dto);

			return CreatedAtAction(nameof(GetById), new { id = result.ShipmentID }, result);

		}

		// PUT api/shipment/5

		[HttpPut("{id}")]

		[Authorize(Roles = "Admin,Supervisor,Logistics")]

		public async Task<IActionResult> Update(int id, [FromBody] ShipmentUpdateDto dto)

		{

			if (!ModelState.IsValid)

				return BadRequest(ModelState);

			var result = await _shipmentService.UpdateAsync(id, dto);

			if (result == null)

				return NotFound(new { message = $"Shipment with ID {id} not found." });

			return Ok(result);

		}

		// DELETE api/shipment/5

		[HttpDelete("{id}")]

		[Authorize(Roles = "Admin")]

		public async Task<IActionResult> Delete(int id)

		{

			var result = await _shipmentService.DeleteAsync(id);

			if (!result)

				return NotFound(new { message = $"Shipment with ID {id} not found." });

			return Ok(new { message = "Shipment deleted successfully." });

		}

	}

}
