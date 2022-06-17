using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using GenerateCouponCapstone2022.Models;

namespace GenerateCouponCapstone2022.Controllers
{
    public class RestaurantDataController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        /// <summary>
        /// display list of all restaurants
        /// select * from restaurant
        /// </summary>
        /// <returns>list of all restaurants</returns>
        // GET: api/RestaurantData/ListRestaurants
        [HttpGet]
        [ResponseType(typeof(RestaurantDto))]
        public IEnumerable<RestaurantDto> ListRestaurants()
        {
            List<Restaurant> Restaurants = db.Restaurants.ToList();
            List<RestaurantDto> RestaurantDtos = new List<RestaurantDto>();

            Restaurants.ForEach(r => RestaurantDtos.Add(new RestaurantDto()
            {
                RestaurantID = r.RestaurantID,
                Name = r.Name,
                Address = r.Address,
                Phone = r.Phone
            }));
            return RestaurantDtos;
        }

        /// <summary>
        /// detail data for selected restaurant
        /// select * from restaurant where restaurantid = 1
        /// </summary>
        /// <param name="id">passing parameter restaurantid</param>
        /// <returns>detail record for selected restaurant</returns>
        // GET: api/RestaurantData/FindRestaurant/5
        [ResponseType(typeof(Restaurant))]
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public IHttpActionResult FindRestaurant(int id)
        {
            Restaurant Restaurant = db.Restaurants.Find(id);
            RestaurantDto RestaurantDtos = new RestaurantDto()
            {
                RestaurantID = Restaurant.RestaurantID,
                Name = Restaurant.Name,
                Address = Restaurant.Address,
                Phone = Restaurant.Phone
            };
            if (Restaurant == null)
            {
                return NotFound();
            }

            return Ok(RestaurantDtos);
        }

        /// <summary>
        /// update record for selected restaurant
        /// update (name, address, ...) restaurant set ("aa", "123st", ...) where restaurantid = 1
        /// </summary>
        /// <param name="id">passing parameter restaurantid</param>
        /// <param name="restaurant">passing parameter restaurant object</param>
        /// <returns>updated data into database for selected restaurant</returns>
        // POST: api/RestaurantData/UpdateRestaurant/5
        [ResponseType(typeof(void))]
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public IHttpActionResult UpdateRestaurant(int id, Restaurant restaurant)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != restaurant.RestaurantID)
            {
                return BadRequest();
            }

            db.Entry(restaurant).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RestaurantExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        /// <summary>
        /// create new restaurant data into database
        /// insert into (name, address, ...) restaurant values ("aa", "123st", ...)
        /// </summary>
        /// <param name="restaurant">passing parameter restaurant object</param>
        /// <returns>create new restaurant data into database</returns>
        // POST: api/RestaurantData/AddRestaurant
        [ResponseType(typeof(Restaurant))]
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public IHttpActionResult AddRestaurant(Restaurant restaurant)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Restaurants.Add(restaurant);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = restaurant.RestaurantID }, restaurant);
        }

        /// <summary>
        /// delete record for selected restaurant 
        /// delete from restaurant where restaurantid = 1
        /// </summary>
        /// <param name="id">passing parameter restaurantid</param>
        /// <returns>delete record of selected restaurant from the database</returns>
        // POST: api/RestaurantData/DeleteRestaurant/5
        [ResponseType(typeof(Restaurant))]
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public IHttpActionResult DeleteRestaurant(int id)
        {
            Restaurant restaurant = db.Restaurants.Find(id);
            if (restaurant == null)
            {
                return NotFound();
            }

            db.Restaurants.Remove(restaurant);
            db.SaveChanges();

            return Ok();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool RestaurantExists(int id)
        {
            return db.Restaurants.Count(e => e.RestaurantID == id) > 0;
        }
    }
}