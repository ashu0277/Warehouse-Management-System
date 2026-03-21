using Microsoft.AspNetCore.Authorization;

using Microsoft.AspNetCore.Mvc;

using WarehousePro.API.DTOs.Warehouse;

using WarehousePro.API.Services.Interfaces;

namespace WarehousePro.API.Controllers

{

	[ApiController]

	[Route("api/[controller]")]

	[Authorize]

	public class WarehouseController : ControllerBase

	{

		private readonly IWarehouseService _warehouseService;

		public WarehouseController(IWarehouseService warehouseService)

		{

			_warehouseService = warehouseService;

		}

		// GET api/warehouse

		[HttpGet]

		public async Task<IActionResult> GetAll()

		{

			var result = await _warehouseService.GetAllAsync();

			return Ok(result);

		}

		// GET api/warehouse/5

		[HttpGet("{id}")]

		public async Task<IActionResult> GetById(int id)

		{

			var result = await _warehouseService.GetByIdAsync(id);

			if (result == null)

				return NotFound(new { message = $"Warehouse with ID {id} not found." });

			return Ok(result);

		}

		// POST api/warehouse

		[HttpPost]

		[Authorize(Roles = "Admin")]

		public async Task<IActionResult> Create([FromBody] WarehouseCreateDto dto)

		{

			if (!ModelState.IsValid)

				return BadRequest(ModelState);

			var result = await _warehouseService.CreateAsync(dto);

			return CreatedAtAction(nameof(GetById), new { id = result.WarehouseID }, result);

		}

		// PUT api/warehouse/5

		[HttpPut("{id}")]

		[Authorize(Roles = "Admin")]

		public async Task<IActionResult> Update(int id, [FromBody] WarehouseUpdateDto dto)

		{

			if (!ModelState.IsValid)

				return BadRequest(ModelState);

			var result = await _warehouseService.UpdateAsync(id, dto);

			if (result == null)

				return NotFound(new { message = $"Warehouse with ID {id} not found." });

			return Ok(result);

		}

		// DELETE api/warehouse/5

		[HttpDelete("{id}")]

		[Authorize(Roles = "Admin")]

		public async Task<IActionResult> Delete(int id)

		{

			var result = await _warehouseService.DeleteAsync(id);

			if (!result)

				return NotFound(new { message = $"Warehouse with ID {id} not found." });

			return Ok(new { message = "Warehouse deleted successfully." });

		}

	}

}
