using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GenerateCouponCapstone2022.Models.ViewModels
{
    public class DetailRestaurant
    {
        public RestaurantDto SelectedRestaurant { get; set; }
        public IEnumerable<CouponDto> KeptCoupons { get; set; }
    }
}