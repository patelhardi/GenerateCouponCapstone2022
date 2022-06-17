using GenerateCouponCapstone2022.Models;
using GenerateCouponCapstone2022.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace GenerateCouponCapstone2022.Controllers
{
    public class CustomerController : Controller
    {
        private static readonly HttpClient client;
        private JavaScriptSerializer jss = new JavaScriptSerializer();
        static CustomerController()
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
        /// display list of all customers
        /// </summary>
        /// <returns>list of all customer page</returns>
        // GET: Customer/List
        [Authorize(Roles = "Admin,Customer")]
        public ActionResult List()
        {
            GetApplicationCookie();
            string url = "CustomerData/ListCustomers";
            HttpResponseMessage response = client.GetAsync(url).Result;
            IEnumerable<CustomerDto> customers = response.Content.ReadAsAsync<IEnumerable<CustomerDto>>().Result;
            return View(customers);
        }

        /// <summary>
        /// display detail of selected customer
        /// </summary>
        /// <param name="id">passing parameter customerid</param>
        /// <returns>detail of selected customer</returns>
        // GET: Customer/Details/5
        [Authorize(Roles = "Admin,Customer")]
        public ActionResult Details(int id)
        {
            GetApplicationCookie();
            DetailCustomer viewModel = new DetailCustomer();
            string url = "CustomerData/FindCustomer/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;
            CustomerDto selectedCustomer = response.Content.ReadAsAsync<CustomerDto>().Result;
            viewModel.SelectedCustomer = selectedCustomer;

            url = "CouponData/ListCouponsForCustomer/" + id;
            response = client.GetAsync(url).Result;
            IEnumerable<CouponDto> keptCoupons = response.Content.ReadAsAsync<IEnumerable<CouponDto>>().Result;
            viewModel.KeptCoupons = keptCoupons;

            url = "RestaurantData/ListRestaurants";
            response = client.GetAsync(url).Result;
            IEnumerable<RestaurantDto> restaurantOptions = response.Content.ReadAsAsync<IEnumerable<RestaurantDto>>().Result;
            viewModel.RestaurantOptions = restaurantOptions;

            return View(viewModel);
        }

        ///- <summary>
        /// add new coupon in the perticular customer
        /// </summary>
        /// <param name="id">passing parameter customer id</param>
        /// <param name="RestaurantID">passing parameter restaurant id</param>
        /// <returns>add coupon in the perticular customer</returns>
        //GET: Customer/AssociateRestaurant/{customerid}
        [HttpGet]
        [Authorize(Roles = "Admin,Customer")]
        [Route("/Customer/AssociateRestaurant/{id}/{RestaurantID}")]
        public ActionResult AssociateRestaurant(int id, int RestaurantID)
        {
            //Debug.WriteLine("userid: " + id);
            GetApplicationCookie();
            //communicate with data controller class
            string url = "CouponData/ListCouponsForRestaurant/" + RestaurantID;
            HttpResponseMessage response = client.GetAsync(url).Result;
            IEnumerable<CouponDto> couponOptions = response.Content.ReadAsAsync<IEnumerable<CouponDto>>().Result;
            /*string url = "CustomerData/AssociateCustomerWithCoupon/" + id + "/" + CouponID;
            HttpContent content = new StringContent("");
            content.Headers.ContentType.MediaType = "application/json";
            HttpResponseMessage response = client.PostAsync(url, content).Result;*/

            return RedirectToAction("AssociateCoupon/" + id);
        }

        /// <summary>
        /// add new coupon in the perticular customer
        /// </summary>
        /// <param name="id">passing parameter customer id</param>
        /// <param name="CouponID">passing parameter coupon id</param>
        /// <returns>add coupon in the perticular customer</returns>
        //POST: Customer/AssociateCoupon/{customerid}/{couponid}
        [HttpPost]
        [Authorize(Roles = "Admin,Customer")]
        [Route("/Customer/AssociateCoupon/{id}/{CouponID}")]
        public ActionResult AssociateCoupon(int id, int CouponID)
        {
            //Debug.WriteLine("userid: " + id);
            GetApplicationCookie();
            //communicate with data controller class
            string url = "CustomerData/AssociateCustomerWithCoupon/" + id + "/" + CouponID;
            HttpContent content = new StringContent("");
            content.Headers.ContentType.MediaType = "application/json";
            HttpResponseMessage response = client.PostAsync(url, content).Result;

            return RedirectToAction("Details/" + id);
        }

        ///- <summary>
        /// remove coupon from the perticular customer
        /// </summary>
        /// <param name="id">passing parameter customer id</param>
        /// <param name="CouponID">passing parameter coupon id</param>
        /// <returns>remove coupon from perticular customer</returns>
        //GET: Customer/UnAssociate/{id}?CouponID={CouponID}
        [HttpGet]
        [Authorize(Roles = "Admin,Customer")]
        public ActionResult UnAssociate(int id, int CouponID)
        {
            GetApplicationCookie();
            //communicate with data controller class
            string url = "CustomerData/UnAssociateCustomerWithCoupon/" + id + "/" + CouponID;
            HttpContent content = new StringContent("");
            content.Headers.ContentType.MediaType = "application/json";
            HttpResponseMessage response = client.PostAsync(url, content).Result;

            return RedirectToAction("Details/" + id);
        }

        // GET: Customer/Error
        public ActionResult Error()
        {
            return View();
        }

        /// <summary>
        /// display create new customer data page
        /// </summary>
        /// <returns>create new customer data</returns>
        // GET: Customer/New
        [Authorize(Roles = "Admin,Customer")]
        public ActionResult New()
        {
            return View();
        }

        /// <summary>
        /// add new customer profile into database
        /// </summary>
        /// <param name="customer">passing parameter customer data</param>
        /// <returns>created new customer record</returns>
        // POST: Customer/Create
        [HttpPost]
        [Authorize(Roles = "Admin,Customer")]
        public ActionResult Create(Customer customer)
        {
            GetApplicationCookie();
            string url = "CustomerData/AddCustomer";

            string jsonpayload = jss.Serialize(customer);

            HttpContent content = new StringContent(jsonpayload);
            content.Headers.ContentType.MediaType = "application/json";

            HttpResponseMessage response = client.PostAsync(url, content).Result;
            if (response.IsSuccessStatusCode)
            {
                TempData["Message"] = "Customer added Successfully...";
                return RedirectToAction("List");
            }
            else
            {
                TempData["Message"] = "Customer can not added...";
                return RedirectToAction("Error");
            }
        }

        /// <summary>
        /// display update customer data page for selected customer
        /// </summary>
        /// <param name="id">passing parameter customerid</param>
        /// <returns>update customer info page</returns>
        // GET: Customer/Edit/5
        [Authorize(Roles = "Admin,Customer")]
        public ActionResult Edit(int id)
        {
            string url = "CustomerData/FindCustomer/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;
            CustomerDto selectedCustomer = response.Content.ReadAsAsync<CustomerDto>().Result;
            return View(selectedCustomer);
        }

        /// <summary>
        /// update record of selected customer into database
        /// </summary>
        /// <param name="id">passing parameter customerid</param>
        /// <param name="customer">passing parameter customer object</param>
        /// <returns>updated record for selected customer into database</returns>
        // POST: Customer/Update/5
        [HttpPost]
        [Authorize(Roles = "Admin,Customer")]
        public ActionResult Update(int id, Customer customer)
        {
            GetApplicationCookie();
            string url = "CustomerData/UpdateCustomer/" + id;

            string jsonpayload = jss.Serialize(customer);

            HttpContent content = new StringContent(jsonpayload);
            content.Headers.ContentType.MediaType = "application/json";

            HttpResponseMessage response = client.PostAsync(url, content).Result;
            if (response.IsSuccessStatusCode)
            {
                TempData["Message"] = "Customer updated Successfully...";
                return RedirectToAction("List");
            }
            else
            {
                TempData["Message"] = "Customer can not updated...";
                return RedirectToAction("Error");
            }
        }

        /// <summary>
        /// display delete confirm page for selected customer
        /// </summary>
        /// <param name="id">passing parameter customer id</param>
        /// <returns>delete confirm page for selected customer</returns>
        // GET: Customer/DeleteConfirm/5
        [Authorize(Roles = "Admin,Customer")]
        public ActionResult DeleteConfirm(int id)
        {
            string url = "CustomerData/FindCustomer/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;

            CustomerDto selectedCustomer = response.Content.ReadAsAsync<CustomerDto>().Result;
            return View(selectedCustomer);
        }

        /// <summary>
        /// delete record of selected customer
        /// </summary>
        /// <param name="id">passing parameter customerid</param>
        /// <returns>redirect to list page and delete record of selected customer</returns>
        // POST: Customer/Delete/5
        [HttpPost]
        [Authorize(Roles = "Admin,Customer")]
        public ActionResult Delete(int id)
        {
            GetApplicationCookie();
            string url = "CustomerData/DeleteCustomer/" + id;
            HttpContent content = new StringContent("");
            content.Headers.ContentType.MediaType = "application/json";

            HttpResponseMessage response = client.PostAsync(url, content).Result;
            if (response.IsSuccessStatusCode)
            {
                TempData["Message"] = "Customer deleted Successfully...";
                return RedirectToAction("List");
            }
            else
            {
                TempData["Message"] = "Customer can not deleted...";
                return RedirectToAction("Error");
            }
        }
    }
}
