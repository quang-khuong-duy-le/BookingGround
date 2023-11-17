using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using BookingGround.Models;

namespace BookingGround.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Message = "Welcome to ASP.NET MVC!";
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

        public ActionResult About()
        {
            return View();
        }

        public ActionResult Contact()
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

        public ActionResult Underconstruction()
        {
            return View();
        }
    }
}
