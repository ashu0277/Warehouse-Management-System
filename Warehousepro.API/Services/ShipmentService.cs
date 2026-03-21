using Microsoft.EntityFrameworkCore;

using WarehousePro.API.Data;

using WarehousePro.API.DTOs.Outbound;

using WarehousePro.API.Models;

using WarehousePro.API.Services.Interfaces;

namespace WarehousePro.API.Services

{

	public class ShipmentService : IShipmentService

	{

		private readonly AppDbContext _context;

		private readonly IAuditLogService _auditLogService;

		private readonly ICurrentUserService _currentUserService;

		public ShipmentService(

			AppDbContext context,

			IAuditLogService auditLogService,

			ICurrentUserService currentUserService)

		{

			_context = context;

			_auditLogService = auditLogService;

			_currentUserService = currentUserService;

		}

		public async Task<List<ShipmentResponseDto>> GetAllAsync()

		{

			return await _context.Shipments

				.Include(s => s.Order)

				.Select(s => MapToResponseDto(s))

				.ToListAsync();

		}

		public async Task<List<ShipmentResponseDto>> GetByOrderIdAsync(int orderId)

		{

			return await _context.Shipments

				.Where(s => s.OrderID == orderId)

				.Include(s => s.Order)

				.Select(s => MapToResponseDto(s))

				.ToListAsync();

		}

		public async Task<ShipmentResponseDto?> GetByIdAsync(int id)

		{

			var shipment = await _context.Shipments

				.Include(s => s.Order)

				.FirstOrDefaultAsync(s => s.ShipmentID == id);

			return shipment == null ? null : MapToResponseDto(shipment);

		}

		public async Task<ShipmentResponseDto> CreateAsync(ShipmentCreateDto dto)

		{

			var shipment = new Shipment

			{

				OrderID = dto.OrderID,

				Carrier = dto.Carrier,

				DispatchDate = dto.DispatchDate,

				DeliveryDate = dto.DeliveryDate,

				CreatedAt = DateTime.UtcNow

			};

			_context.Shipments.Add(shipment);

			await _context.SaveChangesAsync();

			await _context.Entry(shipment).Reference(s => s.Order).LoadAsync();

			// ── Audit Log ─────────────────────────────────────────────────

			await _auditLogService.LogAsync(

				userId: _currentUserService.GetUserId(),

				action: "CREATE",

				resource: "Shipment",

				metadata: $"ID: {shipment.ShipmentID}, OrderID: {shipment.OrderID}, Carrier: {shipment.Carrier}"

			);

			return MapToResponseDto(shipment);

		}

		public async Task<ShipmentResponseDto?> UpdateAsync(int id, ShipmentUpdateDto dto)

		{

			var shipment = await _context.Shipments

				.Include(s => s.Order)

				.FirstOrDefaultAsync(s => s.ShipmentID == id);

			if (shipment == null) return null;

			shipment.Status = dto.Status;

			shipment.DispatchDate = dto.DispatchDate;

			shipment.DeliveryDate = dto.DeliveryDate;

			await _context.SaveChangesAsync();

			// ── Audit Log ─────────────────────────────────────────────────

			await _auditLogService.LogAsync(

				userId: _currentUserService.GetUserId(),

				action: "UPDATE",

				resource: "Shipment",

				metadata: $"ID: {id}, Status: {shipment.Status}"

			);

			return MapToResponseDto(shipment);

		}

		public async Task<bool> DeleteAsync(int id)

		{

			var shipment = await _context.Shipments

				.FirstOrDefaultAsync(s => s.ShipmentID == id);

			if (shipment == null) return false;

			_context.Shipments.Remove(shipment);

			await _context.SaveChangesAsync();

			// ── Audit Log ─────────────────────────────────────────────────

			await _auditLogService.LogAsync(

				userId: _currentUserService.GetUserId(),

				action: "DELETE",

				resource: "Shipment",

				metadata: $"ID: {id}"

			);

			return true;

		}

		private static ShipmentResponseDto MapToResponseDto(Shipment s) => new()

		{

			ShipmentID = s.ShipmentID,

			OrderID = s.OrderID,

			OrderNumber = s.Order?.OrderNumber ?? string.Empty,

			CustomerName = s.Order?.CustomerName ?? string.Empty,

			Carrier = s.Carrier,

			DispatchDate = s.DispatchDate,

			DeliveryDate = s.DeliveryDate,

			Status = s.Status.ToString(),

			CreatedAt = s.CreatedAt

		};

	}

}
