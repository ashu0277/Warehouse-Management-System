using WarehousePro.API.DTOs.Item;

namespace WarehousePro.API.Services.Interfaces

{

	public interface IItemService

	{

		Task<List<ItemResponseDto>> GetAllAsync();

		Task<ItemResponseDto?> GetByIdAsync(int id);

		Task<ItemResponseDto?> GetBySkuAsync(string sku);

		Task<ItemResponseDto> CreateAsync(ItemCreateDto dto);

		Task<ItemResponseDto?> UpdateAsync(int id, ItemUpdateDto dto);

		Task<bool> DeleteAsync(int id);

	}

}
