using WarehousePro.API.DTOs.Inbound;

namespace WarehousePro.API.Services.Interfaces
{
	public interface IPutAwayTaskService
	{
		Task<List<PutAwayTaskResponseDto>> GetAllAsync();
		Task<List<PutAwayTaskResponseDto>> GetByReceiptIdAsync(int receiptId);
		Task<List<PutAwayTaskResponseDto>> GetByUserIdAsync(int userId);
		Task<PutAwayTaskResponseDto?> GetByIdAsync(int id);
		Task<PutAwayTaskResponseDto> CreateAsync(PutAwayTaskCreateDto dto);
		Task<PutAwayTaskResponseDto?> UpdateAsync(int id, PutAwayTaskUpdateDto dto);
		Task<bool> DeleteAsync(int id);
	}
}