using WarehousePro.API.DTOs.Outbound;

namespace WarehousePro.API.Services.Interfaces

{

	public interface IShipmentService

	{

		Task<List<ShipmentResponseDto>> GetAllAsync();

		Task<List<ShipmentResponseDto>> GetByOrderIdAsync(int orderId);

		Task<ShipmentResponseDto?> GetByIdAsync(int id);

		Task<ShipmentResponseDto> CreateAsync(ShipmentCreateDto dto);

		Task<ShipmentResponseDto?> UpdateAsync(int id, ShipmentUpdateDto dto);

		Task<bool> DeleteAsync(int id);

	}

}
