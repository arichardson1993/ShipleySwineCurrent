using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ShipleySwine;

namespace ShipleySwine.Controllers
{
    public class BredGuiltsController : Controller
    {
        private ShipleySwineContext db = new ShipleySwineContext();

        // GET: BredGuilts
        public ActionResult Index()
        {
            return View(db.BredGuilts.ToList());
        }

        // GET: BredGuilts/Details/5
        public ActionResult Details(byte? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BredGuilt bredGuilt = db.BredGuilts.Find(id);
            if (bredGuilt == null)
            {
                return HttpNotFound();
            }
            return View(bredGuilt);
        }

        // GET: BredGuilts/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: BredGuilts/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "BreedId,Breed,Price,EarNotch,BlackCrossGilt,DOB,LitterSize,Sire,SireXBred,DamEN,SireOfDam,SireOfDamXBred,DateBred,SvcSire,SvcSireXBred,DueDate")] BredGuilt bredGuilt)
        {
            if (ModelState.IsValid)
            {
                db.BredGuilts.Add(bredGuilt);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(bredGuilt);
        }

        // GET: BredGuilts/Edit/5
        public ActionResult Edit(byte? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BredGuilt bredGuilt = db.BredGuilts.Find(id);
            if (bredGuilt == null)
            {
                return HttpNotFound();
            }
            return View(bredGuilt);
        }

        // POST: BredGuilts/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "BreedId,Breed,Price,EarNotch,BlackCrossGilt,DOB,LitterSize,Sire,SireXBred,DamEN,SireOfDam,SireOfDamXBred,DateBred,SvcSire,SvcSireXBred,DueDate")] BredGuilt bredGuilt)
        {
            if (ModelState.IsValid)
            {
                db.Entry(bredGuilt).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(bredGuilt);
        }

        // GET: BredGuilts/Delete/5
        public ActionResult Delete(byte? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BredGuilt bredGuilt = db.BredGuilts.Find(id);
            if (bredGuilt == null)
            {
                return HttpNotFound();
            }
            return View(bredGuilt);
        }

        // POST: BredGuilts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(byte id)
        {
            BredGuilt bredGuilt = db.BredGuilts.Find(id);
            db.BredGuilts.Remove(bredGuilt);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
