using Microsoft.AspNetCore.Authorization;

using Microsoft.AspNetCore.Mvc;

using System.Security.Claims;

using WarehousePro.API.DTOs.Auth;

using WarehousePro.API.Services.Interfaces;

namespace WarehousePro.API.Controllers

{

	[ApiController]

	[Route("api/[controller]")]

	public class AuthController : ControllerBase

	{

		private readonly IAuthService _authService;

		public AuthController(IAuthService authService)

		{

			_authService = authService;

		}

		// POST api/auth/login

		[HttpPost("login")]

		[AllowAnonymous]

		public async Task<IActionResult> Login([FromBody] LoginRequestDto dto)

		{

			if (!ModelState.IsValid)

				return BadRequest(ModelState);

			var result = await _authService.LoginAsync(dto);

			if (result == null)

				return Unauthorized(new { message = "Invalid email or password." });

			return Ok(result);

		}

		// POST api/auth/change-password

		[HttpPost("change-password")]

		[Authorize]

		public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto dto)

		{

			if (!ModelState.IsValid)

				return BadRequest(ModelState);

			// Get logged-in user ID from JWT claims

			var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)

						   ?? User.FindFirst("sub");

			if (userIdClaim == null)

				return Unauthorized();

			var userId = int.Parse(userIdClaim.Value);

			var result = await _authService.ChangePasswordAsync(userId, dto);

			if (!result)

				return BadRequest(new { message = "Current password is incorrect." });

			return Ok(new { message = "Password changed successfully." });

		}

	}

}
