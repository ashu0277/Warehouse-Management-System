using Microsoft.EntityFrameworkCore;

using System.Text.Json;

using WarehousePro.API.Data;

using WarehousePro.API.DTOs.Analytics;

using WarehousePro.API.Models;

using WarehousePro.API.Services.Interfaces;

namespace WarehousePro.API.Services

{

	public class WarehouseReportService : IWarehouseReportService

	{

		private readonly AppDbContext _context;

		private readonly IAuditLogService _auditLogService;

		public WarehouseReportService(

			AppDbContext context,

			IAuditLogService auditLogService)

		{

			_context = context;

			_auditLogService = auditLogService;

		}

		public async Task<List<WarehouseReportResponseDto>> GetAllAsync()

		{

			return await _context.WarehouseReports

				.Include(r => r.GeneratedBy)

				.Select(r => MapToResponseDto(r))

				.ToListAsync();

		}

		public async Task<WarehouseReportResponseDto?> GetByIdAsync(int id)

		{

			var report = await _context.WarehouseReports

				.Include(r => r.GeneratedBy)

				.FirstOrDefaultAsync(r => r.ReportID == id);

			return report == null ? null : MapToResponseDto(report);

		}

		public async Task<WarehouseReportResponseDto> CreateAsync(

			WarehouseReportCreateDto dto, int generatedByUserId)

		{

			var metrics = await CalculateMetricsAsync(dto);

			var report = new WarehouseReport

			{

				Scope = dto.Scope,

				Metrics = JsonSerializer.Serialize(metrics),

				GeneratedDate = DateTime.UtcNow,

				GeneratedByUserID = generatedByUserId

			};

			_context.WarehouseReports.Add(report);

			await _context.SaveChangesAsync();

			await _context.Entry(report).Reference(r => r.GeneratedBy).LoadAsync();

			// ── Audit Log ─────────────────────────────────────────────────

			// Note: We use generatedByUserId directly here because

			// ICurrentUserService is not injected in this service —

			// the user ID is already passed as a parameter from the controller

			await _auditLogService.LogAsync(

				userId: generatedByUserId,

				action: "CREATE",

				resource: "WarehouseReport",

				metadata: $"ID: {report.ReportID}, Scope: {report.Scope}"

			);

			return MapToResponseDto(report);

		}

		public async Task<bool> DeleteAsync(int id)

		{

			var report = await _context.WarehouseReports

				.FirstOrDefaultAsync(r => r.ReportID == id);

			if (report == null) return false;

			_context.WarehouseReports.Remove(report);

			await _context.SaveChangesAsync();

			// ── Audit Log ─────────────────────────────────────────────────

			await _auditLogService.LogAsync(

				userId: report.GeneratedByUserID,

				action: "DELETE",

				resource: "WarehouseReport",

				metadata: $"ID: {id}"

			);

			return true;

		}

		// ── KPI Calculator ────────────────────────────────────────────────

		private async Task<ReportMetricsDto> CalculateMetricsAsync(WarehouseReportCreateDto dto)

		{

			var periodFrom = dto.PeriodFrom ?? DateTime.UtcNow.AddDays(-30);

			var periodTo = dto.PeriodTo ?? DateTime.UtcNow;

			var totalPicks = await _context.PickTasks

				.CountAsync(p => p.CreatedAt >= periodFrom && p.CreatedAt <= periodTo);

			var completedPicks = await _context.PickTasks

				.CountAsync(p => p.CreatedAt >= periodFrom
&& p.CreatedAt <= periodTo
&& p.Status == Models.Enums.PickTaskStatus.Picked);

			var pickRate = totalPicks > 0 ? (double)completedPicks / totalPicks * 100 : 0;

			var accuracyPct = totalPicks > 0 ? (double)completedPicks / totalPicks * 100 : 100;

			return new ReportMetricsDto

			{

				PickRate = Math.Round(pickRate, 2),

				AccuracyPct = Math.Round(accuracyPct, 2),

				OrderCycleTime = 0

			};

		}

		// ── Mapper ────────────────────────────────────────────────────────

		private static WarehouseReportResponseDto MapToResponseDto(WarehouseReport r) => new()

		{

			ReportID = r.ReportID,

			Scope = r.Scope.ToString(),

			GeneratedDate = r.GeneratedDate,

			GeneratedByUserID = r.GeneratedByUserID,

			GeneratedByName = r.GeneratedBy?.Name ?? string.Empty,

			Metrics = r.Metrics != null

				? JsonSerializer.Deserialize<ReportMetricsDto>(r.Metrics)

				: null

		};

	}

}
