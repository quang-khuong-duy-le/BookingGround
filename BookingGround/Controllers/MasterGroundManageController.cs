using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BookingGround.Models;
using System.Web.Security;

namespace BookingGround.Controllers
{
    public class MasterGroundManageController : Controller
    {
        //
        // GET: /MasterGroundManage/
        [Authorize(Roles = "admin")]
        public ActionResult Index()
        {
            BookingGroundDataContext db = new BookingGroundDataContext();
            Manage manage = new Manage();
            manage.masterGround = db.tblMasterGrounds.ToList();
            return View(manage);
        }
        [Authorize(Roles = "admin")]
        public ActionResult Create()
        {
            BookingGroundDataContext db = new BookingGroundDataContext();
            Manage manage = new Manage();
            manage.district = db.tblDistricts.ToList();
            return View(manage);
        }

        //
        // POST: /MasterGroundManage/Create

        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                BookingGroundDataContext db = new BookingGroundDataContext();
                tblMasterGround master = new tblMasterGround();

                master = (from mg in db.tblMasterGrounds
                          where mg.name == collection["inputName"].ToString()
                          select mg).FirstOrDefault();
                if (master == null)
                {
                    tblMasterGround master_add = new tblMasterGround();
                    master_add.name = collection["inputName"].ToString();
                    Guid userGuid = (Guid)Membership.GetUser().ProviderUserKey;
                    master_add.owner = userGuid;
                    master_add.districtid = int.Parse(collection["inputDistrict"].ToString());
                    master_add.address = collection["inputAddr"].ToString();
                    master_add.description = collection["inputDesc"].ToString();
                    db.tblMasterGrounds.InsertOnSubmit(master_add);
                    db.SubmitChanges();
                    return RedirectToAction("Index");
                }
                else return View("CreateFail");
            }
            catch
            {
                return View("CreateFail");
            }
        }

        //
        // GET: /MasterGroundManage/Details/5
        [Authorize(Roles = "admin")]
        public ActionResult Update(string id)
        {
            BookingGroundDataContext db = new BookingGroundDataContext();
            Manage manage = new Manage();
            manage.district = db.tblDistricts.ToList();
            manage.selectedMasterGround = (from mg in db.tblMasterGrounds
                                           where mg.id == int.Parse(id)
                                           select mg).FirstOrDefault();
            return View(manage);
        }

        [HttpPost]
        public ActionResult Update(FormCollection collection)
        {
            BookingGroundDataContext db = new BookingGroundDataContext();
            Manage manage = new Manage();
            manage.district = db.tblDistricts.ToList();
            manage.selectedMasterGround = (from mg in db.tblMasterGrounds
                                           where mg.id == int.Parse(collection["inputID"].ToString())
                                           select mg).FirstOrDefault();
            int id = int.Parse(collection["inputID"].ToString());
            tblMasterGround master = (from mg in db.tblMasterGrounds
                                      where mg.id == id
                                      select mg).FirstOrDefault();
            master.name = collection["inputName"].ToString();
            master.districtid = int.Parse(collection["inputDistrict"].ToString());
            master.address = collection["inputAddr"].ToString();
            master.description = collection["inputDesc"].ToString();
            db.SubmitChanges();

            return View("UpdateSuccess");
        }
        [Authorize(Roles = "admin")]
        public ActionResult Delete(int id)
        {
            BookingGroundDataContext db = new BookingGroundDataContext();
            tblMasterGround master = (from mg in db.tblMasterGrounds
                                      where mg.id == id
                                      select mg).FirstOrDefault();
            try
            {
                db.tblMasterGrounds.DeleteOnSubmit(master);
                db.SubmitChanges();
                return View("DeleteSuccess");
            }
            catch
            {
                return View("DeleteFail");
            }

        }


        public ActionResult Details(int id)
        {
            return View();
        }

        //
        // GET: /MasterGroundManage/Create



        //
        // GET: /MasterGroundManage/Edit/5

        public ActionResult Edit(int id)
        {
            return View();
        }

        //
        // POST: /MasterGroundManage/Edit/5

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
        // GET: /MasterGroundManage/Delete/5



        //
        // POST: /MasterGroundManage/Delete/5

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
