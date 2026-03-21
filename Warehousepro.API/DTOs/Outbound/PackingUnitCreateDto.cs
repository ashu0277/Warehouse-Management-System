using System.ComponentModel.DataAnnotations;

namespace WarehousePro.API.DTOs.Outbound
{
	public class PackingUnitCreateDto
	{
		[Required]
		public int OrderID { get; set; }

		[Required]
		[MaxLength(100)]
		public string PackageType { get; set; } = string.Empty;

		[Required]
		[Range(0.01, double.MaxValue, ErrorMessage = "Weight must be greater than 0")]
		public decimal Weight { get; set; }
	}
}