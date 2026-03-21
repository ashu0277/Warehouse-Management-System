using Microsoft.EntityFrameworkCore;

using WarehousePro.API.Data;

using WarehousePro.API.DTOs.Notifications;

using WarehousePro.API.Models;

using WarehousePro.API.Services.Interfaces;

namespace WarehousePro.API.Services

{

	public class NotificationService : INotificationService

	{

		private readonly AppDbContext _context;

		private readonly IAuditLogService _auditLogService;

		private readonly ICurrentUserService _currentUserService;

		public NotificationService(

			AppDbContext context,

			IAuditLogService auditLogService,

			ICurrentUserService currentUserService)

		{

			_context = context;

			_auditLogService = auditLogService;

			_currentUserService = currentUserService;

		}

		public async Task<List<NotificationResponseDto>> GetAllAsync()

		{

			return await _context.Notifications

				.Include(n => n.User)

				.Select(n => MapToResponseDto(n))

				.ToListAsync();

		}

		public async Task<List<NotificationResponseDto>> GetByUserIdAsync(int userId)

		{

			return await _context.Notifications

				.Where(n => n.UserID == userId)

				.Include(n => n.User)

				.OrderByDescending(n => n.CreatedDate)

				.Select(n => MapToResponseDto(n))

				.ToListAsync();

		}

		public async Task<NotificationResponseDto?> GetByIdAsync(int id)

		{

			var notification = await _context.Notifications

				.Include(n => n.User)

				.FirstOrDefaultAsync(n => n.NotificationID == id);

			return notification == null ? null : MapToResponseDto(notification);

		}

		public async Task<NotificationResponseDto> CreateAsync(NotificationCreateDto dto)

		{

			var notification = new Notification

			{

				UserID = dto.UserID,

				Message = dto.Message,

				Category = dto.Category,

				CreatedDate = DateTime.UtcNow

			};

			_context.Notifications.Add(notification);

			await _context.SaveChangesAsync();

			await _context.Entry(notification).Reference(n => n.User).LoadAsync();

			// ── Audit Log ─────────────────────────────────────────────────

			await _auditLogService.LogAsync(

				userId: _currentUserService.GetUserId(),

				action: "CREATE",

				resource: "Notification",

				metadata: $"ID: {notification.NotificationID}, UserID: {notification.UserID}, Category: {notification.Category}"

			);

			return MapToResponseDto(notification);

		}

		public async Task<NotificationResponseDto?> UpdateAsync(int id, NotificationUpdateDto dto)

		{

			var notification = await _context.Notifications

				.Include(n => n.User)

				.FirstOrDefaultAsync(n => n.NotificationID == id);

			if (notification == null) return null;

			notification.Status = dto.Status;

			await _context.SaveChangesAsync();

			// ── Audit Log ─────────────────────────────────────────────────

			await _auditLogService.LogAsync(

				userId: _currentUserService.GetUserId(),

				action: "UPDATE",

				resource: "Notification",

				metadata: $"ID: {id}, Status: {notification.Status}"

			);

			return MapToResponseDto(notification);

		}

		public async Task<bool> DeleteAsync(int id)

		{

			var notification = await _context.Notifications

				.FirstOrDefaultAsync(n => n.NotificationID == id);

			if (notification == null) return false;

			_context.Notifications.Remove(notification);

			await _context.SaveChangesAsync();

			// ── Audit Log ─────────────────────────────────────────────────

			await _auditLogService.LogAsync(

				userId: _currentUserService.GetUserId(),

				action: "DELETE",

				resource: "Notification",

				metadata: $"ID: {id}"

			);

			return true;

		}

		private static NotificationResponseDto MapToResponseDto(Notification n) => new()

		{

			NotificationID = n.NotificationID,

			UserID = n.UserID,

			Message = n.Message,

			Category = n.Category.ToString(),

			Status = n.Status.ToString(),

			CreatedDate = n.CreatedDate

		};

	}

}
