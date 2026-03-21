using Microsoft.AspNetCore.Authorization;

using Microsoft.AspNetCore.Mvc;

using WarehousePro.API.DTOs.User;

using WarehousePro.API.Services.Interfaces;

namespace WarehousePro.API.Controllers

{

	[ApiController]

	[Route("api/[controller]")]

	[Authorize]

	public class UserController : ControllerBase

	{

		private readonly IUserService _userService;

		public UserController(IUserService userService)

		{

			_userService = userService;

		}

		// GET api/user

		[HttpGet]

		[Authorize(Roles = "Admin,Supervisor")]

		public async Task<IActionResult> GetAll()

		{

			var result = await _userService.GetAllAsync();

			return Ok(result);

		}

		// GET api/user/5

		[HttpGet("{id}")]

		[Authorize(Roles = "Admin,Supervisor")]

		public async Task<IActionResult> GetById(int id)

		{

			var result = await _userService.GetByIdAsync(id);

			if (result == null)

				return NotFound(new { message = $"User with ID {id} not found." });

			return Ok(result);

		}

		// POST api/user

		[HttpPost]

		[Authorize(Roles = "Admin")]

		public async Task<IActionResult> Create([FromBody] UserCreateDto dto)

		{

			if (!ModelState.IsValid)

				return BadRequest(ModelState);

			// Check duplicate email

			if (await _userService.EmailExistsAsync(dto.Email))

				return Conflict(new { message = "A user with this email already exists." });

			var result = await _userService.CreateAsync(dto);

			return CreatedAtAction(nameof(GetById), new { id = result.UserID }, result);

		}

		// PUT api/user/5

		[HttpPut("{id}")]

		[Authorize(Roles = "Admin")]

		public async Task<IActionResult> Update(int id, [FromBody] UserUpdateDto dto)

		{

			if (!ModelState.IsValid)

				return BadRequest(ModelState);

			var result = await _userService.UpdateAsync(id, dto);

			if (result == null)

				return NotFound(new { message = $"User with ID {id} not found." });

			return Ok(result);

		}

		// DELETE api/user/5

		[HttpDelete("{id}")]

		[Authorize(Roles = "Admin")]

		public async Task<IActionResult> Delete(int id)

		{

			var result = await _userService.DeleteAsync(id);

			if (!result)

				return NotFound(new { message = $"User with ID {id} not found." });

			return Ok(new { message = "User deleted successfully." });

		}

	}

}
