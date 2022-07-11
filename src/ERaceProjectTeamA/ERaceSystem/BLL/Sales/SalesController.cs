using ERaceSystem.DAL;
using ERaceSystem.VIEWMODELS;
using ERaceSystem.VIEWMODELS.Sales;
using ERaceSystem.ENTITIES;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERaceSystem.BLL.Sales
{
    [DataObject]
    public class SalesController
    {
        [DataObjectMethod(DataObjectMethodType.Select, false)]
        public List<CategorySelectionListItem> Category_List()
        {
            using (var context = new ERaceSystemContext())
            {
                var results = from x in context.Categories
                              select new CategorySelectionListItem
                              {
                                  CategoryId = x.CategoryID,
                                  CategoryText = x.Description
                              };
                return results.ToList();
            }
        }
        [DataObjectMethod(DataObjectMethodType.Select, false)]
        public List<ProductSelectionListItem> Products_List(int categoryID)
        {
            using (var context = new ERaceSystemContext())
            {
                var results = from x in context.Products
                              where x.CategoryID == categoryID 
                              select new ProductSelectionListItem
                              {
                                  ProductId = x.ProductID,
                                  ProductText = x.ItemName
                                  
                              };
                return results.ToList();
            }
        }
        [DataObjectMethod(DataObjectMethodType.Select, false)]
        public PurchaseListItem AddItem(int productID)
        {
            if (productID < 1)
            {
                throw new Exception("Please select and Item to add...");
            }
            else 
            {
                using (var context = new ERaceSystemContext())
                {
                    var results = (from x in context.Products
                                   where x.ProductID == productID
                                   select new PurchaseListItem
                                   {
                                       ProductId = x.ProductID,
                                       ProductDescription = x.ItemName,
                                       Price = x.ItemPrice,
                                       CategoryId = x.CategoryID
                                   }).SingleOrDefault();
                    return results;
                }
            }
            
            
        }
        [DataObjectMethod(DataObjectMethodType.Select, false)]
        public int GetEmployeeID(string name)
        {
            string firstInitial = name.Substring(name.Length - 1);
            string lastName = name.Substring(0, name.Length - 1);
            using (var context = new ERaceSystemContext())
            {
                int results = (from x in context.Employees
                               where x.FirstName.Substring(0,1) == firstInitial 
                               && x.LastName.Equals(lastName) 
                               select x.EmployeeID).FirstOrDefault();

                return results;
            }
                
        }
        [DataObjectMethod(DataObjectMethodType.Select, false)]
        public int SaveInvoice(PurchaseInvoice purchaseInvoice)
        {
            int id = 0;
            List<PurchaseListItem> purchaseList = null;
            purchaseList = purchaseInvoice.PurchaseItems.ToList();                        
            using (var context = new ERaceSystemContext())
            {
                Invoice newInvoice = new Invoice()
                {
                    EmployeeID = (int)purchaseInvoice.EmployeeId,
                    SubTotal = (decimal)purchaseInvoice.SubTotal,
                    GST = (decimal)purchaseInvoice.GST,
                    //Total = (decimal)purchaseInvoice.Total,
                    InvoiceDate = System.DateTime.Now,
                    //InvoiceDetails = (ICollection<InvoiceDetail>)purchaseInvoice.PurchaseItems
                };
                for (int i = 0; i < purchaseInvoice.PurchaseItems.Count(); i++)
                {
                    InvoiceDetail invoiceDetail = new InvoiceDetail();
                    invoiceDetail.Price = purchaseList[i].Price;
                    invoiceDetail.Quantity = purchaseList[i].Quantity;
                    invoiceDetail.ProductID = purchaseList[i].ProductId;
                    newInvoice.InvoiceDetails.Add(invoiceDetail);
                }
                Invoice savedInvoice = context.Invoices.Add(newInvoice);
                //id = context.Invoices.Add(newInvoice);
                int invoiceId = savedInvoice.InvoiceID;                
                id = context.SaveChanges();
            }
            return id;
        }
    }
}
