using GenerateCouponCapstone2022.Models;
using GenerateCouponCapstone2022.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.Diagnostics;

namespace GenerateCouponCapstone2022.Controllers
{
    public class EmailController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        private static readonly HttpClient client;
        private JavaScriptSerializer jss = new JavaScriptSerializer();
        static EmailController()
        {
            HttpClientHandler handler = new HttpClientHandler()
            {
                AllowAutoRedirect = false,
                //cookies are manually set in RequestHeader
                UseCookies = false
            };
            client = new HttpClient(handler);
            client.BaseAddress = new Uri("https://localhost:44308/api/");
        }

        /// <summary>
        /// Grabs the authentication cookie sent to this controller.
        /// For proper WebAPI authentication, you can send a post request with login credentials to the WebAPI and log the access token from the response. The controller already knows this token, so we're just passing it up the chain.
        /// 
        /// Here is a descriptive article which walks through the process of setting up authorization/authentication directly.
        /// https://docs.microsoft.com/en-us/aspnet/web-api/overview/security/individual-accounts-in-web-api
        /// </summary>
        private void GetApplicationCookie()
        {
            string token = "";
            //HTTP client is set up to be reused, otherwise it will exhaust server resources.
            //This is a bit dangerous because a previously authenticated cookie could be cached for
            //a follow-up request from someone else. Reset cookies in HTTP client before grabbing a new one.
            client.DefaultRequestHeaders.Remove("Cookie");
            if (!User.Identity.IsAuthenticated) return;

            HttpCookie cookie = System.Web.HttpContext.Current.Request.Cookies.Get(".AspNet.ApplicationCookie");
            if (cookie != null) token = cookie.Value;

            //collect token as it is submitted to the controller
            //use it to pass along to the WebAPI.
            Debug.WriteLine("Token Submitted is : " + token);
            if (token != "") client.DefaultRequestHeaders.Add("Cookie", ".AspNet.ApplicationCookie=" + token);

            return;
        }

        /// <summary>
        /// display list of all email page
        /// </summary>
        /// <returns>list of all emails</returns>
        // GET: Email/List
        public ActionResult List()
        {
            string url = "EmailData/ListEmails";
            HttpResponseMessage response = client.GetAsync(url).Result;
            IEnumerable<EmailDto> emails = response.Content.ReadAsAsync<IEnumerable<EmailDto>>().Result;
            return View(emails);
        }

        /// <summary>
        /// display detail of selected email
        /// </summary>
        /// <param name="id">passing parameter emailid</param>
        /// <returns>detail of selected email</returns>
        // GET: Email/Details/5
        public ActionResult Details(int id)
        {
            string url = "EmailData/FindEmail/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;
            EmailDto selectedEmail = response.Content.ReadAsAsync<EmailDto>().Result;
            return View(selectedEmail);
        }

        // GET: Email/Error
        public ActionResult Error()
        {
            return View();
        }

        /// <summary>
        /// display create new email page
        /// </summary>
        /// <returns>create new email record</returns>
        // GET: Email/New
        [Authorize(Roles = "Admin")]
        public ActionResult New()
        {
            AddEmail viewModel = new AddEmail();
            string url = "CustomerData/ListCustomersWhereIsSubscribed";
            HttpResponseMessage response = client.GetAsync(url).Result;
            IEnumerable<CustomerDto> customerOptions = response.Content.ReadAsAsync<IEnumerable<CustomerDto>>().Result;
            viewModel.CustomerOptions = customerOptions;

            url = "CouponData/ListCoupons";
            response = client.GetAsync(url).Result;
            IEnumerable<CouponDto> couponOptions = response.Content.ReadAsAsync<IEnumerable<CouponDto>>().Result;
            viewModel.CouponOptions = couponOptions;

            return View(viewModel);
        }

        /// <summary>
        /// create new email record into database
        /// </summary>
        /// <param name="email">passing parameter email object</param>
        /// <returns>create new email record into database</returns>
        // POST: Email/Create
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public ActionResult Create(Email email)
        {
            GetApplicationCookie();
            string url = "EmailData/AddEmail";

            string jsonpayload = jss.Serialize(email);

            HttpContent content = new StringContent(jsonpayload);
            content.Headers.ContentType.MediaType = "application/json";

            HttpResponseMessage response = client.PostAsync(url, content).Result;
            if (response.IsSuccessStatusCode)
            {
                MailMessage mail = new MailMessage();
                //Debug.WriteLine("customerID:" +email.CustomerID);
                Customer customeremail = db.Customers.Find(email.CustomerID);
                //Debug.WriteLine("customerID:" + customeremail.Email);
                //Coupon coupon = db.Coupons.Find(email.CouponID);
                mail.To.Add(customeremail.Email);
                mail.From = new MailAddress("patelhardi5@gmail.com");
                mail.Subject = "Your Loyalty Coupon";
                string Body = email.Message;
                mail.Body = Body;
                mail.IsBodyHtml = true;
                //Debug.WriteLine("Coupon:" + email.Coupon);
                //mail.Attachments.Add(new Attachment(email.Coupon));
                SmtpClient smtp = new SmtpClient();
                smtp.Host = "smtp.gmail.com";
                smtp.Port = 587;
                smtp.UseDefaultCredentials = false;
                smtp.Credentials = new System.Net.NetworkCredential("patelhardi5@gmail.com", "rvtgpvhzdhityleg"); // Enter senders User name and password  
                smtp.EnableSsl = true;
                smtp.Send(mail);
                TempData["Message"] = "Message sent Successfully...";
                return RedirectToAction("New");
            }
            else
            {
                TempData["Message"] = "Message can not sent...";
                return RedirectToAction("Error");
            }
        }

        /// <summary>
        /// display update mail page for selected email
        /// </summary>
        /// <param name="id">passing parameter emailid</param>
        /// <returns>update selected email page</returns>
        // GET: Email/Edit/5
        [Authorize(Roles = "Admin")]
        public ActionResult Edit(int id)
        {
            UpdateEmail viewModel = new UpdateEmail();

            //selected email information
            string url = "EmailData/FindEmail/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;
            EmailDto selectedEmail = response.Content.ReadAsAsync<EmailDto>().Result;
            viewModel.SelectedEmail = selectedEmail;

            //list of customer dropdown 
            url = "CustomerData/ListCustomersWhereIsSubscribed";
            response = client.GetAsync(url).Result;
            IEnumerable<CustomerDto> customerOptions = response.Content.ReadAsAsync<IEnumerable<CustomerDto>>().Result;
            viewModel.CustomerOptions = customerOptions;

            //list of coupon dropdown 
            url = "CouponData/ListCoupons";
            response = client.GetAsync(url).Result;
            IEnumerable<CouponDto> couponOptions = response.Content.ReadAsAsync<IEnumerable<CouponDto>>().Result;
            viewModel.CouponOptions = couponOptions;

            return View(viewModel);
        }

        /// <summary>
        /// update selected email record into database
        /// </summary>
        /// <param name="id">passing parameter emailid</param>
        /// <param name="email">passing parameter email object</param>
        /// <returns>updated email info for selected email</returns>
        // POST: Email/Update/5
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public ActionResult Update(int id, Email email)
        {
            GetApplicationCookie();
            string url = "EmailData/UpdateEmail/" + id;

            string jsonpayload = jss.Serialize(email);

            HttpContent content = new StringContent(jsonpayload);
            content.Headers.ContentType.MediaType = "application/json";

            HttpResponseMessage response = client.PostAsync(url, content).Result;
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("List");
            }
            else
            {
                return RedirectToAction("Error");
            }
        }

        /// <summary>
        /// display delete confirm page for selected email
        /// </summary>
        /// <param name="id">passing parameter emailid</param>
        /// <returns>delete confirm page for selected email</returns>
        // GET: Email/DeleteConfirm/5
        [Authorize(Roles = "Admin")]
        public ActionResult DeleteConfirm(int id)
        {
            string url = "EmailData/FindEmail/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;
            EmailDto selectedEmail = response.Content.ReadAsAsync<EmailDto>().Result;

            return View(selectedEmail);
        }

        /// <summary>
        /// delete record of selected email
        /// </summary>
        /// <param name="id">passing parameter emailid</param>
        /// <returns>deleted record of selected email</returns>
        // POST: Email/Delete/5
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public ActionResult Delete(int id)
        {
            GetApplicationCookie();
            string url = "EmailData/DeleteEmail/" + id;
            HttpContent content = new StringContent("");
            content.Headers.ContentType.MediaType = "application/json";

            HttpResponseMessage response = client.PostAsync(url, content).Result;
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("List");
            }
            else
            {
                return RedirectToAction("Error");
            }
        }
    }
}
