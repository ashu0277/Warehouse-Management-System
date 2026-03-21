using Microsoft.AspNetCore.Authorization;

using Microsoft.AspNetCore.Mvc;

using WarehousePro.API.DTOs.Zone;

using WarehousePro.API.Services.Interfaces;

namespace WarehousePro.API.Controllers

{

	[ApiController]

	[Route("api/[controller]")]

	[Authorize]

	public class ZoneController : ControllerBase

	{

		private readonly IZoneService _zoneService;

		public ZoneController(IZoneService zoneService)

		{

			_zoneService = zoneService;

		}

		// GET api/zone

		[HttpGet]

		public async Task<IActionResult> GetAll()

		{

			var result = await _zoneService.GetAllAsync();

			return Ok(result);

		}

		// GET api/zone/by-warehouse/5

		[HttpGet("by-warehouse/{warehouseId}")]

		public async Task<IActionResult> GetByWarehouse(int warehouseId)

		{

			var result = await _zoneService.GetByWarehouseIdAsync(warehouseId);

			return Ok(result);

		}

		// GET api/zone/5

		[HttpGet("{id}")]

		public async Task<IActionResult> GetById(int id)

		{

			var result = await _zoneService.GetByIdAsync(id);

			if (result == null)

				return NotFound(new { message = $"Zone with ID {id} not found." });

			return Ok(result);

		}

		// POST api/zone

		[HttpPost]

		[Authorize(Roles = "Admin")]

		public async Task<IActionResult> Create([FromBody] ZoneCreateDto dto)

		{

			if (!ModelState.IsValid)

				return BadRequest(ModelState);

			var result = await _zoneService.CreateAsync(dto);

			return CreatedAtAction(nameof(GetById), new { id = result.ZoneID }, result);

		}

		// PUT api/zone/5

		[HttpPut("{id}")]

		[Authorize(Roles = "Admin")]

		public async Task<IActionResult> Update(int id, [FromBody] ZoneUpdateDto dto)

		{

			if (!ModelState.IsValid)

				return BadRequest(ModelState);

			var result = await _zoneService.UpdateAsync(id, dto);

			if (result == null)

				return NotFound(new { message = $"Zone with ID {id} not found." });

			return Ok(result);

		}

		// DELETE api/zone/5

		[HttpDelete("{id}")]

		[Authorize(Roles = "Admin")]

		public async Task<IActionResult> Delete(int id)

		{

			var result = await _zoneService.DeleteAsync(id);

			if (!result)

				return NotFound(new { message = $"Zone with ID {id} not found." });

			return Ok(new { message = "Zone deleted successfully." });

		}

	}

}
