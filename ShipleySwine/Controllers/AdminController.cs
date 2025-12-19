using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ShipleySwine.Controllers
{
    public class AdminController : Controller
    {
        // GET: Admin
        public ActionResult Index()
        {

            if (Session["Authentication"] != null)
            {
                if (Session["Authentication"].ToString() != "Success")
                {
                    return RedirectToAction("Login", "Authentication");
                }
                else
                {
                    return View();
                }
            }
            else
            {
                return RedirectToAction("Login", "Authentication");
            }
        }

        public ActionResult getBannerForm()
        {
            var dataFile = Server.MapPath("~/Assets/Files/bannerText.txt");
            string fileData = System.IO.File.ReadAllText(dataFile);
            ViewBag.bannerText = fileData;
            return View();
        }

        [HttpPost]
        public ActionResult getBannerForm(string newBannerText)
        {
            try{
                var dataFile = Server.MapPath("~/Assets/Files/bannerText.txt");
                System.IO.File.WriteAllText(@dataFile, newBannerText);
                ViewBag.bannerText = newBannerText;
                ViewBag.submitText = "Save Successful";
                return View();
            } catch(Exception e)
            {
                ViewBag.submitText = e.ToString();
                return View();
            }
            
        }
    }
}