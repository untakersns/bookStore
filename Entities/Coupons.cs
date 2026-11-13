using System;
using System.Collections.Generic;
using System.Text;

namespace bookStore.Entities
{
    public abstract class Coupon(int id)
    {
        public int Id { get; set; } = id;
    }

    public class AmountCoupon : Coupon
    {
        public decimal DiscountAmount { get; set; }

        public AmountCoupon(int id, decimal discountAmount) : base(id)
        {
            Id = id;

            if (discountAmount > 1000)
            {
                discountAmount = 1000;
            }

            DiscountAmount = discountAmount <= 0 ? 1m : discountAmount;
        }
    }

    public class PercentCoupon : Coupon
    {
        public int DiscountPercent { get; set; }

        public PercentCoupon(int id, int discountPercent) : base(id)
        {
            Id = id;

            if (discountPercent > 100)
            {
                discountPercent = 99;
            }

            DiscountPercent = discountPercent <= 0 ? 1 : discountPercent;
        }
    }
}
