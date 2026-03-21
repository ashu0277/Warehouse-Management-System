using Microsoft.EntityFrameworkCore;

using WarehousePro.API.Data;

using WarehousePro.API.DTOs.User;

using WarehousePro.API.Models;

using WarehousePro.API.Services.Interfaces;

namespace WarehousePro.API.Services

{

	public class UserService : IUserService

	{

		private readonly AppDbContext _context;

		private readonly IAuditLogService _auditLogService;

		private readonly ICurrentUserService _currentUserService;

		public UserService(

			AppDbContext context,

			IAuditLogService auditLogService,

			ICurrentUserService currentUserService)

		{

			_context = context;

			_auditLogService = auditLogService;

			_currentUserService = currentUserService;

		}

		// ── Get All ───────────────────────────────────────────────────────

		public async Task<List<UserResponseDto>> GetAllAsync()

		{

			return await _context.Users

				.Where(u => !u.IsDeleted)

				.Select(u => MapToResponseDto(u))

				.ToListAsync();

		}

		// ── Get By ID ─────────────────────────────────────────────────────

		public async Task<UserResponseDto?> GetByIdAsync(int id)

		{

			var user = await _context.Users

				.FirstOrDefaultAsync(u => u.UserID == id && !u.IsDeleted);

			return user == null ? null : MapToResponseDto(user);

		}

		// ── Create ────────────────────────────────────────────────────────

		public async Task<UserResponseDto> CreateAsync(UserCreateDto dto)

		{

			var user = new User

			{

				Name = dto.Name,

				Role = dto.Role,

				Email = dto.Email,

				Phone = dto.Phone,

				PasswordHash = dto.Password,  // Plain text — no hashing

				CreatedAt = DateTime.UtcNow

			};

			_context.Users.Add(user);

			await _context.SaveChangesAsync();

			// ── Audit Log ─────────────────────────────────────────────────

			await _auditLogService.LogAsync(

				userId: _currentUserService.GetUserId(),

				action: "CREATE",

				resource: "User",

				metadata: $"ID: {user.UserID}, Name: {user.Name}, Role: {user.Role}, Email: {user.Email}"

			);

			return MapToResponseDto(user);

		}

		// ── Update ────────────────────────────────────────────────────────

		public async Task<UserResponseDto?> UpdateAsync(int id, UserUpdateDto dto)

		{

			var user = await _context.Users

				.FirstOrDefaultAsync(u => u.UserID == id && !u.IsDeleted);

			if (user == null) return null;

			user.Name = dto.Name;

			user.Role = dto.Role;

			user.Phone = dto.Phone;

			user.IsDeleted = dto.IsDeleted;

			await _context.SaveChangesAsync();

			// ── Audit Log ─────────────────────────────────────────────────

			await _auditLogService.LogAsync(

				userId: _currentUserService.GetUserId(),

				action: "UPDATE",

				resource: "User",

				metadata: $"ID: {id}, Name: {user.Name}, Role: {user.Role}"

			);

			return MapToResponseDto(user);

		}

		// ── Soft Delete ───────────────────────────────────────────────────

		public async Task<bool> DeleteAsync(int id)

		{

			var user = await _context.Users

				.FirstOrDefaultAsync(u => u.UserID == id && !u.IsDeleted);

			if (user == null) return false;

			user.IsDeleted = true;

			await _context.SaveChangesAsync();

			// ── Audit Log ─────────────────────────────────────────────────

			await _auditLogService.LogAsync(

				userId: _currentUserService.GetUserId(),

				action: "DELETE",

				resource: "User",

				metadata: $"ID: {id}, Name: {user.Name}"

			);

			return true;

		}

		// ── Email Exists ──────────────────────────────────────────────────

		public async Task<bool> EmailExistsAsync(string email)

		{

			return await _context.Users

				.AnyAsync(u => u.Email == email && !u.IsDeleted);

		}

		// ── Mapper ────────────────────────────────────────────────────────

		private static UserResponseDto MapToResponseDto(User u) => new()

		{

			UserID = u.UserID,

			Name = u.Name,

			Role = u.Role.ToString(),

			Email = u.Email,

			Phone = u.Phone,

			IsDeleted = u.IsDeleted,

			CreatedAt = u.CreatedAt

		};

	}

}
