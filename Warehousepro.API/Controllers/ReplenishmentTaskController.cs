using Microsoft.AspNetCore.Authorization;

using Microsoft.AspNetCore.Mvc;

using WarehousePro.API.DTOs.Replenishment;

using WarehousePro.API.Services.Interfaces;

namespace WarehousePro.API.Controllers

{

	[ApiController]

	[Route("api/[controller]")]

	[Authorize]

	public class ReplenishmentTaskController : ControllerBase

	{

		private readonly IReplenishmentTaskService _replenishmentTaskService;

		public ReplenishmentTaskController(IReplenishmentTaskService replenishmentTaskService)

		{

			_replenishmentTaskService = replenishmentTaskService;

		}

		// GET api/replenishmenttask

		[HttpGet]

		public async Task<IActionResult> GetAll()

		{

			var result = await _replenishmentTaskService.GetAllAsync();

			return Ok(result);

		}

		// GET api/replenishmenttask/by-item/5

		[HttpGet("by-item/{itemId}")]

		public async Task<IActionResult> GetByItem(int itemId)

		{

			var result = await _replenishmentTaskService.GetByItemIdAsync(itemId);

			return Ok(result);

		}

		// GET api/replenishmenttask/5

		[HttpGet("{id}")]

		public async Task<IActionResult> GetById(int id)

		{

			var result = await _replenishmentTaskService.GetByIdAsync(id);

			if (result == null)

				return NotFound(new { message = $"Replenishment task with ID {id} not found." });

			return Ok(result);

		}

		// POST api/replenishmenttask

		[HttpPost]

		[Authorize(Roles = "Admin,Supervisor,Planner")]

		public async Task<IActionResult> Create([FromBody] ReplenishmentTaskCreateDto dto)

		{

			if (!ModelState.IsValid)

				return BadRequest(ModelState);

			var result = await _replenishmentTaskService.CreateAsync(dto);

			return CreatedAtAction(nameof(GetById), new { id = result.ReplenishID }, result);

		}

		// PUT api/replenishmenttask/5

		[HttpPut("{id}")]

		[Authorize(Roles = "Admin,Supervisor,Operator,Planner")]

		public async Task<IActionResult> Update(int id, [FromBody] ReplenishmentTaskUpdateDto dto)

		{

			if (!ModelState.IsValid)

				return BadRequest(ModelState);

			var result = await _replenishmentTaskService.UpdateAsync(id, dto);

			if (result == null)

				return NotFound(new { message = $"Replenishment task with ID {id} not found." });

			return Ok(result);

		}

		// DELETE api/replenishmenttask/5

		[HttpDelete("{id}")]

		[Authorize(Roles = "Admin,Supervisor")]

		public async Task<IActionResult> Delete(int id)

		{

			var result = await _replenishmentTaskService.DeleteAsync(id);

			if (!result)

				return NotFound(new { message = $"Replenishment task with ID {id} not found." });

			return Ok(new { message = "Replenishment task deleted successfully." });

		}

	}

}
