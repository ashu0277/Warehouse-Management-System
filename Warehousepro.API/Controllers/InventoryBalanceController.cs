using Microsoft.AspNetCore.Authorization;

using Microsoft.AspNetCore.Mvc;

using WarehousePro.API.DTOs.Inventory;

using WarehousePro.API.Services.Interfaces;

namespace WarehousePro.API.Controllers

{

	[ApiController]

	[Route("api/[controller]")]

	[Authorize]

	public class InventoryBalanceController : ControllerBase

	{

		private readonly IInventoryBalanceService _inventoryBalanceService;

		public InventoryBalanceController(IInventoryBalanceService inventoryBalanceService)

		{

			_inventoryBalanceService = inventoryBalanceService;

		}

		// GET api/inventorybalance

		[HttpGet]

		public async Task<IActionResult> GetAll()

		{

			var result = await _inventoryBalanceService.GetAllAsync();

			return Ok(result);

		}

		// GET api/inventorybalance/by-item/5

		[HttpGet("by-item/{itemId}")]

		public async Task<IActionResult> GetByItem(int itemId)

		{

			var result = await _inventoryBalanceService.GetByItemIdAsync(itemId);

			return Ok(result);

		}

		// GET api/inventorybalance/by-bin/5

		[HttpGet("by-bin/{binId}")]

		public async Task<IActionResult> GetByBin(int binId)

		{

			var result = await _inventoryBalanceService.GetByBinIdAsync(binId);

			return Ok(result);

		}

		// GET api/inventorybalance/5

		[HttpGet("{id}")]

		public async Task<IActionResult> GetById(int id)

		{

			var result = await _inventoryBalanceService.GetByIdAsync(id);

			if (result == null)

				return NotFound(new { message = $"Inventory balance with ID {id} not found." });

			return Ok(result);

		}

		// PUT api/inventorybalance/5

		[HttpPut("{id}")]

		[Authorize(Roles = "Admin,Planner,Supervisor")]

		public async Task<IActionResult> Update(int id, [FromBody] InventoryBalanceUpdateDto dto)

		{

			if (!ModelState.IsValid)

				return BadRequest(ModelState);

			var result = await _inventoryBalanceService.UpdateAsync(id, dto);

			if (result == null)

				return NotFound(new { message = $"Inventory balance with ID {id} not found." });

			return Ok(result);

		}

	}

}
