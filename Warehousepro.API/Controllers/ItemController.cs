using Microsoft.AspNetCore.Authorization;

using Microsoft.AspNetCore.Mvc;

using WarehousePro.API.DTOs.Item;

using WarehousePro.API.Services.Interfaces;

namespace WarehousePro.API.Controllers

{

	[ApiController]

	[Route("api/[controller]")]

	[Authorize]

	public class ItemController : ControllerBase

	{

		private readonly IItemService _itemService;

		public ItemController(IItemService itemService)

		{

			_itemService = itemService;

		}

		// GET api/item

		[HttpGet]

		public async Task<IActionResult> GetAll()

		{

			var result = await _itemService.GetAllAsync();

			return Ok(result);

		}

		// GET api/item/5

		[HttpGet("{id}")]

		public async Task<IActionResult> GetById(int id)

		{

			var result = await _itemService.GetByIdAsync(id);

			if (result == null)

				return NotFound(new { message = $"Item with ID {id} not found." });

			return Ok(result);

		}

		// GET api/item/sku/ABC123

		[HttpGet("sku/{sku}")]

		public async Task<IActionResult> GetBySku(string sku)

		{

			var result = await _itemService.GetBySkuAsync(sku);

			if (result == null)

				return NotFound(new { message = $"Item with SKU '{sku}' not found." });

			return Ok(result);

		}

		// POST api/item

		[HttpPost]

		[Authorize(Roles = "Admin,Planner")]

		public async Task<IActionResult> Create([FromBody] ItemCreateDto dto)

		{

			if (!ModelState.IsValid)

				return BadRequest(ModelState);

			var result = await _itemService.CreateAsync(dto);

			return CreatedAtAction(nameof(GetById), new { id = result.ItemID }, result);

		}

		// PUT api/item/5

		[HttpPut("{id}")]

		[Authorize(Roles = "Admin,Planner")]

		public async Task<IActionResult> Update(int id, [FromBody] ItemUpdateDto dto)

		{

			if (!ModelState.IsValid)

				return BadRequest(ModelState);

			var result = await _itemService.UpdateAsync(id, dto);

			if (result == null)

				return NotFound(new { message = $"Item with ID {id} not found." });

			return Ok(result);

		}

		// DELETE api/item/5

		[HttpDelete("{id}")]

		[Authorize(Roles = "Admin")]

		public async Task<IActionResult> Delete(int id)

		{

			var result = await _itemService.DeleteAsync(id);

			if (!result)

				return NotFound(new { message = $"Item with ID {id} not found." });

			return Ok(new { message = "Item deleted successfully." });

		}

	}

}
