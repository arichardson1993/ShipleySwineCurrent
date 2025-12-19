using Microsoft.Ajax.Utilities;
using ShipleySwine;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace UploadingFilesUsingMVC.Controllers
{
    public class FileUploadController : Controller
    {
        private ShipleySwineContext db = new ShipleySwineContext();
        // GET: FileUpload    
        public ActionResult Index()
        {
            //var breedlist = db.Boars.ToList();
            var breedlist = db.Boars.Select(e => e.Breed).Distinct().ToList();
            //var list = breedlist.Distinct();
            //var list2 = list.DistinctBy(e => e.Breed).ToList();
            List<SelectListItem> breedDDsli = new List<SelectListItem>();
            //new SelectList(list2);
            foreach(var item in breedlist)
            {
                breedDDsli.Add(new SelectListItem
                {
                    Text= item,
                    Value = item
                });
                
                Debug.WriteLine(item);
            }
            ViewBag.breedDD = breedDDsli;
            
            return View();
        }
        [HttpPost]
        public ActionResult UploadFiles(string incomingbreed, string NameNoSpaces, HttpPostedFileBase file)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    Debug.WriteLine("-------------"+incomingbreed+"-------------");
                    if (file != null)
                    {
                        //Debug.WriteLine(Server.MapPath("~/Boars/" + incomingbreed + "/" + NameNoSpaces));
                        if (Directory.Exists(Server.MapPath("~/Boars/" + incomingbreed + "/" + NameNoSpaces+"/Large")))
                        {
                            List<int> number = new List<int>();
                            IEnumerable<string> images = Directory.EnumerateFiles(Server.MapPath("~/Boars/" + incomingbreed + "/" + NameNoSpaces+"/Large")).Select(fn => Path.GetFileName(fn));
                            if (images.Any())
                            {
                                foreach (var item in images)
                                {
                                    if (item.Substring(item.Length - 4, 4) == ".jpg" || item.Substring(item.Length - 4, 4) == ".png" || item.Substring(item.Length - 4, 4) == ".JPG" || item.Substring(item.Length - 4, 4) == ".PNG")
                                    {
                                        string i = item.Remove(item.Length - 4, 4);
                                        i.Substring(i.Length - 1, 1);
                                        number.Add(Int32.Parse(i.Substring(i.Length - 1, 1)));
                                    }
                                    else
                                    {
                                        string i = item;
                                        number.Add(Int32.Parse(i.Substring(i.Length - 1, 1)));
                                    }

                                }

                                Debug.WriteLine("**************" + file.ContentType + "**********************************");
                                string[] contentType = file.ContentType.Split('/');
                                string path = Server.MapPath("~/Boars/" + incomingbreed + "/" + NameNoSpaces + "/Large/" + NameNoSpaces + (number.Max() + 1) + ".jpg");
                                file.SaveAs(path);
                                Debug.WriteLine(path);
                            }
                            else
                            {
                                string[] contentType = file.ContentType.Split('/');
                                string path = Server.MapPath("~/Boars/" + incomingbreed + "/" + NameNoSpaces + "/Large/" + (NameNoSpaces + 1) + ".jpg");
                                file.SaveAs(path);
                            }

                            Debug.WriteLine("DIRECTORY DOES EXIST!!!!!");
                            //Debug.WriteLine("-----"+path+"-----");
                        }
                        else
                        {
                            Debug.WriteLine("-----Doesnt Exist----");
                            Directory.CreateDirectory(Server.MapPath("~/Boars/" + incomingbreed + "/" + NameNoSpaces +"/Large"));
                            List<int> number = new List<int>();
                            IEnumerable<string> images = Directory.EnumerateFiles(Server.MapPath("~/Boars/" + incomingbreed + "/" + NameNoSpaces+"/Large")).Select(fn => Path.GetFileName(fn));
                            if (images.Any())
                            {
                                foreach (var item in images)
                                {
                                    if (item.Substring(item.Length - 4, 4) == ".jpg" || item.Substring(item.Length - 4, 4) == ".png" || item.Substring(item.Length - 4, 4) == ".JPG" || item.Substring(item.Length - 4, 4) == ".PNG")
                                    {
                                        string i = item.Remove(item.Length - 4, 4);
                                        i.Substring(i.Length - 1, 1);
                                        number.Add(Int32.Parse(i.Substring(i.Length - 1, 1)));
                                    }
                                    else
                                    {
                                        string i = item;
                                        number.Add(Int32.Parse(i.Substring(i.Length - 1, 1)));
                                    }
                                    //file.SaveAs(path);

                                }
                                string[] contentType = file.ContentType.Split('/');
                                string path = Server.MapPath("~/Boars/" + incomingbreed + "/" + NameNoSpaces + "/Large/"+ NameNoSpaces+(number.Max() + 1)+".jpg");
                                file.SaveAs(path);
                            }
                            else
                            {
                                string[] contentType = file.ContentType.Split('/');
                                string path = Server.MapPath("~/Boars/" + incomingbreed + "/" + NameNoSpaces + "/Large/" + (NameNoSpaces + 1) + ".jpg");
                                file.SaveAs(path);
                            }
                        }

                    }
                    ViewBag.FileStatus = "File uploaded successfully.";
                }
                catch (Exception e)
                {

                    //ViewBag.FileStatus = "Error while file uploading.";
                    ViewBag.FileStatus = e.Message.ToString() + e.StackTrace.ToString();
                }

            }
            //Add dropdown list to ViewBag
            var breedlist = db.Boars.ToList();
            var list = breedlist.Distinct();
            var list2 = list.DistinctBy(e => e.Breed).ToList();
            List<SelectListItem> breedDDsli = new List<SelectListItem>();
            new SelectList(list2);
            foreach (var item in list2)
            {
                breedDDsli.Add(new SelectListItem
                {
                    Text = item.Breed,
                    Value = item.Breed
                });

                Debug.WriteLine(item.Breed);
            }
            ViewBag.breedDD = breedDDsli;

            return View("Index");
        }

        public ActionResult getBoarsList(string boarbreed)
        {
            var breedlist = db.Boars.Where(breed => breed.Breed == boarbreed).ToList();
            //var breedlist = db.Boars.Where(breed => breed.Breed == boarbreed).Select(e => e.Name).Distinct().ToList();
            var list = breedlist.Distinct();
            var list2 = list.DistinctBy(e => e.Name).ToList();
            List<SelectListItem> boarsDD = new List<SelectListItem>();
            new SelectList(list2);
            foreach (var item in list2)
            {
                boarsDD.Add(new SelectListItem
                {
                    Text = item.NameNoSpaces,
                    Value = item.NameNoSpaces
                });

                Debug.WriteLine(item.Breed);
            }
            ViewBag.BoarsDD = boarsDD;
            return PartialView("DisplayBoars");
        }

        //public ActionResult boarDetails(string boarName)
        //{
        //    var boarDetails = db.Boars.Where(name => name.Name == boarName);
        //    ViewBag.boarDetails = boarDetails;
        //    return(ViewBag.boar)
        //    //return PartialView("DisplayBoarDetails");
        //}
    }
}