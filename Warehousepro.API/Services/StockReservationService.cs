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

				.Select(sr => MapToResponseDto(sr))

				.ToListAsync();

		}

		// ── Get By Item ID ────────────────────────────────────────────────

		public async Task<List<StockReservationResponseDto>> GetByItemIdAsync(int itemId)

		{

			return await _context.StockReservations

				.Where(sr => sr.ItemID == itemId)

				.Include(sr => sr.Item)

				.Select(sr => MapToResponseDto(sr))

				.ToListAsync();

		}

		// ── Get By ID ─────────────────────────────────────────────────────

		public async Task<StockReservationResponseDto?> GetByIdAsync(int id)

		{

			var sr = await _context.StockReservations

				.Include(sr => sr.Item)

				.FirstOrDefaultAsync(sr => sr.ReservationID == id);

			return sr == null ? null : MapToResponseDto(sr);

		}

		// ── Create ────────────────────────────────────────────────────────

		public async Task<StockReservationResponseDto> CreateAsync(StockReservationCreateDto dto)

		{

			var reservation = new StockReservation

			{

				ItemID = dto.ItemID,

				ReferenceType = dto.ReferenceType,

				ReferenceID = dto.ReferenceID,

				Quantity = dto.Quantity,

				CreatedAt = DateTime.UtcNow

			};

			_context.StockReservations.Add(reservation);

			await _context.SaveChangesAsync();

			await _context.Entry(reservation).Reference(sr => sr.Item).LoadAsync();

			// ── Audit Log ─────────────────────────────────────────────────

			await _auditLogService.LogAsync(

				userId: _currentUserService.GetUserId(),

				action: "CREATE",

				resource: "StockReservation",

				metadata: $"ID: {reservation.ReservationID}, ItemID: {reservation.ItemID}, Qty: {reservation.Quantity}, RefType: {reservation.ReferenceType}"

			);

			return MapToResponseDto(reservation);

		}

		// ── Delete ────────────────────────────────────────────────────────

		public async Task<bool> DeleteAsync(int id)

		{

			var reservation = await _context.StockReservations

				.FirstOrDefaultAsync(sr => sr.ReservationID == id);

			if (reservation == null) return false;

			_context.StockReservations.Remove(reservation);

			await _context.SaveChangesAsync();

			// ── Audit Log ─────────────────────────────────────────────────

			await _auditLogService.LogAsync(

				userId: _currentUserService.GetUserId(),

				action: "DELETE",

				resource: "StockReservation",

				metadata: $"ID: {id}, ItemID: {reservation.ItemID}"

			);

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
