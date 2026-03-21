namespace WarehousePro.API.DTOs.Replenishment
{
	public class SlottingRuleResponseDto
	{
		public int RuleID { get; set; }
		public string Criterion { get; set; } = string.Empty;
		public int Priority { get; set; }
		public string? Description { get; set; }
		public string Status { get; set; } = string.Empty;
		public DateTime CreatedAt { get; set; }
	}
}