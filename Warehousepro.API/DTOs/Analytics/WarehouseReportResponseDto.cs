
using WarehousePro.API.DTOs.Analytics;

namespace WarehousePro.API.DTOs.Analytics

{

	public class WarehouseReportResponseDto

	{

		public int ReportID { get; set; }

		public string Scope { get; set; } = string.Empty;

		public DateTime GeneratedDate { get; set; }

		public int GeneratedByUserID { get; set; }

		public string GeneratedByName { get; set; } = string.Empty;

		public ReportMetricsDto? Metrics { get; set; }

	}

}
