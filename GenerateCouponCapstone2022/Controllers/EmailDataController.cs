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
    public class EmailDataController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        /// <summary>
        /// list of all emails
        /// select * from email
        /// </summary>
        /// <returns>list of all emails</returns>
        // GET: api/EmailData/ListEmails
        [HttpGet]
        public IEnumerable<EmailDto> ListEmails()
        {
            List<Email> Emails = db.Emails.ToList();
            List<EmailDto> EmailDtos = new List<EmailDto>();

            Emails.ForEach(e => EmailDtos.Add(new EmailDto()
            {
                EmailID = e.EmailID,
                Message = e.Message,
                CreatedAt = e.CreatedAt,
                CustomerName = e.Customer.Name,
                CouponCode = e.Coupon.CouponCode
            }));
            return EmailDtos;
        }

        /// <summary>
        /// detail data for selected email
        /// select * from email where emailid = 1
        /// </summary>
        /// <param name="id">passing parameter emailid</param>
        /// <returns>detail of selected email</returns>
        // GET: api/EmailData/FindEmail/5
        [ResponseType(typeof(Email))]
        [HttpGet]
        public IHttpActionResult FindEmail(int id)
        {
            Email Email = db.Emails.Find(id);
            EmailDto EmailDtos = new EmailDto()
            {
                EmailID = Email.EmailID,
                Message = Email.Message,
                CreatedAt = Email.CreatedAt,
                CustomerName = Email.Customer.Name,
                CouponCode = Email.Coupon.CouponCode
            };
            if (Email == null)
            {
                return NotFound();
            }

            return Ok(EmailDtos);
        }

        /// <summary>
        /// update selected email record into database
        /// update (customerid, message, ...) email set (1, "aabbc", ...) where emailid = 1
        /// </summary>
        /// <param name="id">passing parameter emailid</param>
        /// <param name="email">passing prameter email object</param>
        /// <returns>update record of selected email into database</returns>
        // POST: api/EmailData/UpdateEmail/5
        [ResponseType(typeof(void))]
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public IHttpActionResult UpdateEmail(int id, Email email)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != email.EmailID)
            {
                return BadRequest();
            }

            db.Entry(email).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EmailExists(id))
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
        /// insert new email record into database
        /// insert into (customerid, message,...) email values (1, "aabb", ...)
        /// </summary>
        /// <param name="email">passing parameter email object</param>
        /// <returns>created new email record into database</returns>
        // POST: api/EmailData/AddEmail
        [ResponseType(typeof(Email))]
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public IHttpActionResult AddEmail(Email email)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            db.Emails.Add(email);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = email.EmailID }, email);
        }

        /// <summary>
        /// delete selected email record
        /// delete from email where emailid = 1
        /// </summary>
        /// <param name="id">passing parameter emailid</param>
        /// <returns>deleted record for selected email from database</returns>
        // POST: api/EmailData/DeleteEmail/5
        [ResponseType(typeof(Email))]
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public IHttpActionResult DeleteEmail(int id)
        {
            Email email = db.Emails.Find(id);
            if (email == null)
            {
                return NotFound();
            }

            db.Emails.Remove(email);
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

        private bool EmailExists(int id)
        {
            return db.Emails.Count(e => e.EmailID == id) > 0;
        }
    }
}