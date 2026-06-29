using ShipleySwine.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace ShipleySwine.Controllers
{
    public class HomeController : Controller
    {
        private ShipleySwineContext db = new ShipleySwineContext();

        public ActionResult Index()
        {
            var fourMonthsAgo = DateTime.Now.AddDays(-120);
            Debug.WriteLine(fourMonthsAgo);
            var newBoars = db.Boars
                .Where(boar => boar.CreateDate > fourMonthsAgo)
                .ToList();

            // The homepage still needs a boar after the 120-day "new" window expires.
            // Fall back to the most recently added boar instead of calling Random.Next(0).
            if (newBoars.Count == 0)
            {
                var latestBoar = db.Boars
                    .OrderByDescending(boar => boar.CreateDate)
                    .ThenByDescending(boar => boar.Boar_Id)
                    .FirstOrDefault();

                if (latestBoar != null)
                {
                    newBoars.Add(latestBoar);
                }
            }

            Random rand = new Random();
            int randomBoar = newBoars.Count > 1 ? rand.Next(newBoars.Count) : 0;
            HomePageViewModel vm = new HomePageViewModel(newBoars, randomBoar);
            foreach(var nboar in newBoars)
            {
                Debug.WriteLine(nboar.Name);
            }

            Debug.WriteLine(randomBoar);
            var boars = db.Boars.Count();
            var gilts = db.BredGilts.Count();
            Debug.WriteLine(boars);
            ViewBag.BoarCount = boars;
            ViewBag.GiltCount = gilts;
            //var boarCount = boars.Count();
            return View(vm);
        }

        public ActionResult About()
        {
            ViewData["AboutImage"] = Directory.EnumerateFiles(Server.MapPath("~/Assets/AboutUs/")).Select(fn => "~/Assets/AboutUs/"+ Path.GetFileName(fn)); ;
            foreach (var image in (IEnumerable<string>)ViewData["AboutImage"])
            {
                Debug.WriteLine("@@@@@@@@@@@@@@@@@@@@@" + image + "@@@@@@@@@@@@@@@@@@@@@");
            }
            ViewBag.Message = "Your application description page.";
            
            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult Catalog()
        {
            return View();
        }

        //[HttpPost]
        //public ActionResult Email(ContactEmailViewModel vm)
        //{
        //    string subject = vm.subject;
        //    Debug.WriteLine("@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@----EMAIL"+vm.email+"----@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@");
        //    string emailBody = "Message From Contact Us:<br><br>Name: " + vm.fullname + "<br>" + "Email: " + vm.email+"<br>"+"Phone: "+vm.phone+"<br><br><br>Comments:<br>" +vm.comments;
        //    if (SendEmail(vm.email, subject, emailBody) == true)
        //    {
        //        return RedirectToAction("EmailSuccess", "Home");
        //    }
        //    else
        //    {
        //        return RedirectToAction("EmailFailure", "Home");
        //    }
        //}

        public JsonResult Email(string fullname, string email, string phone, string subject, string comments)
        {
            string subjectt = subject;
            Debug.WriteLine("@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@----EMAIL" + email + "----@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@");
            string emailBody = "Message From Contact Us:<br><br>Name: " + fullname + "<br>" + "Email: " + email + "<br>" + "Phone: " + phone + "<br><br><br>Comments:<br>" + comments;
            if (SendEmail(email, subjectt, emailBody) == "true")
            {
                return Json("true");
            }
            else
            {
                return Json(SendEmail(email, subjectt, emailBody));
            }
        }

        public ActionResult EmailSuccess()
        {
            return View();
        }

        public ActionResult EmailFailure()
        {
            return View();
        }

        public ActionResult Winners()
        {
            return View();
        }

        public ActionResult Certificate()
        {
            return View();
        }

        public String SendEmail(string fromEmail, string subject, string emailBody)
        {
            try
            {
                //string senderEmail = System.Configuration.ConfigurationManager.AppSettings["SenderEmail"].ToString();
                //string senderPassword = System.Configuration.ConfigurationManager.AppSettings["SenderPassword"].ToString();
                string ToEmail = "andrew.richardson.667@gmail.com";

                SmtpClient client = new SmtpClient("relay-hosting.secureserver.net", 25);
                client.EnableSsl = false;
                client.Timeout = 100000;
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                client.UseDefaultCredentials = false;

                MailMessage message = new MailMessage();
                message.From = new MailAddress("office@shipleyswine.com");
                message.To.Add(new MailAddress("Josie_shipley@yahoo.com"));
                message.To.Add(new MailAddress("Buckeyeboot@yahoo.com"));
                message.To.Add(new MailAddress("Shipleyswine@yahoo.com"));
                message.To.Add(new MailAddress("shipleywebemail@gmail.com"));
                message.To.Add(new MailAddress("nicholas@t-and-cdata.com"));
                message.To.Add(new MailAddress("andrew@t-and-cdata.com"));
                message.Subject = subject;
                message.Body = emailBody;
                message.IsBodyHtml = true;
                client.Send(message);

                return "true";
            }
            catch (Exception e)
            {

                return $"stacktrace: {e.StackTrace}, inner exception: {e.InnerException}, {e.Message}, {e.Data}";
            }
        }

        public ActionResult getBanner()
        {
            var dataFile = Server.MapPath("~/Assets/Files/bannerText.txt");
            string fileData = System.IO.File.ReadAllText(dataFile);
            ViewBag.bannerText = fileData;
            return PartialView("alertBannerPartialView");
        }

        public ActionResult Supplies()
        {
            return View();
        }
    }
}
