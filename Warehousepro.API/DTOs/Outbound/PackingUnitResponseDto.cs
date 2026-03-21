namespace WarehousePro.API.DTOs.Outbound

{

	public class PackingUnitResponseDto

	{

		public int PackID { get; set; }

		public int OrderID { get; set; }

		public string OrderNumber { get; set; } = string.Empty;

		public string PackageType { get; set; } = string.Empty;

		public decimal Weight { get; set; }

		public string Status { get; set; } = string.Empty;

		public DateTime PackedAt { get; set; }

	}

}
