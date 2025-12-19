using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using PagedList;
using ShipleySwine;
using ShipleySwine.ViewModels;

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

        public ActionResult BoarVideos()
        {
            return View();
        }
        public ActionResult SelectBoar(String selectedBreed)
        {
            List<String> breeds = new List<string>() { "Yorkshire", "Duroc", "Berkshire", "Exotic", "Hampshire" };
            List<Boar> boars;
            Debug.WriteLine(selectedBreed);
            if (!breeds.Contains(selectedBreed))
            {

                //boars = db.Boars.SqlQuery("select * from Boars where Breed not in ('Yorkshire', 'Duroc','Berkshire','Exotic','Hampshire') order by [Order]").ToList();
                boars = db.Boars.OrderBy(o => o.Order).Where(breed => !breeds.Contains(breed.Breed)).ToList();
                return View(boars);

            }
            else
            {
                return View(db.Boars.Where(Breed => Breed.Breed == selectedBreed).OrderBy(order => order.Order).ToList());

            }
            //return View(db.Boars.SqlQuery("SELECT [b].*, STUFF((SELECT '* ' + [sp2].[SellingPoint] FROM[dbo].[SellingPoints] AS[sp2] WHERE[sp2].[SellingPoints_Id] = [sp].[SellingPoints_Id] ORDER BY[sp2].[SellingPoint] ASC FOR XML PATH('')), 1, 0, '') AS[AllSellingPoints] FROM[dbo].[Boars] AS[b] INNER JOIN[dbo].[SellingPoint] AS[sp] ON[sp].[Boar_Id] = [b].[Boar_Id]").ToList());

        }

        public ActionResult NewBoars()
        {
            // var fourMonthsAgo = DateTime.Now.AddDays(-120);
            // Debug.WriteLine(fourMonthsAgo);
            //var newBoars = db.Boars.Where(createDate => createDate.CreateDate > fourMonthsAgo).ToList();
            var newBoars = db.Boars.SqlQuery("select * from boars b where b.Featured != 0 and b.FeaturedOrder != -1 order by b.FeaturedOrder asc").ToList();

            return View(newBoars);
        }

        // GET: Boars/Details/5
        public ActionResult Details(decimal id)
        {

            if (id == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Boar boar = db.Boars.Find(id);
            ViewData["images"] = Directory.EnumerateFiles(Server.MapPath("~/Boars/" + boar.Breed + "/" + boar.NameNoSpaces + "/Large/")).Select(fn => "~/Boars/" + boar.Breed + "/" + boar.NameNoSpaces + "/Large/" + Path.GetFileName(fn));
            //int imageCount = Directory.GetFiles("~/Boars/"+boar.Breed+"/"+boar.NameNoSpaces+"/Large", "*", SearchOption.TopDirectoryOnly).Length;
            int count = 0;
            foreach (var image in (IEnumerable<string>)ViewData["images"])
            {
                Debug.WriteLine("@@@@@@@@@@@@@@@@@@@@@" + image + "@@@@@@@@@@@@@@@@@@@@@");
                count++;
            }
            ViewBag.imagecount = count;
            if (boar == null)
            {
                return HttpNotFound();
            }
            return View(boar);
        }

        // GET: Boars/Create
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
                    ViewBag.BoarId = db.Boars.Max(boarid => boarid.Boar_Id) + 1;
                    return View();
                }
            }
            else
            {
                return RedirectToAction("Login", "Authentication");
            }

        }

        // POST: Boars/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public ActionResult Create([Bind(Include = "Boar_Id,Name,Farrowed,LitterSize,Price,GuaranteedSettle,StressTest,NameNoSpaces,Sire,SireFull,Dam,DamFull,Breed,Order,Featured,FeaturedOrder,EarNotch,RegNum,TestData,BredBy,OwnedBy,Description")] Boar boar)
        {
            if (ModelState.IsValid)
            {
                boar.CreateDate = DateTime.Now;
                string bredBy = boar.BredBy;
                string bredbystring = "Bred By: ";

                string ownedBy = boar.OwnedBy;
                string ownedByString = "Owned By: ";

                boar.BredBy = bredbystring + bredBy;
                boar.OwnedBy = ownedByString + ownedBy;
                db.Boars.Add(boar);
                db.SaveChanges();
                SellingPoint sp = new SellingPoint()
                {
                    Boar_Id = boar.Boar_Id,
                    SellingPoints_Id = db.SellingPoints.Max(spid => spid.SellingPoints_Id) + 1
                };
                db.SellingPoints.Add(sp);
                db.SaveChanges();
                return RedirectToAction("Create", "SellingPoints", new { @id = sp.SellingPoints_Id });
            }
            else
            {
                return RedirectToAction("InvalidModel", new { invalidBoard = boar });
            }

        }

        public ActionResult InvalidModel(Boar invalidBoard)
        {
            return View(invalidBoard);
        }

        // GET: Boars/Edit/5
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
                    Boar boar = db.Boars.Find(id);
                    var sellingPoint = db.SellingPoints.Where(boarId => boarId.Boar_Id == id).Select(spId => spId.SellingPoints_Id).Single();
                    var sellingPoints = db.SellingPoints1.Where(sid => sid.SellingPoints_Id == sellingPoint).ToList();
                    EditBoarsViewModel vm = new EditBoarsViewModel()
                    {
                        boar = boar,
                        sellingpoints = sellingPoints
                    };

                    if (boar == null)
                    {
                        return HttpNotFound();
                    }
                    return View(vm);
                }
            }
            else
            {
                return RedirectToAction("Login", "Authentication");
            }

        }



        public ActionResult EditOrder(string breed)
        {
            if (Session["Authentication"] != null)
            {
                if (Session["Authentication"].ToString() != "Success")
                {
                    return RedirectToAction("Login", "Authentication");
                }
                else
                {
                    List<Boar> boar = new List<Boar>();
                    if (breed == null)
                    {
                        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                    }
                    List<string> breeds = new List<string>() { 
                        "Berkshire",
                        "Hampshire",
                        "Exotic",
                        "Duroc",
                        "Yorkshire"
                    };
                    if (breeds.Contains(breed))
                    {
                        boar = db.Boars.Where(b => b.Breed == breed).ToList();
                    }
                    else
                    {
                        boar = db.Boars.Where(b => !breeds.Contains(b.Breed)).ToList();
                    }
                    List<BoarBreedOrderViewModel> vm = new List<BoarBreedOrderViewModel>();
                    foreach (var item in boar)
                    {
                        vm.Add(new BoarBreedOrderViewModel
                        {
                            boarid = item.Boar_Id,
                            order = (int)item.Order,
                            boarname = item.Name
                        });
                    }
                    //EditBoarsViewModel vm = new EditBoarsViewModel()
                    //{
                    //    boar = boar,
                    //    sellingpoints = sellingPoints
                    //};

                    if (boar == null)
                    {
                        return HttpNotFound();
                    }

                    EditOrderViewViewModel v = new EditOrderViewViewModel()
                    {
                        something = vm
                    };
                    return View(v);
                }
            }
            else
            {
                return RedirectToAction("Login", "Authentication");
            }

        }

        //public ActionResult EditOrder(string breed)
        //{
        //    if (Session["Authentication"] != null)
        //    {
        //        if (Session["Authentication"].ToString() != "Success")
        //        {
        //            return RedirectToAction("Login", "Authentication");
        //        }
        //        else
        //        {
        //            if (breed == null)
        //            {
        //                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //            }
        //            var boar = new BoarBreedOrderViewModel 
        //            { 
        //                boarid = db.Boars.Where(b => b.Breed == breed).FirstOrDefault().Boar_Id,
        //                boarname = db.Boars.Where(b => b.Breed == breed).FirstOrDefault().Name,
        //                order = (int)db.Boars.Where(b => b.Breed == breed).FirstOrDefault().Order
        //            };

        //            //List<BoarBreedOrderViewModel> vm = new List<BoarBreedOrderViewModel>();
        //            //foreach (var item in boar)
        //            //{
        //            //    vm.Add(new BoarBreedOrderViewModel
        //            //    {
        //            //        boarid = item.Boar_Id,
        //            //        order = (int)item.Order,
        //            //        boarname = item.Name
        //            //    });
        //            //}
        //            //EditBoarsViewModel vm = new EditBoarsViewModel()
        //            //{
        //            //    boar = boar,
        //            //    sellingpoints = sellingPoints
        //            //};

        //            if (boar == null)
        //            {
        //                return HttpNotFound();
        //            }

        //            //EditOrderViewViewModel v = new EditOrderViewViewModel()
        //            //{
        //            //    vm = vm
        //            //};
        //            return View(boar);
        //        }
        //    }
        //    else
        //    {
        //        return RedirectToAction("Login", "Authentication");
        //    }

        //}

        [HttpPost]
        public ActionResult EditOrder(EditOrderViewViewModel vm)
        {
            List<Boar> boars = new List<Boar>();
            foreach (var item in vm.something)
            {
                boars.Add(db.Boars.Find(item.boarid));
            }

            foreach (var item in boars)
            {
                foreach (var itemm in vm.something)
                {
                    if (item.Boar_Id == itemm.boarid)
                    {

                        item.Order = itemm.order;

                        db.Entry(item).State = EntityState.Modified;
                        db.SaveChanges();

                    }
                }
            }

            return RedirectToAction("ManageOrder", "Boars");

        }





        // POST: Boars/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(EditBoarsViewModel vm)
        {
            if (ModelState.IsValid)
            {
                db.Entry(vm.boar).State = EntityState.Modified;
                db.SaveChanges();
                foreach (var item in vm.sellingpoints)
                {
                    db.Entry(item).State = EntityState.Modified;
                    db.SaveChanges();
                }

                return RedirectToAction("Manage", "Boars");

            }
            return View(vm);
        }

        // GET: Boars/Delete/5
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
                    Boar boar = db.Boars.Find(id);
                    if (boar == null)
                    {
                        return HttpNotFound();
                    }
                    return View(boar);
                }
            }
            else
            {
                return RedirectToAction("Login", "Authentication");
            }
        }

        // POST: Boars/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(decimal id)
        {
            Boar boar = db.Boars.Find(id);
            db.Boars.Remove(boar);
            db.SaveChanges();
            return RedirectToAction("Manage", "Boars");
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
                    var boars = db.Boars.OrderBy(breed => breed.Breed).ThenBy(name => name.Name).ToList();
                    //var pagedlist = boars.ToPagedList(page ?? 1, 15);
                    return View(boars);
                }
            }
            else
            {
                return RedirectToAction("Login", "Authentication");
            }
        }


        public ActionResult ManageOrder()
        {
            if (Session["Authentication"] != null)
            {
                if (Session["Authentication"].ToString() != "Success")
                {
                    return RedirectToAction("Login", "Authentication");
                }
                else
                {
                    BreedViewModel vm = new BreedViewModel()
                    {
                        breeds = new List<string>(){"Berkshire", "Duroc", "Exotic", "Hampshire", "Yorkshire", "Other"}
                    };

                    return View(vm);

                }
            }
            else
            {
                return RedirectToAction("Login", "Authentication");
            }
        }


        public ActionResult TestView()
        {
            var boars = db.Boars.ToList();

            foreach (var item in boars)
            {
                //Debug.WriteLine("*****************************");
                //Debug.WriteLine("String Before");
                //Debug.WriteLine(item.Description);
                //Debug.WriteLine("String After");
                var nohtml = Regex.Replace(item.Description.Trim(), "<.*?>", String.Empty);
                //Debug.WriteLine(Regex.Replace(item.Description.Trim(), "<.*?>", String.Empty));
                //var removehtml = item.Description;
                //while (removehtml.Contains("<"))
                //{
                //    var left = item.Description.Trim().IndexOf("<");
                //    var right = item.Description.Trim().IndexOf(">");
                //    Debug.WriteLine("Left: " + left + " Right: " + right);
                //    removehtml = item.Description.Trim().Remove(left, right+1);
                //    Debug.WriteLine(removehtml);
                //item.Description.Remove(0, item.Description.Length);
                //Debug.WriteLine("Hopefully Empty string");
                //Debug.WriteLine(item.Description);
                //Debug.WriteLine("String Final");
                item.Description = Regex.Replace(nohtml, @"\s+", " ");
                item.Description.Trim();
                db.Entry(item).State = EntityState.Modified;
                db.SaveChanges();
                Debug.WriteLine("*****************************");
                //}

            }

            return View(boars);
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
