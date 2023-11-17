using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BookingGround.Models
{
    public class ManualBooking
    {
        public List<tblMasterGround> MasterGrounds;
        public List<tblGroundType> GroundTypes;
        public List<tblInterval> Intervals;
        public List<tblTimeTable> TimesTable;
        public List<tblChildGround> ChildGrounds;

        public string Date = "";
        public string cName = "";
        public string cPhone = "";

        public int SelectMasterGround = 0;
        public int SelectGroundType = 0;
        public int SelectInterval = 0;
        public int SelectTimeStartId = 0;
        public int SelectChildGround = 0;
                
        public bool mastChange = false; 
        public bool typeChange = false;
        public bool intervalChange = false;
        public bool childChange = false;
        public bool timeChange = false;

        public bool success = false;
    }
}