using Microsoft.AspNetCore.Authorization;

using Microsoft.AspNetCore.Mvc;

using WarehousePro.API.DTOs.Replenishment;

using WarehousePro.API.Services.Interfaces;

namespace WarehousePro.API.Controllers

{

	[ApiController]

	[Route("api/[controller]")]

	[Authorize]

	public class SlottingRuleController : ControllerBase

	{

		private readonly ISlottingRuleService _slottingRuleService;

		public SlottingRuleController(ISlottingRuleService slottingRuleService)

		{

			_slottingRuleService = slottingRuleService;

		}

		// GET api/slottingrule

		[HttpGet]

		public async Task<IActionResult> GetAll()

		{

			var result = await _slottingRuleService.GetAllAsync();

			return Ok(result);

		}

		// GET api/slottingrule/5

		[HttpGet("{id}")]

		public async Task<IActionResult> GetById(int id)

		{

			var result = await _slottingRuleService.GetByIdAsync(id);

			if (result == null)

				return NotFound(new { message = $"Slotting rule with ID {id} not found." });

			return Ok(result);

		}

		// POST api/slottingrule

		[HttpPost]

		[Authorize(Roles = "Admin,Planner")]

		public async Task<IActionResult> Create([FromBody] SlottingRuleCreateDto dto)

		{

			if (!ModelState.IsValid)

				return BadRequest(ModelState);

			var result = await _slottingRuleService.CreateAsync(dto);

			return CreatedAtAction(nameof(GetById), new { id = result.RuleID }, result);

		}

		// PUT api/slottingrule/5

		[HttpPut("{id}")]

		[Authorize(Roles = "Admin,Planner")]

		public async Task<IActionResult> Update(int id, [FromBody] SlottingRuleUpdateDto dto)

		{

			if (!ModelState.IsValid)

				return BadRequest(ModelState);

			var result = await _slottingRuleService.UpdateAsync(id, dto);

			if (result == null)

				return NotFound(new { message = $"Slotting rule with ID {id} not found." });

			return Ok(result);

		}

		// DELETE api/slottingrule/5

		[HttpDelete("{id}")]

		[Authorize(Roles = "Admin")]

		public async Task<IActionResult> Delete(int id)

		{

			var result = await _slottingRuleService.DeleteAsync(id);

			if (!result)

				return NotFound(new { message = $"Slotting rule with ID {id} not found." });

			return Ok(new { message = "Slotting rule deleted successfully." });

		}

	}

}
