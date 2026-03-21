using Microsoft.AspNetCore.Authorization;

using Microsoft.AspNetCore.Mvc;

using WarehousePro.API.Services.Interfaces;

namespace WarehousePro.API.Controllers

{

	[ApiController]

	[Route("api/[controller]")]

	[Authorize(Roles = "Admin")]

	public class AuditLogController : ControllerBase

	{

		private readonly IAuditLogService _auditLogService;

		public AuditLogController(IAuditLogService auditLogService)

		{

			_auditLogService = auditLogService;

		}

		// GET api/auditlog

		[HttpGet]

		public async Task<IActionResult> GetAll()

		{

			var result = await _auditLogService.GetAllAsync();

			return Ok(result);

		}

		// GET api/auditlog/by-user/5

		[HttpGet("by-user/{userId}")]

		public async Task<IActionResult> GetByUser(int userId)

		{

			var result = await _auditLogService.GetByUserIdAsync(userId);

			return Ok(result);

		}

	}

}
