using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BookingGround.Models;
using System.Web.Security;

namespace BookingGround.Controllers
{
    public class ManualBookingController : Controller
    {
        //
        // GET: /ManualBooking/
        [Authorize(Roles = "groundmng")]
        public ActionResult Index()
        {
            BookingGroundDataContext db = new BookingGroundDataContext();
            ManualBooking model = new ManualBooking();
            Guid uid = (Guid)Membership.GetUser().ProviderUserKey;
            model.MasterGrounds = db.tblMasterGrounds.Where(mg => mg.owner == uid).ToList();
            return View(model);
        }

        [Authorize(Roles = "groundmng")]
        [HttpPost]
        public ActionResult Index(FormCollection collection)
        {
            BookingGroundDataContext db = new BookingGroundDataContext();
            ManualBooking model = new ManualBooking();
            ///////////////////////////////////////////////////////////////////////////////////////
            int mastId = 0;
            string mastCollection = collection["ddlMasterGround"].ToString();
            bool mastChange = false;
            if ((mastCollection.Length > 2) && mastCollection[1] == '+')
            {
                mastCollection = mastCollection.Substring(2);
                mastChange = true;
            }
            bool parseMast = int.TryParse(mastCollection, out mastId);
            ///////////////////////////////////////////////////////////////////////////////////////
            string Date = collection["datepicker"].ToString();
            DateTime d;
            bool parseDate = DateTime.TryParseExact(Date, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out d);
            ///////////////////////////////////////////////////////////////////////////////////////
            int typeId = 0;
            string typeCollection = collection["ddlGroundType"].ToString();
            bool typeChange = false;
            if ((typeCollection.Length > 2) && typeCollection[1] == '+')
            {
                typeCollection = typeCollection.Substring(2);
                typeChange = true;
            }
            bool parseType = int.TryParse(typeCollection, out typeId);
            ///////////////////////////////////////////////////////////////////////////////////////
            int intervalId = 0;
            string intervalCollection = collection["ddlInterval"].ToString();
            bool intervalChange = false;
            if ((intervalCollection.Length > 2) && intervalCollection[1] == '+')
            {
                intervalCollection = intervalCollection.Substring(2);
                intervalChange = true;
            }
            bool parseInterval = int.TryParse(intervalCollection, out intervalId);
            ///////////////////////////////////////////////////////////////////////////////////////
            int timeStartId = 0;
            string timeCollection = collection["ddlTimeTable"].ToString();
            bool timeChange = false;
            if ((timeCollection.Length > 2) && timeCollection[1] == '+')
            {
                timeCollection = timeCollection.Substring(2);
                timeChange = true;
            }
            bool parseTimeStartId = int.TryParse(timeCollection, out timeStartId);
            ///////////////////////////////////////////////////////////////////////////////////////
            int childId = 0;
            string childCollection = collection["ddlChildGround"].ToString();
            bool childChange = false;
            if ((childCollection.Length > 2) && childCollection[1] == '+')
            {
                childCollection = childCollection.Substring(2);
                childChange = true;
            }
            bool parseChildId = int.TryParse(childCollection, out childId);
            ///////////////////////////////////////////////////////////////////////////////////////
            bool hasCname = false;
            model.cName = collection["cName"].ToString();
            if (!model.cName.Equals(""))
            {
                hasCname = true;
            }

            bool hasCphone = false;
            model.cPhone = collection["cPhone"].ToString();
            if (!model.cPhone.Equals(""))
            {
                hasCphone = true;
            }
            ///////////////////////////////////////////////////////////////////////////////////////
            model.mastChange = mastChange;
            model.typeChange = typeChange;
            model.intervalChange = intervalChange;
            model.childChange = childChange;
            model.timeChange = timeChange;
            if (!parseDate)
            {
                model.Date = "";
            }
            else
            {
                model.Date = Date;
            }
            ///////////////////////////////////////////////////////////////////////////////////////

            Guid uid = (Guid)Membership.GetUser().ProviderUserKey;
            model.MasterGrounds = db.tblMasterGrounds.Where(mg => mg.owner == uid).ToList();

            if (parseMast)
            {
                if (parseMast && parseDate)
                {
                    model.SelectMasterGround = mastId;
                    loadType(db, model);
                }

                if (parseType && parseDate)
                {
                    model.SelectGroundType = typeId;
                    loadIntervalMast(db, model);


                    if (parseInterval && !parseChildId && !parseTimeStartId && parseDate)
                    {
                        model.SelectInterval = intervalId;
                        loadChildsTimes_ByInterval(db, model);
                    }

                    if (parseChildId && !parseInterval && !parseTimeStartId && parseDate)
                    {
                        model.SelectChildGround = childId;
                        loadIntervalTimes_ByChilds(db, model);
                    }

                    if (parseInterval && parseChildId && !parseTimeStartId && parseDate)
                    {
                        model.SelectInterval = intervalId;
                        model.SelectChildGround = childId;
                        loadTimes_ByIntervalChild(db, model);
                    }

                    if (parseInterval && parseTimeStartId && !parseChildId && parseDate)
                    {
                        model.SelectInterval = intervalId;
                        model.SelectTimeStartId = timeStartId;
                        loadChilds_ByIntervalTime(db, model);
                    }

                    if (parseChildId && parseTimeStartId && !parseInterval && parseDate)
                    {
                        model.SelectChildGround = childId;
                        model.SelectTimeStartId = timeStartId;
                        loadIntervalTimes_ByChilds(db, model);
                    }

                    if (parseInterval && parseChildId && parseTimeStartId && parseDate)
                    {
                        model.SelectInterval = intervalId;
                        model.SelectChildGround = childId;
                        model.SelectTimeStartId = timeStartId;

                        bool complete = allLoaded(db, model);
                        if (complete)
                        {
                            TempData["finalModel"] = model;
                            return RedirectToAction("Complete");
                        }
                    }
                }
            }

            return View(model);
        }

        //ManualBooking Method
        public void loadType(BookingGroundDataContext db, ManualBooking model)
        {
            var childs = db.tblChildGrounds.Where(cg => cg.masterid == model.SelectMasterGround).Select(c => c.id).ToList();

            var AllTimes = db.tblGroundTimes.Where(gt => childs.Contains(gt.cgroundid)).Select(t => t.id).ToList();

            var Booked = db.tblBookingLists.Where(bl => AllTimes.Contains(bl.groundtimeid) && bl.date.Equals(DateTime.ParseExact(model.Date, "dd/MM/yyyy", null)) && bl.status == "Booking").Select(t => t.groundtimeid).ToList();

            var FreeTimes = AllTimes.Except(Booked);

            var newChilds = db.tblGroundTimes.Where(gt => FreeTimes.Contains(gt.id)).Select(c => c.cgroundid).ToList();

            var typeIds = db.tblChildGrounds.Where(c => newChilds.Contains(c.id)).Select(c => c.typeid).ToList();

            model.GroundTypes = db.tblGroundTypes.Where(gt => typeIds.Contains(gt.id)).ToList();
        }


        public void loadIntervalMast(BookingGroundDataContext db, ManualBooking model)
        {
            var childs = db.tblChildGrounds.Where(cg => cg.masterid == model.SelectMasterGround && cg.typeid == model.SelectGroundType).Select(c => c.id).ToList();

            var AllTimes = db.tblGroundTimes.Where(gt => childs.Contains(gt.cgroundid)).Select(t => t.id).ToList();

            var Booked = db.tblBookingLists.Where(bl => AllTimes.Contains(bl.groundtimeid) && bl.date.Equals(DateTime.ParseExact(model.Date, "dd/MM/yyyy", null)) && bl.status == "Booking").Select(t => t.groundtimeid).ToList();

            var FreeTimes = AllTimes.Except(Booked);

            var newChilds = db.tblGroundTimes.Where(gt => FreeTimes.Contains(gt.id)).Select(c => c.cgroundid).ToList();

            var Intervals = db.tblChildGrounds.Where(cg => newChilds.Contains(cg.id)).Select(i => i.intervalid).ToList();

            model.Intervals = db.tblIntervals.Where(i => Intervals.Contains(i.id)).ToList();

            model.ChildGrounds = db.tblChildGrounds.Where(cg => newChilds.Contains(cg.id)).ToList();
        }


        public void loadChildsTimes_ByInterval(BookingGroundDataContext db, ManualBooking model)
        {
            var childs = model.ChildGrounds.Where(c => c.intervalid == model.SelectInterval).Select(c => c.id).ToList();

            var AllTimes = db.tblGroundTimes.Where(gt => childs.Contains(gt.cgroundid)).Select(t => t.id).ToList();

            var Booked = db.tblBookingLists.Where(bl => AllTimes.Contains(bl.groundtimeid) && bl.date.Equals(DateTime.ParseExact(model.Date, "dd/MM/yyyy", null)) && bl.status == "Booking").Select(t => t.groundtimeid).ToList();

            var FreeTimes = AllTimes.Except(Booked);

            var TimeIds = db.tblGroundTimes.Where(gt => FreeTimes.Contains(gt.id)).Select(t => t.timestartid).ToList();

            model.TimesTable = db.tblTimeTables.Where(tt => TimeIds.Contains(tt.id)).ToList();

            model.ChildGrounds = model.ChildGrounds.Where(cg => cg.intervalid == model.SelectInterval).ToList();
        }


        public void loadIntervalTimes_ByChilds(BookingGroundDataContext db, ManualBooking model)
        {
            var AllTimes = db.tblGroundTimes.Where(gt => gt.cgroundid == model.SelectChildGround).Select(t => t.id).ToList();

            var Booked = db.tblBookingLists.Where(bl => AllTimes.Contains(bl.groundtimeid) && bl.date.Equals(DateTime.ParseExact(model.Date, "dd/MM/yyyy", null)) && bl.status == "Booking").Select(t => t.groundtimeid).ToList();

            var FreeTimes = AllTimes.Except(Booked);

            var TimeIds = db.tblGroundTimes.Where(gt => FreeTimes.Contains(gt.id)).Select(t => t.timestartid).ToList();

            model.TimesTable = db.tblTimeTables.Where(tt => TimeIds.Contains(tt.id)).ToList();

            model.Intervals = model.Intervals.Where(i => i.id == model.ChildGrounds.Where(cg => cg.id == model.SelectChildGround).Select(cg => cg.intervalid).First()).ToList();
        }


        public void loadTimes_ByIntervalChild(BookingGroundDataContext db, ManualBooking model)
        {
            var childs1 = model.ChildGrounds.Where(c => c.intervalid == model.SelectInterval && c.id == model.SelectChildGround).Select(c => c.id).ToList();

            if (childs1.Count > 0)
            {
                var childs = childs1.First();

                var AllTimes = db.tblGroundTimes.Where(t => t.cgroundid == childs).Select(t => t.id).ToList();

                var Booked = db.tblBookingLists.Where(bl => AllTimes.Contains(bl.groundtimeid) && bl.date.Equals(DateTime.ParseExact(model.Date, "dd/MM/yyyy", null)) && bl.status == "Booking").Select(t => t.groundtimeid).ToList();

                var FreeTimes = AllTimes.Except(Booked);

                var TimeIds = db.tblGroundTimes.Where(gt => FreeTimes.Contains(gt.id)).Select(t => t.timestartid).ToList();

                model.TimesTable = db.tblTimeTables.Where(tt => TimeIds.Contains(tt.id)).ToList();

                loadIntervalMast(db, model);
            }
            else
            {
                if (model.intervalChange)
                {
                    loadChildsTimes_ByInterval(db, model);
                    model.SelectChildGround = 0;
                }
                else
                {
                    loadIntervalTimes_ByChilds(db, model);
                    model.SelectInterval = 0;
                }
            }
        }


        public void loadChilds_ByIntervalTime(BookingGroundDataContext db, ManualBooking model)
        {
            loadChildsTimes_ByInterval(db, model);

            var childs = model.ChildGrounds.Where(c => c.intervalid == model.SelectInterval).Select(c => c.id).ToList();

            var AllTimes = db.tblGroundTimes.Where(gt => childs.Contains(gt.cgroundid) && gt.timestartid == model.SelectTimeStartId).Select(gt => gt.id).ToList();

            var Booked = db.tblBookingLists.Where(bl => AllTimes.Contains(bl.groundtimeid) && bl.date.Equals(DateTime.ParseExact(model.Date, "dd/MM/yyyy", null)) && bl.status == "Booking").Select(t => t.groundtimeid).ToList();

            var FreeTimes = AllTimes.Except(Booked);

            var newChilds = db.tblGroundTimes.Where(gt => FreeTimes.Contains(gt.id)).Select(gt => gt.cgroundid).ToList();

            model.ChildGrounds = model.ChildGrounds.Where(c => newChilds.Contains(c.id)).ToList();
        }


        public void loadIntervals_ByChildTime(BookingGroundDataContext db, ManualBooking model)
        {
            loadIntervalTimes_ByChilds(db, model);

            var child = model.ChildGrounds.Where(cg => cg.id == model.SelectChildGround).First();

            model.Intervals = model.Intervals.Where(i => i.id == child.intervalid).ToList();
        }

        public bool allLoaded(BookingGroundDataContext db, ManualBooking model)
        {
            if (model.childChange)
            {
                loadIntervalTimes_ByChilds(db, model);
            }
            else if (model.intervalChange)
            {
                loadChildsTimes_ByInterval(db, model);
            }
            else if (model.timeChange)
            {
                loadTimes_ByIntervalChild(db, model);
            }
            else if (!model.mastChange && !model.typeChange && !model.intervalChange && !model.childChange && !model.timeChange && !model.cName.Equals("") && !model.cPhone.Equals(""))
            {
                if (model.SelectMasterGround != 0 && model.SelectGroundType != 0 && model.SelectInterval != 0 && model.SelectChildGround != 0 && model.SelectTimeStartId != 0)
                {
                    return true;
                }
            }
            if (model.cName.Equals("") || model.cPhone.Equals(""))
            {
                loadTimes_ByIntervalChild(db, model);
            }
            return false;
        }

        // GET: /ManualBooking/Complete
        public ActionResult Complete()
        {
            var model = (ManualBooking)TempData["finalModel"];

            BookingGroundDataContext db = new BookingGroundDataContext();

            var gtEntity = db.tblGroundTimes.Where(gt => gt.cgroundid == model.SelectChildGround && gt.timestartid == model.SelectTimeStartId).First();

            tblBookingList entity = new tblBookingList();
            entity.groundtimeid = gtEntity.id;
            entity.date = DateTime.ParseExact(model.Date, "dd/MM/yyyy", null);
            entity.status = "Booking";
            entity.price = gtEntity.price;
            entity.userid = model.MasterGrounds.First().owner;
            entity.cname = model.cName;
            entity.cphone = model.cPhone;

            var lastValid = db.tblBookingLists.Where(bl => bl.date.Equals(entity.date) && bl.groundtimeid == entity.groundtimeid && bl.status.Equals("Booking")).ToList();


            if (lastValid.Count == 0)
            {
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
        // GET: /ManualBooking/Details/5

        public ActionResult Details(int id)
        {
            return View();
        }

        //
        // GET: /ManualBooking/Create

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /ManualBooking/Create

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
        // GET: /ManualBooking/Edit/5

        public ActionResult Edit(int id)
        {
            return View();
        }

        //
        // POST: /ManualBooking/Edit/5

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
        // GET: /ManualBooking/Delete/5

        public ActionResult Delete(int id)
        {
            return View();
        }

        //
        // POST: /ManualBooking/Delete/5

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
