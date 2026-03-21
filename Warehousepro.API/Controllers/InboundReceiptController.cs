using Microsoft.AspNetCore.Authorization;

using Microsoft.AspNetCore.Mvc;

using WarehousePro.API.DTOs.Inbound;

using WarehousePro.API.Services.Interfaces;

namespace WarehousePro.API.Controllers

{

	[ApiController]

	[Route("api/[controller]")]

	[Authorize]

	public class InboundReceiptController : ControllerBase

	{

		private readonly IInboundReceiptService _inboundReceiptService;

		public InboundReceiptController(IInboundReceiptService inboundReceiptService)

		{

			_inboundReceiptService = inboundReceiptService;

		}

		// GET api/inboundreceipt

		[HttpGet]

		public async Task<IActionResult> GetAll()

		{

			var result = await _inboundReceiptService.GetAllAsync();

			return Ok(result);

		}

		// GET api/inboundreceipt/5

		[HttpGet("{id}")]

		public async Task<IActionResult> GetById(int id)

		{

			var result = await _inboundReceiptService.GetByIdAsync(id);

			if (result == null)

				return NotFound(new { message = $"Inbound receipt with ID {id} not found." });

			return Ok(result);

		}

		// POST api/inboundreceipt

		[HttpPost]

		[Authorize(Roles = "Admin,Supervisor,Operator")]

		public async Task<IActionResult> Create([FromBody] InboundReceiptCreateDto dto)

		{

			if (!ModelState.IsValid)

				return BadRequest(ModelState);

			var result = await _inboundReceiptService.CreateAsync(dto);

			return CreatedAtAction(nameof(GetById), new { id = result.ReceiptID }, result);

		}

		// PUT api/inboundreceipt/5

		[HttpPut("{id}")]

		[Authorize(Roles = "Admin,Supervisor")]

		public async Task<IActionResult> Update(int id, [FromBody] InboundReceiptUpdateDto dto)

		{

			if (!ModelState.IsValid)

				return BadRequest(ModelState);

			var result = await _inboundReceiptService.UpdateAsync(id, dto);

			if (result == null)

				return NotFound(new { message = $"Inbound receipt with ID {id} not found." });

			return Ok(result);

		}

		// DELETE api/inboundreceipt/5

		[HttpDelete("{id}")]

		[Authorize(Roles = "Admin")]

		public async Task<IActionResult> Delete(int id)

		{

			var result = await _inboundReceiptService.DeleteAsync(id);

			if (!result)

				return NotFound(new { message = $"Inbound receipt with ID {id} not found." });

			return Ok(new { message = "Inbound receipt deleted successfully." });

		}

	}

}
