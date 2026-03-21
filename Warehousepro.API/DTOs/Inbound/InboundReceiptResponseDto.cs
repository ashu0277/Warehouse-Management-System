namespace WarehousePro.API.DTOs.Inbound

{

	public class InboundReceiptResponseDto

	{

		public int ReceiptID { get; set; }

		public string ReferenceNo { get; set; } = string.Empty;

		public string Supplier { get; set; } = string.Empty;

		public DateTime ReceiptDate { get; set; }

		public string Status { get; set; } = string.Empty;

		public DateTime CreatedAt { get; set; }

		public int TotalPutAwayTasks { get; set; }

		public int CompletedPutAwayTasks { get; set; }

	}

}
