using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ShipleySwine;
using PagedList;
using PagedList.Mvc;

namespace ShipleySwine.Controllers
{
    public class WinnersController : Controller
    {
        private ShipleySwineContext db = new ShipleySwineContext();

        // GET: Winners
        public ActionResult Index(int? page)
        {
            var winners = db.Winners.SqlQuery("SELECT DISTINCT ID, Name, Won, Pedigree, Act, ImageURL, WinnerId FROM Winners WHERE ID != '103' GROUP BY ID, Name, Won, Pedigree, Act, ImageURL, WinnerId").ToList();
            foreach(var winner in winners)
            {
                Debug.WriteLine(winner.Name+"  " +winner.ImageURL);
            }
            var pagedlist = winners.ToPagedList(page ?? 1, 15);
            Debug.WriteLine(winners.Count);
            return View(pagedlist);
        }

        // GET: Winners/Details/5
        public ActionResult Details(byte? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Winner winner = db.Winners.Find(id);
            if (winner == null)
            {
                return HttpNotFound();
            }
            return View(winner);
        }

        // GET: Winners/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Winners/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,Name,Won,Pedigree,Act,ImageURL,WinnerId")] Winner winner)
        {
            if (ModelState.IsValid)
            {
                db.Winners.Add(winner);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(winner);
        }

        // GET: Winners/Edit/5
        public ActionResult Edit(byte? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Winner winner = db.Winners.Find(id);
            if (winner == null)
            {
                return HttpNotFound();
            }
            return View(winner);
        }

        // POST: Winners/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,Name,Won,Pedigree,Act,ImageURL,WinnerId")] Winner winner)
        {
            if (ModelState.IsValid)
            {
                db.Entry(winner).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(winner);
        }

        // GET: Winners/Delete/5
        public ActionResult Delete(byte? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Winner winner = db.Winners.Find(id);
            if (winner == null)
            {
                return HttpNotFound();
            }
            return View(winner);
        }

        // POST: Winners/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(byte id)
        {
            Winner winner = db.Winners.Find(id);
            db.Winners.Remove(winner);
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
