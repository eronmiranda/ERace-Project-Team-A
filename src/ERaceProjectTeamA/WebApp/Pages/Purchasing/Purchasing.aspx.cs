using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using ERaceSystem.VIEWMODELS.Purchasing;
using ERaceSystem.BLL.Purchasing;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;


namespace WebApp.Pages.Purchasing
{
    public partial class Purchasing : System.Web.UI.Page
    {
        // Used for passing employee ID globally.
        static ApplicationUserManager userManager;
        static Models.ApplicationUser user;

        // Checks user account role for authentication.
        protected void Page_Load(object sender, EventArgs e)
        {
            //if (Request.IsAuthenticated)
            //{
            //    MessageUserControl.ShowInfo("Login Successful");

            //    userManager = Context.GetOwinContext().GetUserManager<ApplicationUserManager>();

            //    user = userManager.FindByName(User.Identity.Name);
            //    var userAllowed = HttpContext.Current.User.IsInRole("Director");

            //    if (!userAllowed)
            //    {
            //        Response.Redirect("Default.aspx");
            //    }

            //    EmployeeNameLbl.Text = new PurchasingController().GetEmployeeName(user.EmployeeId);
            //}
            //else
            //{
            //    Response.Redirect("~/Account/Login.aspx");
            //}
        }
        protected void SelectBtn_Click(object sender, EventArgs e)
        {
            int vendorId = VendorDropDownList.SelectedValue.ToInt();
            MessageUserControl.TryRun(() =>
            {
                if (vendorId == 0)
                {
                    ClearPurchaseOrderItemGridView();
                    throw new Exception("Please select a vendor.");
                }
                else
                {
                    VendorDropDownList.Enabled = false;
                    SelectBtn.Enabled = false;
                    var sysmgr = new PurchasingController();
                    var selectedPurchaseOrder = sysmgr.Get_PurchaseOrderByVendorId(vendorId);
                    VendorNameLbl.Text = selectedPurchaseOrder.VendorName;
                    VendorContactLbl.Text = selectedPurchaseOrder.VendorContact;
                    VendorPhoneLbl.Text = selectedPurchaseOrder.VendorPhone;

                    SubtotalTextBox.Text = String.Format("{0:C2}", selectedPurchaseOrder.Subtotal);
                    TaxTextBox.Text = String.Format("{0:C2}", selectedPurchaseOrder.TaxGST);
                    selectedPurchaseOrder.Total = selectedPurchaseOrder.Subtotal + selectedPurchaseOrder.TaxGST;
                    TotalTextBox.Text = String.Format("{0:C2}", selectedPurchaseOrder.Total);

                    if (selectedPurchaseOrder.PurchaseOrderId == 0)
                    {
                        // Display empty listview
                        ClearPurchaseOrderItemGridView();
                    }
                    else
                    {
                        CommentsTextArea.Text = selectedPurchaseOrder.Comment;
                        List<PurchaseOrderItem> info = sysmgr.List_PurchaseOrderItemsByPurchaseOrderId(selectedPurchaseOrder.PurchaseOrderId, selectedPurchaseOrder.VendorId);

                        UpdatePurchaseOrderItemsGridview(info);
                    }
                }
            });
        }

        // Clears the items on the item list gridview.
        public void ClearPurchaseOrderItemGridView()
        {
            List<PurchaseOrderItem> info = new List<PurchaseOrderItem>();
            UpdatePurchaseOrderItemsGridview(info);
        }
        protected void PlaceOrderBtn_Click(object sender, EventArgs e)
        {
            int vendorId = VendorDropDownList.SelectedValue.ToInt();
            MessageUserControl.TryRun(() =>
            {
                if (VendorDropDownList.Enabled == true)
                {
                    throw new Exception("Please select a vendor before saving any progress");
                }
                else
                {
                    var sysmgr = new PurchasingController();
                    var selectedPurchaseOrder = sysmgr.Get_PurchaseOrderByVendorId(vendorId);
                    selectedPurchaseOrder.PurchaseOrderDate = DateTime.Now;
                    selectedPurchaseOrder.EmployeeId = (int)user.EmployeeId;

                    if (selectedPurchaseOrder.PurchaseOrderId == 0)
                    {
                        throw new Exception("Save your progress first before placing an order");
                    }
                    else
                    {
                        sysmgr.Update_PurchaseOrder(selectedPurchaseOrder);
                        MessageUserControl.ShowInfo("", "ORDER PLACED: Purchase Order ID - " + selectedPurchaseOrder.PurchaseOrderId + " have been placed");
                        CancelBtn_Click(sender, e);
                    }

                }
            });
        }

        // Inserting and updating on Order and Order Details entity.
        // Updates current display.
        protected void SaveBtn_Click(object sender, EventArgs e)
        {
            int vendorId = VendorDropDownList.SelectedValue.ToInt();
            int purchaseOrderId;
            var sysmgr = new PurchasingController();
            MessageUserControl.TryRun(() =>
            {
                if (VendorDropDownList.Enabled == true)
                {
                    throw new Exception("Please select a vendor before saving any progress");
                }
                else
                {
                    List<PurchaseOrderItem> purchaseOrderItems = GetPurchaseOrderItemsFromGridView();
                    decimal subtotal = 0,
                            taxGST = 0,
                            total = 0;
                    foreach (PurchaseOrderItem item in purchaseOrderItems)
                    {
                        subtotal += item.ExtendedCost;
                    }
                    taxGST = subtotal * 0.05m;
                    total = subtotal + taxGST;

                    var selectedPurchaseOrder = sysmgr.Get_PurchaseOrderByVendorId(vendorId);
                    selectedPurchaseOrder.Comment = CommentsTextArea.Text;
                    selectedPurchaseOrder.Subtotal = subtotal;
                    selectedPurchaseOrder.TaxGST = taxGST;
                    selectedPurchaseOrder.EmployeeId = (int)user.EmployeeId;

                    if (selectedPurchaseOrder.PurchaseOrderId == 0)
                    {
                        purchaseOrderId = sysmgr.Create_NewPurchaseOrder(selectedPurchaseOrder);
                        purchaseOrderItems = GetPurchaseOrderItemsFromGridView();
                        sysmgr.SavePurchaseOrderItems(purchaseOrderId, purchaseOrderItems);
                        MessageUserControl.ShowInfo("", "SUCCESS: Purchase Order ID: " + purchaseOrderId + " has been created");
                    }
                    else
                    {
                        purchaseOrderId = sysmgr.Get_PurchaseOrderByVendorId(vendorId).PurchaseOrderId;
                        sysmgr.Update_PurchaseOrder(selectedPurchaseOrder);
                        sysmgr.SavePurchaseOrderItems(purchaseOrderId, purchaseOrderItems);
                        MessageUserControl.ShowInfo("", "SUCCESS: Purchase Order ID: " + purchaseOrderId + " has been updated");
                    }

                    // Updates display if it's succesful.
                    SubtotalTextBox.Text = String.Format("{0:C2}", subtotal);
                    TaxTextBox.Text = String.Format("{0:C2}", taxGST);
                    TotalTextBox.Text = String.Format("{0:C2}", total);
                }
            });
        }

        protected void DeleteBtn_Click(object sender, EventArgs e)
        {
            var sysmgr = new PurchasingController();
            int vendorId = VendorDropDownList.SelectedValue.ToInt();
            MessageUserControl.TryRun(() =>
            {
                if (VendorDropDownList.Enabled == true)
                {
                    ClearPurchaseOrderItemGridView();
                    throw new Exception("Please select a vendor.");
                }
                else
                {
                    var selectedPurchaseOrder = sysmgr.Get_PurchaseOrderByVendorId(vendorId);

                    if (selectedPurchaseOrder.PurchaseOrderId == 0)
                    {
                        MessageUserControl.ShowInfo("", "WARNING: There is no existing Purchase Order to delete.");
                    }
                    else
                    {
                        sysmgr.Delete_Order(selectedPurchaseOrder);
                        MessageUserControl.ShowInfo("", "SUCCESS: Purchase Order ID: " + selectedPurchaseOrder.PurchaseOrderId + " has been deleted");
                        CancelBtn_Click(sender, e);
                    }
                }
            });
        }
        protected void CancelBtn_Click(object sender, EventArgs e)
        {
            VendorDropDownList.ClearSelection();
            SubtotalTextBox.Text = String.Format("{0:C2}", 0);
            TaxTextBox.Text = String.Format("{0:C2}", 0);
            TotalTextBox.Text = String.Format("{0:C2}", 0);
            VendorDropDownList.Enabled = true;
            SelectBtn.Enabled = true;
            VendorNameLbl.Text = "Vendor Name";
            VendorContactLbl.Text = "Contact";
            VendorPhoneLbl.Text = "Phone";
            CommentsTextArea.Text = "";

            ClearPurchaseOrderItemGridView();
        }
        protected void PurchaseOrderItemsList_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            int rowIndex = Convert.ToInt32(e.CommandArgument);
            List<PurchaseOrderItem> purchaseOrderItems = GetPurchaseOrderItemsFromGridView();
            PurchaseOrderItem purchaseOrderItem = purchaseOrderItems[rowIndex];

            if (e.CommandName == "DeleteFromMyPurchaseOrder")
            {
                MessageUserControl.ShowInfo("", "Removed product: " + purchaseOrderItem.ProductName);
                purchaseOrderItems.Remove(purchaseOrderItem);

                UpdatePurchaseOrderItemsGridview(purchaseOrderItems);
                e.Handled = true;
            }
            else if (e.CommandName == "SyncCostsToPurchaseOrderItem")
            {
                // I didn't use DataBind() here to retain the warningLbl visible. It's confusing for the user if the label hides everytime user clicks the sync button
                var sysmgr = new PurchasingController();
                decimal itemSellingPrice = sysmgr.Get_ProductSellingPrice(PurchaseOrderItemGridView.Rows[rowIndex].FindLabel("ProductIdLbl").Text.ToInt());
                string perItemCost = String.Format("{0:C2}", (purchaseOrderItem.UnitCost / purchaseOrderItem.UnitSize));
                string extendedCost = String.Format("{0:C2}", (purchaseOrderItem.UnitCost * purchaseOrderItem.Quantity));
                PurchaseOrderItemGridView.Rows[rowIndex].FindLabel("PerItemCostLbl").Text = perItemCost;
                PurchaseOrderItemGridView.Rows[rowIndex].FindLabel("ExtendedCostLbl").Text = extendedCost;

                if (PurchaseOrderItemGridView.Rows[rowIndex].FindLabel("PerItemCostLbl").Text.Remove(0, 1).ToDecimal() > itemSellingPrice)
                {
                    PurchaseOrderItemGridView.Rows[rowIndex].FindLabel("WarningLbl").Visible = true;
                }
                else
                {
                    PurchaseOrderItemGridView.Rows[rowIndex].FindLabel("WarningLbl").Visible = false;
                }
            }
        }
        protected void InventoryProductsList_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            int productId = Convert.ToInt32(e.CommandArgument);
            var sysmgr = new PurchasingController();
            List<PurchaseOrderItem> purchaseOrderItems = GetPurchaseOrderItemsFromGridView();
            List<InventoryProductItem> inventoryProductItems = GetInventoryProductItemsFromGridView();
            var inventoryProductItem = sysmgr.Get_InventoryProduct(productId);
            var selectedPurchaseOrder = sysmgr.Get_PurchaseOrderByVendorId(VendorDropDownList.SelectedValue.ToInt());
            //selectedPurchaseOrder.EmployeeId = (int)user.EmployeeId;
            int purchaseOrderId = 0;

            if (e.CommandName == "AddToMyPurchaseOrder")
            {
                MessageUserControl.TryRun(() =>
                {
                    if (selectedPurchaseOrder.PurchaseOrderId == 0)
                    {
                        purchaseOrderId = sysmgr.Create_NewPurchaseOrder(selectedPurchaseOrder);
                    }
                    else
                    {
                        purchaseOrderId = selectedPurchaseOrder.PurchaseOrderId;
                    }

                    var newPurchaseOrderItem = new PurchaseOrderItem
                    {
                        ProductId = inventoryProductItem.ProductId,
                        PurchaseOrderId = purchaseOrderId,
                        ProductName = inventoryProductItem.ProductName,
                        Quantity = 1,
                        UnitSize = inventoryProductItem.UnitSize,
                        UnitSizeAndType = inventoryProductItem.UnitSizeAndType,
                        UnitCost = inventoryProductItem.UnitCost,
                        PerItemCost = inventoryProductItem.UnitCost / inventoryProductItem.UnitSize,
                        ExtendedCost = inventoryProductItem.UnitCost
                    };
                    bool containsItem = purchaseOrderItems.Any(item => item.ProductId == newPurchaseOrderItem.ProductId);
                    if (containsItem)
                    {
                        throw new Exception("Product is already in the Purchase Order list");
                    }
                    else
                    {
                        purchaseOrderItems.Add(newPurchaseOrderItem);
                        MessageUserControl.ShowInfo("", "Added product to Purchase Order List: " + inventoryProductItem.ProductName);
                        UpdatePurchaseOrderItemsGridview(purchaseOrderItems);
                        e.Handled = true;
                    }
                });
            }
        }

        List<PurchaseOrderItem> GetPurchaseOrderItemsFromGridView()
        {
            var list = new List<PurchaseOrderItem>();
            foreach (GridViewRow row in PurchaseOrderItemGridView.Rows)
            {
                var item = new PurchaseOrderItem
                {
                    PurchaseOrderId = row.FindLabel("PurchaseOrderIdLbl").Text.ToInt(),
                    PurchaseOrderItemId = row.FindLabel("PurchaseOrderItemIdLbl").Text.ToInt(),
                    ProductId = row.FindLabel("ProductIdLbl").Text.ToInt(),
                    ProductName = row.FindLabel("ProductNameLbl").Text,
                    Quantity = row.FindTextBox("QuantityTextBox").Text.ToInt(),
                    UnitSize = row.FindLabel("UnitSizeLbl").Text.ToInt(),
                    UnitSizeAndType = row.FindLabel("UnitSizeAndTypeLbl").Text,
                    UnitCost = row.FindTextBox("UnitCostTextBox").Text.ToDecimal(),
                    PerItemCost = row.FindLabel("PerItemCostLbl").Text.Remove(0, 1).ToDecimal(),
                    ExtendedCost = row.FindLabel("ExtendedCostLbl").Text.Remove(0, 1).ToDecimal()
                };
                list.Add(item);
            }
            return list;
        }
        List<InventoryProductItem> GetInventoryProductItemsFromGridView()
        {
            var list = new List<InventoryProductItem>();
            foreach (RepeaterItem repeaterItem in InventoryRepeater.Items)
            {
                GridView gridView = (GridView)repeaterItem.FindControl("InventoryProductsGridView");
                foreach (GridViewRow row in gridView.Rows)
                {
                    var item = new InventoryProductItem
                    {
                        ProductId = row.FindLabel("ProductIdLbl").Text.ToInt(),
                        ProductName = row.FindLabel("InventoryProductLbl").Text,
                        ReOrderLevel = row.FindLabel("ReOrderLbl").Text.ToInt(),
                        QuantityOnHand = row.FindLabel("InStockLbl").Text.ToInt(),
                        QuantityOnOrder = row.FindLabel("OnOrderLbl").Text.ToInt(),
                        UnitSize = row.FindLabel("UnitSizeLbl").Text.ToInt(),
                        UnitSizeAndType = row.FindLabel("SizeLbl").Text,
                        UnitCost = row.FindLabel("UnitCostLbl").Text.ToDecimal()
                    };
                    list.Add(item);
                }
            }
            return list;
        }

        // Binding of Order detail grid view
        public void UpdatePurchaseOrderItemsGridview(List<PurchaseOrderItem> purchaseOrderItems)
        {
            PurchaseOrderItemGridView.DataSource = purchaseOrderItems;
            PurchaseOrderItemGridView.DataBind();
        }
    }
    #region Web Extensions
    public static class WebControlExtensions
    {
        public static Label FindLabel(this Control self, string id)
            => self.FindControl(id) as Label;
        public static TextBox FindTextBox(this Control self, string id)
            => self.FindControl(id) as TextBox;
        public static HiddenField FindHiddenField(this Control self, string id)
            => self.FindControl(id) as HiddenField;
        public static CheckBox FindCheckBox(this Control self, string id)
            => self.FindControl(id) as CheckBox;
        public static int ToInt(this string self) => int.Parse(self);
        public static decimal ToDecimal(this string self) => decimal.Parse(self);
    }
    #endregion
}