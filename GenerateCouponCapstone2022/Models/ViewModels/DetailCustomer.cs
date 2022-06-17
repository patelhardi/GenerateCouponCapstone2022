using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GenerateCouponCapstone2022.Models.ViewModels
{
    public class DetailCustomer
    {
        public CustomerDto SelectedCustomer { get; set; }
        public IEnumerable<RestaurantDto> RestaurantOptions { get; set; }

        public IEnumerable<CouponDto> KeptCoupons { get; set; }
    }
}