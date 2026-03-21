using Microsoft.EntityFrameworkCore;

using WarehousePro.API.Data;

using WarehousePro.API.DTOs.Inbound;

using WarehousePro.API.Models;

using WarehousePro.API.Services.Interfaces;

namespace WarehousePro.API.Services

{

	public class InboundReceiptService : IInboundReceiptService

	{

		private readonly AppDbContext _context;

		private readonly IAuditLogService _auditLogService;

		private readonly ICurrentUserService _currentUserService;

		public InboundReceiptService(

			AppDbContext context,

			IAuditLogService auditLogService,

			ICurrentUserService currentUserService)

		{

			_context = context;

			_auditLogService = auditLogService;

			_currentUserService = currentUserService;

		}

		// ── Get All ───────────────────────────────────────────────────────

		public async Task<List<InboundReceiptResponseDto>> GetAllAsync()

		{

			return await _context.InboundReceipts

				.Where(r => !r.IsDeleted)

				.Include(r => r.PutAwayTasks)

				.Select(r => MapToResponseDto(r))

				.ToListAsync();

		}

		// ── Get By ID ─────────────────────────────────────────────────────

		public async Task<InboundReceiptResponseDto?> GetByIdAsync(int id)

		{

			var receipt = await _context.InboundReceipts

				.Include(r => r.PutAwayTasks)

				.FirstOrDefaultAsync(r => r.ReceiptID == id && !r.IsDeleted);

			return receipt == null ? null : MapToResponseDto(receipt);

		}

		// ── Create ────────────────────────────────────────────────────────

		public async Task<InboundReceiptResponseDto> CreateAsync(InboundReceiptCreateDto dto)

		{

			var receipt = new InboundReceipt

			{

				ReferenceNo = dto.ReferenceNo,

				Supplier = dto.Supplier,

				ReceiptDate = dto.ReceiptDate,

				CreatedAt = DateTime.UtcNow

			};

			_context.InboundReceipts.Add(receipt);

			await _context.SaveChangesAsync();

			// ── Audit Log ─────────────────────────────────────────────────

			await _auditLogService.LogAsync(

				userId: _currentUserService.GetUserId(),

				action: "CREATE",

				resource: "InboundReceipt",

				metadata: $"ID: {receipt.ReceiptID}, RefNo: {receipt.ReferenceNo}, Supplier: {receipt.Supplier}"

			);

			return MapToResponseDto(receipt);

		}

		// ── Update ────────────────────────────────────────────────────────

		public async Task<InboundReceiptResponseDto?> UpdateAsync(int id, InboundReceiptUpdateDto dto)

		{

			var receipt = await _context.InboundReceipts

				.Include(r => r.PutAwayTasks)

				.FirstOrDefaultAsync(r => r.ReceiptID == id && !r.IsDeleted);

			if (receipt == null) return null;

			receipt.Status = dto.Status;

			await _context.SaveChangesAsync();

			// ── Audit Log ─────────────────────────────────────────────────

			await _auditLogService.LogAsync(

				userId: _currentUserService.GetUserId(),

				action: "UPDATE",

				resource: "InboundReceipt",

				metadata: $"ID: {id}, Status: {receipt.Status}"

			);

			return MapToResponseDto(receipt);

		}

		// ── Soft Delete ───────────────────────────────────────────────────

		public async Task<bool> DeleteAsync(int id)

		{

			var receipt = await _context.InboundReceipts

				.FirstOrDefaultAsync(r => r.ReceiptID == id && !r.IsDeleted);

			if (receipt == null) return false;

			receipt.IsDeleted = true;

			await _context.SaveChangesAsync();

			// ── Audit Log ─────────────────────────────────────────────────

			await _auditLogService.LogAsync(

				userId: _currentUserService.GetUserId(),

				action: "DELETE",

				resource: "InboundReceipt",

				metadata: $"ID: {id}, RefNo: {receipt.ReferenceNo}"

			);

			return true;

		}

		// ── Mapper ────────────────────────────────────────────────────────

		private static InboundReceiptResponseDto MapToResponseDto(InboundReceipt r) => new()

		{

			ReceiptID = r.ReceiptID,

			ReferenceNo = r.ReferenceNo,

			Supplier = r.Supplier,

			ReceiptDate = r.ReceiptDate,

			Status = r.Status.ToString(),

			CreatedAt = r.CreatedAt,

			TotalPutAwayTasks = r.PutAwayTasks?.Count ?? 0,

			CompletedPutAwayTasks = r.PutAwayTasks?.Count(t =>

				t.Status == Models.Enums.PutAwayStatus.Completed) ?? 0

		};

	}

}
