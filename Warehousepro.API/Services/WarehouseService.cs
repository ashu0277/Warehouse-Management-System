using Microsoft.EntityFrameworkCore;

using WarehousePro.API.Data;

using WarehousePro.API.DTOs.Warehouse;

using WarehousePro.API.Models;

using WarehousePro.API.Services.Interfaces;

namespace WarehousePro.API.Services

{

	public class WarehouseService : IWarehouseService

	{

		private readonly AppDbContext _context;

		private readonly IAuditLogService _auditLogService;

		private readonly ICurrentUserService _currentUserService;

		public WarehouseService(

			AppDbContext context,

			IAuditLogService auditLogService,

			ICurrentUserService currentUserService)

		{

			_context = context;

			_auditLogService = auditLogService;

			_currentUserService = currentUserService;

		}

		// ── Get All ───────────────────────────────────────────────────────

		public async Task<List<WarehouseResponseDto>> GetAllAsync()

		{

			return await _context.Warehouses

				.Where(w => !w.IsDeleted)

				.Include(w => w.Zones)

				.Select(w => MapToResponseDto(w))

				.ToListAsync();

		}

		// ── Get By ID ─────────────────────────────────────────────────────

		public async Task<WarehouseResponseDto?> GetByIdAsync(int id)

		{

			var warehouse = await _context.Warehouses

				.Include(w => w.Zones)

				.FirstOrDefaultAsync(w => w.WarehouseID == id && !w.IsDeleted);

			return warehouse == null ? null : MapToResponseDto(warehouse);

		}

		// ── Create ────────────────────────────────────────────────────────

		public async Task<WarehouseResponseDto> CreateAsync(WarehouseCreateDto dto)

		{

			var warehouse = new Warehouse

			{

				Name = dto.Name,

				Location = dto.Location,

				CreatedAt = DateTime.UtcNow

			};

			_context.Warehouses.Add(warehouse);

			await _context.SaveChangesAsync();

			// ── Audit Log ─────────────────────────────────────────────────

			await _auditLogService.LogAsync(

				userId: _currentUserService.GetUserId(),

				action: "CREATE",

				resource: "Warehouse",

				metadata: $"ID: {warehouse.WarehouseID}, Name: {warehouse.Name}, Location: {warehouse.Location}"

			);

			return MapToResponseDto(warehouse);

		}

		// ── Update ────────────────────────────────────────────────────────

		public async Task<WarehouseResponseDto?> UpdateAsync(int id, WarehouseUpdateDto dto)

		{

			var warehouse = await _context.Warehouses

				.Include(w => w.Zones)

				.FirstOrDefaultAsync(w => w.WarehouseID == id && !w.IsDeleted);

			if (warehouse == null) return null;

			warehouse.Name = dto.Name;

			warehouse.Location = dto.Location;

			warehouse.Status = dto.Status;

			await _context.SaveChangesAsync();

			// ── Audit Log ─────────────────────────────────────────────────

			await _auditLogService.LogAsync(

				userId: _currentUserService.GetUserId(),

				action: "UPDATE",

				resource: "Warehouse",

				metadata: $"ID: {id}, Name: {warehouse.Name}, Status: {warehouse.Status}"

			);

			return MapToResponseDto(warehouse);

		}

		// ── Soft Delete ───────────────────────────────────────────────────

		public async Task<bool> DeleteAsync(int id)

		{

			var warehouse = await _context.Warehouses

				.FirstOrDefaultAsync(w => w.WarehouseID == id && !w.IsDeleted);

			if (warehouse == null) return false;

			warehouse.IsDeleted = true;

			await _context.SaveChangesAsync();

			// ── Audit Log ─────────────────────────────────────────────────

			await _auditLogService.LogAsync(

				userId: _currentUserService.GetUserId(),

				action: "DELETE",

				resource: "Warehouse",

				metadata: $"ID: {id}, Name: {warehouse.Name}"

			);

			return true;

		}

		// ── Mapper ────────────────────────────────────────────────────────

		private static WarehouseResponseDto MapToResponseDto(Warehouse w) => new()

		{

			WarehouseID = w.WarehouseID,

			Name = w.Name,

			Location = w.Location,

			Status = w.Status.ToString(),

			CreatedAt = w.CreatedAt,

			TotalZones = w.Zones?.Count(z => !z.IsDeleted) ?? 0

		};

	}

}
