using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BookingGround.Models;

namespace BookingGround.Controllers
{
    public class BookingController : Controller
    {
        //
        // GET: /Booking/

        [Authorize]
        public ActionResult Index()
        {
            BookingGroundDataContext db = new BookingGroundDataContext();
            Booking model = new Booking();
            model.District = db.tblDistricts.ToList();
            model.ChildGround = null;
            model.MasterGround = null;
            model.GroundType = null;
            return View(model);
        }

        [Authorize]
        [HttpPost]
        public ActionResult Index(FormCollection collection)
        {
            BookingGroundDataContext db = new BookingGroundDataContext();
            Booking model = new Booking();
            //
            int distId = 0;
            string distCollection = collection["ddlDistrict"].ToString();
            bool distChange = false;
            if ((distCollection.Length > 2) && distCollection[1] == '+')
            {
                distCollection = distCollection.Substring(2);
                distChange = true;
            }
            bool parseDist = int.TryParse(distCollection, out distId);
            //
            int typeId = 0;
            string typeCollection = collection["ddlGroundType"].ToString();
            bool typeChange = false;
            if ((typeCollection.Length > 2) && typeCollection[1] == '+')
            {
                typeCollection = typeCollection.Substring(2);
                typeChange = true;
            }
            bool parseType = int.TryParse(typeCollection, out typeId);
            //
            int intervalId = 0;
            string intervalCollection = collection["ddlInterval"].ToString();
            bool intervalChange = false;
            if ((intervalCollection.Length > 2) && intervalCollection[1] == '+')
            {
                intervalCollection = intervalCollection.Substring(2);
                intervalChange = true;
            }            
            bool parseInterval = int.TryParse(intervalCollection, out intervalId);
            //
            int mastId = 0;
            string mastCollection = collection["ddlMasterGround"].ToString();
            bool mastChange = false;
            if ((mastCollection.Length > 2) && mastCollection[1] == '+')
            {
                mastCollection = mastCollection.Substring(2);
                mastChange = true;
            }
            bool parseMastId = int.TryParse(mastCollection, out mastId);
            //
            int timeStartId = 0;
            string timeCollection = collection["ddlTimeTable"].ToString();
            bool timeChange = false;
            if ((timeCollection.Length > 2) && timeCollection[1] == '+')
            {
                timeCollection = timeCollection.Substring(2);
                timeChange = true;
            }
            bool parseTimeStartId = int.TryParse(timeCollection, out timeStartId);
            //
            string Date = collection["datepicker"].ToString();
            DateTime d;
            bool parseDate = DateTime.TryParseExact(Date, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out d);
            //

            model.distChange = distChange;
            model.typeChange = typeChange;
            model.intervalChange = intervalChange;
            model.mastChange = mastChange;
            model.timeChange = timeChange;
            model.Date = Date;

            model.District = db.tblDistricts.ToList();

            if (parseDist)
            {
                load_Type(model, distId);
            }
            if (parseDate || Date.Equals(""))
            {
                if (parseType && !Date.Equals(""))
                {
                    load_Mast_Int_WithoutFree(model, typeId, distId, d);
                }

                if (parseInterval && !parseMastId && !parseTimeStartId && !Date.Equals(""))
                {
                    load_Mast_Start_byInt(model, distId, typeId, intervalId, d);
                }

                if (parseMastId && !parseInterval && !parseTimeStartId && !Date.Equals(""))
                {
                    load_Int_Start_byMast(model, mastId, typeId, d);
                }

                if (parseInterval && parseMastId && !parseTimeStartId && !Date.Equals(""))
                {
                    load_Start_ByIntMast(model, distId, typeId, mastId, intervalId, d);
                }

                if (parseInterval && parseTimeStartId && !parseMastId && !Date.Equals(""))
                {
                    load_Mast_ByIntStart(model, distId, typeId, intervalId, timeStartId, d);
                }

                if (parseMastId && parseTimeStartId && !parseInterval && !Date.Equals(""))
                {
                    load_Int_byMastStart(model, typeId, mastId, timeStartId, d);
                }

                if (parseInterval && parseMastId && parseTimeStartId && !Date.Equals(""))
                {
                    bool complete = allLoaded(model, distId, typeId, intervalId, mastId, timeStartId, d, collection);
                    if (complete)
                    {
                        model.Date = Date;
                        model.earlyCheck = true;
                        TempData["finalModel"] = model;
                        return RedirectToAction("Complete");
                    }
                    else if (model.isGotoBook)
                    {
                        model.earlyCheck = false;
                        TempData["finalModel"] = model;
                        return RedirectToAction("Complete");
                    }
                }
            }
            return View(model);
        }

        //Load DropDownList Method

        public void load_Type(Booking model, int distId)
        {
            BookingGroundDataContext db = new BookingGroundDataContext();

            var MasterGroundIds = (from mg in db.tblMasterGrounds
                                   where mg.districtid == distId
                                   select mg.id).ToList();

            var ChildGroundTypeIds = (from cg in db.tblChildGrounds
                                      where MasterGroundIds.Contains(cg.masterid)
                                      select cg.typeid).ToList();

            model.GroundType = db.tblGroundTypes.Where(gt => ChildGroundTypeIds.Contains(gt.id)).ToList();

            model.SelectDistrictId = distId;
        }

        public void load_Mast_Int_WithoutFree(Booking model, int typeId, int distId, DateTime Date)
        {
            BookingGroundDataContext db = new BookingGroundDataContext();

            var MasterGroundIds = (from mg in db.tblMasterGrounds
                                   where mg.districtid == distId
                                   select mg.id).ToList();

            var ChildGroundIds = (from cg in db.tblChildGrounds
                                  where cg.typeid == typeId && MasterGroundIds.Contains(cg.masterid)
                                  select cg.id).ToList();

            var AllTimes = (from gt in db.tblGroundTimes
                            where ChildGroundIds.Contains(gt.cgroundid)
                            select gt.id).ToList();

            var Booked = (from bl in db.tblBookingLists
                          where AllTimes.Contains(bl.groundtimeid) && bl.date.Equals(Date) && bl.status.Equals("Booking")
                          select bl.groundtimeid).ToList();

            var FreeTimes = AllTimes.Except(Booked);

            var NewChildGroundIds = (from gt in db.tblGroundTimes
                                     where FreeTimes.Contains(gt.id)
                                     select gt.cgroundid).Distinct().ToList();

            var NewMasterGroundIds = (from cg in db.tblChildGrounds
                                      where NewChildGroundIds.Contains(cg.id)
                                      select cg.masterid).ToList();

            model.MasterGround = db.tblMasterGrounds.Where(mg => mg.districtid == distId && NewMasterGroundIds.Contains(mg.id)).ToList();

            var IntervalIds = (from cg in db.tblChildGrounds
                               where NewChildGroundIds.Contains(cg.id)
                               select cg.intervalid).ToList();

            model.Interval = db.tblIntervals.Where(i => IntervalIds.Contains(i.id)).ToList();

            model.SelectGroundTypeId = typeId;
        }

        public void load_Mast_Start_byInt(Booking model, int distId, int typeId, int intervalId, DateTime Date)
        {

            BookingGroundDataContext db = new BookingGroundDataContext();

            var MasterGroundIds = (from mg in db.tblMasterGrounds
                                   where mg.districtid == distId
                                   select mg.id).ToList();

            var ChildGroundIds = (from cg in db.tblChildGrounds
                                  where cg.typeid == typeId && MasterGroundIds.Contains(cg.masterid) && cg.intervalid == intervalId
                                  select cg.id).ToList();

            var AllTimes = (from gt in db.tblGroundTimes
                            where ChildGroundIds.Contains(gt.cgroundid)
                            select gt.id).ToList();

            var Booked = (from bl in db.tblBookingLists
                          where AllTimes.Contains(bl.groundtimeid) && bl.date.Equals(Date) && bl.status.Equals("Booking")
                          select bl.groundtimeid).ToList();

            var AvailableIds = AllTimes.Except(Booked);

            var TimeStartIds = (from gt in db.tblGroundTimes
                                where AvailableIds.Contains(gt.id)
                                select gt.timestartid).Distinct().ToList();

            model.TimeTable = db.tblTimeTables.Where(tb => TimeStartIds.Contains(tb.id)).ToList();

            var ChildFreeIds = (from gt in db.tblGroundTimes
                                where AvailableIds.Contains(gt.id)
                                select gt.cgroundid).Distinct().ToList();

            var NewMasterGroundIds = (from cg in db.tblChildGrounds
                                      where ChildFreeIds.Contains(cg.id)
                                      select cg.masterid).Distinct().ToList();

            model.MasterGround = db.tblMasterGrounds.Where(mg => NewMasterGroundIds.Contains(mg.id)).ToList();

            model.SelectInterval = intervalId;

        }

        public void load_Int_Start_byMast(Booking model, int mastId, int typeId, DateTime Date)
        {
            if (!model.distChange)
            {
                BookingGroundDataContext db = new BookingGroundDataContext();

                var ChildGroundIds = (from cg in db.tblChildGrounds
                                      where cg.masterid == mastId && cg.typeid == typeId
                                      select cg.id).ToList();

                var AllTimes = (from gt in db.tblGroundTimes
                                where ChildGroundIds.Contains(gt.cgroundid)
                                select gt.id).ToList();

                var BookedTimesIds = (from bl in db.tblBookingLists
                                      where AllTimes.Contains(bl.groundtimeid) && bl.date.Equals(Date) && bl.status.Equals("Booking")
                                      select bl.groundtimeid).ToList();

                var FreeTimes = AllTimes.Except(BookedTimesIds);

                var StartTimeFree = (from gt in db.tblGroundTimes
                                     where FreeTimes.Contains(gt.id)
                                     select gt.timestartid).Distinct().ToList();

                model.TimeTable = db.tblTimeTables.Where(tt => StartTimeFree.Contains(tt.id)).ToList();

                var NewChildGroundIds = (from gt in db.tblGroundTimes
                                         where FreeTimes.Contains(gt.id)
                                         select gt.cgroundid).Distinct().ToList();

                var IntervalIds = (from cg in db.tblChildGrounds
                                   where NewChildGroundIds.Contains(cg.id)
                                   select cg.intervalid).Distinct().ToList();

                model.Interval = db.tblIntervals.Where(interval => IntervalIds.Contains(interval.id)).ToList();

                model.SelectMaster = mastId;
            }
        }

        public void load_Start_ByIntMast(Booking model, int distId, int typeId, int mastId, int intervalId, DateTime Date)
        {
            if (!model.distChange)
            {
                BookingGroundDataContext db = new BookingGroundDataContext();

                var ChildGroundIds = (from cg in db.tblChildGrounds
                                      where cg.masterid == mastId && cg.typeid == typeId && cg.intervalid == intervalId
                                      select cg.id).ToList();

                var AllTimes = (from gt in db.tblGroundTimes
                                where ChildGroundIds.Contains(gt.cgroundid)
                                select gt.id).ToList();

                var BookedTimesIds = (from bl in db.tblBookingLists
                                      where AllTimes.Contains(bl.groundtimeid) && bl.date.Equals(Date) && bl.status.Equals("Booking")
                                      select bl.groundtimeid).ToList();

                var FreeTimes = AllTimes.Except(BookedTimesIds);

                var StartTimeFree = (from gt in db.tblGroundTimes
                                     where FreeTimes.Contains(gt.id)
                                     select gt.timestartid).Distinct().ToList();

                model.TimeTable = db.tblTimeTables.Where(tt => StartTimeFree.Contains(tt.id)).ToList();

                model.SelectInterval = intervalId;
                model.SelectMaster = mastId;

                if (model.TimeTable.Count == 0)
                {
                    if (model.intervalChange)
                    {
                        load_Mast_Start_byInt(model, distId, typeId, intervalId, Date);
                        model.SelectInterval = intervalId;
                    }
                    else if (model.mastChange)
                    {
                        load_Int_Start_byMast(model, mastId, typeId, Date);
                        model.SelectMaster = mastId;
                    }
                }
            }
        }

        public void load_Mast_ByIntStart(Booking model, int distId, int typeId, int intervalId, int timeStartId, DateTime Date)
        {
            BookingGroundDataContext db = new BookingGroundDataContext();

            load_Mast_Start_byInt(model, distId, typeId, intervalId, Date);

            var MasterGroundIds = (from mg in db.tblMasterGrounds
                                   where mg.districtid == distId
                                   select mg.id).ToList();

            var ChildGroundIds = (from cg in db.tblChildGrounds
                                  where cg.typeid == typeId && MasterGroundIds.Contains(cg.masterid) && cg.intervalid == intervalId
                                  select cg.id).ToList();

            var AllTimes = (from gt in db.tblGroundTimes
                            where ChildGroundIds.Contains(gt.cgroundid) && gt.timestartid == timeStartId
                            select gt.id).ToList();

            var Booked = (from bl in db.tblBookingLists
                          where AllTimes.Contains(bl.groundtimeid) && bl.date.Equals(Date) && bl.status.Equals("Booking")
                          select bl.groundtimeid).ToList();

            var FreeTimes = AllTimes.Except(Booked);

            var ChildFreeIds = (from gt in db.tblGroundTimes
                                where FreeTimes.Contains(gt.id)
                                select gt.cgroundid).Distinct().ToList();

            var NewMasterGroundIds = (from cg in db.tblChildGrounds
                                      where ChildFreeIds.Contains(cg.id)
                                      select cg.masterid).Distinct().ToList();

            model.MasterGround = db.tblMasterGrounds.Where(mg => NewMasterGroundIds.Contains(mg.id)).ToList();

            model.SelectInterval = intervalId;
            model.SelectTimeStart = timeStartId;

            if (model.MasterGround.Count == 0)
            {
                load_Mast_Start_byInt(model, distId, typeId, intervalId, Date);
                model.SelectTimeStart = 0;
            }
        }

        public void load_Int_byMastStart(Booking model, int typeId, int mastId, int timeStartId, DateTime Date)
        {
            BookingGroundDataContext db = new BookingGroundDataContext();

            load_Int_Start_byMast(model, mastId, typeId, Date);

            var ChildGroundIds = (from cg in db.tblChildGrounds
                                  where cg.masterid == mastId && cg.typeid == typeId
                                  select cg.id).ToList();

            var AllTimes = (from gt in db.tblGroundTimes
                            where ChildGroundIds.Contains(gt.cgroundid) && gt.timestartid == timeStartId
                            select gt.id).ToList();

            var BookedTimesIds = (from bl in db.tblBookingLists
                                  where AllTimes.Contains(bl.groundtimeid) && bl.date.Equals(Date) && bl.status.Equals("Booking")
                                  select bl.groundtimeid).ToList();

            var FreeTimes = AllTimes.Except(BookedTimesIds);

            var NewChildGroundIds = (from gt in db.tblGroundTimes
                                     where FreeTimes.Contains(gt.id)
                                     select gt.cgroundid).Distinct().ToList();

            var IntervalIds = (from cg in db.tblChildGrounds
                               where NewChildGroundIds.Contains(cg.id)
                               select cg.intervalid).Distinct().ToList();

            model.Interval = db.tblIntervals.Where(interval => IntervalIds.Contains(interval.id)).ToList();

            model.SelectMaster = mastId;
            model.SelectTimeStart = timeStartId;

            if (model.Interval.Count == 0)
            {
                load_Int_Start_byMast(model, mastId, typeId, Date);
                model.SelectTimeStart = 0;
            }
        }

        public bool allLoaded(Booking model, int distId, int typeId, int intervalId, int mastId, int timeStartId, DateTime Date, FormCollection collection)
        {
            if (model.timeChange)
            {
                load_Start_ByIntMast(model, distId, typeId, mastId, intervalId, Date);
                load_Mast_Int_WithoutFree(model, typeId, distId, Date);
                model.SelectInterval = intervalId;
                model.SelectMaster = mastId;
                model.SelectTimeStart = timeStartId;
            }
            else if (model.intervalChange)
            {
                load_Mast_Start_byInt(model, distId, typeId, intervalId, Date);
                var isMast = (from m in model.MasterGround
                              where m.id == mastId
                              select m.id).ToList();
                if (isMast.Count > 0)
                {
                    model.SelectMaster = mastId;
                }
                else
                {
                    model.SelectMaster = 0;
                }
                var isTi = (from t in model.TimeTable
                            where t.id == timeStartId
                            select t.id).ToList();
                if (isTi.Count > 0)
                {
                    model.SelectTimeStart = timeStartId;
                }
                else
                {
                    model.SelectTimeStart = 0;
                }
                if (model.SelectTimeStart == 0 && model.SelectMaster != 0)
                {
                    load_Start_ByIntMast(model, distId, typeId, mastId, intervalId, Date);
                    load_Mast_Int_WithoutFree(model, typeId, distId, Date);
                }
                else if (model.SelectTimeStart != 0 && model.SelectMaster == 0)
                {
                    load_Mast_ByIntStart(model, distId, typeId, intervalId, timeStartId, Date);
                }
                model.SelectInterval = intervalId;
            }
            else if (model.mastChange)
            {
                load_Int_Start_byMast(model, mastId, typeId, Date);
                var isIn = (from i in model.Interval
                            where i.id == intervalId
                            select i.id).ToList();
                if (isIn.Count > 0)
                {
                    model.SelectInterval = intervalId;
                }
                else model.SelectInterval = 0;
                var isTi = (from t in model.TimeTable
                            where t.id == timeStartId
                            select t.id).ToList();
                if (isTi.Count > 0)
                {
                    model.SelectTimeStart = timeStartId;
                }
                else
                {
                    model.SelectTimeStart = 0;
                }
                load_Mast_Int_WithoutFree(model, typeId, distId, Date);
                model.SelectMaster = mastId;
                model.SelectTimeStart = timeStartId;
            }
            else if (!model.distChange && !model.typeChange && !model.mastChange && !model.intervalChange && !model.timeChange && distId != 0 && typeId != 0 && intervalId != 0 && mastId != 0 && timeStartId != 0)
            {
                model.isGotoBook = true;
                BookingGroundDataContext db = new BookingGroundDataContext();
                var childs = (from cg in db.tblChildGrounds
                              where cg.masterid == mastId && cg.typeid == typeId && cg.intervalid == intervalId
                              select cg.id).ToList();
                var groundTimeIds = (from gt in db.tblGroundTimes
                                     where childs.Contains(gt.cgroundid) && gt.timestartid == timeStartId
                                     select gt.id).ToList();
                var bookedIds = (from bl in db.tblBookingLists
                                 where groundTimeIds.Contains(bl.groundtimeid) && bl.date.Equals(Date) && bl.status == "Booking"
                                 select bl.groundtimeid).ToList();
                var freeTimeIds = groundTimeIds.Except(bookedIds).ToList();
                if (freeTimeIds.Count() > 0)
                {
                    double price;
                    string p = collection["price"].ToString();
                    string par = p.Substring(5, p.Length - 9);
                    bool parsePrice = double.TryParse(par, out price);
                    model.price = price;

                    var finalGroundTime = freeTimeIds[0];
                    model.SelectGroundTime = finalGroundTime;

                    model.Date = Date.ToString();
                    return true;
                }
                //....
            }
            if (model.SelectMaster != 0 && model.SelectInterval != 0 && model.SelectTimeStart != 0)
            {
                BookingGroundDataContext db = new BookingGroundDataContext();
                var childs = (from cg in db.tblChildGrounds
                              where cg.masterid == model.SelectMaster && cg.typeid == typeId && cg.intervalid == intervalId
                              select cg.id).ToList();
                var groundTimeIds = (from gt in db.tblGroundTimes
                          where childs.Contains(gt.cgroundid) && gt.timestartid == model.SelectTimeStart
                          select gt.id).ToList();
                var bookedIds = (from bl in db.tblBookingLists
                                 where groundTimeIds.Contains(bl.groundtimeid) && bl.date.Equals(Date) && bl.status == "Booking"
                                 select bl.groundtimeid).ToList();
                var freeTimeIds = groundTimeIds.Except(bookedIds).ToList();
                if (freeTimeIds.Count() > 0)
                {
                    var finalGroundTime = freeTimeIds[0];
                    var prices = (from gt in db.tblGroundTimes
                                      where gt.id == finalGroundTime
                                      select gt.price).ToList();
                    var finalPrice = prices[0];
                    model.price = finalPrice;
                }
            }
            return false;
        }
        // GET: /Booking/Complete
        public ActionResult Complete()
        {            
            var model = (Booking)TempData["finalModel"];
            if (model.earlyCheck)
            {
                BookingGroundDataContext db = new BookingGroundDataContext();
                tblBookingList entity = new tblBookingList();
                entity.groundtimeid = model.SelectGroundTime;
                entity.date = DateTime.ParseExact(model.Date, "dd/MM/yyyy", null);
                entity.status = "Booking";
                entity.price = model.price;
                entity.userid = Guid.Parse("fd6b0a84-c1c5-4a79-9614-748ede63cc18");
                db.tblBookingLists.InsertOnSubmit(entity);

                try
                {
                    db.SubmitChanges();
                    model.success = true;
                }
                catch (Exception)
                {
                    model.success = false;
                }
            }
            else
            {
                model.success = false;
            }
            return View(model);
        }


        //
        // GET: /Booking/Details/5

        public ActionResult Details(int id)
        {
            return View();
        }

        //
        // GET: /Booking/Create

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /Booking/Create

        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        //
        // GET: /Booking/Edit/5

        public ActionResult Edit(int id)
        {
            return View();
        }

        //
        // POST: /Booking/Edit/5

        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        //
        // GET: /Booking/Delete/5

        public ActionResult Delete(int id)
        {
            return View();
        }

        //
        // POST: /Booking/Delete/5

        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
