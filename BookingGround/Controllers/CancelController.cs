using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BookingGround.Models;

namespace BookingGround.Controllers
{
    public class CancelController : Controller
    {
        //
        // GET: /Cancel/

        [Authorize]
        public ActionResult Index()
        {
            BookingGroundDataContext db = new BookingGroundDataContext();
            Cancel model = new Cancel();
            model.bookingList = db.tblBookingLists.ToList();
            return View(model);
        }

        [Authorize]
        public ActionResult Cancel(int id)
        {
            BookingGroundDataContext db = new BookingGroundDataContext();
            tblBookingList list = (from l in db.tblBookingLists
                                   where l.id == id
                                   select l).SingleOrDefault();
            db.tblBookingLists.DeleteOnSubmit(list);
            db.SubmitChanges();
            return RedirectToAction("Index");
        }
    }
}
