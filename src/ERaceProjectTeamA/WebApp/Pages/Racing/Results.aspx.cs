using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using ERaceSystem.VIEWMODELS.Racing;
using ERaceSystem.BLL.Racing;

namespace WebApp.Pages.Racing
{
    public partial class Results : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string raceIDString = Request.QueryString["RaceID"];
            int raceID;
            int.TryParse(raceIDString, out raceID);

            if (raceID == 0 || string.IsNullOrEmpty(raceIDString))
            {
                Response.Redirect("Registration.aspx");
            }
            else
            {
                var controller = new RacingController();
                if (controller.RaceIsRun(raceID))
                {
                    SaveTimesButton.OnClientClick = "CallFunction();";
                }
            }

            if (Request.IsAuthenticated)
            {
                MessageUserControl.ShowInfo("Login Successful");

                var userAllowed = HttpContext.Current.User.IsInRole("Race Coordinator");

                if (!userAllowed)
                {
                    Response.Redirect("Default.aspx");
                }
            }
            else
            {
                Response.Redirect("~/Account/Login.aspx");
            }


        }

        protected void SaveTimesButton_Command(object sender, CommandEventArgs e)
        {
            MessageUserControl.TryRun(() =>
            {
                var listView = ResultsListView;
                var items = listView.Items;
                var results = new List<ResultViewModel>();
                var alreadyRun = false;

                foreach (var item in items)
                {
                    var time = item.FindTextBox("TimeTextBox");

                    if (!string.IsNullOrWhiteSpace(item.FindLabel("PlacementLabel").Text))
                    {
                        alreadyRun = true;
                    }

                    var result = new ResultViewModel
                    {
                        RaceDetailID = item.FindHiddenField("RaceDetailIDField").Value.ToString().ToInt(),
                        Name = item.FindLabel("NameLabel").Text,
                        Time = TimeSpan.Parse(item.FindTextBox("TimeTextBox").Text),
                        Penalty = (item.FindControl("PenaltyDDL") as DropDownList).SelectedValue.ToString().ToInt(),
                    };

                    results.Add(result);
                }

                if (alreadyRun)
                {
                    
                }

                var controller = new RacingController();

                controller.UpdateResults(results);

                listView.DataBind();
            }, "Update Successful", "Results Updated");
        }

        protected void BackButton_Command(object sender, CommandEventArgs e)
        {
            Response.Redirect("Registration.aspx");
        }
    }
}