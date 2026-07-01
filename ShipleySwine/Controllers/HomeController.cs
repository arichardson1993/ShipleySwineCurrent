using ShipleySwine.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;
using ShipleySwine.Models;
using System.Xml;
using Newtonsoft.Json;

namespace ShipleySwine.Controllers
{
    public class HomeController : Controller
    {
        private static readonly object ContactSubmissionLock = new object();
        private static readonly HttpClient TurnstileHttpClient = new HttpClient
        {
            Timeout = TimeSpan.FromSeconds(10)
        };
        private const int ContactSubmissionsPerHour = 3;
        private const string TurnstileAlwaysPassTestSecret = "1x0000000000000000000000000000000AA";

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
            TurnstileSettings turnstileSettings = LoadTurnstileSettings();
            ViewBag.TurnstileSiteKey = turnstileSettings.SiteKey;
            ViewBag.TurnstileEnabled = turnstileSettings.IsConfigured;

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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> Email(ContactEmailViewModel vm)
        {
            if (vm == null)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Json("false");
            }

            // Silently accept honeypot submissions so bots do not learn how they were blocked.
            if (!string.IsNullOrWhiteSpace(vm.website))
            {
                return Json("true");
            }

            string normalizedEmail = NormalizeEmailAddress(vm.email);

            if (!ModelState.IsValid ||
                !IsValidEmailAddress(normalizedEmail) ||
                vm.subject.Contains("\r") ||
                vm.subject.Contains("\n"))
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Json("false");
            }

            ContactBlockEntry blockedEntry;
            if (ContactBlockStore.IsBlocked(vm, out blockedEntry))
            {
                Trace.TraceWarning(
                    "Blocked contact submission from '{0}' / '{1}'. Matched block '{2}' created {3:u}.",
                    vm.email,
                    vm.phone,
                    blockedEntry.Id,
                    blockedEntry.CreatedUtc);

                // Pretend the submission succeeded so abusive senders do not learn they were blocked.
                return Json("true");
            }

            TurnstileSettings turnstileSettings = LoadTurnstileSettings();
            if (turnstileSettings.IsConfigured)
            {
                string turnstileToken = Request.Form["cf-turnstile-response"];
                string expectedHostname = Request.Url == null ? null : Request.Url.Host;

                if (!await VerifyTurnstileAsync(
                    turnstileSettings.SecretKey,
                    turnstileToken,
                    Request.UserHostAddress,
                    expectedHostname))
                {
                    Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    return Json("false");
                }
            }

            string duplicateKey;
            if (!TryReserveContactSubmission(vm, out duplicateKey))
            {
                return Json("true");
            }

            string emailBody =
                "Message From Contact Us:<br><br>Name: " + HttpUtility.HtmlEncode(vm.fullname) +
                "<br>Email: " + HttpUtility.HtmlEncode(normalizedEmail) +
                "<br>Phone: " + HttpUtility.HtmlEncode(vm.phone) +
                "<br><br><br>Comments:<br>" + HttpUtility.HtmlEncode(vm.comments).Replace("\r\n", "<br>").Replace("\n", "<br>");

            if (SendEmail(normalizedEmail, vm.subject.Trim(), emailBody))
            {
                return Json("true");
            }

            // Permit a retry when delivery itself failed.
            HttpRuntime.Cache.Remove(duplicateKey);
            Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            return Json("false");
        }

        private bool TryReserveContactSubmission(ContactEmailViewModel vm, out string duplicateKey)
        {
            string fingerprint = string.Join("\n", new[]
            {
                vm.fullname,
                vm.email,
                vm.phone,
                vm.subject,
                vm.comments
            }.Select(value => (value ?? string.Empty).Trim().ToLowerInvariant()));

            duplicateKey = "contact:duplicate:" + HashContactValue(fingerprint);
            string ipAddress = Request.UserHostAddress ?? "unknown";
            string rateKey = "contact:rate:" + HashContactValue(ipAddress);

            lock (ContactSubmissionLock)
            {
                if (HttpRuntime.Cache[duplicateKey] != null)
                {
                    return false;
                }

                int submissionCount = HttpRuntime.Cache[rateKey] is int
                    ? (int)HttpRuntime.Cache[rateKey]
                    : 0;

                if (submissionCount >= ContactSubmissionsPerHour)
                {
                    return false;
                }

                HttpRuntime.Cache.Insert(
                    rateKey,
                    submissionCount + 1,
                    null,
                    DateTime.UtcNow.AddHours(1),
                    System.Web.Caching.Cache.NoSlidingExpiration);

                HttpRuntime.Cache.Insert(
                    duplicateKey,
                    true,
                    null,
                    DateTime.UtcNow.AddHours(24),
                    System.Web.Caching.Cache.NoSlidingExpiration);

                return true;
            }
        }

        private static string HashContactValue(string value)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                return Convert.ToBase64String(sha256.ComputeHash(Encoding.UTF8.GetBytes(value)))
                    .Replace("/", "_")
                    .Replace("+", "-");
            }
        }

        private static string NormalizeEmailAddress(string value)
        {
            return string.IsNullOrWhiteSpace(value)
                ? null
                : Regex.Replace(value, @"\s+", string.Empty).Trim();
        }

        private static bool IsValidEmailAddress(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return false;
            }

            try
            {
                MailAddress address = new MailAddress(value);
                return string.Equals(address.Address, value, StringComparison.OrdinalIgnoreCase);
            }
            catch
            {
                return false;
            }
        }

        private static TurnstileSettings LoadTurnstileSettings()
        {
            string configPath = HostingEnvironment.MapPath("~/App_Data/turnstile.config");
            if (string.IsNullOrWhiteSpace(configPath) || !System.IO.File.Exists(configPath))
            {
                return new TurnstileSettings();
            }

            try
            {
                XmlDocument document = new XmlDocument();
                document.Load(configPath);

                XmlNode siteKeyNode = document.SelectSingleNode("/appSettings/add[@key='TurnstileSiteKey']");
                XmlNode secretKeyNode = document.SelectSingleNode("/appSettings/add[@key='TurnstileSecretKey']");

                return new TurnstileSettings
                {
                    SiteKey = siteKeyNode == null ? null : siteKeyNode.Attributes["value"].Value,
                    SecretKey = secretKeyNode == null ? null : secretKeyNode.Attributes["value"].Value
                };
            }
            catch (Exception exception)
            {
                Trace.TraceError("Unable to load Turnstile configuration: {0}", exception);
                return new TurnstileSettings();
            }
        }

        private static async Task<bool> VerifyTurnstileAsync(
            string secretKey,
            string token,
            string remoteIp,
            string expectedHostname)
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                return false;
            }

            try
            {
                using (FormUrlEncodedContent content = new FormUrlEncodedContent(new Dictionary<string, string>
                {
                    { "secret", secretKey },
                    { "response", token },
                    { "remoteip", remoteIp ?? string.Empty }
                }))
                using (HttpResponseMessage response = await TurnstileHttpClient.PostAsync(
                    "https://challenges.cloudflare.com/turnstile/v0/siteverify",
                    content))
                {
                    if (!response.IsSuccessStatusCode)
                    {
                        return false;
                    }

                    string responseBody = await response.Content.ReadAsStringAsync();
                    TurnstileVerificationResponse verification =
                        JsonConvert.DeserializeObject<TurnstileVerificationResponse>(responseBody);

                    if (verification == null || !verification.success)
                    {
                        return false;
                    }

                    // Cloudflare's documented dummy key is valid only for local/test use.
                    if (string.Equals(secretKey, TurnstileAlwaysPassTestSecret, StringComparison.Ordinal))
                    {
                        return true;
                    }

                    return string.Equals(verification.action, "contact", StringComparison.Ordinal)
                        && string.Equals(
                            NormalizeHostname(verification.hostname),
                            NormalizeHostname(expectedHostname),
                            StringComparison.OrdinalIgnoreCase);
                }
            }
            catch (Exception exception)
            {
                Trace.TraceError("Turnstile verification failed: {0}", exception);
                return false;
            }
        }

        private sealed class TurnstileVerificationResponse
        {
            public bool success { get; set; }
            public string hostname { get; set; }
            public string action { get; set; }
        }

        private static string NormalizeHostname(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return string.Empty;
            }

            string normalized = value.Trim().ToLowerInvariant();
            return normalized.StartsWith("www.", StringComparison.Ordinal) ? normalized.Substring(4) : normalized;
        }

        private sealed class TurnstileSettings
        {
            public string SiteKey { get; set; }
            public string SecretKey { get; set; }

            public bool IsConfigured
            {
                get
                {
                    return !string.IsNullOrWhiteSpace(SiteKey)
                        && !string.IsNullOrWhiteSpace(SecretKey);
                }
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

        public bool SendEmail(string fromEmail, string subject, string emailBody)
        {
            try
            {
                //string senderEmail = System.Configuration.ConfigurationManager.AppSettings["SenderEmail"].ToString();
                //string senderPassword = System.Configuration.ConfigurationManager.AppSettings["SenderPassword"].ToString();
                SmtpClient client = new SmtpClient("relay-hosting.secureserver.net", 25);
                client.EnableSsl = false;
                client.Timeout = 100000;
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                client.UseDefaultCredentials = false;

                using (MailMessage message = new MailMessage())
                {
                    message.From = new MailAddress("office@shipleyswine.com");
                    message.ReplyToList.Add(new MailAddress(fromEmail));
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
                }

                return true;
            }
            catch (Exception e)
            {

                Trace.TraceError("Contact email delivery failed: {0}", e);
                return false;
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
