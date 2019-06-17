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
    public class SellingPointsController : Controller
    {
        private ShipleySwineContext db = new ShipleySwineContext();

        // GET: SellingPoints
        public ActionResult Index()
        {
            return View(db.SellingPoints1.ToList());
        }

        // GET: SellingPoints/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SellingPoint1 sellingPoint1 = db.SellingPoints1.Find(id);
            if (sellingPoint1 == null)
            {
                return HttpNotFound();
            }
            return View(sellingPoint1);
        }

        // GET: SellingPoints/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: SellingPoints/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,SellingPoint,SellingPoints_Id")] SellingPoint1 sellingPoint1)
        {
            if (ModelState.IsValid)
            {
                db.SellingPoints1.Add(sellingPoint1);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(sellingPoint1);
        }

        // GET: SellingPoints/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SellingPoint1 sellingPoint1 = db.SellingPoints1.Find(id);
            if (sellingPoint1 == null)
            {
                return HttpNotFound();
            }
            return View(sellingPoint1);
        }

        // POST: SellingPoints/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,SellingPoint,SellingPoints_Id")] SellingPoint1 sellingPoint1)
        {
            if (ModelState.IsValid)
            {
                db.Entry(sellingPoint1).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(sellingPoint1);
        }

        // GET: SellingPoints/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SellingPoint1 sellingPoint1 = db.SellingPoints1.Find(id);
            if (sellingPoint1 == null)
            {
                return HttpNotFound();
            }
            return View(sellingPoint1);
        }

        // POST: SellingPoints/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            SellingPoint1 sellingPoint1 = db.SellingPoints1.Find(id);
            db.SellingPoints1.Remove(sellingPoint1);
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
