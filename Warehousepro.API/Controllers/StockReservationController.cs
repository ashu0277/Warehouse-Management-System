using Microsoft.AspNetCore.Authorization;

using Microsoft.AspNetCore.Mvc;

using WarehousePro.API.DTOs.Inventory;

using WarehousePro.API.Services.Interfaces;

namespace WarehousePro.API.Controllers

{

	[ApiController]

	[Route("api/[controller]")]

	[Authorize]

	public class StockReservationController : ControllerBase

	{

		private readonly IStockReservationService _stockReservationService;

		public StockReservationController(IStockReservationService stockReservationService)

		{

			_stockReservationService = stockReservationService;

		}

		// GET api/stockreservation

		[HttpGet]

		public async Task<IActionResult> GetAll()

		{

			var result = await _stockReservationService.GetAllAsync();

			return Ok(result);

		}

		// GET api/stockreservation/by-item/5

		[HttpGet("by-item/{itemId}")]

		public async Task<IActionResult> GetByItem(int itemId)

		{

			var result = await _stockReservationService.GetByItemIdAsync(itemId);

			return Ok(result);

		}

		// GET api/stockreservation/5

		[HttpGet("{id}")]

		public async Task<IActionResult> GetById(int id)

		{

			var result = await _stockReservationService.GetByIdAsync(id);

			if (result == null)

				return NotFound(new { message = $"Reservation with ID {id} not found." });

			return Ok(result);

		}

		// POST api/stockreservation

		[HttpPost]

		[Authorize(Roles = "Admin,Supervisor,Planner")]

		public async Task<IActionResult> Create([FromBody] StockReservationCreateDto dto)

		{

			if (!ModelState.IsValid)

				return BadRequest(ModelState);

			var result = await _stockReservationService.CreateAsync(dto);

			return CreatedAtAction(nameof(GetById), new { id = result.ReservationID }, result);

		}

		// DELETE api/stockreservation/5

		[HttpDelete("{id}")]

		[Authorize(Roles = "Admin,Supervisor,Planner")]

		public async Task<IActionResult> Delete(int id)

		{

			var result = await _stockReservationService.DeleteAsync(id);

			if (!result)

				return NotFound(new { message = $"Reservation with ID {id} not found." });

			return Ok(new { message = "Reservation deleted successfully." });

		}

	}

}
