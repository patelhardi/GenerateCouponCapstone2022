using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GenerateCouponCapstone2022.Models
{
    public class Coupon
    {
        [Key]
        public int CouponID { get; set; }
        [Required]
        public string CouponCode { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        public DateTime ExpiryDate { get; set; }
        [Required]
        [AllowHtml]
        public string Description { get; set; }
        public string Image { get; set; }

        /* one-to-many relationship */
        [ForeignKey("Restaurant")]
        public int RestaurantID { get; set; }
        public virtual Restaurant Restaurant { get; set; }

        /* many-to-many relationship */
        public ICollection<Customer> customers { get; set; }
    }

    public class CouponDto
    {
        public int CouponID { get; set; }
        [Required(ErrorMessage = "Please Enter a valid Coupon Code")]
        public string CouponCode { get; set; }
        [Required(ErrorMessage = "Please Enter a Valid Title")]
        public string Title { get; set; }
        [Required(ErrorMessage = "Please Enter a Valid Expiry Date")]
        public DateTime ExpiryDate { get; set; }
        [Required(ErrorMessage = "Please Enter a Description")]
        [AllowHtml]
        public string Description { get; set; }
        public string Image { get; set; }
        public string RName { get; set; }
    }
}