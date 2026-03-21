using System.ComponentModel.DataAnnotations;

using WarehousePro.API.Models.Enums;

namespace WarehousePro.API.DTOs.Warehouse

{

	public class WarehouseUpdateDto

	{

		[Required]

		[MaxLength(150)]

		public string Name { get; set; } = string.Empty;

		[Required]

		[MaxLength(250)]

		public string Location { get; set; } = string.Empty;

		[Required]

		public WarehouseStatus Status { get; set; }

	}

}
