<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Registration.aspx.cs" Inherits="WebApp.Pages.Racing.Registration" %>

<%@ Register Src="~/UserControls/MessageUserControl.ascx" TagPrefix="uc1" TagName="MessageUserControl" %>


<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <uc1:MessageUserControl runat="server" ID="MessageUserControl" />
    
    <asp:Label runat="server" ID="EmployeeNameLabel" ></asp:Label>
    <h1>Registrations</h1>
    <asp:Panel CssClass="row" runat="server">
        <asp:Panel CssClass="col-2" runat="server" Visible="true">
            <h2>Calendar</h2>
            <asp:Calendar ID="RegistrationCalendar" runat="server" OnSelectionChanged="RegistrationCalendar_SelectionChanged">
                <SelectedDayStyle
                    BackColor="Blue" />
            </asp:Calendar>
        </asp:Panel>

        <asp:Panel CssClass="col-3" runat="server" ID="SchedulePanel" Visible="false">
            <h2>Schedule</h2>
            <asp:ObjectDataSource ID="ScheduleODS" runat="server" OldValuesParameterFormatString="original_{0}" SelectMethod="GetSchedulesByDate" TypeName="ERaceSystem.BLL.Racing.RacingController">
                <SelectParameters>
                    <asp:ControlParameter ControlID="RegistrationCalendar" PropertyName="SelectedDate" Name="date" Type="DateTime"></asp:ControlParameter>
                </SelectParameters>
            </asp:ObjectDataSource>
            <asp:ListView ID="ScheduleListView" runat="server" DataKeyNames="RaceID" OnItemCommand="ScheduleListView_ItemCommand"  DataSourceID="ScheduleODS">
                <AlternatingItemTemplate>
                    <tr style="background-color: #FFFFFF; color: #284775;">
                        <td>
                            <asp:LinkButton runat="server" ID="ViewRaceButton" CommandName="Select" CommandArgument='<%# Eval("RaceID") %>'>View</asp:LinkButton>
                            <asp:HiddenField Value='<%# Eval("RaceID") %>' runat="server" ID="RaceIDLabel" /></td>
                        </td>
                        <td>
                            <asp:Label Text='<%# Eval("Time", "{0:h tt}") %>' runat="server" ID="TimeLabel" /></td>
                        <td>
                            <asp:Label Text='<%# Eval("Competition") %>' runat="server" ID="CompetitionLabel" /></td>
                        <td>
                            <asp:Label Text='<%# Eval("Run") %>' runat="server" ID="RunLabel" /></td>
                        <td>
                            <asp:Label Text='<%# Eval("Drivers") %>' runat="server" ID="DriversLabel" /></td>
                    </tr>
                </AlternatingItemTemplate>
                <EmptyDataTemplate>
                    <table runat="server" style="background-color: #FFFFFF; border-collapse: collapse; border-color: #999999; border-style: none; border-width: 1px;">
                        <tr>
                            <td>No Races on that day.</td>
                        </tr>
                    </table>
                </EmptyDataTemplate>
                <ItemTemplate>
                    <tr style="background-color: #E0FFFF; color: #333333;">
                        <td>
                            <asp:LinkButton runat="server" ID="ViewRaceButton" CommandName="Select" CommandArgument='<%# Eval("RaceID") %>'>View</asp:LinkButton>
                            <asp:HiddenField Value='<%# Eval("RaceID") %>' runat="server" ID="RaceIDLabel" />
                        </td>
                        <td>
                            <asp:Label Text='<%# Eval("Time", "{0:h tt}") %>' runat="server" ID="TimeLabel" /></td>
                        <td>
                            <asp:Label Text='<%# Eval("Competition") %>' runat="server" ID="CompetitionLabel" /></td>
                        <td>
                            <asp:Label Text='<%# Eval("Run") %>' runat="server" ID="RunLabel" /></td>
                        <td>
                            <asp:Label Text='<%# Eval("Drivers") %>' runat="server" ID="DriversLabel" /></td>
                    </tr>
                </ItemTemplate>
                <LayoutTemplate>
                    <table runat="server">
                        <tr runat="server">
                            <td runat="server">
                                <table runat="server" id="itemPlaceholderContainer" style="background-color: #FFFFFF; border-collapse: collapse; border-color: #999999; border-style: none; border-width: 1px; font-family: Verdana, Arial, Helvetica, sans-serif;" border="1">
                                    <tr runat="server" style="background-color: #E0FFFF; color: #333333;">
                                        <th runat="server"></th>
                                        <th runat="server">Time</th>
                                        <th runat="server">Competition</th>
                                        <th runat="server">Run</th>
                                        <th runat="server">Drivers</th>
                                    </tr>
                                    <tr runat="server" id="itemPlaceholder"></tr>
                                </table>
                            </td>
                        </tr>
                        <tr runat="server">
                            <td runat="server" style="text-align: center; background-color: #5D7B9D; font-family: Verdana, Arial, Helvetica, sans-serif; color: #FFFFFF">
                            </td>
                        </tr>
                    </table>
                </LayoutTemplate>
                <SelectedItemTemplate>
                    <tr style="background-color: #E2DED6; font-weight: bold; color: #333333;">
                        <td>
                            <asp:LinkButton runat="server" ID="ViewRaceButton"><i class="fas fa-check"></i> <i class="fas fa-arrow-right"></i></i></asp:LinkButton>
                            <asp:HiddenField Value='<%# Eval("RaceID") %>' runat="server" ID="RaceIDLabel" />

                        </td>
                        <td>
                            <asp:Label Text='<%# Eval("Time", "{0:h tt}") %>' runat="server" ID="TimeLabel" /></td>
                        <td>
                            <asp:Label Text='<%# Eval("Competition") %>' runat="server" ID="CompetitionLabel" /></td>
                        <td>
                            <asp:Label Text='<%# Eval("Run") %>' runat="server" ID="RunLabel" /></td>
                        <td>
                            <asp:Label Text='<%# Eval("Drivers") %>' runat="server" ID="DriversLabel" /></td>
                    </tr>
                </SelectedItemTemplate>
            </asp:ListView>
        </asp:Panel>


        <asp:Panel runat="server" Visible="false" ID="RosterPanel" class="col-3">
            <div class="d-flex justify-content-between">
                <h2>Roster</h2>
                <asp:Button ID="ResultsButton" runat="server" OnCommand="ResultsButton_Command" CommandArgument='<%# ScheduleListView.SelectedDataKey.Value.ToString() %>'
                    CssClass="btn btn-primary" Text="Record Race Times"/>
            </div>
            <asp:ObjectDataSource ID="RosterODS"
                runat="server"
                SelectMethod="GetRosterByRaceID"
                UpdateMethod="UpdateRoster"
                InsertMethod="AddRoster"
                OldValuesParameterFormatString="original_{0}"
                TypeName="ERaceSystem.BLL.Racing.RacingController"
                OnInserting="RosterODS_Inserting"
                OnUpdating="RosterODS_Updating"
                OnSelected="SelectCheckForException"
                OnUpdated="UpdateCheckForException"
                OnInserted="InsertCheckForException">
                <InsertParameters>
                    <asp:Parameter Name="roster" Type="Object"></asp:Parameter>
                    <asp:Parameter Name="employee" Type="String"></asp:Parameter>
                </InsertParameters>
                <SelectParameters>
                    <asp:ControlParameter ControlID="ScheduleListView" PropertyName="SelectedValue" DefaultValue="0" Name="raceID" Type="Int32"></asp:ControlParameter>
                </SelectParameters>
            </asp:ObjectDataSource>
            <asp:ListView 
                ID="RosterListView" 
                runat="server" 
                DataSourceID="RosterODS" 
                InsertItemPosition="LastItem"
                ItemType="ERaceSystem.VIEWMODELS.Racing.RosterViewModel" >
                <AlternatingItemTemplate>
                    <tr style="background-color: #FFFFFF; color: #284775;">
                        <td>
                            <asp:LinkButton runat="server" CommandName="Edit" Text="Edit" ID="EditButton" />
                        </td>
                            <td>
                        <asp:Label Text='<%# Eval("Name") %>' runat="server" ID="NameLabel" /></td>
                            <td>
                                <asp:Label Text='<%# Eval("RaceFee", "{0:#,###.##}") %>' runat="server" ID="RaceFeeLabel" /></td>
                        <td>
                            <asp:Label Text='<%# Eval("RentalFee", "{0:#,###.##}") %>' runat="server" ID="RentalFeeLabel" /></td>
                        <td>
                            <asp:Label Text='<%# Eval("Placement") %>' runat="server" ID="PlacementLabel" /></td>
                        <td>
                            <asp:CheckBox Checked='<%# Eval("Refunded") %>' runat="server" ID="RefundedCheckBox" Enabled="false" /></td>
                    </tr>
                </AlternatingItemTemplate>
                <EditItemTemplate>
                    <tr style="background-color: #999999;">
                        <td>
                            <asp:LinkButton runat="server" CommandName="Cancel" Text="Cancel" ID="CancelButton" />
                            <asp:LinkButton runat="server" CommandName="Update" Text="Save" ID="UpdateButton" CommandArgument='<%# Item.RaceDetailID %>' />
                            <asp:HiddenField runat="server" Value='<%# Bind("RaceDetailID") %>' ID="RaceDetailIDField" />
                        </td>
                        <td>
                            <asp:Label Text='<%# Eval("Name") %>' runat="server" ID="NameLabel" /></td>
                        <td>
                            <asp:Label Text='<%# Eval("RaceFee", "{0:#,###.##}") %>' runat="server" ID="RaceFeeLabel" /></td>
                        <td>
                            <asp:ObjectDataSource ID="CarClassODS" runat="server" OldValuesParameterFormatString="original_{0}" SelectMethod="GetCarClassesFromRaceID" TypeName="ERaceSystem.BLL.Racing.RacingController" >
                                <SelectParameters>
                                    <asp:ControlParameter ControlID="ScheduleListView" PropertyName="SelectedValue" Name="raceID" Type="Int32" DefaultValue="0"></asp:ControlParameter>
                                </SelectParameters>
                            </asp:ObjectDataSource>

                            <%--<asp:TextBox Text= runat="server" ID="CarClassTextBox" /></td>--%>
                            <asp:DropDownList ID="CarClassDDL" runat="server" DataSourceID="CarClassODS" DataTextField="ClassName" DataValueField="ClassID" AutoPostBack="true"
                                SelectedValue='<%# Eval("CarClassID") %>'>
                            </asp:DropDownList>
                        <td colspan="2">
                            <asp:HiddenField runat="server" ID="SerialNumberField" Value='<%# Bind("SerialNumber") %>' />

                            <asp:ObjectDataSource ID="CarODS" runat="server" OldValuesParameterFormatString="original_{0}" SelectMethod="GetCarsFromClassIDPlusCurrentCar" TypeName="ERaceSystem.BLL.Racing.RacingController" >
                                <SelectParameters>
                                    <asp:ControlParameter ControlID="CarClassDDL" PropertyName="SelectedValue" DefaultValue="0" Name="carClassID" Type="Int32"></asp:ControlParameter>
                                    <asp:ControlParameter ControlID="ScheduleListView" PropertyName="SelectedValue" DefaultValue="0" Name="raceID" Type="Int32"></asp:ControlParameter>
                                    <asp:ControlParameter ControlID="RaceDetailIDField" PropertyName="Value" DefaultValue="" Name="raceDetailID" Type="Int32"></asp:ControlParameter>
                                </SelectParameters>
                            </asp:ObjectDataSource>

                            <asp:DropDownList ID="CarDDL" runat="server" DataSourceID="CarODS" DataTextField="SerialNumber" DataValueField="SerialNumber" ></asp:DropDownList></td>
                       </tr >

                    <tr style="background-color: #999999;" />
                        <td> </td>
                        <td colspan="2">
                            <asp:TextBox Text='<%# Bind("Comment") %>' runat="server" ID="CommentTextBox" PlaceHolder="Comment" /></td>
                        <td colspan="2">
                            <asp:TextBox Text='<%# Bind("Reason") %>' runat="server" ID="ReasonTextBox" PlaceHolder="Reason" /></td>
                        <td>
                            <asp:CheckBox Checked='<%# Bind("Refunded") %>' runat="server" ID="RefundedCheckBox" Text="Refunded" /></td>
                    </tr>
                </EditItemTemplate>
                <EmptyDataTemplate>
                    <table runat="server" style="background-color: #FFFFFF; border-collapse: collapse; border-color: #999999; border-style: none; border-width: 1px;">
                        <tr>
                            <td>No drivers found.</td>
                        </tr>
                    </table>
                </EmptyDataTemplate>
                <InsertItemTemplate>
                    <tr style="">
                        <td>
                            <asp:LinkButton runat="server" CommandName="Insert" Text="Add" ID="InsertButton" />
                        </td>
                        <td>
                            <%--<asp:Label Text='<%# Bind("Name") %>' runat="server" ID="NameTextBox" />--%>
                        <asp:ObjectDataSource ID="DriverODS" runat="server" OldValuesParameterFormatString="original_{0}" SelectMethod="GetDriversNotInRaceByRaceID" TypeName="ERaceSystem.BLL.Racing.RacingController">
                            <SelectParameters>
                                <asp:ControlParameter ControlID="ScheduleListView" PropertyName="SelectedValue" DefaultValue="" Name="raceID" Type="Int32"></asp:ControlParameter>
                            </SelectParameters>
                        </asp:ObjectDataSource>
                        <asp:DropDownList ID="DriverDDL" runat="server" DataSourceID="DriverODS" DataTextField="Name" DataValueField="MemberID" SelectedValue='<%# Bind("MemberID") %>'></asp:DropDownList></td>
                        <td>
                            <%--<asp:TextBox Text='<%# Bind("RaceFee") %>' runat="server" ID="RaceFeeTextBox" />--%>
                            <asp:ObjectDataSource ID="RaceFeeODS" runat="server" OldValuesParameterFormatString="original_{0}" SelectMethod="GetRaceFees" TypeName="ERaceSystem.BLL.Racing.RacingController"></asp:ObjectDataSource>
                            <asp:DropDownList ID="RaceFeeDDL" runat="server" DataSourceID="RaceFeeODS" SelectedValue='<%# Bind("RaceFee") %>' AppendDataBoundItems="true" OnSelectedIndexChanged="RaceFeeDDL_SelectedIndexChanged" AutoPostBack="true">
                                <asp:ListItem Value="0">Enter Race Fee</asp:ListItem>
                            </asp:DropDownList></td>
                        <td>
                            <asp:ObjectDataSource ID="InsertCarClassODS" runat="server" OldValuesParameterFormatString="original_{0}" SelectMethod="GetCarClassesFromRaceID" TypeName="ERaceSystem.BLL.Racing.RacingController" >
                                <SelectParameters>
                                    <asp:ControlParameter ControlID="ScheduleListView" PropertyName="SelectedValue" Name="raceID" Type="Int32" DefaultValue="0"></asp:ControlParameter>
                                </SelectParameters>
                            </asp:ObjectDataSource>

                            <%--<asp:TextBox Text= runat="server" ID="CarClassTextBox" /></td>--%>
                            <asp:DropDownList ID="InsertCarClassDDL" runat="server" DataSourceID="InsertCarClassODS" DataTextField="ClassName" DataValueField="ClassID" AutoPostBack="true" >
                                <asp:ListItem Value="null">Select a Class</asp:ListItem>
                            </asp:DropDownList>
                        <td colspan="2">
                            <asp:ObjectDataSource ID="InsertCarODS" runat="server" OldValuesParameterFormatString="original_{0}" SelectMethod="GetCarsFromClassID" TypeName="ERaceSystem.BLL.Racing.RacingController">
                                <SelectParameters>
                                    <asp:ControlParameter ControlID="InsertCarClassDDL" PropertyName="SelectedValue" DefaultValue="0" Name="carClassID" Type="Int32"></asp:ControlParameter>
                                    <asp:ControlParameter ControlID="ScheduleListView" PropertyName="SelectedValue" DefaultValue="0" Name="raceID" Type="Int32"></asp:ControlParameter>
                                </SelectParameters>
                            </asp:ObjectDataSource>

                            <%--<asp:TextBox Text='<%# Bind("SerialNumber") %>' runat="server" ID="SerialNumberTextBox" />--%>
                            <asp:DropDownList ID="InsertCarDDL" runat="server" DataSourceID="InsertCarODS" DataTextField="SerialNumber" DataValueField="SerialNumber">
                                <asp:ListItem Value="null">Select a Car</asp:ListItem>
                            </asp:DropDownList></td>
                    </tr>
                    <tr>
                        <td></td>
                        <td></td>
                        <td><asp:TextBox id="FeeTextBox" TextMode="Number" step="0.01" min="0" runat="server" Visible="true" placeholder="Enter Fee Here"></asp:TextBox></td>
                        <td></td>
                        <td></td>
                        <td></td>
                    </tr>
                </InsertItemTemplate>
                <ItemTemplate>
                    <tr style="background-color: #E0FFFF; color: #333333;">
                        <td>
                            <asp:LinkButton runat="server" CommandName="Edit" Text="Edit" ID="EditButton" />
                        </td>
                        <td>
                            <asp:Label Text='<%# Eval("Name") %>' runat="server" ID="NameLabel" /></td>
                        <td>
                            <asp:Label Text='<%# Eval("RaceFee", "{0:#,###.##}") %>' runat="server" ID="RaceFeeLabel" /></td>
                        <td>
                            <asp:Label Text='<%# Eval("RentalFee", "{0:#,###.##}") %>' runat="server" ID="RentalFeeLabel" /></td>
                        <td>
                            <asp:Label Text='<%# Eval("Placement") %>' runat="server" ID="PlacementLabel" /></td>
                        <td>
                            <asp:CheckBox Checked='<%# Eval("Refunded") %>' runat="server" ID="RefundedCheckBox" Enabled="false" /></td>
                    </tr>
                </ItemTemplate>
                <LayoutTemplate>
                    <table runat="server">
                        <tr runat="server">
                            <td runat="server">
                                <table runat="server" id="itemPlaceholderContainer" style="background-color: #FFFFFF; border-collapse: collapse; border-color: #999999; border-style: none; border-width: 1px; font-family: Verdana, Arial, Helvetica, sans-serif;" border="1">
                                    <tr runat="server" style="background-color: #E0FFFF; color: #333333;">
                                        <th runat="server"></th>
                                        <th runat="server">Name</th>
                                        <th runat="server">RaceFee</th>
                                        <th runat="server">RentalFee</th>
                                        <th runat="server">Placement</th>
                                        <th runat="server">Refunded</th>
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

        </asp:Panel>
    </asp:Panel>

</asp:Content>
