using System.ComponentModel.DataAnnotations;

using WarehousePro.API.Models.Enums;

namespace WarehousePro.API.DTOs.Zone

{

	public class ZoneCreateDto

	{

		[Required]

		public int WarehouseID { get; set; }

		[Required]

		[MaxLength(100)]

		public string Name { get; set; } = string.Empty;

		[Required]

		public ZoneType ZoneType { get; set; }

	}

}
