using Microsoft.AspNetCore.Authorization;

using Microsoft.AspNetCore.Mvc;

using System.Security.Claims;

using WarehousePro.API.DTOs.Analytics;

using WarehousePro.API.Services.Interfaces;

namespace WarehousePro.API.Controllers

{

	[ApiController]

	[Route("api/[controller]")]

	[Authorize]

	public class WarehouseReportController : ControllerBase

	{

		private readonly IWarehouseReportService _warehouseReportService;

		public WarehouseReportController(IWarehouseReportService warehouseReportService)

		{

			_warehouseReportService = warehouseReportService;

		}

		// GET api/warehousereport

		[HttpGet]

		[Authorize(Roles = "Admin,Supervisor,Planner")]

		public async Task<IActionResult> GetAll()

		{

			var result = await _warehouseReportService.GetAllAsync();

			return Ok(result);

		}

		// GET api/warehousereport/5

		[HttpGet("{id}")]

		[Authorize(Roles = "Admin,Supervisor,Planner")]

		public async Task<IActionResult> GetById(int id)

		{

			var result = await _warehouseReportService.GetByIdAsync(id);

			if (result == null)

				return NotFound(new { message = $"Report with ID {id} not found." });

			return Ok(result);

		}

		// POST api/warehousereport

		[HttpPost]

		[Authorize(Roles = "Admin,Supervisor,Planner")]

		public async Task<IActionResult> Create([FromBody] WarehouseReportCreateDto dto)

		{

			if (!ModelState.IsValid)

				return BadRequest(ModelState);

			// Get logged-in user ID from JWT

			var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)

						   ?? User.FindFirst("sub");

			if (userIdClaim == null)

				return Unauthorized();

			var userId = int.Parse(userIdClaim.Value);

			var result = await _warehouseReportService.CreateAsync(dto, userId);

			return CreatedAtAction(nameof(GetById), new { id = result.ReportID }, result);

		}

		// DELETE api/warehousereport/5

		[HttpDelete("{id}")]

		[Authorize(Roles = "Admin")]

		public async Task<IActionResult> Delete(int id)

		{

			var result = await _warehouseReportService.DeleteAsync(id);

			if (!result)

				return NotFound(new { message = $"Report with ID {id} not found." });

			return Ok(new { message = "Report deleted successfully." });

		}

	}

}
