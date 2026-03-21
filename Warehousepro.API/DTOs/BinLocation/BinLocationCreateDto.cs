using System.ComponentModel.DataAnnotations;

namespace WarehousePro.API.DTOs.BinLocation
{
	public class BinLocationCreateDto
	{
		[Required]
		public int ZoneID { get; set; }

		[Required]
		[MaxLength(50)]
		public string Code { get; set; } = string.Empty;

		[Required]
		[Range(1, int.MaxValue, ErrorMessage = "Capacity must be greater than 0")]
		public int Capacity { get; set; }
	}
}