using System.ComponentModel.DataAnnotations;
using WarehousePro.API.Models.Enums;

namespace WarehousePro.API.DTOs.Zone
{
	public class ZoneUpdateDto
	{
		[Required]
		[MaxLength(100)]
		public string Name { get; set; } = string.Empty;

		[Required]
		public ZoneType ZoneType { get; set; }

		[Required]
		public bool IsDeleted { get; set; }
	}
}