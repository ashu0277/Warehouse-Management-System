using Microsoft.EntityFrameworkCore;
using WarehousePro.API.Data;
using WarehousePro.API.DTOs.Inventory;
using WarehousePro.API.Models;
using WarehousePro.API.Services.Interfaces;

namespace WarehousePro.API.Services
{
	public class StockReservationService : IStockReservationService
	{
		private readonly AppDbContext _context;
		private readonly IAuditLogService _auditLogService;
		private readonly ICurrentUserService _currentUserService;

		public StockReservationService(
		  AppDbContext context,
		  IAuditLogService auditLogService,
		  ICurrentUserService currentUserService)
		{
			_context = context;
			_auditLogService = auditLogService;
			_currentUserService = currentUserService;
		}

		// ── Get All ───────────────────────────────────────────────────────
		public async Task<List<StockReservationResponseDto>> GetAllAsync()
		{
			return await _context.StockReservations
			  .Include(sr => sr.Item)
			  .Include(sr => sr.BinLocation)
			  .Select(sr => MapToResponseDto(sr))
			  .ToListAsync();
		}

		// ── Get By Item ID ────────────────────────────────────────────────
		public async Task<List<StockReservationResponseDto>> GetByItemIdAsync(int itemId)
		{
			return await _context.StockReservations
			  .Where(sr => sr.ItemID == itemId)
			  .Include(sr => sr.Item)
			  .Include(sr => sr.BinLocation)
			  .Select(sr => MapToResponseDto(sr))
			  .ToListAsync();
		}

		// ── Get By ID ─────────────────────────────────────────────────────
		public async Task<StockReservationResponseDto?> GetByIdAsync(int id)
		{
			var sr = await _context.StockReservations
			  .Include(sr => sr.Item)
			  .Include(sr => sr.BinLocation)
			  .FirstOrDefaultAsync(sr => sr.ReservationID == id);

			return sr == null ? null : MapToResponseDto(sr);
		}

		// ── Create ────────────────────────────────────────────────────────
		public async Task<StockReservationResponseDto> CreateAsync(StockReservationCreateDto dto)
		{
			// ─────────────────────────────────────────────────────────────────
			// VALIDATION: Check if enough stock is available
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

			if (availableQuantity < dto.Quantity)
			{
				throw new InvalidOperationException(
				  $"Insufficient stock. Available: {availableQuantity}, Requested: {dto.Quantity}");
			}

			// ─────────────────────────────────────────────────────────────────
			// Create the reservation record
			// ─────────────────────────────────────────────────────────────────
			var reservation = new StockReservation
			{
				ItemID = dto.ItemID,
				BinID = dto.BinID,
				ReferenceType = dto.ReferenceType,
				ReferenceID = dto.ReferenceID,
				Quantity = dto.Quantity,
				CreatedAt = DateTime.UtcNow
			};

			_context.StockReservations.Add(reservation);
			await _context.SaveChangesAsync();

			// ─────────────────────────────────────────────────────────────────
			// CRITICAL FIX: Update InventoryBalance.ReservedQuantity
			// ─────────────────────────────────────────────────────────────────
			balance.ReservedQuantity += dto.Quantity;
			balance.LastUpdated = DateTime.UtcNow;
			await _context.SaveChangesAsync();

			await _context.Entry(reservation).Reference(sr => sr.Item).LoadAsync();
			await _context.Entry(reservation).Reference(sr => sr.BinLocation).LoadAsync();

			// ── Audit Log ─────────────────────────────────────────────────
			await _auditLogService.LogAsync(
			  userId: _currentUserService.GetUserId(),
			  action: "CREATE",
			  resource: "StockReservation",
			  metadata: $"ID: {reservation.ReservationID}, ItemID: {reservation.ItemID}, BinID: {reservation.BinID}, Qty: {reservation.Quantity}, RefType: {reservation.ReferenceType}"
			);

			await _auditLogService.LogAsync(
			  userId: _currentUserService.GetUserId(),
			  action: "AUTO_UPDATE",
			  resource: "InventoryBalance",
			  metadata: $"Reservation created → ItemID: {dto.ItemID}, BinID: {dto.BinID}, Reserved: +{dto.Quantity}, NewReserved: {balance.ReservedQuantity}"
			);

			return MapToResponseDto(reservation);
		}

		// ── Delete ────────────────────────────────────────────────────────
		public async Task<bool> DeleteAsync(int id)
		{
			var reservation = await _context.StockReservations
			  .FirstOrDefaultAsync(sr => sr.ReservationID == id);

			if (reservation == null) return false;

			// ─────────────────────────────────────────────────────────────────
			// CRITICAL FIX: Release ReservedQuantity when deleting reservation
			// ─────────────────────────────────────────────────────────────────
			var balance = await _context.InventoryBalances
			  .FirstOrDefaultAsync(ib =>
				ib.ItemID == reservation.ItemID &&
				ib.BinID == reservation.BinID);

			if (balance != null)
			{
				balance.ReservedQuantity = Math.Max(0, balance.ReservedQuantity - reservation.Quantity);
				balance.LastUpdated = DateTime.UtcNow;
			}

			_context.StockReservations.Remove(reservation);
			await _context.SaveChangesAsync();

			// ── Audit Log ─────────────────────────────────────────────────
			await _auditLogService.LogAsync(
			  userId: _currentUserService.GetUserId(),
			  action: "DELETE",
			  resource: "StockReservation",
			  metadata: $"ID: {id}, ItemID: {reservation.ItemID}, BinID: {reservation.BinID}"
			);

			if (balance != null)
			{
				await _auditLogService.LogAsync(
				  userId: _currentUserService.GetUserId(),
				  action: "AUTO_UPDATE",
				  resource: "InventoryBalance",
				  metadata: $"Reservation deleted → ItemID: {reservation.ItemID}, BinID: {reservation.BinID}, Released: {reservation.Quantity}, NewReserved: {balance.ReservedQuantity}"
				);
			}

			return true;
		}

		// ── Mapper ────────────────────────────────────────────────────────
		private static StockReservationResponseDto MapToResponseDto(StockReservation sr) => new()
		{
			ReservationID = sr.ReservationID,
			ItemID = sr.ItemID,
			SKU = sr.Item?.SKU ?? string.Empty,
			ReferenceType = sr.ReferenceType.ToString(),
			ReferenceID = sr.ReferenceID,
			Quantity = sr.Quantity,
			CreatedAt = sr.CreatedAt
		};
	}
}

