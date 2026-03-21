using WarehousePro.API.DTOs.Zone;

namespace WarehousePro.API.Services.Interfaces
{
	public interface IZoneService
	{
		Task<List<ZoneResponseDto>> GetAllAsync();
		Task<List<ZoneResponseDto>> GetByWarehouseIdAsync(int warehouseId);
		Task<ZoneResponseDto?> GetByIdAsync(int id);
		Task<ZoneResponseDto> CreateAsync(ZoneCreateDto dto);
		Task<ZoneResponseDto?> UpdateAsync(int id, ZoneUpdateDto dto);
		Task<bool> DeleteAsync(int id);
	}
}