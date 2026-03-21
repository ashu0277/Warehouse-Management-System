using WarehousePro.API.DTOs.Replenishment;

namespace WarehousePro.API.Services.Interfaces
{
	public interface IReplenishmentTaskService
	{
		Task<List<ReplenishmentTaskResponseDto>> GetAllAsync();
		Task<List<ReplenishmentTaskResponseDto>> GetByItemIdAsync(int itemId);
		Task<ReplenishmentTaskResponseDto?> GetByIdAsync(int id);
		Task<ReplenishmentTaskResponseDto> CreateAsync(ReplenishmentTaskCreateDto dto);
		Task<ReplenishmentTaskResponseDto?> UpdateAsync(int id, ReplenishmentTaskUpdateDto dto);
		Task<bool> DeleteAsync(int id);
	}
}