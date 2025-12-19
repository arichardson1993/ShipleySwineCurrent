using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Web;
using System.Web.Mvc;
using ShipleySwine;
using ShipleySwine.ViewModels;

namespace ShipleySwine.Controllers
{
    public class BredGuiltsController : Controller
    {
        private ShipleySwineContext db = new ShipleySwineContext();

        // GET: BredGilts
        public ActionResult Index()
        {
            
            return RedirectToAction("Index", "BredGilts");
        }

        public ActionResult Email(decimal? number)
        {
            EmailViewModel vm = new EmailViewModel()
            {
                bg = db.BredGilts.Find(number),
                email = "",
                name = "",
                phone = "",
                comments = ""
            };
            Debug.WriteLine("@@@@@@@@@@@@@@@@@@@@@" + number + "@@@@@@@@@@@@@@@@@@@@@");
            return View(vm);

        }
        [HttpPost]
        public ActionResult Email(EmailViewModel vm)
        {
            string subject = "I'm interested in a Bred Gilt";
            string emailBody = "Email:" + vm.email + "<br>Name: " + vm.name + "<br>Phone: " + vm.phone + "<br>Comments:" + vm.comments + "<br><br><br>I am interested in the following Bred Gilt<br>Breed: " + vm.bg.Breed + "<br>EN: " + vm.bg.EarNotch + "<br>Price: $" + vm.bg.Price + "<br>Sire: " + vm.bg.Sire + "<br>Sire of Dam: " + vm.bg.SireOfDam + "<br>Litter Size: " + vm.bg.LitterSize + "<br>Dam Ear Notch: " + vm.bg.DamEN + "<br>Date Bred: " + vm.bg.DateBred + "<br>Date of Birth: " + vm.bg.DOB + "<br>Due Date: " + vm.bg.DueDate;
            if (SendEmail(vm.email, subject, emailBody) == true)
            {
                return RedirectToAction("EmailSuccess", "BredGilts");
            }
            else
            {
                return RedirectToAction("EmailFailure", "BredGilts");
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

        // GET: BredGilts/Details/5
        public ActionResult Details(byte? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BredGuilt bredGuilt = db.BredGilts.Find(id);
            if (bredGuilt == null)
            {
                return HttpNotFound();
            }
            return View(bredGuilt);
        }

        // GET: BredGilts/Create
        public ActionResult Create()
        {
            if (Session["Authentication"] != null)
            {
                if (Session["Authentication"].ToString() != "Success")
                {
                    return RedirectToAction("Login", "Authentication");
                }
                else
                {
                    try
                    {
                        ViewData["key"] = db.BredGilts.Max(key => key.BredGuiltId) + 1;
                    }
                    catch (Exception e)
                    {
                        ViewData["key"] = 1;
                    }
                    //ViewData["key"] = db.BredGilts.Max(key => key.BredGuiltId) + 1;
                    return View();
                }
            }
            else
            {
                return RedirectToAction("Login", "Authentication");
            }

        }







        // POST: BredGilts/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "BreedId,Breed,Price,EarNotch,BlackCrossGilt,DOB,LitterSize,Sire,SireXBred,DamEN,SireOfDam,SireOfDamXBred,DateBred,SvcSire,SvcSireXBred,DueDate")] BredGuilt bredGuilt)
        {
            if (ModelState.IsValid)
            {
                //bredGuilt.BredGuiltId = db.BredGilts.Max(key => key.BredGuiltId) + 1;
                db.BredGilts.Add(bredGuilt);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(bredGuilt);
        }

        // GET: BredGilts/Edit/5

        public ActionResult Edit(decimal id)
        {
            if (Session["Authentication"] != null)
            {
                if (Session["Authentication"].ToString() != "Success")
                {
                    return RedirectToAction("Login", "Authentication");
                }
                else
                {
                    if (id == 0)
                    {
                        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                    }
                    BredGuilt bredGuilt = db.BredGilts.Find(id);
                    if (bredGuilt == null)
                    {
                        return HttpNotFound();
                    }
                    return View(bredGuilt);
                }
            }
            else
            {
                return RedirectToAction("Login", "Authentication");
            }

        }
        /*
                public ActionResult Edit(byte? id)
                {
                    if (id == null)
                    {
                        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                    }
                    BredGuilt bredGuilt = db.BredGilts.Find(id);
                    if (bredGuilt == null)
                    {
                        return HttpNotFound();
                    }
                    return View(bredGuilt);
                }
        */
        // POST: BredGilts/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "BredGuiltId,BreedId,Breed,Price,EarNotch,BlackCrossGilt,DOB,LitterSize,Sire,SireXBred,DamEN,SireOfDam,SireOfDamXBred,DateBred,SvcSire,SvcSireXBred,DueDate")] BredGuilt bredGuilt)
        {
            if (ModelState.IsValid)
            {
                db.Entry(bredGuilt).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(bredGuilt);
        }

        // GET: BredGilts/Delete/5

        public ActionResult Delete(decimal id)
        {
            if (Session["Authentication"] != null)
            {
                if (Session["Authentication"].ToString() != "Success")
                {
                    return RedirectToAction("Login", "Authentication");
                }
                else
                {
                    if (id == 0)
                    {
                        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                    }
                    BredGuilt bredGuilt = db.BredGilts.Find(id);
                    if (bredGuilt == null)
                    {
                        return HttpNotFound();
                    }
                    return View(bredGuilt);
                }
            }
            else
            {
                return RedirectToAction("Login", "Authentication");
            }
        }

        /*
                public ActionResult Delete(byte? id)
                {
                    if (id == null)
                    {
                        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                    }
                    BredGuilt bredGuilt = db.BredGilts.Find(id);
                    if (bredGuilt == null)
                    {
                        return HttpNotFound();
                    }
                    return View(bredGuilt);
                }
        */

        // POST: BredGilts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(byte id)
        {
            BredGuilt bredGuilt = db.BredGilts.Find(id);
            db.BredGilts.Remove(bredGuilt);
            db.SaveChanges();
            return RedirectToAction("Index");
        }


        public ActionResult Manage()
        {
            if (Session["Authentication"] != null)
            {
                if (Session["Authentication"].ToString() != "Success")
                {
                    return RedirectToAction("Login", "Authentication");
                }
                else
                {
                    //db.BredGilts.OrderBy(breed => breed.Breed).ThenByDescending(price => price.Price)
                    var bredGilts = db.BredGilts.OrderBy(breed => breed.Breed).ThenByDescending(price => price.Price).ToList();
                    //var pagedlist = boars.ToPagedList(page ?? 1, 15);
                    return View(bredGilts);
                }
            }
            else
            {
                return RedirectToAction("Login", "Authentication");
            }
        }


        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        public JsonResult SendMailToUser()
        {
            bool result = false;

            //result = SendEmail();

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public bool SendEmail(string fromEmail, string subject, string emailBody)
        {
            try
            {
                string senderEmail = System.Configuration.ConfigurationManager.AppSettings["SenderEmail"].ToString();
                //string senderPassword = System.Configuration.ConfigurationManager.AppSettings["SenderPassword"].ToString();
                //string ToEmail = "andrew.richardson.667@gmail.com";

                //SmtpClient client = new SmtpClient("relay-hosting.secureserver.net", 25);
                //client.EnableSsl = true;
                //client.Timeout = 100000;
                //client.DeliveryMethod = SmtpDeliveryMethod.Network;
                //client.UseDefaultCredentials = true;
                //client.Credentials = new NetworkCredential(senderEmail, senderPassword);
                MailMessage message = new MailMessage();
                message.From = new MailAddress("shipleywebemail@gmail.com");
                message.To.Add(new MailAddress("Josie_shipley@yahoo.com"));
                message.To.Add(new MailAddress("Buckeyeboot@yahoo.com"));
                message.To.Add(new MailAddress("Shipleyswine@yahoo.com"));
                message.To.Add(new MailAddress("shipleywebemail@gmail.com"));
                message.To.Add(new MailAddress("nicholas.richardson.0505@gmail.com"));
                message.Subject = subject;
                message.Body = emailBody;
                message.IsBodyHtml = true;
                SmtpClient client = new SmtpClient();
                client.Send(message);
                //MailMessage message = new MailMessage(senderEmail, ToEmail, subject, emailBody);
                //message.BodyEncoding = UTF8Encoding.UTF8;
                //client.Send(message);

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
