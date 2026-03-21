using Microsoft.EntityFrameworkCore;

using WarehousePro.API.Data;

using WarehousePro.API.DTOs.Zone;

using WarehousePro.API.Models;

using WarehousePro.API.Services.Interfaces;

namespace WarehousePro.API.Services

{

	public class ZoneService : IZoneService

	{

		private readonly AppDbContext _context;

		private readonly IAuditLogService _auditLogService;

		private readonly ICurrentUserService _currentUserService;

		public ZoneService(

			AppDbContext context,

			IAuditLogService auditLogService,

			ICurrentUserService currentUserService)

		{

			_context = context;

			_auditLogService = auditLogService;

			_currentUserService = currentUserService;

		}

		// ── Get All ───────────────────────────────────────────────────────

		public async Task<List<ZoneResponseDto>> GetAllAsync()

		{

			return await _context.Zones

				.Where(z => !z.IsDeleted)

				.Include(z => z.Warehouse)

				.Include(z => z.BinLocations)

				.Select(z => MapToResponseDto(z))

				.ToListAsync();

		}

		// ── Get By Warehouse ID ───────────────────────────────────────────

		public async Task<List<ZoneResponseDto>> GetByWarehouseIdAsync(int warehouseId)

		{

			return await _context.Zones

				.Where(z => z.WarehouseID == warehouseId && !z.IsDeleted)

				.Include(z => z.Warehouse)

				.Include(z => z.BinLocations)

				.Select(z => MapToResponseDto(z))

				.ToListAsync();

		}

		// ── Get By ID ─────────────────────────────────────────────────────

		public async Task<ZoneResponseDto?> GetByIdAsync(int id)

		{

			var zone = await _context.Zones

				.Include(z => z.Warehouse)

				.Include(z => z.BinLocations)

				.FirstOrDefaultAsync(z => z.ZoneID == id && !z.IsDeleted);

			return zone == null ? null : MapToResponseDto(zone);

		}

		// ── Create ────────────────────────────────────────────────────────

		public async Task<ZoneResponseDto> CreateAsync(ZoneCreateDto dto)

		{

			var zone = new Zone

			{

				WarehouseID = dto.WarehouseID,

				Name = dto.Name,

				ZoneType = dto.ZoneType,

				CreatedAt = DateTime.UtcNow

			};

			_context.Zones.Add(zone);

			await _context.SaveChangesAsync();

			await _context.Entry(zone).Reference(z => z.Warehouse).LoadAsync();

			// ── Audit Log ─────────────────────────────────────────────────

			await _auditLogService.LogAsync(

				userId: _currentUserService.GetUserId(),

				action: "CREATE",

				resource: "Zone",

				metadata: $"ID: {zone.ZoneID}, Name: {zone.Name}, Type: {zone.ZoneType}, WarehouseID: {zone.WarehouseID}"

			);

			return MapToResponseDto(zone);

		}

		// ── Update ────────────────────────────────────────────────────────

		public async Task<ZoneResponseDto?> UpdateAsync(int id, ZoneUpdateDto dto)

		{

			var zone = await _context.Zones

				.Include(z => z.Warehouse)

				.Include(z => z.BinLocations)

				.FirstOrDefaultAsync(z => z.ZoneID == id && !z.IsDeleted);

			if (zone == null) return null;

			zone.Name = dto.Name;

			zone.ZoneType = dto.ZoneType;

			zone.IsDeleted = dto.IsDeleted;

			await _context.SaveChangesAsync();

			// ── Audit Log ─────────────────────────────────────────────────

			await _auditLogService.LogAsync(

				userId: _currentUserService.GetUserId(),

				action: "UPDATE",

				resource: "Zone",

				metadata: $"ID: {id}, Name: {zone.Name}, Type: {zone.ZoneType}"

			);

			return MapToResponseDto(zone);

		}

		// ── Soft Delete ───────────────────────────────────────────────────

		public async Task<bool> DeleteAsync(int id)

		{

			var zone = await _context.Zones

				.FirstOrDefaultAsync(z => z.ZoneID == id && !z.IsDeleted);

			if (zone == null) return false;

			zone.IsDeleted = true;

			await _context.SaveChangesAsync();

			// ── Audit Log ─────────────────────────────────────────────────

			await _auditLogService.LogAsync(

				userId: _currentUserService.GetUserId(),

				action: "DELETE",

				resource: "Zone",

				metadata: $"ID: {id}, Name: {zone.Name}"

			);

			return true;

		}

		// ── Mapper ────────────────────────────────────────────────────────

		private static ZoneResponseDto MapToResponseDto(Zone z) => new()

		{

			ZoneID = z.ZoneID,

			WarehouseID = z.WarehouseID,

			WarehouseName = z.Warehouse?.Name ?? string.Empty,

			Name = z.Name,

			ZoneType = z.ZoneType.ToString(),

			TotalBins = z.BinLocations?.Count(b => !b.IsDeleted) ?? 0

		};

	}

}
