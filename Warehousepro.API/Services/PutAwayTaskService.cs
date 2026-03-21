using Microsoft.EntityFrameworkCore;

using WarehousePro.API.Data;

using WarehousePro.API.DTOs.Inbound;

using WarehousePro.API.Models;

using WarehousePro.API.Models.Enums;

using WarehousePro.API.Services.Interfaces;

namespace WarehousePro.API.Services

{

	public class PutAwayTaskService : IPutAwayTaskService

	{

		private readonly AppDbContext _context;

		private readonly IAuditLogService _auditLogService;

		private readonly ICurrentUserService _currentUserService;

		// ── Minimum stock threshold ───────────────────────────────────────

		// If total available stock falls below this → low stock notification fires

		private const int MinimumStockThreshold = 10;

		public PutAwayTaskService(

			AppDbContext context,

			IAuditLogService auditLogService,

			ICurrentUserService currentUserService)

		{

			_context = context;

			_auditLogService = auditLogService;

			_currentUserService = currentUserService;

		}

		// ── Get All ───────────────────────────────────────────────────────

		public async Task<List<PutAwayTaskResponseDto>> GetAllAsync()

		{

			return await _context.PutAwayTasks

				.Include(p => p.InboundReceipt)

				.Include(p => p.Item)

				.Include(p => p.TargetBin)

				.Include(p => p.AssignedTo)

				.Select(p => MapToResponseDto(p))

				.ToListAsync();

		}

		// ── Get By Receipt ID ─────────────────────────────────────────────

		public async Task<List<PutAwayTaskResponseDto>> GetByReceiptIdAsync(int receiptId)

		{

			return await _context.PutAwayTasks

				.Where(p => p.ReceiptID == receiptId)

				.Include(p => p.InboundReceipt)

				.Include(p => p.Item)

				.Include(p => p.TargetBin)

				.Include(p => p.AssignedTo)

				.Select(p => MapToResponseDto(p))

				.ToListAsync();

		}

		// ── Get By User ID ────────────────────────────────────────────────

		public async Task<List<PutAwayTaskResponseDto>> GetByUserIdAsync(int userId)

		{

			return await _context.PutAwayTasks

				.Where(p => p.AssignedToUserID == userId)

				.Include(p => p.InboundReceipt)

				.Include(p => p.Item)

				.Include(p => p.TargetBin)

				.Include(p => p.AssignedTo)

				.Select(p => MapToResponseDto(p))

				.ToListAsync();

		}

		// ── Get By ID ─────────────────────────────────────────────────────

		public async Task<PutAwayTaskResponseDto?> GetByIdAsync(int id)

		{

			var task = await _context.PutAwayTasks

				.Include(p => p.InboundReceipt)

				.Include(p => p.Item)

				.Include(p => p.TargetBin)

				.Include(p => p.AssignedTo)

				.FirstOrDefaultAsync(p => p.TaskID == id);

			return task == null ? null : MapToResponseDto(task);

		}

		// ── Create ────────────────────────────────────────────────────────

		public async Task<PutAwayTaskResponseDto> CreateAsync(PutAwayTaskCreateDto dto)

		{

			var task = new PutAwayTask

			{

				ReceiptID = dto.ReceiptID,

				ItemID = dto.ItemID,

				Quantity = dto.Quantity,

				TargetBinID = dto.TargetBinID,

				AssignedToUserID = dto.AssignedToUserID,

				CreatedAt = DateTime.UtcNow

			};

			_context.PutAwayTasks.Add(task);

			await _context.SaveChangesAsync();

			await _context.Entry(task).Reference(p => p.InboundReceipt).LoadAsync();

			await _context.Entry(task).Reference(p => p.Item).LoadAsync();

			await _context.Entry(task).Reference(p => p.TargetBin).LoadAsync();

			if (task.AssignedToUserID.HasValue)

				await _context.Entry(task).Reference(p => p.AssignedTo).LoadAsync();

			// ── Audit Log ─────────────────────────────────────────────────

			await _auditLogService.LogAsync(

				userId: _currentUserService.GetUserId(),

				action: "CREATE",

				resource: "PutAwayTask",

				metadata: $"ID: {task.TaskID}, ReceiptID: {task.ReceiptID}, " +

						  $"ItemID: {task.ItemID}, BinID: {task.TargetBinID}, Qty: {task.Quantity}"

			);

			return MapToResponseDto(task);

		}

		// ── Update ────────────────────────────────────────────────────────

		public async Task<PutAwayTaskResponseDto?> UpdateAsync(int id, PutAwayTaskUpdateDto dto)

		{

			var task = await _context.PutAwayTasks

				.Include(p => p.InboundReceipt)

				.Include(p => p.Item)

				.Include(p => p.TargetBin)

				.Include(p => p.AssignedTo)

				.FirstOrDefaultAsync(p => p.TaskID == id);

			if (task == null) return null;

			task.AssignedToUserID = dto.AssignedToUserID;

			task.Status = dto.Status;

			if (dto.TargetBinID.HasValue)

				task.TargetBinID = dto.TargetBinID.Value;

			// ══════════════════════════════════════════════════════════════

			// AUTO LOGIC — fires when operator marks task as Completed

			// ══════════════════════════════════════════════════════════════

			if (dto.Status == PutAwayStatus.Completed)

			{

				task.CompletedAt = DateTime.UtcNow;

				// ─────────────────────────────────────────────────────────

				// AUTO STEP 1 — Update InventoryBalance

				// Check if a balance record already exists for this item+bin

				// ─────────────────────────────────────────────────────────

				var balance = await _context.InventoryBalances

					.FirstOrDefaultAsync(ib =>

						ib.ItemID == task.ItemID &&

						ib.BinID == task.TargetBinID);

				if (balance != null)

				{

					// Balance record exists → just increase quantity

					balance.QuantityOnHand += task.Quantity;

					balance.LastUpdated = DateTime.UtcNow;

				}

				else

				{

					// First time this item is stored in this bin → create new record

					_context.InventoryBalances.Add(new InventoryBalance

					{

						ItemID = task.ItemID,

						BinID = task.TargetBinID,

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

					metadata: $"PutAway completed → " +

							  $"ItemID: {task.ItemID}, BinID: {task.TargetBinID}, " +

							  $"Added: {task.Quantity} units"

				);

				// ─────────────────────────────────────────────────────────

				// AUTO STEP 2 — Check if total stock is below threshold

				// Sum available stock for this item across ALL bins

				// ─────────────────────────────────────────────────────────

				var totalAvailableStock = await _context.InventoryBalances

					.Where(ib => ib.ItemID == task.ItemID)

					.SumAsync(ib => ib.QuantityOnHand - ib.ReservedQuantity);

				if (totalAvailableStock < MinimumStockThreshold)

				{

					// ─────────────────────────────────────────────────────

					// AUTO STEP 3 — Create low stock notification

					// Supervisor / Planner will see this and create

					// a ReplenishmentTask manually

					// ─────────────────────────────────────────────────────

					_context.Notifications.Add(new Notification

					{

						UserID = _currentUserService.GetUserId(),

						Message = $"Low Stock Alert: Item {task.Item?.SKU ?? task.ItemID.ToString()} " +

									  $"has only {totalAvailableStock} units available. " +

									  $"Minimum threshold is {MinimumStockThreshold}. " +

									  $"Please create a Replenishment Task.",

						Category = NotificationCategory.Inventory,

						Status = NotificationStatus.Unread,

						CreatedDate = DateTime.UtcNow

					});

					await _context.SaveChangesAsync();

					// Log the low stock alert

					await _auditLogService.LogAsync(

						userId: _currentUserService.GetUserId(),

						action: "LOW_STOCK_ALERT",

						resource: "InventoryBalance",

						metadata: $"ItemID: {task.ItemID}, SKU: {task.Item?.SKU}, " +

								  $"TotalAvailable: {totalAvailableStock}, " +

								  $"Threshold: {MinimumStockThreshold}"

					);

				}

			}

			await _context.SaveChangesAsync();

			// ── Audit Log for task update ─────────────────────────────────

			await _auditLogService.LogAsync(

				userId: _currentUserService.GetUserId(),

				action: "UPDATE",

				resource: "PutAwayTask",

				metadata: $"ID: {id}, Status: {task.Status}, AssignedTo: {task.AssignedToUserID}"

			);

			return MapToResponseDto(task);

		}

		// ── Delete ────────────────────────────────────────────────────────

		public async Task<bool> DeleteAsync(int id)

		{

			var task = await _context.PutAwayTasks

				.FirstOrDefaultAsync(p => p.TaskID == id);

			if (task == null) return false;

			_context.PutAwayTasks.Remove(task);

			await _context.SaveChangesAsync();

			// ── Audit Log ─────────────────────────────────────────────────

			await _auditLogService.LogAsync(

				userId: _currentUserService.GetUserId(),

				action: "DELETE",

				resource: "PutAwayTask",

				metadata: $"ID: {id}"

			);

			return true;

		}

		// ── Mapper ────────────────────────────────────────────────────────

		private static PutAwayTaskResponseDto MapToResponseDto(PutAwayTask p) => new()

		{

			TaskID = p.TaskID,

			ReceiptID = p.ReceiptID,

			ReferenceNo = p.InboundReceipt?.ReferenceNo ?? string.Empty,

			ItemID = p.ItemID,

			SKU = p.Item?.SKU ?? string.Empty,

			ItemDescription = p.Item?.Description,

			Quantity = p.Quantity,

			TargetBinID = p.TargetBinID,

			TargetBinCode = p.TargetBin?.Code ?? string.Empty,

			AssignedToUserID = p.AssignedToUserID,

			AssignedToName = p.AssignedTo?.Name,

			Status = p.Status.ToString(),

			CreatedAt = p.CreatedAt,

			CompletedAt = p.CompletedAt

		};

	}

}
