using ShipleySwine.Models;
using ShipleySwine.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ShipleySwine.Controllers
{
    public class AuthenticationController : Controller
    {
        // GET: Authentication
        public ActionResult Login()
        {
            ViewBag.Message = TempData["Message"];
            return View();
        }

        [HttpPost]
        public ActionResult Login(AuthenticationViewModel vm)
        {
            //if (ModelState.IsValid)
            //{
            //    if (vm.user.userName == System.Configuration.ConfigurationManager.AppSettings["Admin"].ToString() && vm.user.password == System.Configuration.ConfigurationManager.AppSettings["adminPass"].ToString())
            //    {
            //        Session["Authentication"] = "Success";
            //        return RedirectToAction("Index", "Admin");
            //    }
            //    else
            //    {
            //        TempData["Message"] = "Login failed, Incorrect username/password";
            //        return Redirect("/ShipleySwine/Authentication/Login");
            //        //return RedirectToAction("Login", "Authentication");
            //    }
            //}
            //else
            //{
            //    //return RedirectToAction("Login", "Authentication");
            //    return Redirect("/ShipleySwine/Authentication/Login");
            //}

            if (ModelState.IsValid)
            {
                if (vm.user.userName == System.Configuration.ConfigurationManager.AppSettings["Admin"].ToString() && vm.user.password == System.Configuration.ConfigurationManager.AppSettings["adminPass"].ToString())
                {
                    Session["Authentication"] = "Success";
                    return RedirectToAction("Index", "Admin");
                }
                else
                {
                    TempData["Message"] = "Login failed, Incorrect username/password";
                    ViewBag.Message = "Login failed, Incorrect username/password";
                    return View();
                }
            }
            else
            {
                return View();
            }
        }
    }
}