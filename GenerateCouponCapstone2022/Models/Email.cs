using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GenerateCouponCapstone2022.Models
{
    public class Email
    {
        [Key]
        public int EmailID { get; set; }
        [AllowHtml]
        public string Message { get; set; }
        public DateTime CreatedAt { get; set; }

        /* one-to-many relationship */
        [ForeignKey("Customer")]
        public int CustomerID { get; set; }
        public virtual Customer Customer { get; set; }

        [ForeignKey("Coupon")]
        public int CouponID { get; set; }
        public virtual Coupon Coupon { get; set; }
    }
    public class EmailDto
    {
        public int EmailID { get; set; }
        [AllowHtml]
        public string Message { get; set; }
        public DateTime CreatedAt { get; set; }
        public string CustomerName { get; set; }
        public string CouponCode { get; set; }
    }
}