using Microsoft.EntityFrameworkCore;
using WarehousePro.API.Data;
using WarehousePro.API.DTOs.Outbound;
using WarehousePro.API.Models;
using WarehousePro.API.Services.Interfaces;

namespace WarehousePro.API.Services
{
	public class PackingUnitService : IPackingUnitService
	{
		private readonly AppDbContext _context;
		private readonly IAuditLogService _auditLogService;
		private readonly ICurrentUserService _currentUserService;

		public PackingUnitService(
			AppDbContext context,
			IAuditLogService auditLogService,
			ICurrentUserService currentUserService)
		{
			_context = context;
			_auditLogService = auditLogService;
			_currentUserService = currentUserService;
		}

		public async Task<List<PackingUnitResponseDto>> GetAllAsync()
		{
			return await _context.PackingUnits
				.Include(p => p.Order)
				.Select(p => MapToResponseDto(p))
				.ToListAsync();
		}

		public async Task<List<PackingUnitResponseDto>> GetByOrderIdAsync(int orderId)
		{
			return await _context.PackingUnits
				.Where(p => p.OrderID == orderId)
				.Include(p => p.Order)
				.Select(p => MapToResponseDto(p))
				.ToListAsync();
		}

		public async Task<PackingUnitResponseDto?> GetByIdAsync(int id)
		{
			var pack = await _context.PackingUnits
				.Include(p => p.Order)
				.FirstOrDefaultAsync(p => p.PackID == id);

			return pack == null ? null : MapToResponseDto(pack);
		}

		public async Task<PackingUnitResponseDto> CreateAsync(PackingUnitCreateDto dto)
		{
			var pack = new PackingUnit
			{
				OrderID = dto.OrderID,
				PackageType = dto.PackageType,
				Weight = dto.Weight,
				PackedAt = DateTime.UtcNow
			};

			_context.PackingUnits.Add(pack);
			await _context.SaveChangesAsync();

			await _context.Entry(pack).Reference(p => p.Order).LoadAsync();

			// ── Audit Log ─────────────────────────────────────────────────
			await _auditLogService.LogAsync(
				userId: _currentUserService.GetUserId(),
				action: "CREATE",
				resource: "PackingUnit",
				metadata: $"ID: {pack.PackID}, OrderID: {pack.OrderID}, Type: {pack.PackageType}, Weight: {pack.Weight}"
			);

			return MapToResponseDto(pack);
		}

		public async Task<PackingUnitResponseDto?> UpdateAsync(int id, PackingUnitUpdateDto dto)
		{
			var pack = await _context.PackingUnits
				.Include(p => p.Order)
				.FirstOrDefaultAsync(p => p.PackID == id);

			if (pack == null) return null;

			pack.Status = dto.Status;
			await _context.SaveChangesAsync();

			// ── Audit Log ─────────────────────────────────────────────────
			await _auditLogService.LogAsync(
				userId: _currentUserService.GetUserId(),
				action: "UPDATE",
				resource: "PackingUnit",
				metadata: $"ID: {id}, Status: {pack.Status}"
			);

			return MapToResponseDto(pack);
		}

		public async Task<bool> DeleteAsync(int id)
		{
			var pack = await _context.PackingUnits
				.FirstOrDefaultAsync(p => p.PackID == id);

			if (pack == null) return false;

			_context.PackingUnits.Remove(pack);
			await _context.SaveChangesAsync();

			// ── Audit Log ─────────────────────────────────────────────────
			await _auditLogService.LogAsync(
				userId: _currentUserService.GetUserId(),
				action: "DELETE",
				resource: "PackingUnit",
				metadata: $"ID: {id}"
			);

			return true;
		}

		private static PackingUnitResponseDto MapToResponseDto(PackingUnit p) => new()
		{
			PackID = p.PackID,
			OrderID = p.OrderID,
			OrderNumber = p.Order?.OrderNumber ?? string.Empty,
			PackageType = p.PackageType,
			Weight = p.Weight,
			Status = p.Status.ToString(),
			PackedAt = p.PackedAt
		};
	}
}