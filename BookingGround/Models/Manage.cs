using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BookingGround.Models
{
    public class Manage
    {
        public List<tblDistrict> district;
        public List<tblMasterGround> masterGround;
        public List<tblChildGround> childGround;
        public List<tblGroundType> type;
        public List<tblInterval> interval;
        public tblMasterGround selectedMasterGround;
        public tblChildGround selectedChildGround;
    }
}