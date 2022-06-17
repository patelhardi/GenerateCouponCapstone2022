using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using GenerateCouponCapstone2022.Models;
using GenerateCouponCapstone2022.Models.ViewModels;
using Microsoft.AspNet.Identity;

namespace GenerateCouponCapstone2022.Controllers
{
    public class CouponController : Controller
    {
        private static readonly HttpClient client;
        private JavaScriptSerializer jss = new JavaScriptSerializer();
        static CouponController()
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
        /// Display list of all coupons and seperate it as per the expiry date
        /// </summary>
        /// <returns>list of coupons</returns>
        // GET: Coupon/List
        [HttpGet]
        public ActionResult List()
        {
            ListCoupon viewModel = new ListCoupon();

            string url = "CouponData/ListCoupons";
            HttpResponseMessage response = client.GetAsync(url).Result;
            IEnumerable<CouponDto> listCoupons = response.Content.ReadAsAsync<IEnumerable<CouponDto>>().Result;
            viewModel.ListCoupons = listCoupons;

            url = "CouponData/ListExpiredCoupons";
            response = client.GetAsync(url).Result;
            IEnumerable<CouponDto> expiredCoupons = response.Content.ReadAsAsync<IEnumerable<CouponDto>>().Result;
            viewModel.ExpiredCoupons = expiredCoupons;

            return View(viewModel);
        }

        /// <summary>
        /// display details of one coupon
        /// </summary>
        /// <param name="id">passing parameter couponid</param>
        /// <returns>detail of selected coupon</returns>
        // GET: Coupon/Details/5
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public ActionResult Details(int id)
        {
            DetailCoupon viewModel = new DetailCoupon();

            string url = "CouponData/FindCoupon/" +id;
            HttpResponseMessage response = client.GetAsync(url).Result;
            CouponDto selectedCoupon = response.Content.ReadAsAsync<CouponDto>().Result;
            viewModel.SelectedCoupon = selectedCoupon;

            url = "CustomerData/ListCustomersForCoupon/" + id;
            response = client.GetAsync(url).Result;
            IEnumerable<CustomerDto> keptCustomers = response.Content.ReadAsAsync<IEnumerable<CustomerDto>>().Result;
            //Debug.WriteLine(keptCustomers);
            viewModel.KeptCustomers = keptCustomers;

            return View(viewModel);
        }

        // GET: Coupon/Error
        public ActionResult Error()
        {
            return View();
        }

        /// <summary>
        /// display create new coupon page
        /// </summary>
        /// <returns>create new coupon page</returns>
        // GET: Coupon/New
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public ActionResult New()
        {
            UpdateCoupon viewModel = new UpdateCoupon();
            string url = "RestaurantData/ListRestaurants";
            HttpResponseMessage response = client.GetAsync(url).Result;
            IEnumerable<RestaurantDto> restaurantOptions = response.Content.ReadAsAsync<IEnumerable<RestaurantDto>>().Result;
            viewModel.RestaurantOptions = restaurantOptions;
            return View(viewModel);
        }

        /// <summary>
        /// create new coupon object
        /// </summary>
        /// <param name="coupon">passing parameter coupon info</param>
        /// <param name="CouponPic">passing parameter coupon pic object</param>
        /// <returns>list of all coupons page and display new added coupon</returns>
        // POST: Coupon/Create
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public ActionResult Create(Coupon coupon, HttpPostedFileBase CouponPic)
        {
            GetApplicationCookie();
            string url;
            HttpResponseMessage response;
            if (CouponPic != null)
            {
                url = "CouponData/UploadCouponPic";

                MultipartFormDataContent requestcontent = new MultipartFormDataContent();
                HttpContent imagecontent = new StreamContent(CouponPic.InputStream);
                coupon.Image = CouponPic.FileName;
                requestcontent.Add(imagecontent, "CouponPic", CouponPic.FileName);
                response = client.PostAsync(url, requestcontent).Result;
            }
            url = "CouponData/AddCoupon";

            string jsonpayload = jss.Serialize(coupon);

            HttpContent content = new StringContent(jsonpayload);
            content.Headers.ContentType.MediaType = "application/json";
            response = client.PostAsync(url, content).Result;
            if (response.IsSuccessStatusCode)
            {
                TempData["Message"] = "Coupon added Successfully...";
                return RedirectToAction("List");
            }
            else
            {
                TempData["Message"] = "Coupon can not added...";
                return RedirectToAction("Error");
            }
        }

        /// <summary>
        /// display update selected coupon page
        /// </summary>
        /// <param name="id">passing parameter couponid</param>
        /// <returns>update coupon page</returns>
        // GET: Coupon/Edit/5
        [Authorize(Roles = "Admin")]
        public ActionResult Edit(int id)
        {
            UpdateCoupon viewModel = new UpdateCoupon();

            //selected coupon information
            string url = "CouponData/FindCoupon/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;
            CouponDto selectedCoupon = response.Content.ReadAsAsync<CouponDto>().Result;
            viewModel.SelectedCoupon = selectedCoupon;

            //list of restaurant dropdown 
            url = "RestaurantData/ListRestaurants";
            response = client.GetAsync(url).Result;
            IEnumerable<RestaurantDto> restaurantOptions = response.Content.ReadAsAsync<IEnumerable<RestaurantDto>>().Result;
            viewModel.RestaurantOptions = restaurantOptions;
            return View(viewModel);
        }

        /// <summary>
        /// update selected coupon data
        /// </summary>
        /// <param name="id">passing parameter couponid</param>
        /// <param name="coupon">passing parameter coupon info</param>
        /// <param name="CouponPic">passing parameter coupon image info</param>
        /// <returns>list all coupons page and display updated data </returns>
        // POST: Coupon/Update/5
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public ActionResult Update(int id, Coupon coupon, HttpPostedFileBase CouponPic)
        {
            GetApplicationCookie();
            string url = "CouponData/UpdateCoupon/" + id;

            string jsonpayload = jss.Serialize(coupon);

            HttpContent content = new StringContent(jsonpayload);
            content.Headers.ContentType.MediaType = "application/json";

            HttpResponseMessage response = client.PostAsync(url, content).Result;
            if (response.IsSuccessStatusCode && CouponPic != null)
            {
                url = "CouponData/UploadCouponPic/" + id;

                MultipartFormDataContent requestcontent = new MultipartFormDataContent();
                HttpContent imagecontent = new StreamContent(CouponPic.InputStream);
                requestcontent.Add(imagecontent, "CouponPic", CouponPic.FileName);
                response = client.PostAsync(url, requestcontent).Result;
                //Debug.WriteLine("Response after upload pic:" + response);
                TempData["Message"] = "Coupon updated Successfully...";
                return RedirectToAction("List");
            }
            else if (response.IsSuccessStatusCode)
            {
                TempData["Message"] = "Coupon updated Successfully...";
                return RedirectToAction("List");
            }
            else
            {
                TempData["Message"] = "Coupon Can not updated...";
                return RedirectToAction("Error");
            }
        }

        /// <summary>
        /// display delete confirm page for selected coupon
        /// </summary>
        /// <param name="id">passing parameter couponid</param>
        /// <returns>delete confirm page</returns>
        // GET: Coupon/DeleteConfirm/5
        [Authorize(Roles = "Admin")]
        public ActionResult DeleteConfirm(int id)
        {
            string url = "CouponData/FindCoupon/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;

            CouponDto selectedCoupon = response.Content.ReadAsAsync<CouponDto>().Result;
            return View(selectedCoupon);
        }

        /// <summary>
        /// delete record of selected coupon
        /// </summary>
        /// <param name="id">passing parameter couponid</param>
        /// <returns>list of all coupon page and delete selected coupon data</returns>
        // POST: Coupon/Delete/5
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public ActionResult Delete(int id)
        {
            GetApplicationCookie();
            string url = "CouponData/DeleteCoupon/" + id;
            HttpContent content = new StringContent("");
            content.Headers.ContentType.MediaType = "application/json";

            HttpResponseMessage response = client.PostAsync(url, content).Result;
            if (response.IsSuccessStatusCode)
            {
                TempData["Message"] = "Coupon deleted Successfully...";
                return RedirectToAction("List");
            }
            else
            {
                TempData["Message"] = "Coupon can not deleted...";
                return RedirectToAction("Error");
            }
        }
    }
}
