using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BookingGround.Models;

namespace BookingGround.Controllers
{
    public class SearchController : Controller
    {
        //
        // GET: /Search/
        [Authorize]
        public ActionResult Index()
        {
            BookingGroundDataContext db = new BookingGroundDataContext();
            Search model = new Search();
            model.district = db.tblDistricts.ToList();
            return View(model);
        }


        [HttpPost]
        public ActionResult Index(FormCollection collection)
        {
            BookingGroundDataContext db = new BookingGroundDataContext();
            Search model = new Search();
            model.district = db.tblDistricts.ToList();

            int districtID = int.Parse(collection["ddldistrict"].ToString());
            model.selectDistrict = districtID;
            model.master = db.tblMasterGrounds.Where(b => b.districtid == districtID).ToList();

            var mastIds = (from m in model.master
                           select m.id).ToList();

            var childs = (from c in db.tblChildGrounds
                          where mastIds.Contains(c.masterid)
                          select c).ToList();

            model.childs = childs;



            return View(model);
        }
        //
        // GET: /Search/Details/5

        public ActionResult Details(int id)
        {
            return View();
        }

        //
        // GET: /Search/Create

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /Search/Create

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
        // GET: /Search/Edit/5

        public ActionResult Edit(int id)
        {
            return View();
        }

        //
        // POST: /Search/Edit/5

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
        // GET: /Search/Delete/5

        public ActionResult Delete(int id)
        {
            return View();
        }

        //
        // POST: /Search/Delete/5

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