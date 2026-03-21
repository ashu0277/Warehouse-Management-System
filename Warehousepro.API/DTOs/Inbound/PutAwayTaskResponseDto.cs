namespace WarehousePro.API.DTOs.Inbound

{

	public class PutAwayTaskResponseDto

	{

		public int TaskID { get; set; }

		public int ReceiptID { get; set; }

		public string ReferenceNo { get; set; } = string.Empty;

		public int ItemID { get; set; }

		public string SKU { get; set; } = string.Empty;

		public string? ItemDescription { get; set; }

		public int Quantity { get; set; }

		public int TargetBinID { get; set; }

		public string TargetBinCode { get; set; } = string.Empty;

		public int? AssignedToUserID { get; set; }

		public string? AssignedToName { get; set; }

		public string Status { get; set; } = string.Empty;

		public DateTime CreatedAt { get; set; }

		public DateTime? CompletedAt { get; set; }

	}

}
