using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GenerateCouponCapstone2022.Models.ViewModels
{
    public class AddEmail
    {
        public IEnumerable<CustomerDto> CustomerOptions { get; set; }
        public IEnumerable<CouponDto> CouponOptions { get; set; }
    }
}