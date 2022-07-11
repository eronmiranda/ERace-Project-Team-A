using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using ERaceSystem.BLL.Racing;
using ERaceSystem.VIEWMODELS.Racing;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Owin;

namespace WebApp.Pages.Racing
{
    public partial class Registration : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request.IsAuthenticated)
            {
                MessageUserControl.ShowInfo("Login Successful");

                var userManager = Context.GetOwinContext().GetUserManager<ApplicationUserManager>();

                var user = userManager.FindByName(User.Identity.Name);
                var userAllowed = HttpContext.Current.User.IsInRole("Race Coordinator");

                if (!userAllowed)
                {
                    Response.Redirect("Default.aspx");
                }

                EmployeeNameLabel.Text = new RacingController().GetEmployeeName(user.EmployeeId);
            }
            else
            {
                Response.Redirect("~/Account/Login.aspx");
            }
        }

        protected void RegistrationCalendar_SelectionChanged(object sender, EventArgs e)
        {
            RosterPanel.Visible = false;
            SchedulePanel.Visible = true;
            ScheduleListView.SelectedIndex = -1;
        }

        protected void ScheduleListView_ItemCommand(object sender, ListViewCommandEventArgs e)
        {
            RosterPanel.Visible = true;
            RosterListView.EditIndex = -1;
        }

        #region RosterODS Pre Execute
        protected void RosterODS_Inserting(object sender, ObjectDataSourceMethodEventArgs e)
        {
            IOrderedDictionary parameters = e.InputParameters;

            var item = RosterListView.InsertItem;
            var raceFee = (item.FindControl("RaceFeeDDL") as DropDownList).SelectedValue.ToDecimal();
            if (raceFee == 0)
            {
                raceFee = string.IsNullOrWhiteSpace(item.FindTextBox("FeeTextBox").Text) ? 0 : item.FindTextBox("FeeTextBox").Text.ToDecimal();
                raceFee = Math.Abs(raceFee);
            }

            RosterViewModel roster = new RosterViewModel()
            {
                RaceID = ScheduleListView.SelectedDataKey.Value.ToString().ToInt(),
                MemberID = (item.FindControl("DriverDDL") as DropDownList).SelectedValue.ToInt(),
                RaceFee = raceFee,
                CarClassID = (item.FindControl("InsertCarClassDDL") as DropDownList).SelectedValue.ToInt(),
                SerialNumber = (item.FindControl("InsertCarDDL") as DropDownList).SelectedValue
            };

            var userManager = Context.GetOwinContext().GetUserManager<ApplicationUserManager>();
            var user = userManager.FindByName(User.Identity.Name);
            roster.EmployeeID = user.EmployeeId;

            parameters.Clear();
            parameters.Add("roster", roster);
        }

        protected void RosterODS_Updating(object sender, ObjectDataSourceMethodEventArgs e)
        {
            IOrderedDictionary parameters = e.InputParameters;
            
            var item = RosterListView.EditItem;

            RosterViewModel roster = new RosterViewModel
            {
                RaceDetailID = item.FindHiddenField("RaceDetailIDField").Value.ToString().ToInt(),
                RaceID = ScheduleListView.SelectedDataKey.Value.ToString().ToInt(),
                Comment = item.FindTextBox("CommentTextBox").Text,
                Reason = item.FindTextBox("ReasonTextBox").Text,
                Refunded = item.FindCheckBox("RefundedCheckBox").Checked
            };

            String serialNumber = (item.FindControl("CarDDL") as DropDownList).SelectedValue;
            roster.SerialNumber = serialNumber == "0" ? null : serialNumber;

            parameters.Clear();
            parameters.Add("roster", roster);
        }
        #endregion

        #region RosterODS Post Execute
        protected void CheckForException(object sender, ObjectDataSourceStatusEventArgs e)
        {
            MessageUserControl.HandleDataBoundException(e);
        }

        protected void SelectCheckForException(object sender, ObjectDataSourceStatusEventArgs e)
        {
            MessageUserControl.HandleDataBoundException(e);
        }

        protected void InsertCheckForException(object sender, ObjectDataSourceStatusEventArgs e)
        {
            if (e.Exception == null)
            {
                MessageUserControl.ShowInfo("Success", "Registration has been added.");
            }
            else
            {
                MessageUserControl.HandleDataBoundException(e);
            }
        }
        protected void UpdateCheckForException(object sender, ObjectDataSourceStatusEventArgs e)
        {
            if (e.Exception == null)
            {
                MessageUserControl.ShowInfo("Success", "Registration has been updated.");
            }
            else
            {
                MessageUserControl.HandleDataBoundException(e);
            }
        }
#endregion

        

        protected void ResultsButton_Command(object sender, CommandEventArgs e)
        {
            var raceID = ScheduleListView.SelectedDataKey.Value.ToString();

            Response.Redirect($"Results.aspx?RaceID={raceID}");
        }

        protected void RosterListView_ItemEditing(object sender, ListViewEditEventArgs e)
        {
            RosterListView.EditIndex = e.NewEditIndex;
            RosterListView.DataBind();
            var serialNumber = RosterListView.Items[e.NewEditIndex].FindHiddenField("SerialNumberField").Value;

            var ddl = RosterListView.EditItem.FindControl("CarDDL") as DropDownList;
            if (serialNumber == "0" || string.IsNullOrEmpty(serialNumber))
            {
                ddl.SelectedValue = "0";
            }
            else
            {
                ddl.SelectedValue = serialNumber;
            }
        }

        protected void RaceFeeDDL_SelectedIndexChanged(object sender, EventArgs e)
        {
            var ddl = sender as DropDownList;

            if (ddl.SelectedValue == "0")
            {
                RosterListView.InsertItem.FindControl("FeeTextBox").Visible = true;
            }
            else
            {
                RosterListView.InsertItem.FindControl("FeeTextBox").Visible = false;
            }

            
        }
    }
}