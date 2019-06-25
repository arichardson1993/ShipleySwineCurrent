using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ShipleySwine;

namespace ShipleySwine.Controllers
{
    public class BoarsController : Controller
    {
        private ShipleySwineContext db = new ShipleySwineContext();

        // GET: Boars
        public ActionResult Index()
        {
            //return View(db.Boars.SqlQuery("SELECT [b].*, STUFF((SELECT '* ' + [sp2].[SellingPoint] FROM[dbo].[SellingPoints] AS[sp2] WHERE[sp2].[SellingPoints_Id] = [sp].[SellingPoints_Id] ORDER BY[sp2].[SellingPoint] ASC FOR XML PATH('')), 1, 0, '') AS[AllSellingPoints] FROM[dbo].[Boars] AS[b] INNER JOIN[dbo].[SellingPoint] AS[sp] ON[sp].[Boar_Id] = [b].[Boar_Id]").ToList());
            return View(db.Boars.ToList());
        }

        public ActionResult SelectBoar(String selectedBreed)
        {
            List<String> breeds = new List<string>() { "Yorkshire", "Duroc", "Berkshire", "Exotic", "Hampshire" };
            List<Boar> boars;
            Debug.WriteLine(selectedBreed);
            if(!breeds.Contains(selectedBreed)){

                boars = db.Boars.SqlQuery("select * from Boars where Breed not in ('Yorkshire', 'Duroc','Berkshire','Exotic','Hampshire')").ToList();
                return View(boars);

            }
            else
            {
                return View(db.Boars.Where(Breed => Breed.Breed == selectedBreed).ToList());

            }
            //return View(db.Boars.SqlQuery("SELECT [b].*, STUFF((SELECT '* ' + [sp2].[SellingPoint] FROM[dbo].[SellingPoints] AS[sp2] WHERE[sp2].[SellingPoints_Id] = [sp].[SellingPoints_Id] ORDER BY[sp2].[SellingPoint] ASC FOR XML PATH('')), 1, 0, '') AS[AllSellingPoints] FROM[dbo].[Boars] AS[b] INNER JOIN[dbo].[SellingPoint] AS[sp] ON[sp].[Boar_Id] = [b].[Boar_Id]").ToList());
            
        }

        // GET: Boars/Details/5
        public ActionResult Details(decimal id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Boar boar = db.Boars.Find(id);
            if (boar == null)
            {
                return HttpNotFound();
            }
            return View(boar);
        }

        // GET: Boars/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Boars/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Boar_Id,Name,Farrowed,LitterSize,Price,GuaranteedSettle,StressTest,NameNoSpaces,Sire,SireFull,Dam,DamFull,Breed,Order,Featured,FeaturedOrder,EarNotch,RegNum,TestData,BredBy,OwnedBy,Description")] Boar boar)
        {
            if (ModelState.IsValid)
            {
                db.Boars.Add(boar);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(boar);
        }

        // GET: Boars/Edit/5
        public ActionResult Edit(decimal id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Boar boar = db.Boars.Find(id);
            if (boar == null)
            {
                return HttpNotFound();
            }
            return View(boar);
        }

        // POST: Boars/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Boar_Id,Name,Farrowed,LitterSize,Price,GuaranteedSettle,StressTest,NameNoSpaces,Sire,SireFull,Dam,DamFull,Breed,Order,Featured,FeaturedOrder,EarNotch,RegNum,TestData,BredBy,OwnedBy,Description")] Boar boar)
        {
            if (ModelState.IsValid)
            {
                db.Entry(boar).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(boar);
        }

        // GET: Boars/Delete/5
        public ActionResult Delete(decimal id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Boar boar = db.Boars.Find(id);
            if (boar == null)
            {
                return HttpNotFound();
            }
            return View(boar);
        }

        // POST: Boars/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(decimal id)
        {
            Boar boar = db.Boars.Find(id);
            db.Boars.Remove(boar);
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
