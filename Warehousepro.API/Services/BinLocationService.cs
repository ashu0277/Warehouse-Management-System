using Microsoft.EntityFrameworkCore;

using WarehousePro.API.Data;

using WarehousePro.API.DTOs.BinLocation;

using WarehousePro.API.Models;

using WarehousePro.API.Services.Interfaces;

namespace WarehousePro.API.Services

{

	public class BinLocationService : IBinLocationService

	{

		private readonly AppDbContext _context;

		private readonly IAuditLogService _auditLogService;

		private readonly ICurrentUserService _currentUserService;

		public BinLocationService(

			AppDbContext context,

			IAuditLogService auditLogService,

			ICurrentUserService currentUserService)

		{

			_context = context;

			_auditLogService = auditLogService;

			_currentUserService = currentUserService;

		}

		// ── Get All ───────────────────────────────────────────────────────

		public async Task<List<BinLocationResponseDto>> GetAllAsync()

		{

			return await _context.BinLocations

				.Where(b => !b.IsDeleted)

				.Include(b => b.Zone).ThenInclude(z => z.Warehouse)

				.Select(b => MapToResponseDto(b))

				.ToListAsync();

		}

		// ── Get By Zone ID ────────────────────────────────────────────────

		public async Task<List<BinLocationResponseDto>> GetByZoneIdAsync(int zoneId)

		{

			return await _context.BinLocations

				.Where(b => b.ZoneID == zoneId && !b.IsDeleted)

				.Include(b => b.Zone).ThenInclude(z => z.Warehouse)

				.Select(b => MapToResponseDto(b))

				.ToListAsync();

		}

		// ── Get By ID ─────────────────────────────────────────────────────

		public async Task<BinLocationResponseDto?> GetByIdAsync(int id)

		{

			var bin = await _context.BinLocations

				.Include(b => b.Zone).ThenInclude(z => z.Warehouse)

				.FirstOrDefaultAsync(b => b.BinID == id && !b.IsDeleted);

			return bin == null ? null : MapToResponseDto(bin);

		}

		// ── Create ────────────────────────────────────────────────────────

		public async Task<BinLocationResponseDto> CreateAsync(BinLocationCreateDto dto)

		{

			var bin = new BinLocation

			{

				ZoneID = dto.ZoneID,

				Code = dto.Code,

				Capacity = dto.Capacity,

				CreatedAt = DateTime.UtcNow

			};

			_context.BinLocations.Add(bin);

			await _context.SaveChangesAsync();

			await _context.Entry(bin).Reference(b => b.Zone).LoadAsync();

			await _context.Entry(bin.Zone).Reference(z => z.Warehouse).LoadAsync();

			// ── Audit Log ─────────────────────────────────────────────────

			await _auditLogService.LogAsync(

				userId: _currentUserService.GetUserId(),

				action: "CREATE",

				resource: "BinLocation",

				metadata: $"ID: {bin.BinID}, Code: {bin.Code}, Capacity: {bin.Capacity}, ZoneID: {bin.ZoneID}"

			);

			return MapToResponseDto(bin);

		}

		// ── Update ────────────────────────────────────────────────────────

		public async Task<BinLocationResponseDto?> UpdateAsync(int id, BinLocationUpdateDto dto)

		{

			var bin = await _context.BinLocations

				.Include(b => b.Zone).ThenInclude(z => z.Warehouse)

				.FirstOrDefaultAsync(b => b.BinID == id && !b.IsDeleted);

			if (bin == null) return null;

			bin.Code = dto.Code;

			bin.Capacity = dto.Capacity;

			bin.Status = dto.Status;

			await _context.SaveChangesAsync();

			// ── Audit Log ─────────────────────────────────────────────────

			await _auditLogService.LogAsync(

				userId: _currentUserService.GetUserId(),

				action: "UPDATE",

				resource: "BinLocation",

				metadata: $"ID: {id}, Code: {bin.Code}, Status: {bin.Status}"

			);

			return MapToResponseDto(bin);

		}

		// ── Soft Delete ───────────────────────────────────────────────────

		public async Task<bool> DeleteAsync(int id)

		{

			var bin = await _context.BinLocations

				.FirstOrDefaultAsync(b => b.BinID == id && !b.IsDeleted);

			if (bin == null) return false;

			bin.IsDeleted = true;

			await _context.SaveChangesAsync();

			// ── Audit Log ─────────────────────────────────────────────────

			await _auditLogService.LogAsync(

				userId: _currentUserService.GetUserId(),

				action: "DELETE",

				resource: "BinLocation",

				metadata: $"ID: {id}, Code: {bin.Code}"

			);

			return true;

		}

		// ── Mapper ────────────────────────────────────────────────────────

		private static BinLocationResponseDto MapToResponseDto(BinLocation b) => new()

		{

			BinID = b.BinID,

			ZoneID = b.ZoneID,

			ZoneName = b.Zone?.Name ?? string.Empty,

			WarehouseName = b.Zone?.Warehouse?.Name ?? string.Empty,

			Code = b.Code,

			Capacity = b.Capacity,

			Status = b.Status.ToString()

		};

	}

}
