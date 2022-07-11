using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERaceSystem.VIEWMODELS.Racing
{
    #region Registration
    public class ScheduleViewModel
    {
        // ID
        public int RaceID { get; set; }

        // Display
        public DateTime Time { get; set; }
        public string Competition { get; set; }
        public string Run { get; set; }
        public int Drivers { get; set; }
    }

    public class RosterViewModel
    {
        // IDs
        public int RaceDetailID { get; set; }
        public int MemberID { get; set; }
        public int RaceID { get; set; }

        // Display
        public string Name { get; set; }
        public decimal RaceFee { get; set; }
        public decimal RentalFee { get; set; }
        public int? Placement { get; set; }
        public bool Refunded { get; set; }

        public int? CarClassID { get; set; }
        public string SerialNumber { get; set; }
        public string Comment { get; set; }
        public string Reason { get; set; }

        public int? EmployeeID { get; set; }
    }

    public class DriverViewModel
    {
        public int MemberID { get; set; }
        public String Name { get; set; }
    }


    

    public class CarClassViewModel
    {
        public int ClassID { get; set; }
        public String ClassName { get; set; }
    }

    public class CarViewModel
    {
        public int CarID { get; set; }
        public String SerialNumber { get; set; }
    }
#endregion

    #region Results

    public class ResultViewModel
    {
        // ID
        public int RaceDetailID { get; set; }

        // Display
        public string Name { get; set; }
        public TimeSpan Time { get; set; }
        public int? Penalty { get; set; }
        public int? Placement { get; set; }
    }

    public class PenaltyViewModel
    {
        public int? PenaltyID { get; set; }
        public string Description { get; set; }
    }

    #endregion
}
