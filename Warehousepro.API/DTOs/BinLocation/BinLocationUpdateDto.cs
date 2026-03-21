using System.ComponentModel.DataAnnotations;
using WarehousePro.API.Models.Enums;

namespace WarehousePro.API.DTOs.BinLocation
{
	public class BinLocationUpdateDto
	{
		[Required]
		[MaxLength(50)]
		public string Code { get; set; } = string.Empty;

		[Required]
		[Range(1, int.MaxValue)]
		public int Capacity { get; set; }

		[Required]
		public BinStatus Status { get; set; }
	}
}