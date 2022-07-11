<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Purchases.aspx.cs" Inherits="WebApp.Pages.Sales.Purchases" %>

<%@ Register Src="~/UserControls/MessageUserControl.ascx" TagPrefix="uc1" TagName="MessageUserControl" %>


<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <h1>In-Store Purchases</h1>
    <uc1:MessageUserControl runat="server" ID="MessageUserControl" />
    <asp:Label ID="LabelTopError" runat="server" ></asp:Label>
    <div class="row">
        <asp:DropDownList ID="CategoryDropDown" runat="server" DataSourceID="ObjectDataSource1" DataTextField="CategoryText" DataValueField="CategoryId" AppendDataBoundItems="True" OnSelectedIndexChanged="CategoryDropDown_SelectedIndexChanged" AutoPostBack="True">
            <asp:ListItem Value="0">Select a Category</asp:ListItem>
        </asp:DropDownList>
        <asp:ObjectDataSource ID="ObjectDataSource1" runat="server" OldValuesParameterFormatString="original_{0}" SelectMethod="Category_List" TypeName="ERaceSystem.BLL.Sales.SalesController"></asp:ObjectDataSource>
        <asp:DropDownList ID="ProductsDropDown" runat="server" AppendDataBoundItems="true" AutoPostBack="true" DataTextField="ProductText" DataValueField="ProductId">
            <asp:ListItem Value="0">Select a Product</asp:ListItem>
        </asp:DropDownList>
        <asp:ObjectDataSource ID="ObjectDataSource2" runat="server"></asp:ObjectDataSource>
        <asp:TextBox ID="QuantityOfItemSelectedTextBox" runat="server" placeholder="Qty" AutoPostBack="false" TextMode="Number" min="1" Width="50px" Text="1"/>
        <asp:Button ID="AddItemButton" runat="server" Text="Add" OnClick="AddButtonClicked" />        
    </div>
    <div class="row">
        <asp:GridView ID="CartItems" runat="server" CssClass="table table-hover table-sm" ItemType="ERaceSystem.VIEWMODELS.Sales.PurchaseListItem" DataKeyNames="ProductId" AutoGenerateColumns="false" SelectedIndex ="-1" OnRowCommand="CartItems_RowCommand">
            <Columns>
                <asp:BoundField  DataField="ProductDescription" HeaderText="Product" />                
                <asp:TemplateField HeaderText="Quantity">
                    <ItemTemplate>                                                        
                        <asp:TextBox ID="QuantityOfItemSelectedGridViewTextBox" runat="server" placeholder="Qty" AutoPostBack="false" TextMode="Number" min="1" Width="50px" Text='<%# Item.Quantity %>' />
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Price">
                    <ItemTemplate>
                        <asp:LinkButton ID="RefreshCart" runat="server" CommandName="RefreshCart" CommandArgument="<%# Container.DataItemIndex %>" CssClass="btn fa fa-refresh" BorderWidth="2px" BorderColor="Green" ForeColor="Green">
                            
                        </asp:LinkButton>
                        <asp:Label runat="server" ID="ItemPrice" Text="<%# Item.Price %>" />
                    </ItemTemplate>
                </asp:TemplateField>
                <%--<asp:TemplateField HeaderText="Amount">
                    <ItemTemplate>
                        
                        <asp:Label ID="Amount" Text="0.00" runat="server" TextMode="Number" />
                    </ItemTemplate>
                </asp:TemplateField>--%>
                <asp:BoundField  DataField="Amount" HeaderText="Amount" /> 
                <asp:TemplateField HeaderText="Remove">
                    <ItemTemplate>
                        <asp:LinkButton ID="RemoveFromCart" runat="server" CommandName="RemoveFromCart" CommandArgument="<%# Container.DataItemIndex %>" CssClass="btn fa" Text="X"  BorderWidth="2px" BorderColor="Red" ForeColor="Red">

                        </asp:LinkButton>
                        <asp:HiddenField ID="ProductID" runat="server" Value="<%# Item.ProductId %>" Visible="false"/>
                    </ItemTemplate>
                </asp:TemplateField>
                <%--<asp:TemplateField >
                    <ItemTemplate>
                        
                    </ItemTemplate>
                </asp:TemplateField>--%>
            </Columns>
        </asp:GridView>
    </div>
    <div class="row" >
        <div class="col-sm-1">
            &nbsp
            &nbsp
            <asp:LinkButton ID="MakePurchase" runat="server" BorderWidth="2px" BorderColor="Black" ForeColor="White" BackColor="#009900" OnClick="PurchaseButton_OnClick">
                <h3>  Purchase  </h3>
                <p>  Cash/Debit  </p>
            </asp:LinkButton>
        </div>
        <div class="col-sm-1">
            &nbsp
            &nbsp
            test</div>
        <div class="col-sm-2">
            &nbsp
            &nbsp
            <div class="row">
                <asp:Label ID="Subtotal" Text="$0.00" runat="server"  BorderWidth="1px" Width="100px" />
                
            </div>
            <div class="row">
                <asp:Label ID="Tax" Text="$0.00" runat="server" BorderWidth="1px" Width="100px"/>
            </div>
            <div class="row">
                <asp:Label ID="Total" Text="$0.00" runat="server" BorderWidth="1px" Width="100px"/>
            </div>
        </div>
    </div>
    

</asp:Content>
