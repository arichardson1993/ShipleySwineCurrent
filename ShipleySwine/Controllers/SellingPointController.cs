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
    public class SellingPointController : Controller
    {
        private ShipleySwineContext db = new ShipleySwineContext();

        // GET: SellingPoint
        public ActionResult Index()
        {
            var sellingPoints = db.SellingPoints.Include(s => s.Boar);
            return View(sellingPoints.ToList());
        }

        // GET: SellingPoint/Details/5
        public ActionResult Details(decimal id)
        {
            if (id == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SellingPoint sellingPoint = db.SellingPoints.Find(id);
            if (sellingPoint == null)
            {
                return HttpNotFound();
            }
            return View(sellingPoint);
        }

        // GET: SellingPoint/Create
        public ActionResult Create()
        {
            ViewBag.Boar_Id = new SelectList(db.Boars, "Boar_Id", "Name");
            return View();
        }

        // POST: SellingPoint/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "SellingPoints_Id,Boar_Id")] SellingPoint sellingPoint)
        {
            if (ModelState.IsValid)
            {
                db.SellingPoints.Add(sellingPoint);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.Boar_Id = new SelectList(db.Boars, "Boar_Id", "Name", sellingPoint.Boar_Id);
            return View(sellingPoint);
        }

        // GET: SellingPoint/Edit/5
        public ActionResult Edit(decimal id)
        {
            if (id == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SellingPoint sellingPoint = db.SellingPoints.Find(id);
            if (sellingPoint == null)
            {
                return HttpNotFound();
            }
            ViewBag.Boar_Id = new SelectList(db.Boars, "Boar_Id", "Name", sellingPoint.Boar_Id);
            return View(sellingPoint);
        }

        // POST: SellingPoint/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "SellingPoints_Id,Boar_Id")] SellingPoint sellingPoint)
        {
            if (ModelState.IsValid)
            {
                db.Entry(sellingPoint).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.Boar_Id = new SelectList(db.Boars, "Boar_Id", "Name", sellingPoint.Boar_Id);
            return View(sellingPoint);
        }

        // GET: SellingPoint/Delete/5
        public ActionResult Delete(decimal id)
        {
            if (id == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SellingPoint sellingPoint = db.SellingPoints.Find(id);
            if (sellingPoint == null)
            {
                return HttpNotFound();
            }
            return View(sellingPoint);
        }

        // POST: SellingPoint/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(decimal id)
        {
            SellingPoint sellingPoint = db.SellingPoints.Find(id);
            db.SellingPoints.Remove(sellingPoint);
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
