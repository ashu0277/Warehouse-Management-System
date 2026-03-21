using System.ComponentModel.DataAnnotations;

namespace WarehousePro.API.DTOs.Item
{
	public class ItemCreateDto
	{
		[Required]
		[MaxLength(100)]
		public string SKU { get; set; } = string.Empty;

		[MaxLength(250)]
		public string? Description { get; set; }

		[Required]
		[MaxLength(50)]
		public string UnitOfMeasure { get; set; } = string.Empty;
	}
}