using System.ComponentModel.DataAnnotations;
using WarehousePro.API.Models.Enums;

namespace WarehousePro.API.DTOs.Analytics
{
	public class WarehouseReportCreateDto
	{
		[Required]
		public ReportScope Scope { get; set; }

		public int? WarehouseID { get; set; }
		public int? ZoneID { get; set; }
		public DateTime? PeriodFrom { get; set; }
		public DateTime? PeriodTo { get; set; }
	}
}