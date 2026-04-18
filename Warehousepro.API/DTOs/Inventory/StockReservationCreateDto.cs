using System.ComponentModel.DataAnnotations;

using WarehousePro.API.Models.Enums;

namespace WarehousePro.API.DTOs.Inventory

{

	public class StockReservationCreateDto

	{

		[Required]

		public int ItemID { get; set; }

		[Required]

		public int BinID { get; set; }

		[Required]

		public ReservationReferenceType ReferenceType { get; set; }

		[Required]

		public int ReferenceID { get; set; }

		[Required]

		[Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1")]

		public int Quantity { get; set; }

	}

}

