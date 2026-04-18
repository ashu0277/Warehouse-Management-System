using System.ComponentModel.DataAnnotations;

using System.ComponentModel.DataAnnotations.Schema;

using WarehousePro.API.Models.Enums;

namespace WarehousePro.API.Models

{

	/// <summary>

	/// Reserves a quantity of an Item for either an Order or a Replenishment.

	/// Many StockReservations belong to One Item.

	/// ReferenceID points to OrderID or ReplenishID depending on ReferenceType.

	/// </summary>

	[Table("StockReservations")]

	public class StockReservation

	{

		[Key]

		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]

		public int ReservationID { get; set; }

		// FK → Item

		[Required]

		public int ItemID { get; set; }

		// FK → BinLocation (which bin is this reserved from)

		[Required]

		public int BinID { get; set; }

		/// <summary>

		/// Order = reserved for customer order

		/// Replenishment = reserved for internal move

		/// </summary>

		public ReservationReferenceType ReferenceType { get; set; }

		/// <summary>

		/// Holds the OrderID or ReplenishID value

		/// depending on ReferenceType above.

		/// </summary>

		public int ReferenceID { get; set; }

		[Required]

		public int Quantity { get; set; }

		public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

		// ── Navigation Property ────────────────────────────────────────────

		// Many StockReservations → One Item

		[ForeignKey("ItemID")]

		public Item Item { get; set; } = null!;

		// Many StockReservations → One BinLocation

		[ForeignKey("BinID")]

		public BinLocation BinLocation { get; set; } = null!;

	}

}

