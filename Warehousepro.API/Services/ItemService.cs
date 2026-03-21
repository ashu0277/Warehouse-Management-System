using Microsoft.EntityFrameworkCore;

using WarehousePro.API.Data;

using WarehousePro.API.DTOs.Item;

using WarehousePro.API.Models;

using WarehousePro.API.Services.Interfaces;

namespace WarehousePro.API.Services

{

	public class ItemService : IItemService

	{

		private readonly AppDbContext _context;

		private readonly IAuditLogService _auditLogService;

		private readonly ICurrentUserService _currentUserService;

		public ItemService(

			AppDbContext context,

			IAuditLogService auditLogService,

			ICurrentUserService currentUserService)

		{

			_context = context;

			_auditLogService = auditLogService;

			_currentUserService = currentUserService;

		}

		// ── Get All ───────────────────────────────────────────────────────

		public async Task<List<ItemResponseDto>> GetAllAsync()

		{

			var items = await _context.Items

				.Where(i => !i.IsDeleted)

				.Include(i => i.InventoryBalances)

				.ToListAsync();

			return items.Select(MapToResponseDto).ToList();

		}

		// ── Get By ID ─────────────────────────────────────────────────────

		public async Task<ItemResponseDto?> GetByIdAsync(int id)

		{

			var item = await _context.Items

				.Include(i => i.InventoryBalances)

				.FirstOrDefaultAsync(i => i.ItemID == id && !i.IsDeleted);

			return item == null ? null : MapToResponseDto(item);

		}

		// ── Get By SKU ────────────────────────────────────────────────────

		public async Task<ItemResponseDto?> GetBySkuAsync(string sku)

		{

			var item = await _context.Items

				.Include(i => i.InventoryBalances)

				.FirstOrDefaultAsync(i => i.SKU == sku && !i.IsDeleted);

			return item == null ? null : MapToResponseDto(item);

		}

		// ── Create ────────────────────────────────────────────────────────

		public async Task<ItemResponseDto> CreateAsync(ItemCreateDto dto)

		{

			var item = new Item

			{

				SKU = dto.SKU,

				Description = dto.Description,

				UnitOfMeasure = dto.UnitOfMeasure,

				CreatedAt = DateTime.UtcNow

			};

			_context.Items.Add(item);

			await _context.SaveChangesAsync();

			// ── Audit Log ─────────────────────────────────────────────────

			await _auditLogService.LogAsync(

				userId: _currentUserService.GetUserId(),

				action: "CREATE",

				resource: "Item",

				metadata: $"ID: {item.ItemID}, SKU: {item.SKU}, UOM: {item.UnitOfMeasure}"

			);

			return MapToResponseDto(item);

		}

		// ── Update ────────────────────────────────────────────────────────

		public async Task<ItemResponseDto?> UpdateAsync(int id, ItemUpdateDto dto)

		{

			var item = await _context.Items

				.Include(i => i.InventoryBalances)

				.FirstOrDefaultAsync(i => i.ItemID == id && !i.IsDeleted);

			if (item == null) return null;

			item.Description = dto.Description;

			item.UnitOfMeasure = dto.UnitOfMeasure;

			item.Status = dto.Status;

			await _context.SaveChangesAsync();

			// ── Audit Log ─────────────────────────────────────────────────

			await _auditLogService.LogAsync(

				userId: _currentUserService.GetUserId(),

				action: "UPDATE",

				resource: "Item",

				metadata: $"ID: {id}, SKU: {item.SKU}, Status: {item.Status}"

			);

			return MapToResponseDto(item);

		}

		// ── Soft Delete ───────────────────────────────────────────────────

		public async Task<bool> DeleteAsync(int id)

		{

			var item = await _context.Items

				.FirstOrDefaultAsync(i => i.ItemID == id && !i.IsDeleted);

			if (item == null) return false;

			item.IsDeleted = true;

			await _context.SaveChangesAsync();

			// ── Audit Log ─────────────────────────────────────────────────

			await _auditLogService.LogAsync(

				userId: _currentUserService.GetUserId(),

				action: "DELETE",

				resource: "Item",

				metadata: $"ID: {id}, SKU: {item.SKU}"

			);

			return true;

		}

		// ── Mapper ────────────────────────────────────────────────────────

		private static ItemResponseDto MapToResponseDto(Item i) => new()

		{

			ItemID = i.ItemID,

			SKU = i.SKU,

			Description = i.Description,

			UnitOfMeasure = i.UnitOfMeasure,

			Status = i.Status.ToString(),

			CreatedAt = i.CreatedAt,

			TotalQuantityOnHand = i.InventoryBalances?.Sum(b => b.QuantityOnHand) ?? 0,

			TotalReservedQuantity = i.InventoryBalances?.Sum(b => b.ReservedQuantity) ?? 0,

			TotalAvailableQuantity = i.InventoryBalances?.Sum(b => b.AvailableQuantity) ?? 0

		};

	}

}
