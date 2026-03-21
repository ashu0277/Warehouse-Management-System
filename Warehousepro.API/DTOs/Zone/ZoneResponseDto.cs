namespace WarehousePro.API.DTOs.Zone

{

	public class ZoneResponseDto

	{

		public int ZoneID { get; set; }

		public int WarehouseID { get; set; }

		public string WarehouseName { get; set; } = string.Empty;

		public string Name { get; set; } = string.Empty;

		public string ZoneType { get; set; } = string.Empty;

		public int TotalBins { get; set; }

	}

}
