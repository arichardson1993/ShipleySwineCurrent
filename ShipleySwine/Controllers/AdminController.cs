using ShipleySwine.Models;
using ShipleySwine.ViewModels;
using System;
using System.Web.Mvc;

namespace ShipleySwine.Controllers
{
    public class AdminController : Controller
    {
        private bool EnsureAuthenticated()
        {
            return Session["Authentication"] != null && Session["Authentication"].ToString() == "Success";
        }

        public ActionResult Index()
        {
            if (!EnsureAuthenticated())
            {
                return RedirectToAction("Login", "Authentication");
            }

            return View();
        }

        public ActionResult getBannerForm()
        {
            if (!EnsureAuthenticated())
            {
                return RedirectToAction("Login", "Authentication");
            }

            string dataFile = Server.MapPath("~/Assets/Files/bannerText.txt");
            string fileData = System.IO.File.ReadAllText(dataFile);
            ViewBag.bannerText = fileData;
            return View();
        }

        [HttpPost]
        public ActionResult getBannerForm(string newBannerText)
        {
            if (!EnsureAuthenticated())
            {
                return RedirectToAction("Login", "Authentication");
            }

            try
            {
                string dataFile = Server.MapPath("~/Assets/Files/bannerText.txt");
                System.IO.File.WriteAllText(dataFile, newBannerText);
                ViewBag.bannerText = newBannerText;
                ViewBag.submitText = "Save Successful";
                return View();
            }
            catch (Exception e)
            {
                ViewBag.submitText = e.ToString();
                return View();
            }
        }

        public ActionResult ContactBlocks()
        {
            if (!EnsureAuthenticated())
            {
                return RedirectToAction("Login", "Authentication");
            }

            ContactBlockAdminViewModel viewModel = new ContactBlockAdminViewModel
            {
                Blocks = ContactBlockStore.GetAll(),
                StatusMessage = TempData["ContactBlockStatus"] as string
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ContactBlocks(ContactBlockAdminViewModel model)
        {
            if (!EnsureAuthenticated())
            {
                return RedirectToAction("Login", "Authentication");
            }

            ContactBlockAdminViewModel viewModel = new ContactBlockAdminViewModel
            {
                Email = model?.Email,
                Phone = model?.Phone,
                Reason = model?.Reason
            };

            try
            {
                ContactBlockStore.Add(model?.Email, model?.Phone, model?.Keyword, model?.Reason);
                viewModel.StatusMessage = "Block added.";
                ModelState.Clear();
            }
            catch (Exception ex)
            {
                viewModel.ErrorMessage = ex.Message;
            }

            viewModel.Blocks = ContactBlockStore.GetAll();
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RemoveContactBlock(Guid id)
        {
            if (!EnsureAuthenticated())
            {
                return RedirectToAction("Login", "Authentication");
            }

            ContactBlockStore.Remove(id);
            TempData["ContactBlockStatus"] = "Block removed.";
            return RedirectToAction("ContactBlocks");
        }
    }
}
