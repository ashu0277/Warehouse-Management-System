using Microsoft.EntityFrameworkCore;

using WarehousePro.API.Data;

using WarehousePro.API.DTOs.Replenishment;

using WarehousePro.API.Models;

using WarehousePro.API.Services.Interfaces;

namespace WarehousePro.API.Services

{

	public class SlottingRuleService : ISlottingRuleService

	{

		private readonly AppDbContext _context;

		private readonly IAuditLogService _auditLogService;

		private readonly ICurrentUserService _currentUserService;

		public SlottingRuleService(

			AppDbContext context,

			IAuditLogService auditLogService,

			ICurrentUserService currentUserService)

		{

			_context = context;

			_auditLogService = auditLogService;

			_currentUserService = currentUserService;

		}

		public async Task<List<SlottingRuleResponseDto>> GetAllAsync()

		{

			return await _context.SlottingRules

				.Select(r => MapToResponseDto(r))

				.ToListAsync();

		}

		public async Task<SlottingRuleResponseDto?> GetByIdAsync(int id)

		{

			var rule = await _context.SlottingRules

				.FirstOrDefaultAsync(r => r.RuleID == id);

			return rule == null ? null : MapToResponseDto(rule);

		}

		public async Task<SlottingRuleResponseDto> CreateAsync(SlottingRuleCreateDto dto)

		{

			var rule = new SlottingRule

			{

				Criterion = dto.Criterion,

				Priority = dto.Priority,

				Description = dto.Description,

				CreatedAt = DateTime.UtcNow

			};

			_context.SlottingRules.Add(rule);

			await _context.SaveChangesAsync();

			// ── Audit Log ─────────────────────────────────────────────────

			await _auditLogService.LogAsync(

				userId: _currentUserService.GetUserId(),

				action: "CREATE",

				resource: "SlottingRule",

				metadata: $"ID: {rule.RuleID}, Criterion: {rule.Criterion}, Priority: {rule.Priority}"

			);

			return MapToResponseDto(rule);

		}

		public async Task<SlottingRuleResponseDto?> UpdateAsync(int id, SlottingRuleUpdateDto dto)

		{

			var rule = await _context.SlottingRules

				.FirstOrDefaultAsync(r => r.RuleID == id);

			if (rule == null) return null;

			rule.Criterion = dto.Criterion;

			rule.Priority = dto.Priority;

			rule.Description = dto.Description;

			rule.Status = dto.Status;

			await _context.SaveChangesAsync();

			// ── Audit Log ─────────────────────────────────────────────────

			await _auditLogService.LogAsync(

				userId: _currentUserService.GetUserId(),

				action: "UPDATE",

				resource: "SlottingRule",

				metadata: $"ID: {id}, Criterion: {rule.Criterion}, Priority: {rule.Priority}, Status: {rule.Status}"

			);

			return MapToResponseDto(rule);

		}

		public async Task<bool> DeleteAsync(int id)

		{

			var rule = await _context.SlottingRules

				.FirstOrDefaultAsync(r => r.RuleID == id);

			if (rule == null) return false;

			_context.SlottingRules.Remove(rule);

			await _context.SaveChangesAsync();

			// ── Audit Log ─────────────────────────────────────────────────

			await _auditLogService.LogAsync(

				userId: _currentUserService.GetUserId(),

				action: "DELETE",

				resource: "SlottingRule",

				metadata: $"ID: {id}"

			);

			return true;

		}

		private static SlottingRuleResponseDto MapToResponseDto(SlottingRule r) => new()

		{

			RuleID = r.RuleID,

			Criterion = r.Criterion.ToString(),

			Priority = r.Priority,

			Description = r.Description,

			Status = r.Status.ToString(),

			CreatedAt = r.CreatedAt

		};

	}

}
