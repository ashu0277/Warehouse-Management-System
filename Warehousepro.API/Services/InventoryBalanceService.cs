using Microsoft.EntityFrameworkCore;

using WarehousePro.API.Data;

using WarehousePro.API.DTOs.Inventory;

using WarehousePro.API.Models;

using WarehousePro.API.Services.Interfaces;

namespace WarehousePro.API.Services

{

	public class InventoryBalanceService : IInventoryBalanceService

	{

		private readonly AppDbContext _context;

		private readonly IAuditLogService _auditLogService;

		private readonly ICurrentUserService _currentUserService;

		public InventoryBalanceService(

			AppDbContext context,

			IAuditLogService auditLogService,

			ICurrentUserService currentUserService)

		{

			_context = context;

			_auditLogService = auditLogService;

			_currentUserService = currentUserService;

		}

		// ── Get All ───────────────────────────────────────────────────────

		public async Task<List<InventoryBalanceResponseDto>> GetAllAsync()

		{

			return await _context.InventoryBalances

				.Include(ib => ib.Item)

				.Include(ib => ib.BinLocation)

					.ThenInclude(b => b.Zone)

						.ThenInclude(z => z.Warehouse)

				.Select(ib => MapToResponseDto(ib))

				.ToListAsync();

		}

		// ── Get By Item ID ────────────────────────────────────────────────

		public async Task<List<InventoryBalanceResponseDto>> GetByItemIdAsync(int itemId)

		{

			return await _context.InventoryBalances

				.Where(ib => ib.ItemID == itemId)

				.Include(ib => ib.Item)

				.Include(ib => ib.BinLocation)

					.ThenInclude(b => b.Zone)

						.ThenInclude(z => z.Warehouse)

				.Select(ib => MapToResponseDto(ib))

				.ToListAsync();

		}

		// ── Get By Bin ID ─────────────────────────────────────────────────

		public async Task<List<InventoryBalanceResponseDto>> GetByBinIdAsync(int binId)

		{

			return await _context.InventoryBalances

				.Where(ib => ib.BinID == binId)

				.Include(ib => ib.Item)

				.Include(ib => ib.BinLocation)

					.ThenInclude(b => b.Zone)

						.ThenInclude(z => z.Warehouse)

				.Select(ib => MapToResponseDto(ib))

				.ToListAsync();

		}

		// ── Get By ID ─────────────────────────────────────────────────────

		public async Task<InventoryBalanceResponseDto?> GetByIdAsync(int id)

		{

			var ib = await _context.InventoryBalances

				.Include(ib => ib.Item)

				.Include(ib => ib.BinLocation)

					.ThenInclude(b => b.Zone)

						.ThenInclude(z => z.Warehouse)

				.FirstOrDefaultAsync(ib => ib.BalanceID == id);

			return ib == null ? null : MapToResponseDto(ib);

		}

		// ── Update ────────────────────────────────────────────────────────

		public async Task<InventoryBalanceResponseDto?> UpdateAsync(int id, InventoryBalanceUpdateDto dto)

		{

			var ib = await _context.InventoryBalances

				.Include(ib => ib.Item)

				.Include(ib => ib.BinLocation)

					.ThenInclude(b => b.Zone)

						.ThenInclude(z => z.Warehouse)

				.FirstOrDefaultAsync(ib => ib.BalanceID == id);

			if (ib == null) return null;

			ib.QuantityOnHand = dto.QuantityOnHand;

			ib.ReservedQuantity = dto.ReservedQuantity;

			ib.LastUpdated = DateTime.UtcNow;

			await _context.SaveChangesAsync();

			// ── Audit Log ─────────────────────────────────────────────────

			await _auditLogService.LogAsync(

				userId: _currentUserService.GetUserId(),

				action: "UPDATE",

				resource: "InventoryBalance",

				metadata: $"ID: {id}, ItemID: {ib.ItemID}, BinID: {ib.BinID}, OnHand: {ib.QuantityOnHand}, Reserved: {ib.ReservedQuantity}"

			);

			return MapToResponseDto(ib);

		}

		// ── Mapper ────────────────────────────────────────────────────────

		private static InventoryBalanceResponseDto MapToResponseDto(InventoryBalance ib) => new()

		{

			BalanceID = ib.BalanceID,

			ItemID = ib.ItemID,

			SKU = ib.Item?.SKU ?? string.Empty,

			ItemDescription = ib.Item?.Description,

			BinID = ib.BinID,

			BinCode = ib.BinLocation?.Code ?? string.Empty,

			ZoneName = ib.BinLocation?.Zone?.Name ?? string.Empty,

			WarehouseName = ib.BinLocation?.Zone?.Warehouse?.Name ?? string.Empty,

			QuantityOnHand = ib.QuantityOnHand,

			ReservedQuantity = ib.ReservedQuantity,

			AvailableQuantity = ib.AvailableQuantity,

			LastUpdated = ib.LastUpdated

		};

	}

}
