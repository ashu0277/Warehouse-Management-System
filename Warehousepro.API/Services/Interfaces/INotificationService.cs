using WarehousePro.API.DTOs.Notifications;

namespace WarehousePro.API.Services.Interfaces

{

	public interface INotificationService

	{

		Task<List<NotificationResponseDto>> GetAllAsync();

		Task<List<NotificationResponseDto>> GetByUserIdAsync(int userId);

		Task<NotificationResponseDto?> GetByIdAsync(int id);

		Task<NotificationResponseDto> CreateAsync(NotificationCreateDto dto);

		Task<NotificationResponseDto?> UpdateAsync(int id, NotificationUpdateDto dto);

		Task<bool> DeleteAsync(int id);

	}

}
