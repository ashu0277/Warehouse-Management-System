using WarehousePro.API.DTOs.Outbound;

namespace WarehousePro.API.Services.Interfaces
{
	public interface IPackingUnitService
	{
		Task<List<PackingUnitResponseDto>> GetAllAsync();
		Task<List<PackingUnitResponseDto>> GetByOrderIdAsync(int orderId);
		Task<PackingUnitResponseDto?> GetByIdAsync(int id);
		Task<PackingUnitResponseDto> CreateAsync(PackingUnitCreateDto dto);
		Task<PackingUnitResponseDto?> UpdateAsync(int id, PackingUnitUpdateDto dto);
		Task<bool> DeleteAsync(int id);
	}
}