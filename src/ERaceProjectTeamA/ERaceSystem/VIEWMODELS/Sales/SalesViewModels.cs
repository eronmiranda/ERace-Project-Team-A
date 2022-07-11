using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERaceSystem.VIEWMODELS.Sales
{
	public class CategorySelectionListItem
	{
		public int CategoryId { get; set; }
		public string CategoryText { get; set; }
	}
	public class ProductSelectionListItem
	{
		public int ProductId { get; set; }
		public string ProductText { get; set; }
	}
	public class PurchaseListItem
	{
		public int ProductId { get; set; }
		public int InvoiceId { get; set; }
		public string ProductDescription { get; set; }
		public int Quantity { get; set; }
		public decimal Price { get; set; }

		public decimal Amount { get; set; }
		public double RestockCharge { get; set; }
		public int CategoryId { get; set; }
	}
	public class PurchaseInvoice
	{
		public int PurchaseInvoiceId { get; set; }
		public double SubTotal { get; set; }
		public double GST { get; set; }
		public double Total { get; set; }
		public double EmployeeId { get; set; }
		public IEnumerable<PurchaseListItem> PurchaseItems { get; set; }
	}
	public class RefundInvoice
	{
		public int RefundInvoiceId { get; set; }
		public double SubTotal { get; set; }
		public double GST { get; set; }
		public double Total { get; set; }
		public IEnumerable<RefundListItem> RefundItems { get; set; }


	}
	public class RefundListItem
	{
		public int ProductId { get; set; }
		public int InvoiceId { get; set; }
		public string ProductDescription { get; set; }
		public int Quantity { get; set; }
		public double Price { get; set; }
		public double RestockCharge { get; set; }
		public string RefundReason { get; set; }
	}

}
