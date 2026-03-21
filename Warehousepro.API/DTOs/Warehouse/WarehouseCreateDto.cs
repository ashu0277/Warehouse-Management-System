using System.ComponentModel.DataAnnotations;

namespace WarehousePro.API.DTOs.Warehouse
{
	public class WarehouseCreateDto
	{
		[Required]
		[MaxLength(150)]
		public string Name { get; set; } = string.Empty;

		[Required]
		[MaxLength(250)]
		public string Location { get; set; } = string.Empty;
	}
}