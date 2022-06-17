using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GenerateCouponCapstone2022.Models.ViewModels
{
    public class UpdateCoupon
    {
        public CouponDto SelectedCoupon { get; set; }
        public IEnumerable<RestaurantDto> RestaurantOptions { get; set; }
    }
}