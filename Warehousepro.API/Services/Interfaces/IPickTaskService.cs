using WarehousePro.API.DTOs.Outbound;

namespace WarehousePro.API.Services.Interfaces

{

	public interface IPickTaskService

	{

		Task<List<PickTaskResponseDto>> GetAllAsync();

		Task<List<PickTaskResponseDto>> GetByOrderIdAsync(int orderId);

		Task<List<PickTaskResponseDto>> GetByUserIdAsync(int userId);

		Task<PickTaskResponseDto?> GetByIdAsync(int id);

		Task<PickTaskResponseDto> CreateAsync(PickTaskCreateDto dto);

		Task<PickTaskResponseDto?> UpdateAsync(int id, PickTaskUpdateDto dto);

		Task<bool> DeleteAsync(int id);

	}

}
