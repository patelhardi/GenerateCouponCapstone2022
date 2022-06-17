using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using GenerateCouponCapstone2022.Models;
using GenerateCouponCapstone2022.Models.ViewModels;

namespace GenerateCouponCapstone2022.Controllers
{
    public class RestaurantController : Controller
    {
        private static readonly HttpClient client;
        private JavaScriptSerializer jss = new JavaScriptSerializer();
        static RestaurantController()
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
        /// display list of all restaurant page
        /// </summary>
        /// <returns>list of all restaurant page</returns>
        // GET: Restaurant/List
        public ActionResult List()
        {
            GetApplicationCookie();
            string url = "RestaurantData/ListRestaurants";
            HttpResponseMessage response = client.GetAsync(url).Result;
            IEnumerable<RestaurantDto> restaurants = response.Content.ReadAsAsync<IEnumerable<RestaurantDto>>().Result;
            return View(restaurants);
        }

        /// <summary>
        /// display detail page for selected restaurant
        /// </summary>
        /// <param name="id">passing parameter restaurantid</param>
        /// <returns>detail restaurant page for selected restaurant</returns>
        // GET: Restaurant/Details/5
        [Authorize(Roles = "Admin")]
        public ActionResult Details(int id)
        {
            GetApplicationCookie();

            DetailRestaurant viewModel = new DetailRestaurant();

            string url = "RestaurantData/FindRestaurant/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;
            RestaurantDto selectedRestaurant = response.Content.ReadAsAsync<RestaurantDto>().Result;
            viewModel.SelectedRestaurant = selectedRestaurant;

            url = "CouponData/ListCouponsForRestaurant/" + id;
            response = client.GetAsync(url).Result;
            IEnumerable<CouponDto> keptCoupons = response.Content.ReadAsAsync<IEnumerable<CouponDto>>().Result;
            viewModel.KeptCoupons = keptCoupons;

            return View(viewModel);
        }

        // GET: Restaurant/Error
        public ActionResult Error()
        {
            return View();
        }

        /// <summary>
        /// display create new restaurant info page
        /// </summary>
        /// <returns>create new restaurant page</returns>
        // GET: Restaurant/New
        [Authorize(Roles = "Admin")]
        public ActionResult New()
        {
            return View();
        }

        /// <summary>
        /// create record for new restaurant
        /// </summary>
        /// <param name="restaurant">passing parameter restaurant object</param>
        /// <returns>create new restaurant data into database</returns>
        // POST: Restaurant/Create
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public ActionResult Create(Restaurant restaurant)
        {
            GetApplicationCookie();
            string url = "RestaurantData/AddRestaurant";

            string jsonpayload = jss.Serialize(restaurant);

            HttpContent content = new StringContent(jsonpayload);
            content.Headers.ContentType.MediaType = "application/json";

            HttpResponseMessage response = client.PostAsync(url, content).Result;
            if (response.IsSuccessStatusCode)
            {
                TempData["Message"] = "Restaurant added Successfully...";
                return RedirectToAction("List");
            }
            else
            {
                TempData["Message"] = "Restaurant can not added...";
                return RedirectToAction("Error");
            }
        }

        /// <summary>
        /// display update restaurant data page for selected restaurant
        /// </summary>
        /// <param name="id">passing parameter restaurantid</param>
        /// <returns>update selected restaurant page</returns>
        // GET: Restaurant/Edit/5
        [Authorize(Roles = "Admin")]
        public ActionResult Edit(int id)
        {
            string url = "RestaurantData/FindRestaurant/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;
            RestaurantDto selectedRestaurant = response.Content.ReadAsAsync<RestaurantDto>().Result;
            return View(selectedRestaurant);
        }

        /// <summary>
        /// update data for selected restaurant
        /// </summary>
        /// <param name="id">passing parameter restaurantid</param>
        /// <param name="restaurant">passing parameter restaurant object</param>
        /// <returns>update data for selected restaurant into database</returns>
        // POST: Restaurant/Update/5
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public ActionResult Update(int id, Restaurant restaurant)
        {
            GetApplicationCookie();
            string url = "RestaurantData/UpdateRestaurant/" + id;

            string jsonpayload = jss.Serialize(restaurant);

            HttpContent content = new StringContent(jsonpayload);
            content.Headers.ContentType.MediaType = "application/json";

            HttpResponseMessage response = client.PostAsync(url, content).Result;
            if (response.IsSuccessStatusCode)
            {
                TempData["Message"] = "Restaurant updated Successfully...";
                return RedirectToAction("List");
            }
            else
            {
                TempData["Message"] = "Restaurant can not updated...";
                return RedirectToAction("Error");
            }
        }

        /// <summary>
        /// display delete confirm page for selected restaurant
        /// </summary>
        /// <param name="id">passing parameter restaurantid</param>
        /// <returns>delete confirm page for selected restaurant</returns>
        // GET: Restaurant/DeleteConfirm/5
        [Authorize(Roles = "Admin")]
        public ActionResult DeleteConfirm(int id)
        {
            string url = "RestaurantData/FindRestaurant/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;

            RestaurantDto selectedRestaurant = response.Content.ReadAsAsync<RestaurantDto>().Result;
            return View(selectedRestaurant);
        }

        /// <summary>
        /// delete record of selected restaurant
        /// </summary>
        /// <param name="id">passing parameter restaurantid</param>
        /// <returns>deleted record for selected restaurant</returns>
        // POST: Restaurant/Delete/5
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public ActionResult Delete(int id)
        {
            GetApplicationCookie();
            string url = "RestaurantData/DeleteRestaurant/" + id;
            HttpContent content = new StringContent("");
            content.Headers.ContentType.MediaType = "application/json";

            HttpResponseMessage response = client.PostAsync(url, content).Result;
            if (response.IsSuccessStatusCode)
            {
                TempData["Message"] = "Restaurant deleted Successfully...";
                return RedirectToAction("List");
            }
            else
            {
                TempData["Message"] = "Restaurant can not deleted...";
                return RedirectToAction("Error");
            }
        }
    }
}
