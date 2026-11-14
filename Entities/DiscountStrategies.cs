using System;
using System.Collections.Generic;
using System.Text;

namespace bookStore.Entities
{
    //public class DiscountSelector
    //{
    //    //метод для возвращения выбранной стратегии скидки для последующего расчета
    //    public static IDiscountStrategy? SelectDiscountStrategy(List<SalesCard> salesCards, List<AmountCoupon> amountCoupons, List<PercentCoupon> percentCoupons)
    //    {
    //        //Console.WriteLine("\nВыберите тип скидки:", Console.ForegroundColor = ConsoleColor.Yellow);
    //        //Console.ResetColor();
    //        //Console.WriteLine("0. Без скидки");
    //        //Console.WriteLine("1. Скидочная карта");
    //        //Console.WriteLine("2. Купон на фиксированную сумму");
    //        //Console.WriteLine("3. Купон на процент");

    //        if (!int.TryParse(Console.ReadLine(), out int choice))
    //        {
    //            return null;
    //        }

    //        switch (choice)
    //        {
    //            case 0:
    //                return new NoDiscountStrategy();

    //            case 1:
    //                if (!int.TryParse(Console.ReadLine(), out int cardId))
    //                {
    //                    return null;
    //                }

    //                SalesCard? card = salesCards
    //                    .FirstOrDefault(c => c.Id == cardId);

    //                if (card == null)
    //                {
    //                    return null;
    //                }

    //                return new SalesCardDiscountStrategy(card);

    //            case 2:
    //                Console.RE

    //                if (Console.ReadLine() == null)
    //                {
    //                    return null;
    //                }

    //                AmountCoupon? amountCoupon = amountCoupons
    //                    .FirstOrDefault(c => c.Id == amountId);

    //                if (amountCoupon == null)
    //                {
    //                    return null;
    //                }

    //                return new CouponAmountDiscountStrategy(amountCoupon);

    //            case 3:
    //                if (!int.TryParse(Console.ReadLine(), out int percentId))
    //                {
    //                    return null;
    //                }

    //                PercentCoupon? percentCoupon = percentCoupons
    //                    .FirstOrDefault(c => c.Name == percentId);

    //                if (percentCoupon == null)
    //                {
    //                    return null;
    //                }

    //                return new CouponPercentDiscountStrategy(percentCoupon);

    //            default:
    //                return null;
    //        }
    //    }
    //}

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