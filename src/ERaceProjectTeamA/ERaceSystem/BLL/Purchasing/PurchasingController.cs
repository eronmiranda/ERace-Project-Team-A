using ERaceSystem.DAL;
using ERaceSystem.VIEWMODELS;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ERaceSystem.VIEWMODELS.Purchasing;
using ERaceSystem.ENTITIES;
namespace ERaceSystem.BLL.Purchasing
{
    [DataObject]
    public class PurchasingController
    {
        #region Purchase Order
        [DataObjectMethod(DataObjectMethodType.Select, false)]
        public List<SelectionList> List_VendorNames()
        {
            using (var context = new ERaceSystemContext())
            {
                var results = from x in context.Vendors
                              orderby x.Name
                              select new SelectionList
                              {
                                  IdValue = x.VendorID,
                                  DisplayText = x.Name
                              };
                var list = results.ToList();
                list.Insert(0, new SelectionList { IdValue = 0, DisplayText = "Select Vendor..." });
                return list;
            }
        }

        [DataObjectMethod(DataObjectMethodType.Select, false)]
        public PurchaseOrder Get_PurchaseOrderByVendorId(int vendorId)
        {
            using (var context = new ERaceSystemContext())
            {
                var vendor = (from x in context.Vendors
                              where x.VendorID == vendorId
                              select x).SingleOrDefault();

                //var vend = context.Vendors.FirstOrDefault(x => x.VendorID == vendorId);

                var result = (from x in context.Orders
                              where x.VendorID == vendorId &&
                                    x.OrderNumber == null &&
                                    x.OrderDate == null
                              select new PurchaseOrder
                              {
                                  PurchaseOrderId = x.OrderID,
                                  VendorId = x.VendorID,
                                  VendorName = vendor.Name,
                                  VendorContact = vendor.Contact,
                                  VendorPhone = vendor.Phone,
                                  Comment = x.Comment,
                                  Subtotal = x.SubTotal,
                                  TaxGST = x.TaxGST
                              }).SingleOrDefault();
                if(result == null)
                {
                    result = new PurchaseOrder
                    {
                        VendorId = vendor.VendorID,
                        VendorName = vendor.Name,
                        VendorContact = vendor.Contact,
                        VendorPhone = vendor.Phone
                    };
                }
                return result;
            }
        }

        [DataObjectMethod(DataObjectMethodType.Select, false)]
        public List<PurchaseOrderItem> List_PurchaseOrderItemsByPurchaseOrderId(int purchaseOrderId, int vendorId)
        {
            using (var context = new ERaceSystemContext())
            {
                var results = from x in context.OrderDetails
                              orderby x.Product.ItemName
                              where x.OrderID == purchaseOrderId
                              select new PurchaseOrderItem
                              {
                                  PurchaseOrderId = x.OrderID,
                                  ProductId = x.ProductID,
                                  PurchaseOrderItemId = x.OrderDetailID,
                                  ProductName = x.Product.ItemName,
                                  Quantity = x.Quantity,
                                  UnitSize = (from y in x.Product.VendorCatalogs
                                              where y.VendorID == vendorId
                                              select y.OrderUnitSize).FirstOrDefault(),
                                  UnitSizeAndType = (from y in x.Product.VendorCatalogs
                                                     where y.VendorID == vendorId
                                                     select y.OrderUnitType).FirstOrDefault() == "each" ?
                                              "each" :
                                              (from y in x.Product.VendorCatalogs
                                               where y.VendorID == vendorId
                                               select y.OrderUnitSize).FirstOrDefault().ToString() + " per case",
                                  UnitCost = x.Cost,
                                  PerItemCost = x.Cost / (from y in x.Product.VendorCatalogs
                                                 where y.VendorID == vendorId
                                                 select y.OrderUnitSize).FirstOrDefault(),
                                  ExtendedCost = x.Cost * x.Quantity
                                  };
                return results.ToList();
            }
        }

        public decimal Get_ProductSellingPrice(int productId)
        {
            using (var context = new ERaceSystemContext())
            {
                var productPrice = (from x in context.Products
                                    where x.ProductID == productId
                                    select x.ItemPrice).FirstOrDefault();
                return productPrice;
            }
        }

        #region CRUD
        [DataObjectMethod(DataObjectMethodType.Insert, false)]
        public int Create_NewPurchaseOrder(PurchaseOrder purchaseOrder)
        {
            Order newPurchaseOrder = null;
            using (var context = new ERaceSystemContext())
            {
                List<Exception> brokenRules = new List<Exception>();
                newPurchaseOrder = new Order
                {
                    OrderNumber = null,
                    OrderDate = null,
                    EmployeeID = purchaseOrder.EmployeeId, //This is temporary
                    TaxGST = purchaseOrder.TaxGST,
                    SubTotal = purchaseOrder.Subtotal,
                    VendorID = purchaseOrder.VendorId,
                    Comment = purchaseOrder.Comment,
                    Closed = false
                };
                context.Orders.Add(newPurchaseOrder);
                context.SaveChanges();
            }
            return newPurchaseOrder.OrderID;
        }
        [DataObjectMethod(DataObjectMethodType.Insert, false)]
        public void Create_PurchaseOrderItem(PurchaseOrderItem purchaseOrderItem)
        {
            using (var context = new ERaceSystemContext())
            {
                List<Exception> brokenRules = new List<Exception>();

                OrderDetail newPurchaseOrderItem = new OrderDetail
                {
                    OrderID = purchaseOrderItem.PurchaseOrderId,
                    ProductID = purchaseOrderItem.ProductId,
                    Quantity = purchaseOrderItem.Quantity,
                    OrderUnitSize = purchaseOrderItem.UnitSize,
                    Cost = purchaseOrderItem.UnitCost
                };
                context.OrderDetails.Add(newPurchaseOrderItem);
                context.SaveChanges();
            }
        }

        [DataObjectMethod(DataObjectMethodType.Update, false)]
        public void Update_PurchaseOrder(PurchaseOrder purchaseOrder)
        {
            using (var context = new ERaceSystemContext())
            {
                List<Exception> brokenRules = new List<Exception>();
                Order info;
                if (purchaseOrder.PurchaseOrderDate != null)
                {
                    info = new Order()
                    {
                        OrderID = purchaseOrder.PurchaseOrderId,
                        OrderNumber = purchaseOrder.PurchaseOrderId + 1,
                        OrderDate = purchaseOrder.PurchaseOrderDate,
                        TaxGST = purchaseOrder.TaxGST,
                        SubTotal = purchaseOrder.Subtotal,
                        VendorID = purchaseOrder.VendorId,
                        EmployeeID = purchaseOrder.EmployeeId,
                        Comment = purchaseOrder.Comment,
                        Closed = true
                    };
                }
                else
                {
                    info = new Order()
                    {
                        OrderID = purchaseOrder.PurchaseOrderId,
                        OrderNumber = null,
                        OrderDate = null,
                        EmployeeID = purchaseOrder.EmployeeId,
                        TaxGST = purchaseOrder.TaxGST,
                        SubTotal = purchaseOrder.Subtotal,
                        VendorID = purchaseOrder.VendorId,
                        Comment = purchaseOrder.Comment,
                        Closed = false
                    };
                }

                context.Entry(info).State = System.Data.Entity.EntityState.Modified;
                context.SaveChanges();
            }
        }
        [DataObjectMethod(DataObjectMethodType.Update, false)]
        public void Update_PurchaseOrderItem(PurchaseOrderItem purchaseOrderItem)
        {
            using (var context = new ERaceSystemContext())
            {
                List<Exception> brokenRules = new List<Exception>();
                var existing = context.OrderDetails.Find(purchaseOrderItem.PurchaseOrderItemId);
                existing.Quantity = purchaseOrderItem.Quantity;
                existing.Cost = purchaseOrderItem.UnitCost;

                context.Entry(existing).State = System.Data.Entity.EntityState.Modified;
                context.SaveChanges();
            }
        }

        [DataObjectMethod(DataObjectMethodType.Delete, false)]
        public void Delete_Order(PurchaseOrder purchaseOrder)
        {
            DeleteOldPurchaseOrderItems(purchaseOrder.PurchaseOrderId);
            using (var context = new ERaceSystemContext())
            {
                Orders_Delete(purchaseOrder.PurchaseOrderId);
            }
        }

        public void Orders_Delete(int purchaseOrderId)
        {
            using (var context = new ERaceSystemContext())
            {
                var existing = context.Orders.Find(purchaseOrderId);
                context.Orders.Remove(existing);
                context.SaveChanges();
            }
        }


        #endregion
        #endregion

        public void SavePurchaseOrderItems(int purchaseOrderId, List<PurchaseOrderItem> newList)
        {
            using (var context = new ERaceSystemContext())
            {
                DeleteOldPurchaseOrderItems(purchaseOrderId);
                foreach (PurchaseOrderItem item in newList)
                {
                    Create_PurchaseOrderItem(item);
                }
            }
        }

        public InventoryProductItem Get_InventoryProduct(int productId)
        {
            using (var context = new ERaceSystemContext())
            {
                InventoryProductItem item = (from x in context.Products
                                             where x.ProductID == productId
                                             select new InventoryProductItem
                                             {
                                                ProductId = x.ProductID,
                                                ProductName = x.ItemName,
                                                ReOrderLevel = x.ReOrderLevel,
                                                QuantityOnHand = x.QuantityOnHand,
                                                QuantityOnOrder = x.QuantityOnOrder,
                                                UnitSize = (from y in context.VendorCatalogs
                                                            where y.ProductID == productId
                                                            select y.OrderUnitSize).FirstOrDefault(),
                                                UnitCost = (from y in context.VendorCatalogs
                                                            where y.ProductID == productId
                                                            select y.OrderUnitCost).FirstOrDefault(),
                                                UnitSizeAndType = (from y in context.VendorCatalogs
                                                             where y.ProductID == productId
                                                             select y.OrderUnitType).FirstOrDefault() == "each" ? 
                                                             "each" :
                                                             (from y in context.VendorCatalogs
                                                              where y.ProductID == productId
                                                              select y.OrderUnitSize).FirstOrDefault().ToString() + " per case",
                                             }).SingleOrDefault();
                return item;
            }
        }

        public void DeleteOldPurchaseOrderItems(int purchaseOrderId)
        {
            using (var context = new ERaceSystemContext())
            {
                List<OrderDetail> oldList = (from x in context.OrderDetails
                                             where x.OrderID == purchaseOrderId
                                             select x).ToList();
                foreach (OrderDetail item in oldList)
                {
                    context.OrderDetails.Remove(item);
                }
                context.SaveChanges();
            }  
        }

        #region Inventory
        //This is for repeater
        [DataObjectMethod(DataObjectMethodType.Select, false)]
        public List<InventoryList> List_InventoryByVendorId(int vendorId)
        {
            using (var context = new ERaceSystemContext())
            {
                var results = from x in context.VendorCatalogs
                              where x.VendorID == vendorId
                              group x by x.Product.Category.Description into categoryGroup
                              select new InventoryList
                              {
                                  CategoryName = categoryGroup.Key,
                                  Products = from y in categoryGroup
                                             from x in y.Product.VendorCatalogs
                                             where x.VendorID == vendorId
                                             select new InventoryProductItem
                                             {
                                                 ProductId = y.Product.ProductID,
                                                 ProductName = y.Product.ItemName,
                                                 ReOrderLevel = y.Product.ReOrderLevel,
                                                 QuantityOnHand = y.Product.QuantityOnHand,
                                                 QuantityOnOrder = y.Product.QuantityOnOrder,
                                                 UnitSize = x.OrderUnitSize,
                                                 UnitCost = x.OrderUnitCost,
                                                 UnitSizeAndType = x.OrderUnitType == "each" ? "each" : x.OrderUnitSize.ToString() + " per case"
                                             }
                              };
                return results.ToList();
            }
        }
        #endregion

        public string GetEmployeeName(int? employeeId)
        {
            using (var context = new ERaceSystemContext())
            {
                return context.Employees.Where(x => x.EmployeeID == employeeId).Select(x => x.FirstName + " " + x.LastName).SingleOrDefault();
            }
        }
    }
}
