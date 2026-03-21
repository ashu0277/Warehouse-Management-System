using System.ComponentModel.DataAnnotations;
using WarehousePro.API.Models.Enums;

namespace WarehousePro.API.DTOs.Replenishment
{
	public class ReplenishmentTaskUpdateDto
	{
		[Required]
		public ReplenishmentStatus Status { get; set; }
	}
}