using Microsoft.AspNetCore.Authorization;

using Microsoft.AspNetCore.Mvc;

using WarehousePro.API.DTOs.BinLocation;

using WarehousePro.API.Services.Interfaces;

namespace WarehousePro.API.Controllers

{

	[ApiController]

	[Route("api/[controller]")]

	[Authorize]

	public class BinLocationController : ControllerBase

	{

		private readonly IBinLocationService _binLocationService;

		public BinLocationController(IBinLocationService binLocationService)

		{

			_binLocationService = binLocationService;

		}

		// GET api/binlocation

		[HttpGet]

		public async Task<IActionResult> GetAll()

		{

			var result = await _binLocationService.GetAllAsync();

			return Ok(result);

		}

		// GET api/binlocation/by-zone/5

		[HttpGet("by-zone/{zoneId}")]

		public async Task<IActionResult> GetByZone(int zoneId)

		{

			var result = await _binLocationService.GetByZoneIdAsync(zoneId);

			return Ok(result);

		}

		// GET api/binlocation/5

		[HttpGet("{id}")]

		public async Task<IActionResult> GetById(int id)

		{

			var result = await _binLocationService.GetByIdAsync(id);

			if (result == null)

				return NotFound(new { message = $"Bin with ID {id} not found." });

			return Ok(result);

		}

		// POST api/binlocation

		[HttpPost]

		[Authorize(Roles = "Admin")]

		public async Task<IActionResult> Create([FromBody] BinLocationCreateDto dto)

		{

			if (!ModelState.IsValid)

				return BadRequest(ModelState);

			var result = await _binLocationService.CreateAsync(dto);

			return CreatedAtAction(nameof(GetById), new { id = result.BinID }, result);

		}

		// PUT api/binlocation/5

		[HttpPut("{id}")]

		[Authorize(Roles = "Admin,Supervisor")]

		public async Task<IActionResult> Update(int id, [FromBody] BinLocationUpdateDto dto)

		{

			if (!ModelState.IsValid)

				return BadRequest(ModelState);

			var result = await _binLocationService.UpdateAsync(id, dto);

			if (result == null)

				return NotFound(new { message = $"Bin with ID {id} not found." });

			return Ok(result);

		}

		// DELETE api/binlocation/5

		[HttpDelete("{id}")]

		[Authorize(Roles = "Admin")]

		public async Task<IActionResult> Delete(int id)

		{

			var result = await _binLocationService.DeleteAsync(id);

			if (!result)

				return NotFound(new { message = $"Bin with ID {id} not found." });

			return Ok(new { message = "Bin deleted successfully." });

		}

	}

}
