using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERaceSystem.VIEWMODELS.Receiving
{
	public class VendorSelectionListItem
	{
		public int VendorID { get; set; }
		public string Name { get; set; }
		public string Address { get; set; }
		public string Phone { get; set; }
		public string Contact { get; set; }
	}
	public class ProductListItem
	{
		public int ProductID { get; set; }
		public string ItemName { get; set; }
		public int QuantityOrdered { get; set; }
		public int OrderedUnits { get; set; }
		public int QuantityOutstanding { get; set; }
		public int ReceivedUnits { get; set; }
		public int RejectedUnits { get; set; }
		public string RejectedReason { get; set; }
		public int SalvagedItems { get; set; }
	}
	public class UnorderedItemsListItems
	{
		public int ItemID { get; set; }
		public string ItemName { get; set; }
		public int VendorProductID { get; set; }
		public int Quantity { get; set; }
	}

	public class ReceiveOrders
	{
		public int ReceiveOrderID { get; set; }
		public string OrderID { get; set; }
		public int ReceiveDate { get; set; }
	}

}
