using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERaceSystem.VIEWMODELS.Purchasing
{
	#region Purchase Order
	public class SelectionList
	{
		public int IdValue { get; set; }
		public string DisplayText { get; set; }
	}

	public class PurchaseOrder
	{
		// IDs.
		public int PurchaseOrderId { get; set; }
		public int VendorId { get; set; }
		public int EmployeeId { get; set; }

		// Nulls if order not placed yet.
		public int? PurchaseOrderNumber { get; set; }
		public DateTime? PurchaseOrderDate { get; set; }

		// Display.
		public string VendorName { get; set; }
		public string VendorContact { get; set; }
		public string VendorPhone { get; set; }
		public string Comment { get; set; }
		public decimal Subtotal { get; set; }
		public decimal TaxGST { get; set; }
		public decimal Total { get; set; }
	}

	public class PurchaseOrderItem
	{
		// IDs (not visible to user)
		public int PurchaseOrderItemId { get; set; }
		public int PurchaseOrderId { get; set; }
		public int ProductId { get; set; }

		// Display.
		public string ProductName { get; set; }
		public int Quantity { get; set; }
		public int UnitSize { get; set; }
		public string UnitSizeAndType { get; set; }
		public decimal UnitCost { get; set; }

		// Calculated Display.
		public decimal PerItemCost { get; set; }
		public decimal ExtendedCost { get; set; }
	}
	#endregion

	#region Inventory
	public class InventoryList
	{
		// Display.
		public string CategoryName { get; set; }
		public IEnumerable<InventoryProductItem> Products { get; set; }
	}
	public class InventoryProductItem
    {
        // IDs.
        public int ProductId { get; set; }

		// Display.
        public string ProductName { get; set; }
        public int ReOrderLevel { get; set; }
        public int QuantityOnHand { get; set; }
        public int QuantityOnOrder { get; set; }
        public int UnitSize { get; set; }
		public string UnitSizeAndType { get; set; }
		public decimal UnitCost { get; set; }
	}
	#endregion
}
