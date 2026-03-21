using WarehousePro.API.DTOs.Analytics;

namespace WarehousePro.API.Services.Interfaces
{
	public interface IWarehouseReportService
	{
		Task<List<WarehouseReportResponseDto>> GetAllAsync();
		Task<WarehouseReportResponseDto?> GetByIdAsync(int id);
		Task<WarehouseReportResponseDto> CreateAsync(WarehouseReportCreateDto dto, int generatedByUserId);
		Task<bool> DeleteAsync(int id);
	}
}