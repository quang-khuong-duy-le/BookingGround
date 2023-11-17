using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BookingGround.Models
{
    public class Booking
    {
        public List<tblDistrict> District { get; set; }
        public List<tblGroundType> GroundType { get; set; }
        public List<tblMasterGround> MasterGround { get; set; }
        public List<tblChildGround> ChildGround { get; set; }
        public List<tblInterval> Interval { get; set; }
        public List<tblTimeTable> TimeTable { get; set; }

        public int SelectDistrictId = 0;
        public int SelectGroundTypeId = 0;
        public int SelectInterval = 0;
        public int SelectMaster = 0;
        public int SelectTimeStart = 0;

        public bool distChange;
        public bool typeChange;
        public bool intervalChange;
        public bool mastChange;
        public bool timeChange;

        public string Date = "";
        public double price = 0.0;
        public int SelectGroundTime = 0;

        public bool isGotoBook = false;
        public bool earlyCheck = false;
        public bool success = false;
    }
}