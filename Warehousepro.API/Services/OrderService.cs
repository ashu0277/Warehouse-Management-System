using Microsoft.EntityFrameworkCore;

using WarehousePro.API.Data;

using WarehousePro.API.DTOs.Order;

using WarehousePro.API.Models;

using WarehousePro.API.Services.Interfaces;

namespace WarehousePro.API.Services

{

	public class OrderService : IOrderService

	{

		private readonly AppDbContext _context;

		private readonly IAuditLogService _auditLogService;

		private readonly ICurrentUserService _currentUserService;

		public OrderService(

			AppDbContext context,

			IAuditLogService auditLogService,

			ICurrentUserService currentUserService)

		{

			_context = context;

			_auditLogService = auditLogService;

			_currentUserService = currentUserService;

		}

		// ── Get All ───────────────────────────────────────────────────────

		public async Task<List<OrderResponseDto>> GetAllAsync()

		{

			return await _context.Orders

				.Where(o => !o.IsDeleted)

				.Include(o => o.PickTasks)

				.Include(o => o.Shipments)

				.Select(o => MapToResponseDto(o))

				.ToListAsync();

		}

		// ── Get By ID ─────────────────────────────────────────────────────

		public async Task<OrderResponseDto?> GetByIdAsync(int id)

		{

			var order = await _context.Orders

				.Include(o => o.PickTasks)

				.Include(o => o.Shipments)

				.FirstOrDefaultAsync(o => o.OrderID == id && !o.IsDeleted);

			return order == null ? null : MapToResponseDto(order);

		}

		// ── Get By Order Number ───────────────────────────────────────────

		public async Task<OrderResponseDto?> GetByOrderNumberAsync(string orderNumber)

		{

			var order = await _context.Orders

				.Include(o => o.PickTasks)

				.Include(o => o.Shipments)

				.FirstOrDefaultAsync(o => o.OrderNumber == orderNumber && !o.IsDeleted);

			return order == null ? null : MapToResponseDto(order);

		}

		// ── Create ────────────────────────────────────────────────────────

		public async Task<OrderResponseDto> CreateAsync(OrderCreateDto dto)

		{

			var order = new Order

			{

				OrderNumber = dto.OrderNumber,

				CustomerName = dto.CustomerName,

				DeliveryAddress = dto.DeliveryAddress,

				OrderDate = dto.OrderDate,

				RequiredDate = dto.RequiredDate,

				CreatedAt = DateTime.UtcNow

			};

			_context.Orders.Add(order);

			await _context.SaveChangesAsync();

			// ── Audit Log ─────────────────────────────────────────────────

			await _auditLogService.LogAsync(

				userId: _currentUserService.GetUserId(),

				action: "CREATE",

				resource: "Order",

				metadata: $"ID: {order.OrderID}, OrderNo: {order.OrderNumber}, Customer: {order.CustomerName}"

			);

			return MapToResponseDto(order);

		}

		// ── Update ────────────────────────────────────────────────────────

		public async Task<OrderResponseDto?> UpdateAsync(int id, OrderUpdateDto dto)

		{

			var order = await _context.Orders

				.Include(o => o.PickTasks)

				.Include(o => o.Shipments)

				.FirstOrDefaultAsync(o => o.OrderID == id && !o.IsDeleted);

			if (order == null) return null;

			order.DeliveryAddress = dto.DeliveryAddress;

			order.RequiredDate = dto.RequiredDate;

			order.Status = dto.Status;

			await _context.SaveChangesAsync();

			// ── Audit Log ─────────────────────────────────────────────────

			await _auditLogService.LogAsync(

				userId: _currentUserService.GetUserId(),

				action: "UPDATE",

				resource: "Order",

				metadata: $"ID: {id}, Status: {order.Status}"

			);

			return MapToResponseDto(order);

		}

		// ── Soft Delete ───────────────────────────────────────────────────

		public async Task<bool> DeleteAsync(int id)

		{

			var order = await _context.Orders

				.FirstOrDefaultAsync(o => o.OrderID == id && !o.IsDeleted);

			if (order == null) return false;

			order.IsDeleted = true;

			await _context.SaveChangesAsync();

			// ── Audit Log ─────────────────────────────────────────────────

			await _auditLogService.LogAsync(

				userId: _currentUserService.GetUserId(),

				action: "DELETE",

				resource: "Order",

				metadata: $"ID: {id}, OrderNo: {order.OrderNumber}"

			);

			return true;

		}

		// ── Mapper ────────────────────────────────────────────────────────

		private static OrderResponseDto MapToResponseDto(Order o) => new()

		{

			OrderID = o.OrderID,

			OrderNumber = o.OrderNumber,

			CustomerName = o.CustomerName,

			DeliveryAddress = o.DeliveryAddress,

			OrderDate = o.OrderDate,

			RequiredDate = o.RequiredDate,

			Status = o.Status.ToString(),

			CreatedAt = o.CreatedAt,

			TotalPickTasks = o.PickTasks?.Count ?? 0,

			CompletedPickTasks = o.PickTasks?.Count(t =>

				t.Status == Models.Enums.PickTaskStatus.Picked) ?? 0,

			TotalShipments = o.Shipments?.Count ?? 0

		};

	}

}
