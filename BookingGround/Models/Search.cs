using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BookingGround.Models
{
    public class Search
    {
        public List<tblDistrict> district;
        public List<tblMasterGround> master;
        public List<tblChildGround> childs;
        public List<String> users;


        public int selectDistrict = 0;
        public int number5 = 0;
        public int number7 = 0;
        public int number9 = 0;
        public int number11 = 0;
    }
}