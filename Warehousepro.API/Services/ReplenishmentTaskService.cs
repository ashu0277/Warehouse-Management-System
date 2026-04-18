using Microsoft.EntityFrameworkCore;

using WarehousePro.API.Data;

using WarehousePro.API.DTOs.Replenishment;

using WarehousePro.API.Models;

using WarehousePro.API.Services.Interfaces;

namespace WarehousePro.API.Services

{

	public class ReplenishmentTaskService : IReplenishmentTaskService

	{

		private readonly AppDbContext _context;

		private readonly IAuditLogService _auditLogService;

		private readonly ICurrentUserService _currentUserService;

		public ReplenishmentTaskService(

		  AppDbContext context,

		  IAuditLogService auditLogService,

		  ICurrentUserService currentUserService)

		{

			_context = context;

			_auditLogService = auditLogService;

			_currentUserService = currentUserService;

		}

		public async Task<List<ReplenishmentTaskResponseDto>> GetAllAsync()

		{

			return await _context.ReplenishmentTasks

			  .Include(r => r.Item)

			  .Include(r => r.FromBin).ThenInclude(b => b.Zone)

			  .Include(r => r.ToBin).ThenInclude(b => b.Zone)

			  .Select(r => MapToResponseDto(r))

			  .ToListAsync();

		}

		public async Task<List<ReplenishmentTaskResponseDto>> GetByItemIdAsync(int itemId)

		{

			return await _context.ReplenishmentTasks

			  .Where(r => r.ItemID == itemId)

			  .Include(r => r.Item)

			  .Include(r => r.FromBin).ThenInclude(b => b.Zone)

			  .Include(r => r.ToBin).ThenInclude(b => b.Zone)

			  .Select(r => MapToResponseDto(r))

			  .ToListAsync();

		}

		public async Task<ReplenishmentTaskResponseDto?> GetByIdAsync(int id)

		{

			var task = await _context.ReplenishmentTasks

			  .Include(r => r.Item)

			  .Include(r => r.FromBin).ThenInclude(b => b.Zone)

			  .Include(r => r.ToBin).ThenInclude(b => b.Zone)

			  .FirstOrDefaultAsync(r => r.ReplenishID == id);

			return task == null ? null : MapToResponseDto(task);

		}

		public async Task<ReplenishmentTaskResponseDto> CreateAsync(ReplenishmentTaskCreateDto dto)

		{

			var task = new ReplenishmentTask

			{

				ItemID = dto.ItemID,

				FromBinID = dto.FromBinID,

				ToBinID = dto.ToBinID,

				Quantity = dto.Quantity,

				CreatedAt = DateTime.UtcNow

			};

			_context.ReplenishmentTasks.Add(task);

			await _context.SaveChangesAsync();

			await _context.Entry(task).Reference(r => r.Item).LoadAsync();

			await _context.Entry(task).Reference(r => r.FromBin).LoadAsync();

			await _context.Entry(task).Reference(r => r.ToBin).LoadAsync();

			// ── Audit Log ─────────────────────────────────────────────────

			await _auditLogService.LogAsync(

			  userId: _currentUserService.GetUserId(),

			  action: "CREATE",

			  resource: "ReplenishmentTask",

			  metadata: $"ID: {task.ReplenishID}, ItemID: {task.ItemID}, FromBin: {task.FromBinID}, ToBin: {task.ToBinID}, Qty: {task.Quantity}"

			);

			return MapToResponseDto(task);

		}

		public async Task<ReplenishmentTaskResponseDto?> UpdateAsync(int id, ReplenishmentTaskUpdateDto dto)

		{

			var task = await _context.ReplenishmentTasks

			  .Include(r => r.Item)

			  .Include(r => r.FromBin).ThenInclude(b => b.Zone)

			  .Include(r => r.ToBin).ThenInclude(b => b.Zone)

			  .FirstOrDefaultAsync(r => r.ReplenishID == id);

			if (task == null) return null;

			task.Status = dto.Status;

			// ══════════════════════════════════════════════════════════════
			// AUTO LOGIC — fires when task is marked as Completed
			// ══════════════════════════════════════════════════════════════

			if (dto.Status == Models.Enums.ReplenishmentStatus.Completed)

			{

				task.CompletedAt = DateTime.UtcNow;

				// ─────────────────────────────────────────────────────────
				// AUTO STEP 1 — Decrease quantity from source bin (FromBin)
				// ─────────────────────────────────────────────────────────

				var fromBalance = await _context.InventoryBalances

				  .FirstOrDefaultAsync(ib =>

					ib.ItemID == task.ItemID &&

					ib.BinID == task.FromBinID);

				if (fromBalance != null)

				{

					// Decrease quantity from source bin

					fromBalance.QuantityOnHand = Math.Max(0,

					  fromBalance.QuantityOnHand - task.Quantity);

					fromBalance.LastUpdated = DateTime.UtcNow;

				}

				// ─────────────────────────────────────────────────────────
				// AUTO STEP 2 — Increase quantity in destination bin (ToBin)
				// ─────────────────────────────────────────────────────────

				var toBalance = await _context.InventoryBalances

				  .FirstOrDefaultAsync(ib =>

					ib.ItemID == task.ItemID &&

					ib.BinID == task.ToBinID);

				if (toBalance != null)

				{

					// Balance exists → increase quantity

					toBalance.QuantityOnHand += task.Quantity;

					toBalance.LastUpdated = DateTime.UtcNow;

				}

				else

				{

					// First time this item is in destination bin → create new record

					_context.InventoryBalances.Add(new InventoryBalance

					{

						ItemID = task.ItemID,

						BinID = task.ToBinID,

						QuantityOnHand = task.Quantity,

						ReservedQuantity = 0,

						LastUpdated = DateTime.UtcNow

					});

				}

				await _context.SaveChangesAsync();

				// Log the automatic inventory update

				await _auditLogService.LogAsync(

				  userId: _currentUserService.GetUserId(),

				  action: "AUTO_UPDATE",

				  resource: "InventoryBalance",

				  metadata: $"Replenishment completed → " +

						$"ItemID: {task.ItemID}, FromBin: {task.FromBinID}, " +

						$"ToBin: {task.ToBinID}, Moved: {task.Quantity} units"

				);

			}

			await _context.SaveChangesAsync();

			// ── Audit Log for task update ─────────────────────────────────

			await _auditLogService.LogAsync(

			  userId: _currentUserService.GetUserId(),

			  action: "UPDATE",

			  resource: "ReplenishmentTask",

			  metadata: $"ID: {id}, Status: {task.Status}"

			);

			return MapToResponseDto(task);

		}

		public async Task<bool> DeleteAsync(int id)

		{

			var task = await _context.ReplenishmentTasks

			  .FirstOrDefaultAsync(r => r.ReplenishID == id);

			if (task == null) return false;

			_context.ReplenishmentTasks.Remove(task);

			await _context.SaveChangesAsync();

			// ── Audit Log ─────────────────────────────────────────────────

			await _auditLogService.LogAsync(

			  userId: _currentUserService.GetUserId(),

			  action: "DELETE",

			  resource: "ReplenishmentTask",

			  metadata: $"ID: {id}"

			);

			return true;

		}

		private static ReplenishmentTaskResponseDto MapToResponseDto(ReplenishmentTask r) => new()

		{

			ReplenishID = r.ReplenishID,

			ItemID = r.ItemID,

			SKU = r.Item?.SKU ?? string.Empty,

			ItemDescription = r.Item?.Description,

			FromBinID = r.FromBinID,

			FromBinCode = r.FromBin?.Code ?? string.Empty,

			FromZoneName = r.FromBin?.Zone?.Name ?? string.Empty,

			ToBinID = r.ToBinID,

			ToBinCode = r.ToBin?.Code ?? string.Empty,

			ToZoneName = r.ToBin?.Zone?.Name ?? string.Empty,

			Quantity = r.Quantity,

			Status = r.Status.ToString(),

			CreatedAt = r.CreatedAt,

			CompletedAt = r.CompletedAt

		};

	}

}
