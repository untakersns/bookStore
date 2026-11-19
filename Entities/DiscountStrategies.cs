using System;
using System.Collections.Generic;
using System.Linq;

namespace bookStore.Entities
{
    public class CompositeDiscount : IDiscount
    {
        private readonly List<IDiscount> strategies;

        public string Description => strategies.Count == 0
            ? "Без скидки"
            : string.Join(" + ", strategies.Select(s => s.Description));

        public CompositeDiscount(IEnumerable<IDiscount> strategies)
        {
            this.strategies = strategies?.ToList() ?? [];
        }

        public decimal ApplyDiscount(
            decimal total,
            decimal? salesCardPercent,
            decimal? couponPercent,
            List<decimal> amountDiscounts)
        {
            foreach (IDiscount strategy in strategies)
            {
                total = strategy.ApplyDiscount(
                    total, salesCardPercent, couponPercent, amountDiscounts);
            }

            return total;
        }
    }

    internal class DiscountBuilder
    {
        private SalesCard? salesCard;
        private PercentCoupon? percentCoupon;
        private readonly List<AmountCoupon> amountCoupons = [];

        public SalesCard? SetSalesCard(SalesCard card)
        {
            if (card == null)
                return null;

            SalesCard? previous = salesCard;
            salesCard = card;
            return previous;
        }

        public bool AddCoupon(Coupon coupon, out Coupon? replacedCoupon)
        {
            replacedCoupon = null;

            switch (coupon)
            {
                case PercentCoupon percent:
                    if (percentCoupon != null &&
                        string.Equals(percentCoupon.Name, percent.Name,
                            StringComparison.OrdinalIgnoreCase))
                        return false;

                    replacedCoupon = percentCoupon;
                    percentCoupon = percent;
                    return true;

                case AmountCoupon amount:
                    if (amountCoupons.Any(existing =>
                        string.Equals(existing.Name, amount.Name,
                            StringComparison.OrdinalIgnoreCase)))
                        return false;

                    amountCoupons.Add(amount);
                    return true;

                default:
                    return false;
            }
        }

        public IDiscount Build()
        {
            List<IDiscount> strategies = [];

            if (salesCard != null)
                strategies.Add(new SalesCardDiscount());

            if (percentCoupon != null)
                strategies.Add(new CouponPercentDiscount());

            if (amountCoupons.Count > 0)
                strategies.Add(new CouponAmountDiscount());

            return new CompositeDiscount(strategies);
        }

        public (decimal? SalesCardPercent, decimal? CouponPercent, List<decimal> AmountDiscounts)
            GetDiscountValues()
        {
            return (
                salesCard?.SalesAmount,
                percentCoupon?.DiscountPercent,
                amountCoupons.Select(c => c.DiscountAmount).ToList()
            );
        }

        public List<string> GetUsedCouponNames()
        {
            List<string> names = [];

            if (percentCoupon != null)
                names.Add(percentCoupon.Name);

            names.AddRange(amountCoupons.Select(c => c.Name));

            return names;
        }

        public string GetDiscountDescription()
        {
            List<string> descriptions = [];

            if (salesCard != null)
                descriptions.Add(
                    $"Скидочная карта #{salesCard.Number} (-{salesCard.SalesAmount}%)");

            if (percentCoupon != null)
                descriptions.Add(
                    $"Купон #{percentCoupon.Name} (-{percentCoupon.DiscountPercent}%)");

            descriptions.AddRange(amountCoupons.Select(c =>
                $"Купон #{c.Name} (-{c.DiscountAmount} руб.)"));

            return descriptions.Count == 0
                ? "Без скидки"
                : string.Join(" + ", descriptions);
        }

        public void Clear()
        {
            salesCard = null;
            percentCoupon = null;
            amountCoupons.Clear();
        }
    }

    public interface IDiscount
    {
        decimal ApplyDiscount(
            decimal total,
            decimal? salesCardPercent,
            decimal? couponPercent,
            List<decimal> amountDiscounts);

        string Description { get; }
    }

    public class SalesCardDiscount : IDiscount
    {
        public string Description => "Скидка по карте";

        public decimal ApplyDiscount(
            decimal total, decimal? salesCardPercent,
            decimal? couponPercent, List<decimal> amountDiscounts)
        {
            if (salesCardPercent == null)
                return total;

            decimal discount = total * salesCardPercent.Value / 100m;
            return Math.Max(0m, total - discount);
        }
    }

    public class CouponPercentDiscount : IDiscount
    {
        public string Description => "Процентный купон";

        public decimal ApplyDiscount(
            decimal total, decimal? salesCardPercent,
            decimal? couponPercent, List<decimal> amountDiscounts)
        {
            if (couponPercent is null)
                return total;

            decimal discount = total * couponPercent.Value / 100m;
            return Math.Max(0m, total - discount);
        }
    }

    public class CouponAmountDiscount : IDiscount
    {
        public string Description => "Купоны на фиксированную сумму";

        public decimal ApplyDiscount(
            decimal total, decimal? salesCardPercent,
            decimal? couponPercent, List<decimal> amountDiscounts)
        {
            foreach (decimal amount in amountDiscounts)
                total = Math.Max(0m, total - amount);

            return total;
        }
    }
}
