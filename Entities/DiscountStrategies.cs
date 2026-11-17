using System;
using System.Collections.Generic;
using System.Text;

namespace bookStore.Entities
{
    public interface IDiscountStrategy
    {
        decimal ApplyDiscount(decimal total);

        string Description { get; }
    }

    public class NoDiscountStrategy : IDiscountStrategy
    {
        public string Description => "Без скидки";

        public decimal ApplyDiscount(decimal total)
        {
            return total;
        }
    }

    public class SalesCardDiscountStrategy(SalesCard salesCard) : IDiscountStrategy
    {
        private readonly SalesCard SalesCard = salesCard;

        public string Description => $"Скидочная карта #{SalesCard.Number} ({SalesCard.SalesAmount}%)";

        public decimal ApplyDiscount(decimal total)
        {
            decimal discount = total * SalesCard.SalesAmount / 100m;
            return total - discount;
        }
    }

    public class CouponAmountDiscountStrategy(AmountCoupon coupon) : IDiscountStrategy
    {
        private readonly AmountCoupon Coupon = coupon;

        public string Description => $"Купон #{Coupon.Name} (-{Coupon.DiscountAmount} руб.)";

        public decimal ApplyDiscount(decimal total)
        {
            return Math.Max(0m, total - Coupon.DiscountAmount);
        }
    }

    public class CouponPercentDiscountStrategy(PercentCoupon coupon) : IDiscountStrategy
    {
        private readonly PercentCoupon Coupon = coupon;

        public string Description => $"Купон #{Coupon.Name} (-{Coupon.DiscountPercent}%)";

        public decimal ApplyDiscount(decimal total)
        {
            decimal discount = total * Coupon.DiscountPercent / 100m;
            return total - discount;
        }
    }
}