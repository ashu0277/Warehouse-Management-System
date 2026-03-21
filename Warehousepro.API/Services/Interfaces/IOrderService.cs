using WarehousePro.API.DTOs.Order;

namespace WarehousePro.API.Services.Interfaces

{

	public interface IOrderService

	{

		Task<List<OrderResponseDto>> GetAllAsync();

		Task<OrderResponseDto?> GetByIdAsync(int id);

		Task<OrderResponseDto?> GetByOrderNumberAsync(string orderNumber);

		Task<OrderResponseDto> CreateAsync(OrderCreateDto dto);

		Task<OrderResponseDto?> UpdateAsync(int id, OrderUpdateDto dto);

		Task<bool> DeleteAsync(int id);

	}

}
