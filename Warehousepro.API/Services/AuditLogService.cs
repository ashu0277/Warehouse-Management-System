using Microsoft.EntityFrameworkCore;

using WarehousePro.API.Data;

using WarehousePro.API.Models;

using WarehousePro.API.Services.Interfaces;

namespace WarehousePro.API.Services

{

	public class AuditLogService : IAuditLogService

	{

		private readonly AppDbContext _context;

		public AuditLogService(AppDbContext context)

		{

			_context = context;

		}

		public async Task LogAsync(int userId, string action,

								   string resource, string? metadata = null)

		{

			var log = new AuditLog

			{

				UserID = userId,

				Action = action,

				Resource = resource,

				Timestamp = DateTime.UtcNow,

				Metadata = metadata

			};

			_context.AuditLogs.Add(log);

			await _context.SaveChangesAsync();

		}

		public async Task<List<AuditLog>> GetAllAsync()

		{

			return await _context.AuditLogs

				.Include(a => a.User)

				.OrderByDescending(a => a.Timestamp)

				.ToListAsync();

		}

		public async Task<List<AuditLog>> GetByUserIdAsync(int userId)

		{

			return await _context.AuditLogs

				.Where(a => a.UserID == userId)

				.Include(a => a.User)

				.OrderByDescending(a => a.Timestamp)

				.ToListAsync();

		}

	}

}
