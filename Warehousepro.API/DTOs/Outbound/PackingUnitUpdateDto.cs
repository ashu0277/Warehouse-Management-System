using System.ComponentModel.DataAnnotations;
using WarehousePro.API.Models.Enums;

namespace WarehousePro.API.DTOs.Outbound
{
	public class PackingUnitUpdateDto
	{
		[Required]
		public PackingStatus Status { get; set; }
	}
}