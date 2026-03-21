using System.ComponentModel.DataAnnotations;
using WarehousePro.API.Models.Enums;

namespace WarehousePro.API.DTOs.Replenishment
{
	public class SlottingRuleUpdateDto
	{
		[Required]
		public SlottingCriterion Criterion { get; set; }

		[Required]
		[Range(1, int.MaxValue)]
		public int Priority { get; set; }

		[MaxLength(500)]
		public string? Description { get; set; }

		[Required]
		public SlottingRuleStatus Status { get; set; }
	}
}