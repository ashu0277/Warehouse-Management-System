using WarehousePro.API.DTOs.Inbound;

namespace WarehousePro.API.Services.Interfaces
{
	public interface IInboundReceiptService
	{
		Task<List<InboundReceiptResponseDto>> GetAllAsync();
		Task<InboundReceiptResponseDto?> GetByIdAsync(int id);
		Task<InboundReceiptResponseDto> CreateAsync(InboundReceiptCreateDto dto);
		Task<InboundReceiptResponseDto?> UpdateAsync(int id, InboundReceiptUpdateDto dto);
		Task<bool> DeleteAsync(int id);
	}
}