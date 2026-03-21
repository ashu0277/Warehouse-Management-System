namespace WarehousePro.API.DTOs.Replenishment
{
	public class ReplenishmentTaskResponseDto
	{
		public int ReplenishID { get; set; }
		public int ItemID { get; set; }
		public string SKU { get; set; } = string.Empty;
		public string? ItemDescription { get; set; }
		public int FromBinID { get; set; }
		public string FromBinCode { get; set; } = string.Empty;
		public string FromZoneName { get; set; } = string.Empty;
		public int ToBinID { get; set; }
		public string ToBinCode { get; set; } = string.Empty;
		public string ToZoneName { get; set; } = string.Empty;
		public int Quantity { get; set; }
		public string Status { get; set; } = string.Empty;
		public DateTime CreatedAt { get; set; }
		public DateTime? CompletedAt { get; set; }
	}
}