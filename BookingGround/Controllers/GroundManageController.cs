using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using BookingGround.Models;
namespace BookingGround.Controllers
{
    public class GroundManageController : Controller
    {
        //
        // GET: /GroundManage/
        [Authorize]
        public ActionResult Index()
        {
            LoginManage lm = new LoginManage();
            var un = Membership.GetUser();
            if (un == null)
            {
                lm.isLogin = false;
            }
            else
            {
                lm.isLogin = true;
            }
            return View(lm);
        }

        //
        // GET: /GroundManage/Details/5

        public ActionResult Details(int id)
        {
            return View();
        }

        //
        // GET: /GroundManage/Create

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /GroundManage/Create

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
        // GET: /GroundManage/Edit/5

        public ActionResult Edit(int id)
        {
            return View();
        }

        //
        // POST: /GroundManage/Edit/5

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
        // GET: /GroundManage/Delete/5

        public ActionResult Delete(int id)
        {
            return View();
        }

        //
        // POST: /GroundManage/Delete/5

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
