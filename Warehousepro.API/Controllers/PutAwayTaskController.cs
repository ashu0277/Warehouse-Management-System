using Microsoft.AspNetCore.Authorization;

using Microsoft.AspNetCore.Mvc;

using WarehousePro.API.DTOs.Inbound;

using WarehousePro.API.Services.Interfaces;

namespace WarehousePro.API.Controllers

{

	[ApiController]

	[Route("api/[controller]")]

	[Authorize]

	public class PutAwayTaskController : ControllerBase

	{

		private readonly IPutAwayTaskService _putAwayTaskService;

		public PutAwayTaskController(IPutAwayTaskService putAwayTaskService)

		{

			_putAwayTaskService = putAwayTaskService;

		}

		// GET api/putawaytask

		[HttpGet]

		public async Task<IActionResult> GetAll()

		{

			var result = await _putAwayTaskService.GetAllAsync();

			return Ok(result);

		}

		// GET api/putawaytask/by-receipt/5

		[HttpGet("by-receipt/{receiptId}")]

		public async Task<IActionResult> GetByReceipt(int receiptId)

		{

			var result = await _putAwayTaskService.GetByReceiptIdAsync(receiptId);

			return Ok(result);

		}

		// GET api/putawaytask/by-user/5

		[HttpGet("by-user/{userId}")]

		public async Task<IActionResult> GetByUser(int userId)

		{

			var result = await _putAwayTaskService.GetByUserIdAsync(userId);

			return Ok(result);

		}

		// GET api/putawaytask/5

		[HttpGet("{id}")]

		public async Task<IActionResult> GetById(int id)

		{

			var result = await _putAwayTaskService.GetByIdAsync(id);

			if (result == null)

				return NotFound(new { message = $"PutAway task with ID {id} not found." });

			return Ok(result);

		}

		// POST api/putawaytask

		[HttpPost]

		[Authorize(Roles = "Admin,Supervisor")]

		public async Task<IActionResult> Create([FromBody] PutAwayTaskCreateDto dto)

		{

			if (!ModelState.IsValid)

				return BadRequest(ModelState);

			var result = await _putAwayTaskService.CreateAsync(dto);

			return CreatedAtAction(nameof(GetById), new { id = result.TaskID }, result);

		}

		// PUT api/putawaytask/5

		[HttpPut("{id}")]

		[Authorize(Roles = "Admin,Supervisor,Operator")]

		public async Task<IActionResult> Update(int id, [FromBody] PutAwayTaskUpdateDto dto)

		{

			if (!ModelState.IsValid)

				return BadRequest(ModelState);

			var result = await _putAwayTaskService.UpdateAsync(id, dto);

			if (result == null)

				return NotFound(new { message = $"PutAway task with ID {id} not found." });

			return Ok(result);

		}

		// DELETE api/putawaytask/5

		[HttpDelete("{id}")]

		[Authorize(Roles = "Admin,Supervisor")]

		public async Task<IActionResult> Delete(int id)

		{

			var result = await _putAwayTaskService.DeleteAsync(id);

			if (!result)

				return NotFound(new { message = $"PutAway task with ID {id} not found." });

			return Ok(new { message = "PutAway task deleted successfully." });

		}

	}

}
