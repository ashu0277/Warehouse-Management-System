using WarehousePro.API.DTOs.Inventory;

namespace WarehousePro.API.Services.Interfaces
{
	public interface IStockReservationService
	{
		Task<List<StockReservationResponseDto>> GetAllAsync();
		Task<List<StockReservationResponseDto>> GetByItemIdAsync(int itemId);
		Task<StockReservationResponseDto?> GetByIdAsync(int id);
		Task<StockReservationResponseDto> CreateAsync(StockReservationCreateDto dto);
		Task<bool> DeleteAsync(int id);
	}
}