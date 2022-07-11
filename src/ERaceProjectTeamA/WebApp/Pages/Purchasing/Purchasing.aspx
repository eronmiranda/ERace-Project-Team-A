<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Purchasing.aspx.cs" Inherits="WebApp.Pages.Purchasing.Purchasing" %>

<%@ Register Src="~/UserControls/MessageUserControl.ascx" TagPrefix="uc1" TagName="MessageUserControl" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="container-xl" style="max-width:1440px;">
        <%--Message User Control--%>
        <div class="row">
            <div class="col-md-12">
                <uc1:MessageUserControl runat="server" ID="MessageUserControl" />
            </div>
        </div>
        <asp:Label runat="server" ID="EmployeeNameLbl" ></asp:Label>
        <div class="row">
            <%--Purchase Order Column--%>
            <div class="col-auto" style="margin-right:5rem;">
                <%--Purchase Order Header--%>
                <div class="row">
                    <h3>Purchase Order</h3>
                </div>
                <br />
                <%--DropDown and Buttons row--%>
                <div class="row">
                    <div class="col-sm-auto" style="padding:0px;">
                        <asp:Label ID="VendorLbl" runat="server" Text="Vendor"></asp:Label> &nbsp;&nbsp;
                    </div>
                    <%--Vendor DropDown--%>
                    <div class="col-md-auto" style="padding:0px">
                        <asp:ObjectDataSource ID="VendorOBS" runat="server" OldValuesParameterFormatString="original_{0}" SelectMethod="List_VendorNames" TypeName="ERaceSystem.BLL.Purchasing.PurchasingController"></asp:ObjectDataSource>
                        <asp:DropDownList ID="VendorDropDownList" runat="server" DataSourceID="VendorOBS" DataTextField="DisplayText" DataValueField="IdValue">
                            <asp:ListItem Value="0">Select...</asp:ListItem>
                        </asp:DropDownList>
                    </div>
                    <%--Buttons--%>
                    <div class="col no-gutters d-flex" style="padding-right:0px">
                        <div class="col-auto">
                            <asp:Button ID="SelectBtn" runat="server" Text="Select" OnClick="SelectBtn_Click" />&nbsp;&nbsp;
                        </div>
                        <div class="col-auto">
                            <asp:Button ID="PlaceOrderBtn" runat="server" Text="Place Order" OnClick="PlaceOrderBtn_Click" />&nbsp;&nbsp;
                        </div>
                        <div class="col-auto">
                            <asp:Button ID="SaveBtn" runat="server" Text="Save" OnClick="SaveBtn_Click" />&nbsp;&nbsp;
                        </div>
                        <div class="col-auto">
                            <asp:Button ID="DeleteBtn" runat="server" Text="Delete" OnClick="DeleteBtn_Click" OnClientClick = "return confirm('Are you sure you want to delete?');"/>&nbsp;&nbsp;
                        </div>
                        <div class="col-auto">
                            <asp:Button ID="CancelBtn" runat="server" Text="Cancel" OnClick="CancelBtn_Click" />&nbsp;&nbsp;
                        </div>
                    </div>
                </div>
                <br />
                <%--View Purchase Order Row--%>
                <div class="row">
                    <%--Vendor Information--%>
                    <div class="col-auto">
                        <div class="row">
                            <asp:Label ID="VendorNameLbl" runat="server" Text="Vendor Name"></asp:Label>&nbsp;-&nbsp;
                            <asp:Label ID="VendorContactLbl" runat="server" Text="Contact"></asp:Label>&nbsp;-&nbsp;
                            <asp:Label ID="VendorPhoneLbl" runat="server" Text="Phone"></asp:Label>
                        </div>
                        <br />
                        <%--Comments Area--%>
                        <div class="row">
                            <asp:TextBox ID="CommentsTextArea" TextMode="MultiLine" runat="server"
                                placeholder="Comments"
                                Columns="50" Rows="2"
                                Wrap="true" style="resize:none;"></asp:TextBox>
                        </div>
                    </div>
                    <%--Cost Breakdown--%>
                    <div class="col d-flex flex-column justify-content-between">
                        <div class="row justify-content-end">
                            &nbsp;&nbsp<asp:Label ID="SubtotalLbl" runat="server" Text="Subtotal"></asp:Label> &nbsp;&nbsp;
                            <asp:TextBox ID="SubtotalTextBox" runat="server" ReadOnly="true" style="text-align:right;"></asp:TextBox> &nbsp;
                        </div>
                        <div class="row justify-content-end">
                            &nbsp;&nbsp<asp:Label ID="TaxLbl" runat="server" Text="Tax"></asp:Label> &nbsp;&nbsp;
                            <asp:TextBox ID="TaxTextBox" runat="server" ReadOnly="true" style="text-align:right;"></asp:TextBox> &nbsp;
                        </div>
                        <div class="row justify-content-end">
                            &nbsp;&nbsp<asp:Label ID="TotaLbl" runat="server" Text="Total"></asp:Label> &nbsp;&nbsp;
                            <asp:TextBox ID="TotalTextBox" runat="server" ReadOnly="true" style="text-align:right;"></asp:TextBox> &nbsp;
                        </div>
                    </div>
                </div>
                <br />
                <%--Placeholder for the purchase order ID--%>
                <div class="row">
                    <%--Purchase Order Item Grid View--%>
                    <asp:GridView ID="PurchaseOrderItemGridView" runat="server"
                        AutoGenerateColumns="False"
                        ItemType="ERaceSystem.VIEWMODELS.Purchasing.PurchaseOrderItem"
                        BorderStyle="None" GridLines="None" CssClass="table table-hover table-striped"
                        OnRowCommand="PurchaseOrderItemsList_RowCommand">
                        <Columns>
                            
                            <%--Hidden fields (IDs)--%>
                            <asp:TemplateField>
                                <ItemTemplate>
                                    <asp:Label ID="PurchaseOrderIdLbl"
                                        runat="server"
                                        Text='<%# Item.PurchaseOrderId %>'
                                        Visible="false"/>

                                    <asp:Label ID="ProductIdLbl"
                                        runat="server"
                                        Text='<%# Item.ProductId %>'
                                        Visible="false"/>

                                    <asp:Label ID="UnitSizeLbl"
                                        runat="server"
                                        Text='<%# Item.UnitSize %>'
                                        Visible="false"/>
                                    <asp:Label ID="PurchaseOrderItemIdLbl"
                                        runat="server"
                                        Text='<%# Item.PurchaseOrderItemId %>'
                                        Visible="false"/>
                                </ItemTemplate>
                            </asp:TemplateField>

                            <%--Delete Button--%>
                            <asp:TemplateField ItemStyle-VerticalAlign="Top">
                                <ItemTemplate>
                                    <asp:LinkButton ID="DeleteFromPurchaseOrderItem" runat="server"
                                        CommandName="DeleteFromMyPurchaseOrder"
                                        CommandArgument="<%# Container.DataItemIndex %>"
                                        CssClass="btn">
                                        <i class="fa fa-times" style="color:red;"></i>&nbsp;
                                    </asp:LinkButton>
                                </ItemTemplate>
                            </asp:TemplateField>

                            <asp:TemplateField HeaderText="Product Name" ItemStyle-VerticalAlign="Top">
                                <ItemTemplate>
                                    <asp:Label ID="ProductNameLbl"
                                        runat="server"
                                        Text='<%# Item.ProductName %>' />
                                </ItemTemplate>
                            </asp:TemplateField>

                            <asp:TemplateField HeaderText="Quantity" ItemStyle-VerticalAlign="Top">
                                <ItemTemplate>
                                    <asp:TextBox ID="QuantityTextBox"
                                        Text='<%# Item.Quantity %>'
                                        Width="50"
                                        runat="server"
                                        Style="text-align: right;" />
                                </ItemTemplate>
                            </asp:TemplateField>

                            <asp:TemplateField HeaderText="Unit Type" ItemStyle-VerticalAlign="Top">
                                <ItemTemplate>
                                    <asp:Label ID="UnitSizeAndTypeLbl"
                                        runat="server"
                                        Text='<%# Item.UnitSizeAndType %>' />
                                </ItemTemplate>
                            </asp:TemplateField>

                            <asp:TemplateField HeaderText="Unit Cost" ItemStyle-VerticalAlign="Top">
                                <ItemTemplate>
                                    <asp:TextBox ID="UnitCostTextBox"
                                        Text='<%# String.Format("{0:F2}", Item.UnitCost)%>'
                                        Width="70"
                                        runat="server"
                                        Style="text-align: right;" />
                                </ItemTemplate>
                            </asp:TemplateField>

                            <asp:TemplateField HeaderText="Per-item Cost" ItemStyle-VerticalAlign="Top" ItemStyle-CssClass="d-flex flex-row justify-content-between">
                                <ItemTemplate>
                                    <asp:LinkButton ID="SyncCosts" runat="server"
                                        CommandName="SyncCostsToPurchaseOrderItem"
                                        CommandArgument="<%# Container.DataItemIndex %>"
                                        CssClass="btn"
                                        Style="padding:0;">
                                        <i class="fa fa-refresh" style="color:dodgerblue"></i>
                                    </asp:LinkButton>

                                    <asp:Label ID="WarningLbl" runat="server" Visible="false" >
<%--                                        <i class="fa fa-exclamation" aria-hidden="true" style="color:red "></i>
                                        <i class="fa fa-exclamation" aria-hidden="true" style="color:red"></i>--%>
                                        <span style="color:red;">!!</span>
                                    </asp:Label>

                                    <asp:Label ID="PerItemCostLbl" runat="server"
                                        Text='<%# String.Format("{0:C2}",Item.PerItemCost) %>'
                                        Style="text-align: right;"/>
                                </ItemTemplate>
                            </asp:TemplateField>

                            <asp:TemplateField HeaderText="Extended Cost" ItemStyle-VerticalAlign="Top">
                                <ItemTemplate>
                                    <asp:Label ID="ExtendedCostLbl"
                                        runat="server"
                                        Width="100"
                                        Text='<%# String.Format("{0:C2}", Item.ExtendedCost)%>'
                                        Style="text-align: right;" />
                                </ItemTemplate>
                            </asp:TemplateField>

                        </Columns>
                        <EmptyDataTemplate>
                            <table runat="server" style="background-color: #FFFFFF; border-collapse: collapse; border-color: #999999; border-style: none; border-width: 1px;">
                                <tr>
                                    <td>No data was returned.</td>
                                </tr>
                            </table>
                        </EmptyDataTemplate> 
                    </asp:GridView>
                </div>
            </div>
            <%--Inventory Column--%>
            <div class="col">
                <%--Inventory Header--%>
                <div class="row">
                    <h3>Inventory</h3>
                </div>
                <br />
                <div class="row flex-column">
                    <asp:Repeater ID="InventoryRepeater" runat="server" 
                        DataSourceID="InventoryODS"
                        ItemType="ERaceSystem.VIEWMODELS.Purchasing.InventoryList">
                        <ItemTemplate>
                            <div class="row">
                                <div class="col">
                                    <h4><u><%# Item.CategoryName %></u></h4>
                                </div>
                            </div>
                            <div class="row">
                                <asp:GridView ID="InventoryProductsGridView" runat="server"
                                    DataSource="<%# Item.Products %>"
                                    AutoGenerateColumns="false"
                                    BorderStyle="None" GridLines="None" CssClass="table table-hover table-striped"
                                    OnRowCommand="InventoryProductsList_RowCommand">
                                    <Columns>
                                       <%--Hidden fields (IDs)--%>
                                        <asp:TemplateField>
                                            <ItemTemplate>
                                                <asp:Label ID="ProductIdLbl"
                                                    runat="server"
                                                    Text='<%# Eval("ProductId") %>'
                                                    Visible="false"/>

                                                <asp:Label ID="UnitSizeLbl"
                                                    runat="server"
                                                    Text='<%# Eval("UnitSize") %>'
                                                    Visible="false"/>
                                                <asp:Label ID="UnitCostLbl"
                                                    runat="server"
                                                    Text='<%# Eval("UnitCost") %>'
                                                    Visible="false"/>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField>
                                            <ItemTemplate>
                                                <asp:LinkButton ID="AddToPurchaseOrderItem" runat="server"
                                                    CommandName="AddToMyPurchaseOrder"
                                                    CommandArgument='<%# Eval("ProductId") %>'
                                                    CssClass="btn">
                                                    <i class="fa fa-plus" aria-hidden="true" style="color:forestgreen"></i>&nbsp;
                                                </asp:LinkButton>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Product" ItemStyle-VerticalAlign="Top">
                                            <ItemTemplate>
                                                <asp:Label ID="InventoryProductLbl"
                                                    runat="server"
                                                    Text='<%# Eval("ProductName") %>' />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Reorder" ItemStyle-VerticalAlign="Top" ItemStyle-Width="5%">
                                            <ItemTemplate>
                                                <asp:Label ID="ReOrderLbl"
                                                    runat="server"
                                                    Text='<%# Eval("ReOrderLevel") %>' />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="In Stock" ItemStyle-VerticalAlign="Top" ItemStyle-Width="15%">
                                            <ItemTemplate>
                                                <asp:Label ID="InStockLbl"
                                                    runat="server"
                                                    Text='<%# Eval("QuantityOnHand") %>' />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="On Order" ItemStyle-VerticalAlign="Top" ItemStyle-Width="15%">
                                            <ItemTemplate>
                                                <asp:Label ID="OnOrderLbl"
                                                    runat="server"
                                                    Text='<%# Eval("QuantityOnOrder") %>' />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Size" ItemStyle-VerticalAlign="Top" ItemStyle-Width="20%">
                                            <ItemTemplate>
                                                <asp:Label ID="SizeLbl"
                                                    runat="server"
                                                    Text='<%# Eval("UnitSizeAndType") %>' />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                    </Columns>
                                </asp:GridView>
                            </div>
                            
                        </ItemTemplate>
                        <SeparatorTemplate>
                            <hr style="height:3px;" />
                        </SeparatorTemplate>
                    </asp:Repeater>
                    <%--ODS--%>
                    <asp:ObjectDataSource ID="InventoryODS" runat="server" OldValuesParameterFormatString="original_{0}" SelectMethod="List_InventoryByVendorId" TypeName="ERaceSystem.BLL.Purchasing.PurchasingController">
                        <SelectParameters>
                            <asp:ControlParameter ControlID="VendorDropDownList" PropertyName="SelectedValue" Name="vendorId" Type="Int32"></asp:ControlParameter>
                        </SelectParameters>
                    </asp:ObjectDataSource>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
