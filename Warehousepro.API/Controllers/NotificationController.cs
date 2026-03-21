using Microsoft.AspNetCore.Authorization;

using Microsoft.AspNetCore.Mvc;

using WarehousePro.API.DTOs.Notifications;

using WarehousePro.API.Services.Interfaces;

namespace WarehousePro.API.Controllers

{

	[ApiController]

	[Route("api/[controller]")]

	[Authorize]

	public class NotificationController : ControllerBase

	{

		private readonly INotificationService _notificationService;

		public NotificationController(INotificationService notificationService)

		{

			_notificationService = notificationService;

		}

		// GET api/notification

		[HttpGet]

		[Authorize(Roles = "Admin,Supervisor")]

		public async Task<IActionResult> GetAll()

		{

			var result = await _notificationService.GetAllAsync();

			return Ok(result);

		}

		// GET api/notification/by-user/5

		[HttpGet("by-user/{userId}")]

		public async Task<IActionResult> GetByUser(int userId)

		{

			var result = await _notificationService.GetByUserIdAsync(userId);

			return Ok(result);

		}

		// GET api/notification/5

		[HttpGet("{id}")]

		public async Task<IActionResult> GetById(int id)

		{

			var result = await _notificationService.GetByIdAsync(id);

			if (result == null)

				return NotFound(new { message = $"Notification with ID {id} not found." });

			return Ok(result);

		}

		// POST api/notification

		[HttpPost]

		[Authorize(Roles = "Admin,Supervisor")]

		public async Task<IActionResult> Create([FromBody] NotificationCreateDto dto)

		{

			if (!ModelState.IsValid)

				return BadRequest(ModelState);

			var result = await _notificationService.CreateAsync(dto);

			return CreatedAtAction(nameof(GetById), new { id = result.NotificationID }, result);

		}

		// PUT api/notification/5  (mark as read/dismissed)

		[HttpPut("{id}")]

		public async Task<IActionResult> Update(int id, [FromBody] NotificationUpdateDto dto)

		{

			if (!ModelState.IsValid)

				return BadRequest(ModelState);

			var result = await _notificationService.UpdateAsync(id, dto);

			if (result == null)

				return NotFound(new { message = $"Notification with ID {id} not found." });

			return Ok(result);

		}

		// DELETE api/notification/5

		[HttpDelete("{id}")]

		[Authorize(Roles = "Admin,Supervisor")]

		public async Task<IActionResult> Delete(int id)

		{

			var result = await _notificationService.DeleteAsync(id);

			if (!result)

				return NotFound(new { message = $"Notification with ID {id} not found." });

			return Ok(new { message = "Notification deleted successfully." });

		}

	}

}
