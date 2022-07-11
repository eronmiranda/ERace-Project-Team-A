<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Results.aspx.cs" Inherits="WebApp.Pages.Racing.Results" %>

<%@ Register Src="~/UserControls/MessageUserControl.ascx" TagPrefix="uc1" TagName="MessageUserControl" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <uc1:MessageUserControl runat="server" ID="MessageUserControl" />

    <div class="d-flex">
        <h1 class="mr-5 pr-5">Race Results</h1>
        <asp:Button runat="server" ID="SaveTimesButton" OnCommand="SaveTimesButton_Command" CssClass="btn btn-primary ml-5" Text="Save Times" />
        <asp:Button runat="server" ID="BackButton" Text="Back" CssClass =" btn btn-secondary ml-5" OnCommand="BackButton_Command" />
    </div>

    <asp:ObjectDataSource ID="ResultsODS" runat="server" DataObjectTypeName="System.Collections.Generic.List`1[[ERaceSystem.VIEWMODELS.Racing.ResultViewModel, ERaceSystem, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null]]" OldValuesParameterFormatString="original_{0}" SelectMethod="GetResultsByRaceID" TypeName="ERaceSystem.BLL.Racing.RacingController" UpdateMethod="UpdateResults">
        <SelectParameters>
            <asp:QueryStringParameter QueryStringField="RaceID" Name="raceID" Type="Int32"></asp:QueryStringParameter>
        </SelectParameters>
    </asp:ObjectDataSource>

    <asp:ObjectDataSource ID="PenaltiesODS" runat="server" OldValuesParameterFormatString="original_{0}" SelectMethod="GetPenalties" TypeName="ERaceSystem.BLL.Racing.RacingController"></asp:ObjectDataSource>

    <asp:ListView ID="ResultsListView" runat="server" DataSourceID="ResultsODS">
        <AlternatingItemTemplate>
            <tr style="background-color: #FFFFFF; color: #284775;">
                    <asp:HiddenField Value='<%# Eval("RaceDetailID") %>' runat="server" ID="RaceDetailIDField" />
                <td>
                    <asp:Label Text='<%# Eval("Name") %>' runat="server" ID="NameLabel" /></td>
                <td>
                    <asp:TextBox Text='<%# Bind("Time") %>' runat="server" ID="TimeTextBox" format="HH:mm:ss" placeholder="HH:MM:SS" /></td>
                <td>
                    <asp:DropDownList ID="PenaltyDDL" runat="server" DataSourceID="PenaltiesODS" DataTextField="Description" DataValueField="PenaltyID" selectedvalue='<%# Bind("Penalty") %>'>
                    </asp:DropDownList>
                </td>
                <td>
                    <asp:Label Text='<%# Eval("Placement") %>' runat="server" ID="PlacementLabel" /></td>
            </tr>
        </AlternatingItemTemplate>
        <EmptyDataTemplate>
            <table runat="server" style="background-color: #FFFFFF; border-collapse: collapse; border-color: #999999; border-style: none; border-width: 1px;">
                <tr>
                    <td>No roster was found.</td>
                </tr>
            </table>
        </EmptyDataTemplate>
        <ItemTemplate>
            <tr style="background-color: #E0FFFF; color: #333333;">
                    <asp:HiddenField Value='<%# Eval("RaceDetailID") %>' runat="server" ID="RaceDetailIDField" />
                <td>
                    <asp:Label Text='<%# Eval("Name") %>' runat="server" ID="NameLabel" /></td>
                <td>
                    <asp:TextBox Text='<%# Bind("Time") %>' runat="server" ID="TimeTextBox" format="HH:mm:ss" placeholder="HH:MM:SS"/></td>
                <td>
                    <asp:DropDownList ID="PenaltyDDL" runat="server" DataSourceID="PenaltiesODS" DataTextField="Description" DataValueField="PenaltyID" selectedvalue='<%# Bind("Penalty") %>'>
                    </asp:DropDownList>
                </td>
                <td>
                    <asp:Label Text='<%# Eval("Placement") %>' runat="server" ID="PlacementLabel" /></td>
            </tr>
        </ItemTemplate>
        <LayoutTemplate>
            <table runat="server">
                <tr runat="server">
                    <td runat="server">
                        <table runat="server" id="itemPlaceholderContainer" style="background-color: #FFFFFF; border-collapse: collapse; border-color: #999999; border-style: none; border-width: 1px; font-family: Verdana, Arial, Helvetica, sans-serif;" border="1">
                            <tr runat="server" style="background-color: #E0FFFF; color: #333333;">
                                <th runat="server">Name</th>
                                <th runat="server">Time</th>
                                <th runat="server">Pentalty</th>
                                <th runat="server">Placement</th>
                            </tr>
                            <tr runat="server" id="itemPlaceholder"></tr>
                        </table>
                    </td>
                </tr>
                <tr runat="server">
                    <td runat="server" style="text-align: center; background-color: #5D7B9D; font-family: Verdana, Arial, Helvetica, sans-serif; color: #FFFFFF"></td>
                </tr>
            </table>
        </LayoutTemplate>
    </asp:ListView>

    <script type="text/javascript">
       function CallFunction() {
           return confirm("Are you sure you wish to overide the race times?");
       }
    </script>

</asp:Content>
