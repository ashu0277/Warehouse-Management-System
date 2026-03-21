using WarehousePro.API.DTOs.Replenishment;

namespace WarehousePro.API.Services.Interfaces

{

	public interface ISlottingRuleService

	{

		Task<List<SlottingRuleResponseDto>> GetAllAsync();

		Task<SlottingRuleResponseDto?> GetByIdAsync(int id);

		Task<SlottingRuleResponseDto> CreateAsync(SlottingRuleCreateDto dto);

		Task<SlottingRuleResponseDto?> UpdateAsync(int id, SlottingRuleUpdateDto dto);

		Task<bool> DeleteAsync(int id);

	}

}
