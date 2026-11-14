using System;
using System.Collections.Generic;
using System.Text;

namespace bookStore.Entities
{
    public abstract class Coupon(string name)
    {
        public string Name { get; set; } = name;
    }

    public class AmountCoupon : Coupon
    {
        public decimal DiscountAmount { get; set; }

        public AmountCoupon(string name, decimal discountAmount) : base(name)
        {
            Name = name;

            if (discountAmount > 1000)
            {
                discountAmount = 1000;
            }

            DiscountAmount = discountAmount <= 0 ? 1m : discountAmount;
        }

        public override string ToString()
        {
            return $"{Name}: {DiscountAmount}";
        }
    }

    public class PercentCoupon : Coupon
    {
        public int DiscountPercent { get; set; }

        public PercentCoupon(string name, int discountPercent) : base(name)
        {
            Name = name;

            if (discountPercent > 100)
            {
                discountPercent = 99;
            }

            DiscountPercent = discountPercent <= 0 ? 1 : discountPercent;
        }

        public override string ToString()
        {
            return $"{Name}: {DiscountPercent}%";
        }
    }
}
