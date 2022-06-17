using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GenerateCouponCapstone2022.Models.ViewModels
{
    public class DetailCoupon
    {
        public CouponDto SelectedCoupon { get; set; }
        public IEnumerable<CustomerDto> KeptCustomers { get; set; }
    }
}