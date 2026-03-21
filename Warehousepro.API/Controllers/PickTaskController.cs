using Microsoft.AspNetCore.Authorization;

using Microsoft.AspNetCore.Mvc;

using WarehousePro.API.DTOs.Outbound;

using WarehousePro.API.Services.Interfaces;

namespace WarehousePro.API.Controllers

{

	[ApiController]

	[Route("api/[controller]")]

	[Authorize]

	public class PickTaskController : ControllerBase

	{

		private readonly IPickTaskService _pickTaskService;

		public PickTaskController(IPickTaskService pickTaskService)

		{

			_pickTaskService = pickTaskService;

		}

		// GET api/picktask

		[HttpGet]

		public async Task<IActionResult> GetAll()

		{

			var result = await _pickTaskService.GetAllAsync();

			return Ok(result);

		}

		// GET api/picktask/by-order/5

		[HttpGet("by-order/{orderId}")]

		public async Task<IActionResult> GetByOrder(int orderId)

		{

			var result = await _pickTaskService.GetByOrderIdAsync(orderId);

			return Ok(result);

		}

		// GET api/picktask/by-user/5

		[HttpGet("by-user/{userId}")]

		public async Task<IActionResult> GetByUser(int userId)

		{

			var result = await _pickTaskService.GetByUserIdAsync(userId);

			return Ok(result);

		}

		// GET api/picktask/5

		[HttpGet("{id}")]

		public async Task<IActionResult> GetById(int id)

		{

			var result = await _pickTaskService.GetByIdAsync(id);

			if (result == null)

				return NotFound(new { message = $"Pick task with ID {id} not found." });

			return Ok(result);

		}

		// POST api/picktask

		[HttpPost]

		[Authorize(Roles = "Admin,Supervisor")]

		public async Task<IActionResult> Create([FromBody] PickTaskCreateDto dto)

		{

			if (!ModelState.IsValid)

				return BadRequest(ModelState);

			var result = await _pickTaskService.CreateAsync(dto);

			return CreatedAtAction(nameof(GetById), new { id = result.PickTaskID }, result);

		}

		// PUT api/picktask/5

		[HttpPut("{id}")]

		[Authorize(Roles = "Admin,Supervisor,Operator")]

		public async Task<IActionResult> Update(int id, [FromBody] PickTaskUpdateDto dto)

		{

			if (!ModelState.IsValid)

				return BadRequest(ModelState);

			var result = await _pickTaskService.UpdateAsync(id, dto);

			if (result == null)

				return NotFound(new { message = $"Pick task with ID {id} not found." });

			return Ok(result);

		}

		// DELETE api/picktask/5

		[HttpDelete("{id}")]

		[Authorize(Roles = "Admin,Supervisor")]

		public async Task<IActionResult> Delete(int id)

		{

			var result = await _pickTaskService.DeleteAsync(id);

			if (!result)

				return NotFound(new { message = $"Pick task with ID {id} not found." });

			return Ok(new { message = "Pick task deleted successfully." });

		}

	}

}
