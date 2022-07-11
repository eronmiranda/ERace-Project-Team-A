using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using ERaceSystem.VIEWMODELS.Sales;
using ERaceSystem.BLL.Sales;
using System.Globalization;


namespace WebApp.Pages.Sales
{
    public partial class Purchases : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request.IsAuthenticated && User.IsInRole("Clerk"))
            {
                LabelTopError.Text = "Login Successful as a Customer";
                
            }
            else
            {
                Response.Redirect("~/Account/Login.aspx");
            }
        }
        protected void CategoryDropDown_SelectedIndexChanged(object sender, EventArgs e)
        {
            //this method is called when a category is seleceted from the Category Drop down
            // Retrieves a list of products with the categoryId of the selected Category
            // Populates the ProductsDropDown with the list.
            ProductsDropDown.Items.Clear();
            var test = CategoryDropDown.SelectedValue;
            LabelTopError.Text = test.ToString();
            List<ProductSelectionListItem> productList = new List<ProductSelectionListItem>();
            SalesController controller = new SalesController();
            productList = controller.Products_List(int.Parse(test));
            ProductSelectionListItem placeholder = new ProductSelectionListItem() { ProductId = 0, ProductText = "Select a Product" };
            productList.Insert(0, placeholder);            
            ProductsDropDown.DataSource = productList;
            ProductsDropDown.DataBind();

        }
        protected void AddButtonClicked(object sender, EventArgs e)
        {
            
            int productid = int.Parse(ProductsDropDown.SelectedValue);
            SalesController controller = new SalesController();
            PurchaseListItem item = new PurchaseListItem();
            List<PurchaseListItem> cartList = new List<PurchaseListItem>();


            MessageUserControl.TryRun(() => {
                item = controller.AddItem(int.Parse(ProductsDropDown.SelectedValue));
            }, "", "Item Added");
            if (item.ProductDescription == null)
            {

            }
            else 
            {
                item.Quantity = int.Parse(QuantityOfItemSelectedTextBox.Text);
                item.Amount = item.Quantity * item.Price;

                LabelTopError.Text = item.ProductDescription + " " + item.Price + " " + item.ProductId;

                cartList = GetItemsFromGridView();

                int loopCount = 0;
                int cartListInitialCount = cartList.Count;
                if (cartList.Count == 0)
                {
                    cartList.Add(item);
                }
                else
                {
                    foreach (var listItem in cartList)
                    {
                        if (listItem.ProductId == item.ProductId)
                        {
                            listItem.Quantity = listItem.Quantity + item.Quantity;
                            listItem.Amount = listItem.Quantity * listItem.Price;
                            loopCount = 1;
                            break;
                        }
                        else { loopCount = 0; }
                    }
                }
                if (cartListInitialCount > 0 && loopCount == 0) { cartList.Add(item); }
                BindCartGridView(cartList);
            }                        
        }
        protected void BindCartGridView(List<PurchaseListItem> cartList) 
        {
            decimal subtotal = 0;
            decimal tax = 0;
            decimal total = 0;
            decimal taxableSubtotal = 0;

            CartItems.DataSource = cartList.ToList();
            CartItems.DataBind();
            

            //bind the Subtotal Tax and Total
            foreach (var item in cartList)
            {
                subtotal += (decimal)item.Amount;                
            }
            tax = subtotal * (decimal)0.05;
            total = subtotal + tax;

            var subtotalStr = subtotal.ToString("C2");
            var taxStr = tax.ToString("C2");
            var totalStr = total.ToString("C2");



            subtotal = decimal.Round(subtotal, 2, MidpointRounding.AwayFromZero);
            Subtotal.Text = subtotalStr;
            Tax.Text = taxStr;
            Total.Text = totalStr;

        }
        protected void CartItems_RowCommand(object sender, GridViewCommandEventArgs e) 
        {
            int rowIndex = Convert.ToInt32(e.CommandArgument);
            List<PurchaseListItem> cartList = GetItemsFromGridView();
            if (e.CommandName == "RefreshCart")
            {
                BindCartGridView(cartList);
            }
            else if (e.CommandName == "RemoveFromCart") 
            {
                cartList.Remove(cartList[rowIndex]);
                
                BindCartGridView(cartList);
            }

        }
        List<PurchaseListItem> GetItemsFromGridView() 
        {            
            //var test = CartItems.Rows.Count;
            List<PurchaseListItem> cartList = new List<PurchaseListItem>();
            foreach (GridViewRow row in CartItems.Rows)
            {
                var Item = new PurchaseListItem();

                Item.ProductDescription = row.Cells[0].Text;
                Item.Price = row.FindLabel("ItemPrice").Text.ToDecimal();
                Item.Quantity = row.FindTextBox("QuantityOfItemSelectedGridViewTextBox").Text.ToInt();
                Item.Amount = Item.Price * Item.Quantity;
                Item.ProductId = row.FindHiddenField("ProductID").Value.ToInt();
                
                cartList.Add(Item);
            }
            return cartList;
        }
        protected void PurchaseButton_OnClick(Object sender, EventArgs e) 
        {
            LabelTopError.Text = "Purchase test...";
            //get total start values
            decimal subtotalInit = Subtotal.Text.ToString().Remove(0, 1).ToDecimal();
            
            //update amount fields - get list and rebind
            List<PurchaseListItem> cartList = GetItemsFromGridView();
            BindCartGridView(cartList);
            //compare new subtotal
            decimal subtotalAfterUpdate = Subtotal.Text.Remove(0,1).ToDecimal();
            if (subtotalInit != subtotalAfterUpdate)
            {
                //Purchase button was pressed without refreshing after changing a quantity
                PageUtility.MessageBox(this, "Quantities of Items have changed since last refresh. Updated New Total:    " + Total.Text);
            }
            //save the purchase  
            SalesController controller = new SalesController();
            //get EmployeeID
            int employeeId = controller.GetEmployeeID(User.Identity.Name);
            //get subtotal
            decimal subtotal = Subtotal.Text.Remove(0, 1).ToDecimal();
            //get GST
            decimal tax = Tax.Text.Remove(0, 1).ToDecimal();
            //get Total
            decimal total = Total.Text.Remove(0, 1).ToDecimal();
            //get items list of items. will be turned into ICollection<InvoiceDetail> in controller
            List<PurchaseListItem> purchaseCartList = GetItemsFromGridView();
            //Create PurchaseInvoice()
            PurchaseInvoice newInvoice = new PurchaseInvoice();
            newInvoice.EmployeeId = employeeId;
            newInvoice.SubTotal = (double)subtotal;
            newInvoice.GST = (double)tax;
            newInvoice.Total = (double)total;
            newInvoice.PurchaseItems = purchaseCartList;
            int saveReturn = controller.SaveInvoice(newInvoice);
            AddItemButton.Enabled = false;
            MakePurchase.Enabled = false;
            CartItems.Enabled = false;
            




            //update the item quantities in inventory

            //Invoice results remain on screen. Show Invoice Number
            //Items cannot be added or edited after the payment in processed.


        }
        protected void ClearButton_OnClick(Object sender, EventArgs e) 
        {
            //clear cart and prepare for new sale.
        }
        public static class PageUtility //copied from the internet. https://stackoverflow.com/questions/16370465/how-to-display-an-alert-box-from-c-sharp-in-asp-net
        {
            public static void MessageBox(System.Web.UI.Page page, string strMsg)
            {
                //+ character added after strMsg "')"
                ScriptManager.RegisterClientScriptBlock(page, page.GetType(), "alertMessage", "alert('" + strMsg + "')", true);

            }
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