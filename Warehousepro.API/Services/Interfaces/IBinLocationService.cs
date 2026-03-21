using WarehousePro.API.DTOs.BinLocation;

namespace WarehousePro.API.Services.Interfaces
{
	public interface IBinLocationService
	{
		Task<List<BinLocationResponseDto>> GetAllAsync();
		Task<List<BinLocationResponseDto>> GetByZoneIdAsync(int zoneId);
		Task<BinLocationResponseDto?> GetByIdAsync(int id);
		Task<BinLocationResponseDto> CreateAsync(BinLocationCreateDto dto);
		Task<BinLocationResponseDto?> UpdateAsync(int id, BinLocationUpdateDto dto);
		Task<bool> DeleteAsync(int id);
	}
}