using Microsoft.EntityFrameworkCore;

using WarehousePro.API.Data;

using WarehousePro.API.DTOs.Outbound;

using WarehousePro.API.Models;

using WarehousePro.API.Models.Enums;

using WarehousePro.API.Services.Interfaces;

namespace WarehousePro.API.Services

{

	public class PickTaskService : IPickTaskService

	{

		private readonly AppDbContext _context;

		private readonly IAuditLogService _auditLogService;

		private readonly ICurrentUserService _currentUserService;

		// ── Minimum stock threshold ───────────────────────────────────────
		// If stock falls below this after picking → low stock notification fires
		private const int MinimumStockThreshold = 10;

		public PickTaskService(

		  AppDbContext context,

		  IAuditLogService auditLogService,

		  ICurrentUserService currentUserService)

		{

			_context = context;

			_auditLogService = auditLogService;

			_currentUserService = currentUserService;

		}

		// ── Get All ───────────────────────────────────────────────────────

		public async Task<List<PickTaskResponseDto>> GetAllAsync()

		{

			return await _context.PickTasks

			  .Include(p => p.Order)

			  .Include(p => p.Item)

			  .Include(p => p.BinLocation).ThenInclude(b => b.Zone)

			  .Include(p => p.AssignedTo)

			  .Select(p => MapToResponseDto(p))

			  .ToListAsync();

		}

		// ── Get By Order ID ───────────────────────────────────────────────

		public async Task<List<PickTaskResponseDto>> GetByOrderIdAsync(int orderId)

		{

			return await _context.PickTasks

			  .Where(p => p.OrderID == orderId)

			  .Include(p => p.Order)

			  .Include(p => p.Item)

			  .Include(p => p.BinLocation).ThenInclude(b => b.Zone)

			  .Include(p => p.AssignedTo)

			  .Select(p => MapToResponseDto(p))

			  .ToListAsync();

		}

		// ── Get By User ID ────────────────────────────────────────────────

		public async Task<List<PickTaskResponseDto>> GetByUserIdAsync(int userId)

		{

			return await _context.PickTasks

			  .Where(p => p.AssignedToUserID == userId)

			  .Include(p => p.Order)

			  .Include(p => p.Item)

			  .Include(p => p.BinLocation).ThenInclude(b => b.Zone)

			  .Include(p => p.AssignedTo)

			  .Select(p => MapToResponseDto(p))

			  .ToListAsync();

		}

		// ── Get By ID ─────────────────────────────────────────────────────

		public async Task<PickTaskResponseDto?> GetByIdAsync(int id)

		{

			var task = await _context.PickTasks

			  .Include(p => p.Order)

			  .Include(p => p.Item)

			  .Include(p => p.BinLocation).ThenInclude(b => b.Zone)

			  .Include(p => p.AssignedTo)

			  .FirstOrDefaultAsync(p => p.PickTaskID == id);

			return task == null ? null : MapToResponseDto(task);

		}

		// ── Create ────────────────────────────────────────────────────────

		public async Task<PickTaskResponseDto> CreateAsync(PickTaskCreateDto dto)

		{
			// ─────────────────────────────────────────────────────────────────
			// VALIDATION: Check if enough stock is available in the bin
			// ─────────────────────────────────────────────────────────────────
			var balance = await _context.InventoryBalances
			  .FirstOrDefaultAsync(ib =>
				ib.ItemID == dto.ItemID &&
				ib.BinID == dto.BinID);

			if (balance == null)
			{
				throw new InvalidOperationException(
				  $"No inventory found for ItemID {dto.ItemID} in BinID {dto.BinID}");
			}

			var availableQuantity = balance.QuantityOnHand - balance.ReservedQuantity;

			if (availableQuantity < dto.PickQuantity)
			{
				throw new InvalidOperationException(
				  $"Insufficient stock in bin. Available: {availableQuantity}, Requested: {dto.PickQuantity}");
			}

			// ─────────────────────────────────────────────────────────────────
			// Create the pick task
			// ─────────────────────────────────────────────────────────────────
			var task = new PickTask

			{

				OrderID = dto.OrderID,

				ItemID = dto.ItemID,

				BinID = dto.BinID,

				PickQuantity = dto.PickQuantity,

				AssignedToUserID = dto.AssignedToUserID,

				CreatedAt = DateTime.UtcNow

			};

			_context.PickTasks.Add(task);

			await _context.SaveChangesAsync();

			await _context.Entry(task).Reference(p => p.Order).LoadAsync();

			await _context.Entry(task).Reference(p => p.Item).LoadAsync();

			await _context.Entry(task).Reference(p => p.BinLocation).LoadAsync();

			if (task.AssignedToUserID.HasValue)

				await _context.Entry(task).Reference(p => p.AssignedTo).LoadAsync();

			// ── Audit Log ─────────────────────────────────────────────────

			await _auditLogService.LogAsync(

			  userId: _currentUserService.GetUserId(),

			  action: "CREATE",

			  resource: "PickTask",

			  metadata: $"ID: {task.PickTaskID}, OrderID: {task.OrderID}, " +

					$"ItemID: {task.ItemID}, BinID: {task.BinID}, Qty: {task.PickQuantity}"

			);

			return MapToResponseDto(task);

		}

		// ── Update ────────────────────────────────────────────────────────

		public async Task<PickTaskResponseDto?> UpdateAsync(int id, PickTaskUpdateDto dto)

		{

			var task = await _context.PickTasks

			  .Include(p => p.Order)

			  .Include(p => p.Item)

			  .Include(p => p.BinLocation).ThenInclude(b => b.Zone)

			  .Include(p => p.AssignedTo)

			  .FirstOrDefaultAsync(p => p.PickTaskID == id);

			if (task == null) return null;

			task.AssignedToUserID = dto.AssignedToUserID;

			task.Status = dto.Status;

			// ══════════════════════════════════════════════════════════════
			// AUTO LOGIC — fires when operator marks task as Picked
			// ══════════════════════════════════════════════════════════════

			if (dto.Status == PickTaskStatus.Picked)

			{

				task.CompletedAt = DateTime.UtcNow;

				// ─────────────────────────────────────────────────────────
				// AUTO STEP 1 — Decrease InventoryBalance
				// Find balance record for this item in this bin
				// ─────────────────────────────────────────────────────────

				var balance = await _context.InventoryBalances

				  .FirstOrDefaultAsync(ib =>

					ib.ItemID == task.ItemID &&

					ib.BinID == task.BinID);

				if (balance != null)

				{

					// Decrease OnHand quantity

					balance.QuantityOnHand = Math.Max(0,

					  balance.QuantityOnHand - task.PickQuantity);

					// Release the reservation since it's now physically picked

					balance.ReservedQuantity = Math.Max(0,

					  balance.ReservedQuantity - task.PickQuantity);

					balance.LastUpdated = DateTime.UtcNow;

					await _context.SaveChangesAsync();

					// Log the automatic inventory decrease

					await _auditLogService.LogAsync(

					  userId: _currentUserService.GetUserId(),

					  action: "AUTO_UPDATE",

					  resource: "InventoryBalance",

					  metadata: $"Pick completed → " +

							$"ItemID: {task.ItemID}, BinID: {task.BinID}, " +

							$"Removed: {task.PickQuantity} units, " +

							$"Remaining OnHand: {balance.QuantityOnHand}"

					);

					// ─────────────────────────────────────────────────────
					// AUTO STEP 2 — Check total stock across all bins
					// ─────────────────────────────────────────────────────

					var totalAvailableStock = await _context.InventoryBalances

					  .Where(ib => ib.ItemID == task.ItemID)

					  .SumAsync(ib => ib.QuantityOnHand - ib.ReservedQuantity);

					if (totalAvailableStock < MinimumStockThreshold)

					{

						// ─────────────────────────────────────────────────
						// AUTO STEP 3 — Create low stock notification
						// Supervisor / Planner will see this and create
						// a ReplenishmentTask manually
						// ─────────────────────────────────────────────────

						_context.Notifications.Add(new Notification

						{

							UserID = _currentUserService.GetUserId(),

							Message = $"Low Stock Alert: Item {task.Item?.SKU ?? task.ItemID.ToString()} " +

								$"has only {totalAvailableStock} units available after picking. " +

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

						  metadata: $"After picking → ItemID: {task.ItemID}, " +

								$"SKU: {task.Item?.SKU}, " +

								$"TotalAvailable: {totalAvailableStock}, " +

								$"Threshold: {MinimumStockThreshold}"

						);

					}

				}

			}

			await _context.SaveChangesAsync();

			// ── Audit Log for task update ─────────────────────────────────

			await _auditLogService.LogAsync(

			  userId: _currentUserService.GetUserId(),

			  action: "UPDATE",

			  resource: "PickTask",

			  metadata: $"ID: {id}, Status: {task.Status}, AssignedTo: {task.AssignedToUserID}"

			);

			return MapToResponseDto(task);

		}

		// ── Delete ────────────────────────────────────────────────────────

		public async Task<bool> DeleteAsync(int id)

		{

			var task = await _context.PickTasks

			  .FirstOrDefaultAsync(p => p.PickTaskID == id);

			if (task == null) return false;

			_context.PickTasks.Remove(task);

			await _context.SaveChangesAsync();

			// ── Audit Log ─────────────────────────────────────────────────

			await _auditLogService.LogAsync(

			  userId: _currentUserService.GetUserId(),

			  action: "DELETE",

			  resource: "PickTask",

			  metadata: $"ID: {id}"

			);

			return true;

		}

		// ── Mapper ────────────────────────────────────────────────────────

		private static PickTaskResponseDto MapToResponseDto(PickTask p) => new()

		{

			PickTaskID = p.PickTaskID,

			OrderID = p.OrderID,

			OrderNumber = p.Order?.OrderNumber ?? string.Empty,

			ItemID = p.ItemID,

			SKU = p.Item?.SKU ?? string.Empty,

			ItemDescription = p.Item?.Description,

			BinID = p.BinID,

			BinCode = p.BinLocation?.Code ?? string.Empty,

			ZoneName = p.BinLocation?.Zone?.Name ?? string.Empty,

			PickQuantity = p.PickQuantity,

			AssignedToUserID = p.AssignedToUserID,

			AssignedToName = p.AssignedTo?.Name,

			Status = p.Status.ToString(),

			CreatedAt = p.CreatedAt,

			CompletedAt = p.CompletedAt

		};

	}

}

