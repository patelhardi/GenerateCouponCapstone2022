using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;
using GenerateCouponCapstone2022.Models;
using Microsoft.AspNet.Identity;

namespace GenerateCouponCapstone2022.Controllers
{
    public class CouponDataController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        /// <summary>
        /// display list of all coupons
        /// select * from coupons where expirydate is greater than or equal to todaydate
        /// </summary>
        /// <returns>list of all coupons which are not expired</returns>
        // GET: api/CouponData/ListCoupons
        [HttpGet]
        public IEnumerable<CouponDto> ListCoupons()
        {
            List<Coupon> Coupons = db.Coupons.Where(coup => coup.ExpiryDate >= DateTime.Now).OrderBy(e => e.ExpiryDate).ToList();
            List<CouponDto> CouponDtos = new List<CouponDto>();

            Coupons.ForEach(c => CouponDtos.Add(new CouponDto() {
                CouponID = c.CouponID,
                Title = c.Title,
                CouponCode = c.CouponCode,
                ExpiryDate = c.ExpiryDate,
                Description = c.Description,
                Image = c.Image,
                RName = c.Restaurant.Name
            }));
            return CouponDtos;
        }

        /// <summary>
        /// display list of all coupons 
        /// select * from coupons where expirydate less than todaydate
        /// </summary>
        /// <returns>list of all expired coupons </returns>
        // GET: api/CouponData/ListExpiredCoupons
        [HttpGet]
        public IEnumerable<CouponDto> ListExpiredCoupons()
        {
            List<Coupon> Coupons = db.Coupons.Where(coup => coup.ExpiryDate < DateTime.Now).OrderBy(e => e.ExpiryDate).ToList();
            List<CouponDto> CouponDtos = new List<CouponDto>();

            Coupons.ForEach(c => CouponDtos.Add(new CouponDto()
            {
                CouponID = c.CouponID,
                Title = c.Title,
                CouponCode = c.CouponCode,
                ExpiryDate = c.ExpiryDate,
                Description = c.Description,
                Image = c.Image,
                RName = c.Restaurant.Name
            }));
            return CouponDtos;
        }

        /// <summary>
        /// display list of coupons for perticular restaurant
        /// </summary>
        /// <param name="id">passing parameter restaurant id</param>
        /// <returns>list of coupons for perticular restaurant</returns>
        // GET: api/CouponData/ListCouponsForRestaurant/1
        [HttpGet]
        [ResponseType(typeof(CouponDto))]
        public IHttpActionResult ListCouponsForRestaurant(int id)
        {
            List<Coupon> Coupons = db.Coupons.Where(
                r => r.RestaurantID == id
            ).ToList();
            List<CouponDto> CouponDtos = new List<CouponDto>();

            Coupons.ForEach(c => CouponDtos.Add(new CouponDto()
            {
                CouponID = c.CouponID,
                CouponCode = c.CouponCode,
                Title = c.Title,
                ExpiryDate = c.ExpiryDate,
                RName = c.Restaurant.Name
            }));
            return Ok(CouponDtos);
        }

        /// <summary>
        /// display list of coupons for perticular restaurant which is not expired and customer does not have in the used list
        /// </summary>
        /// <param name="id">passing parameter restaurant id</param>
        /// <returns>list of coupons for perticular restaurant</returns>
        // GET: api/CouponData/ListCouponsForRestaurantwithNotExpiredCoupons/1
        [HttpGet]
        [ResponseType(typeof(CouponDto))]
        public IHttpActionResult ListCouponsForRestaurantwithNotExpiredCoupons(int id)
        {
            string UserId = User.Identity.GetUserId();
            List<Coupon> Coupons = db.Coupons.Where(
                r => r.RestaurantID == id && r.ExpiryDate >= DateTime.Now && !r.customers.Any(c => c.UserID == UserId)
            ).ToList();
            List<CouponDto> CouponDtos = new List<CouponDto>();

            Coupons.ForEach(c => CouponDtos.Add(new CouponDto()
            {
                CouponID = c.CouponID,
                CouponCode = c.CouponCode,
                Title = c.Title,
                ExpiryDate = c.ExpiryDate,
                RName = c.Restaurant.Name
            }));
            return Ok(CouponDtos);
        }

        /// <summary>
        /// display list of coupons for perticular customer
        /// </summary>
        /// <param name="id">passing parameter customer id</param>
        /// <returns>list of coupons</returns>
        // GET: api/CouponData/ListCouponsForCustomer/1
        [HttpGet]
        [ResponseType(typeof(CouponDto))]
        public IHttpActionResult ListCouponsForCustomer(int id)
        {
            List<Coupon> Coupons = db.Coupons.Where(
                c => c.customers.Any(
                    cid => cid.CustomerID == id
            )).ToList();
            List<CouponDto> CouponsDtos = new List<CouponDto>();

            Coupons.ForEach(c => CouponsDtos.Add(new CouponDto()
            {
                CouponID = c.CouponID,
                CouponCode = c.CouponCode,
                Title = c.Title,
                RName = c.Restaurant.Name,
                ExpiryDate = c.ExpiryDate
            }));
            return Ok(CouponsDtos);
        }

        /// <summary>
        /// display detail of selected coupon
        /// select * from coupon where couponid = 1
        /// </summary>
        /// <param name="id">passing parameter couponid</param>
        /// <returns>detail of one coupon</returns>
        // GET: api/CouponData/FindCoupon/5
        [ResponseType(typeof(Coupon))]
        [HttpGet]
        public IHttpActionResult FindCoupon(int id)
        {
            Coupon Coupon = db.Coupons.Find(id);
            CouponDto CouponDtos = new CouponDto()
            {
                CouponID = Coupon.CouponID,
                CouponCode = Coupon.CouponCode,
                Title = Coupon.Title,
                ExpiryDate = Coupon.ExpiryDate,
                Description = Coupon.Description,
                Image = Coupon.Image,
                RName = Coupon.Restaurant.Name
            };
            if (Coupon == null)
            {
                return NotFound();
            }

            return Ok(CouponDtos);
        }

        /// <summary>
        /// update selected coupon data
        /// update coupon set ('aa', 'bb',...) where couponid = 1
        /// </summary>
        /// <param name="id">passing parameter couponid</param>
        /// <param name="coupon">passing parameter coupon data</param>
        /// <returns>updated coupon data into database</returns>
        // POST: api/CouponData/UpdateCoupon/5
        [ResponseType(typeof(void))]
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public IHttpActionResult UpdateCoupon(int id, Coupon coupon)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != coupon.CouponID)
            {
                return BadRequest();
            }

            db.Entry(coupon).State = EntityState.Modified;
            db.Entry(coupon).Property(c => c.Image).IsModified = false;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CouponExists(id))
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
        /// Receives coupon picture data, uploads it to the webserver and updates the coupon's image option
        /// update coupon image while edit coupon data
        /// </summary>
        /// <param name="id">the coupon id</param>
        /// <returns>status code 200 if successful.</returns>
        /// <example>
        /// POST: api/CouponData/UploadCouponPic/3
        /// HEADER: enctype=multipart/form-data
        /// FORM-DATA: image
        /// </example>
        /// https://stackoverflow.com/questions/28369529/how-to-set-up-a-web-api-controller-for-multipart-form-data

        [HttpPost]
        public IHttpActionResult UploadCouponPic(int id)
        {
            if (Request.Content.IsMimeMultipartContent())
            {
                Debug.WriteLine("Received multipart form data.");

                int numfiles = HttpContext.Current.Request.Files.Count;
                Debug.WriteLine("Files Received: " + numfiles);

                //Check if a file is posted
                if (numfiles == 1 && HttpContext.Current.Request.Files[0] != null)
                {
                    var couponPic = HttpContext.Current.Request.Files[0];
                    //Check if the file is empty
                    if (couponPic.ContentLength > 0)
                    {
                        //establish valid file types (can be changed to other file extensions if desired!)
                        var valtypes = new[] { "jpeg", "jpg", "png", "gif" };
                        var extension = Path.GetExtension(couponPic.FileName).Substring(1);
                        var fName = Path.GetFileNameWithoutExtension(couponPic.FileName);
                        //Check the extension of the file
                        if (valtypes.Contains(extension))
                        {
                            try
                            {
                                //file name is the id of the image
                                string fn = fName + "." + extension;

                                //get a direct file path to ~/Content/animals/{id}.{extension}
                                string path = Path.Combine(HttpContext.Current.Server.MapPath("~/Content/Images/Coupons/"), fn);

                                //save the file
                                couponPic.SaveAs(path);

                                //Update the coupon image fields in the database
                                Coupon SelectedCoupon = db.Coupons.Find(id);
                                SelectedCoupon.Image = fn;
                                db.Entry(SelectedCoupon).State = EntityState.Modified;

                                db.SaveChanges();

                            }
                            catch (Exception ex)
                            {
                                //Debug.WriteLine("Coupon Image was not saved successfully.");
                                //Debug.WriteLine("Exception:" + ex);
                                return BadRequest();
                            }
                        }
                    }

                }
                return Ok();
            }
            else
            {
                //not multipart form data
                return BadRequest();
            }
        }

        /// <summary>
        /// Receives coupon picture data, uploads it to the webserver and updates the coupon's HasPic option
        /// Receive image while create coupon object
        /// </summary>
        /// <returns>status code 200 if successful.</returns>
        /// <example>
        /// POST: api/CouponData/UploadCouponPic
        /// HEADER: enctype=multipart/form-data
        /// FORM-DATA: image
        /// </example>
        /// https://stackoverflow.com/questions/28369529/how-to-set-up-a-web-api-controller-for-multipart-form-data

        [HttpPost]
        public IHttpActionResult UploadCouponPic()
        {
            if (Request.Content.IsMimeMultipartContent())
            {
                Debug.WriteLine("Received multipart form data.");

                int numfiles = HttpContext.Current.Request.Files.Count;
                Debug.WriteLine("Files Received: " + numfiles);

                //Check if a file is posted
                if (numfiles == 1 && HttpContext.Current.Request.Files[0] != null)
                {
                    var couponPic = HttpContext.Current.Request.Files[0];
                    //Check if the file is empty
                    if (couponPic.ContentLength > 0)
                    {
                        //establish valid file types (can be changed to other file extensions if desired!)
                        var valtypes = new[] { "jpeg", "jpg", "png", "gif" };
                        var extension = Path.GetExtension(couponPic.FileName).Substring(1);
                        var fName = Path.GetFileNameWithoutExtension(couponPic.FileName);
                        //Check the extension of the file
                        if (valtypes.Contains(extension))
                        {
                            try
                            {
                                //file name is the id of the image
                                string fn = fName + "." + extension;
                                //Debug.WriteLine("File Name:" + fn);

                                //get a direct file path to ~/Content/Coupons/{fName}.{extension}
                                string path = Path.Combine(HttpContext.Current.Server.MapPath("~/Content/Images/Coupons/"), fn);

                                //save the file
                                couponPic.SaveAs(path);
                            }
                            catch (Exception ex)
                            {
                                Debug.WriteLine("Coupon Image was not saved successfully.");
                                Debug.WriteLine("Exception:" + ex);
                                return BadRequest();
                            }
                        }
                    }

                }
                return Ok();
            }
            else
            {
                //not multipart form data
                return BadRequest();
            }
        }

        /// <summary>
        /// create new coupon object
        /// insert into coupon values ('aa','bb',...)
        /// </summary>
        /// <param name="coupon">passing parameter coupon data</param>
        /// <returns>created new coupon data into database</returns>
        // POST: api/CouponData/AddCoupon
        [ResponseType(typeof(Coupon))]
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public IHttpActionResult AddCoupon(Coupon coupon)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            db.Coupons.Add(coupon);
            db.SaveChanges();
            
            return CreatedAtRoute("DefaultApi", new { id = coupon.CouponID }, coupon);
        }

        /// <summary>
        /// delete record of selected coupon
        /// delete from coupon where couponid = 1
        /// </summary>
        /// <param name="id">passing parameter couponid</param>
        /// <returns>deleted record of selected coupon from the database</returns>
        // POST: api/CouponData/DeleteCoupon/5
        [ResponseType(typeof(Coupon))]
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public IHttpActionResult DeleteCoupon(int id)
        {
            Coupon coupon = db.Coupons.Find(id);
            if (coupon == null)
            {
                return NotFound();
            }

            db.Coupons.Remove(coupon);
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

        private bool CouponExists(int id)
        {
            return db.Coupons.Count(e => e.CouponID == id) > 0;
        }
    }
}