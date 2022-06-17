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
using Microsoft.AspNet.Identity;

namespace GenerateCouponCapstone2022.Controllers
{
    public class CustomerDataController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        /// <summary>
        /// display list of all customers
        /// select * from customer
        /// for admin role - display all customer list
        /// for customer role - display logged in customer list
        /// </summary>
        /// <returns>list of all customers</returns>
        // GET: api/CustomerData/ListCustomers
        [HttpGet]
        [ResponseType(typeof(CustomerDto))]
        [Authorize(Roles = "Admin,Customer")]
        public IEnumerable<CustomerDto> ListCustomers()
        {
            bool isAdmin = User.IsInRole("Admin");

            List<Customer> Customers; 
            if (isAdmin) Customers = db.Customers.ToList();
            else
            {
                string UserId = User.Identity.GetUserId();
                Customers = db.Customers.Where(c => c.UserID == UserId).ToList();
            }
            List<CustomerDto> CustomerDtos = new List<CustomerDto>();

            Customers.ForEach(c => CustomerDtos.Add(new CustomerDto()
            {
                CustomerID = c.CustomerID,
                Name = c.Name,
                Email = c.Email,
                Phone = c.Phone,
                IsSubscribed = c.IsSubscribed,
                UserId = c.UserID
            }));
            return CustomerDtos;
        }

        /// <summary>
        /// list of all customers where subscribed field is true
        /// select * from customer where issubscribed = true
        /// </summary>
        /// <returns>list of all subscribed customer list</returns>
        // GET: api/CustomerData/ListCustomersWhereIsSubscribed
        [HttpGet]
        public IEnumerable<CustomerDto> ListCustomersWhereIsSubscribed()
        {
            List<Customer> Customers = db.Customers.Where(C => C.IsSubscribed == true).ToList();
            List<CustomerDto> CustomerDtos = new List<CustomerDto>();

            Customers.ForEach(c => CustomerDtos.Add(new CustomerDto()
            {
                CustomerID = c.CustomerID,
                Name = c.Name,
                Email = c.Email,
                Phone = c.Phone,
                IsSubscribed = c.IsSubscribed
            }));
            return CustomerDtos;
        }

        /// <summary>
        /// display list of customers for perticular coupon
        /// </summary>
        /// <param name="id">passing parameter coupon id</param>
        /// <returns>list of customers for perticular coupon</returns>
        // GET: api/CustomerData/ListCustomersForCoupon/1
        [HttpGet]
        [ResponseType(typeof(CustomerDto))]
        public IHttpActionResult ListCustomersForCoupon(int id)
        {
            List<Customer> Customers = db.Customers.Where(
                c => c.coupons.Any(
                    cid => cid.CouponID == id
            )).ToList();
            List<CustomerDto> CustomerDtos = new List<CustomerDto>();

            Customers.ForEach(c => CustomerDtos.Add(new CustomerDto()
            {
                CustomerID = c.CustomerID,
                Name = c.Name,
                Phone = c.Phone,
                Email = c.Email,
                IsSubscribed = c.IsSubscribed
            }));
            return Ok(CustomerDtos);
        }

        /// <summary>
        /// add new coupon in the perticular customer
        /// </summary>
        /// <param name="userid">passing parameter customer id</param>
        /// <param name="couponid">passing parameter coupon id</param>
        /// <returns>add new coupon in the perticular customer</returns>
        // POST: api/CustomerData/AssociateCustomerWithCoupon/2/7
        [ResponseType(typeof(void))]
        [HttpPost]
        [Route("api/CustomerData/AssociateCustomerWithCoupon/{id}/{couponid}")]
        public IHttpActionResult AssociateCustomerWithCoupon(int id, int couponid)
        {
            Customer SelectedCustomer = db.Customers.Include(c => c.coupons).Where(cid => cid.CustomerID == id).FirstOrDefault();
            Coupon SelectedCoupon = db.Coupons.Find(couponid);

            SelectedCustomer.coupons.Add(SelectedCoupon);
            db.SaveChanges();
            return Ok();
        }

        /// <summary>
        /// detail of selected customer
        /// select * from customer where id = 1
        /// </summary>
        /// <param name="id">passing parameter customerid</param>
        /// <returns>detail of selected customer</returns>
        // GET: api/CustomerData/FindCustomer/5
        [ResponseType(typeof(Customer))]
        [HttpGet]
        [Authorize(Roles = "Admin,Customer")]
        public IHttpActionResult FindCustomer(int id)
        {
            Customer Customer = db.Customers.Find(id);
            CustomerDto CustomerDtos = new CustomerDto()
            {
                CustomerID = Customer.CustomerID,
                Name = Customer.Name,
                Email = Customer.Email,
                Phone = Customer.Phone,
                IsSubscribed = Customer.IsSubscribed,
                UserId = Customer.UserID
            };
            if (Customer == null)
            {
                return NotFound();
            }
            //do not process if the (user is not an admin) and (the booking does not belong to the user)
            bool isAdmin = User.IsInRole("Admin");
            //Forbidden() isn't a natively implemented status like BadRequest()
            if (!isAdmin && (Customer.UserID != User.Identity.GetUserId())) return StatusCode(HttpStatusCode.Forbidden);
            return Ok(CustomerDtos);
        }

        /// <summary>
        /// update customer record for selected customer into database
        /// update customer (name, address, ...) set ("aa", "123st", ...) where customerid = 1 
        /// </summary>
        /// <param name="id">passing parameter customerid</param>
        /// <param name="customer">passing parmeter customer object</param>
        /// <returns>update record of selected customer into database</returns>
        // POST: api/CustomerData/UpdateCustomer/5
        [ResponseType(typeof(void))]
        [HttpPost]
        [Authorize(Roles = "Admin,Customer")]
        public IHttpActionResult UpdateCustomer(int id, Customer customer)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != customer.CustomerID)
            {
                return BadRequest();
            }

            //do not process if the (user is not an admin) and (the booking does not belong to the user)
            bool isAdmin = User.IsInRole("Admin");
            //Forbidden() isn't a natively implemented status like BadRequest()
            if (!isAdmin && (customer.UserID != User.Identity.GetUserId()))
            {
                //Debug.WriteLine("not allowed. booking user" + Booking.UserID + " user " + User.Identity.GetUserId());
                return StatusCode(HttpStatusCode.Forbidden);
            }

            db.Entry(customer).State = EntityState.Modified;
            db.Entry(customer).Property(c => c.UserID).IsModified = false;
            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CustomerExists(id))
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
        /// create new customer detail
        /// insert into (name, address, phone,...) customer values ("aa", "123st",...)
        /// </summary>passing parameter customer object
        /// <param name="customer"></param>
        /// <returns>create customer record into database</returns>
        // POST: api/CustomerData/AddCustomer
        [ResponseType(typeof(Customer))]
        [HttpPost]
        [Authorize(Roles = "Admin,Customer")]
        public IHttpActionResult AddCustomer(Customer customer)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            customer.UserID = User.Identity.GetUserId();

            db.Customers.Add(customer);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = customer.CustomerID }, customer);
        }

        /// <summary>
        /// delete record of selected customer
        /// delete from customer where customerid = 1
        /// </summary>
        /// <param name="id">passing parameter customerid</param>
        /// <returns>delete record of selected customer into database</returns>
        // POST: api/CustomerData/DeleteCustomer/5
        [ResponseType(typeof(Customer))]
        [HttpPost]
        [Authorize(Roles = "Admin,Customer")]
        public IHttpActionResult DeleteCustomer(int id)
        {
            Customer customer = db.Customers.Find(id);
            if (customer == null)
            {
                return NotFound();
            }

            //do not process if the (user is not an admin) and (the booking does not belong to the user)
            bool isAdmin = User.IsInRole("Admin");
            //Forbidden() isn't a natively implemented status like BadRequest()
            if (!isAdmin && (customer.UserID != User.Identity.GetUserId())) return StatusCode(HttpStatusCode.Forbidden);

            db.Customers.Remove(customer);
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

        private bool CustomerExists(int id)
        {
            return db.Customers.Count(e => e.CustomerID == id) > 0;
        }
    }
}