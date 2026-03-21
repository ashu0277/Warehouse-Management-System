using WarehousePro.API.DTOs.Inventory;

namespace WarehousePro.API.Services.Interfaces

{

	public interface IInventoryBalanceService

	{

		Task<List<InventoryBalanceResponseDto>> GetAllAsync();

		Task<List<InventoryBalanceResponseDto>> GetByItemIdAsync(int itemId);

		Task<List<InventoryBalanceResponseDto>> GetByBinIdAsync(int binId);

		Task<InventoryBalanceResponseDto?> GetByIdAsync(int id);

		Task<InventoryBalanceResponseDto?> UpdateAsync(int id, InventoryBalanceUpdateDto dto);

	}

}
