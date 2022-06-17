using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace GenerateCouponCapstone2022.Models
{
    public class Customer
    {
        [Key]
        public int CustomerID { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        [DataType(DataType.EmailAddress)]
        [EmailAddress]
        public string Email { get; set; }
        public string Phone { get; set; }
        [Required]
        public bool IsSubscribed { get; set; }

        [ForeignKey("ApplicationUser")]
        public string UserID { get; set; }
        public virtual ApplicationUser ApplicationUser { get; set; }

        /* many-to-many relationship */
        public ICollection<Coupon> coupons{ get; set; }
    }
    public class CustomerDto
    {
        public int CustomerID { get; set; }
        [Required(ErrorMessage = "Please Enter a valid Customer Name")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Please Enter a valid Email Address")]
        [DataType(DataType.EmailAddress)]
        [EmailAddress]
        public string Email { get; set; }
        public string Phone { get; set; }
        [Required(ErrorMessage = "Do you want to subscribe?")]
        public bool IsSubscribed { get; set; }
        public string UserId { get; set; }
    }
}