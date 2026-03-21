using WarehousePro.API.Models;

namespace WarehousePro.API.Services.Interfaces

{

	public interface IAuditLogService

	{

		Task LogAsync(int userId, string action, string resource, string? metadata = null);

		Task<List<AuditLog>> GetAllAsync();

		Task<List<AuditLog>> GetByUserIdAsync(int userId);

	}

}
