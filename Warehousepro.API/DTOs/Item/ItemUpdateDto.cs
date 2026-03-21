using System.ComponentModel.DataAnnotations;

using WarehousePro.API.Models.Enums;

namespace WarehousePro.API.DTOs.Item

{

	public class ItemUpdateDto

	{

		[MaxLength(250)]

		public string? Description { get; set; }

		[Required]

		[MaxLength(50)]

		public string UnitOfMeasure { get; set; } = string.Empty;

		[Required]

		public ItemStatus Status { get; set; }

	}

}
