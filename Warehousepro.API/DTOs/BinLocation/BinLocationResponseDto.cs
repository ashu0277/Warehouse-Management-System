namespace WarehousePro.API.DTOs.BinLocation

{

	public class BinLocationResponseDto

	{

		public int BinID { get; set; }

		public int ZoneID { get; set; }

		public string ZoneName { get; set; } = string.Empty;

		public string WarehouseName { get; set; } = string.Empty;

		public string Code { get; set; } = string.Empty;

		public int Capacity { get; set; }

		public string Status { get; set; } = string.Empty;

	}

}
