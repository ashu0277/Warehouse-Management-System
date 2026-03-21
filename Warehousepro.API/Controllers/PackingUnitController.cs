using Microsoft.AspNetCore.Authorization;

using Microsoft.AspNetCore.Mvc;

using WarehousePro.API.DTOs.Outbound;

using WarehousePro.API.Services.Interfaces;

namespace WarehousePro.API.Controllers

{

	[ApiController]

	[Route("api/[controller]")]

	[Authorize]

	public class PackingUnitController : ControllerBase

	{

		private readonly IPackingUnitService _packingUnitService;

		public PackingUnitController(IPackingUnitService packingUnitService)

		{

			_packingUnitService = packingUnitService;

		}

		// GET api/packingunit

		[HttpGet]

		public async Task<IActionResult> GetAll()

		{

			var result = await _packingUnitService.GetAllAsync();

			return Ok(result);

		}

		// GET api/packingunit/by-order/5

		[HttpGet("by-order/{orderId}")]

		public async Task<IActionResult> GetByOrder(int orderId)

		{

			var result = await _packingUnitService.GetByOrderIdAsync(orderId);

			return Ok(result);

		}

		// GET api/packingunit/5

		[HttpGet("{id}")]

		public async Task<IActionResult> GetById(int id)

		{

			var result = await _packingUnitService.GetByIdAsync(id);

			if (result == null)

				return NotFound(new { message = $"Packing unit with ID {id} not found." });

			return Ok(result);

		}

		// POST api/packingunit

		[HttpPost]

		[Authorize(Roles = "Admin,Supervisor,Operator")]

		public async Task<IActionResult> Create([FromBody] PackingUnitCreateDto dto)

		{

			if (!ModelState.IsValid)

				return BadRequest(ModelState);

			var result = await _packingUnitService.CreateAsync(dto);

			return CreatedAtAction(nameof(GetById), new { id = result.PackID }, result);

		}

		// PUT api/packingunit/5

		[HttpPut("{id}")]

		[Authorize(Roles = "Admin,Supervisor,Logistics")]

		public async Task<IActionResult> Update(int id, [FromBody] PackingUnitUpdateDto dto)

		{

			if (!ModelState.IsValid)

				return BadRequest(ModelState);

			var result = await _packingUnitService.UpdateAsync(id, dto);

			if (result == null)

				return NotFound(new { message = $"Packing unit with ID {id} not found." });

			return Ok(result);

		}

		// DELETE api/packingunit/5

		[HttpDelete("{id}")]

		[Authorize(Roles = "Admin")]

		public async Task<IActionResult> Delete(int id)

		{

			var result = await _packingUnitService.DeleteAsync(id);

			if (!result)

				return NotFound(new { message = $"Packing unit with ID {id} not found." });

			return Ok(new { message = "Packing unit deleted successfully." });

		}

	}

}
