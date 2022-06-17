using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GenerateCouponCapstone2022.Models.ViewModels
{
    public class ListCoupon
    {
        public IEnumerable<CouponDto> ListCoupons { get; set; }
        public IEnumerable<CouponDto> ExpiredCoupons { get; set; }
    }
}