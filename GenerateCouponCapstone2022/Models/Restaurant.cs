using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace GenerateCouponCapstone2022.Models
{
    public class Restaurant
    {
        [Key]
        public int RestaurantID { get; set; }
        [Required]
        public string Name { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
    }
    public class RestaurantDto
    {
        public int RestaurantID { get; set; }
        [Required(ErrorMessage = "Please Enter a valid Restaurant Name")]
        public string Name { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
    }
}