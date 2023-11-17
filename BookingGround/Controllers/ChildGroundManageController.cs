using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BookingGround.Models;

namespace BookingGround.Controllers
{
    public class ChildGroundManageController : Controller
    {
        //
        // GET: /ChildGroundManage/
        [Authorize(Roles = "groundmng, admin")]
        public ActionResult Index()
        {
            BookingGroundDataContext db = new BookingGroundDataContext();
            Manage manage = new Manage();
            manage.selectedMasterGround = (from mg in db.tblMasterGrounds
                                           where mg.id == 1
                                           select mg).SingleOrDefault();
            manage.masterGround = db.tblMasterGrounds.ToList();
            manage.childGround = (from cg in db.tblChildGrounds
                                  where cg.masterid == 1
                                  select cg).ToList();

            return View(manage);
        }

        [Authorize(Roles = "groundmng, admin")]
        [HttpPost]
        public ActionResult Index(FormCollection collection)
        {
            BookingGroundDataContext db = new BookingGroundDataContext();
            Manage manage = new Manage();
            manage.selectedMasterGround = (from mg in db.tblMasterGrounds
                                           where mg.name.Equals(collection["master_ground"].ToString())
                                           select mg).SingleOrDefault();
            manage.masterGround = db.tblMasterGrounds.ToList();
            manage.childGround = (from cg in db.tblChildGrounds
                                  where cg.masterid == manage.selectedMasterGround.id
                                  select cg).ToList();

            return View(manage);
        }

        [Authorize(Roles = "groundmng, admin")]
        public ActionResult Create()
        {
            BookingGroundDataContext db = new BookingGroundDataContext();
            Manage manage = new Manage();
            manage.masterGround = db.tblMasterGrounds.ToList();
            manage.type = db.tblGroundTypes.ToList();
            manage.interval = db.tblIntervals.ToList();
            return View(manage);
        }

        //
        // POST: /ChildGroundManage/Create

        [Authorize(Roles = "groundmng, admin")]
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                BookingGroundDataContext db = new BookingGroundDataContext();
                Manage manage = new Manage();
                tblChildGround child = new tblChildGround();
                child.masterid = int.Parse(collection["inputMaster"].ToString());
                child.intervalid = int.Parse(collection["inputInterval"].ToString());
                child.typeid = int.Parse(collection["inputType"].ToString());
                int no = 0;
                var child_list = (from cg in db.tblChildGrounds
                                  where cg.masterid == child.masterid
                                  select cg).ToList();
                foreach (tblChildGround c in child_list)
                {
                    if (c.no >= no) no = c.no + 1;
                }
                child.no = no;
                db.tblChildGrounds.InsertOnSubmit(child);
                db.SubmitChanges();
                return RedirectToAction("Index");
            }
            catch
            {
                return View("CreateFail");
            }
        }

        [Authorize(Roles = "groundmng, admin")]
        public ActionResult Delete(int id)
        {
            try
            {
                BookingGroundDataContext db = new BookingGroundDataContext();
                Manage manage = new Manage();
                tblChildGround child = new tblChildGround();
                child = (from cg in db.tblChildGrounds
                         where cg.id == id
                         select cg).SingleOrDefault();
                db.tblChildGrounds.DeleteOnSubmit(child);
                List<tblGroundTime> gtime_list = new List<tblGroundTime>();
                gtime_list = (from gt in db.tblGroundTimes
                         where gt.cgroundid == id
                         select gt).ToList();
                db.tblGroundTimes.DeleteAllOnSubmit(gtime_list);
                db.SubmitChanges();
                return RedirectToAction("Index");
            }
            catch
            {
                return View("DeleteFail");
            }
        }

        [Authorize(Roles = "groundmng, admin")]
        public ActionResult Update(string id)
        {
            BookingGroundDataContext db = new BookingGroundDataContext();
            Manage manage = new Manage();
            manage.selectedChildGround = (from cg in db.tblChildGrounds
                                          where cg.id == int.Parse(id)
                                          select cg).FirstOrDefault();
            manage.type = db.tblGroundTypes.ToList();
            manage.interval = db.tblIntervals.ToList();
            return View(manage);
        }

        [Authorize(Roles = "groundmng, admin")]
        public ActionResult Update(FormCollection collection)
        {
            try
            {
                BookingGroundDataContext db = new BookingGroundDataContext();
                Manage manage = new Manage();
                tblChildGround child = (from cg in db.tblChildGrounds
                                        where cg.id == int.Parse(collection["inputID"].ToString())
                                        select cg).SingleOrDefault();
                child.intervalid = int.Parse(collection["inputInterval"].ToString());
                child.typeid = int.Parse(collection["inputType"].ToString());
                db.SubmitChanges();
                return View("UpdateSuccess");
            }
            catch
            {
                return View("UpdateFail");
            }
        }
        //
        // GET: /ChildGroundManage/Details/5

        public ActionResult Details(int id)
        {
            return View();
        }

        //
        // GET: /ChildGroundManage/Create



        //
        // GET: /ChildGroundManage/Edit/5

        public ActionResult Edit(int id)
        {
            return View();
        }

        //
        // POST: /ChildGroundManage/Edit/5

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
        // GET: /ChildGroundManage/Delete/5



        //
        // POST: /ChildGroundManage/Delete/5

        
    }
}
