using System;
using System.Collections.Generic;
using System.Text;

namespace bookStore.Entities
{
    public class CompositeDiscount : IDiscount
    {
        private readonly List<IDiscount> strategies = [];

        public string Description =>
            strategies.Count == 0
                ? "Без скидки"
                : string.Join(" + ", strategies.Select(s => s.Description));

        public void Add(IDiscount strategy)
        {
            if(strategy == null)
            {
                return;
            }

            strategies.Add(strategy);
        }

        public void Clear()
        {
            strategies.Clear();
        }

        public decimal ApplyDiscount(decimal total)
        {
            foreach (var strategy in strategies)
            {
                total = strategy.ApplyDiscount(total);
            }
            return total;
        }
    }

    internal class DiscountBuilder(CompositeDiscount composite)
    {
        private readonly CompositeDiscount Composite = composite ?? new CompositeDiscount();
        private SalesCard? salesCard;
        private PercentCoupon? percentCoupon;
        private readonly List<AmountCoupon> amountCoupons = [];

        public SalesCard? SetSalesCard(SalesCard card)
        {
            if(card == null)
            {
                return new(string.Empty, 0);
            }

            SalesCard? previous = salesCard;
            salesCard = card;

            return previous;
        }

        public bool SetPercentCoupon(PercentCoupon coupon, out PercentCoupon? replacedCoupon)
        {
            if (coupon == null)
            {
                replacedCoupon = new(string.Empty, 0);
                return false;
            }

            if (percentCoupon != null && 
                string.Equals(
                    percentCoupon.Name,
                    coupon.Name, 
                    StringComparison.OrdinalIgnoreCase))
            {
                replacedCoupon = null;
                return false;
            }

            replacedCoupon = percentCoupon;
            percentCoupon = coupon;

            return true;
        }

        public bool AddAmountCoupon(AmountCoupon coupon)
        {
            if (coupon == null)
            {
                return false;
            }

            if (amountCoupons.Any(c => 
                string.Equals(
                    c.Name,
                    coupon.Name,
                    StringComparison.OrdinalIgnoreCase)))
            {
                return false;
            }

            amountCoupons.Add(coupon);
            return true;
        }

        //формирование скидки
        public CompositeDiscount Build()
        {
            CompositeDiscount composite = new();
            if (salesCard != null)
            {
                composite.Add(new SalesCardDiscount(salesCard));
            }
            if (percentCoupon != null)
            {
                composite.Add(new CouponPercentDiscount(percentCoupon));
            }
            foreach (var amountCoupon in amountCoupons)
            {
                composite.Add(new CouponAmountDiscount(amountCoupon));
            }
            return composite;
        }

        //возвращает купоны которые были использованы в текущей сборке скидок
        public List<string> GetUsedCouponNames()
        {
            List<string> names = [];

            if(percentCoupon != null)
            {
                names.Add(percentCoupon.Name);
            }

            names.AddRange(amountCoupons.Select(c => c.Name));

            return names;
        }

        public void Clear()
        {
            salesCard = null;
            percentCoupon = null;
            amountCoupons.Clear();

            Composite.Clear();
        }
    }

    public interface IDiscount
    {
        decimal ApplyDiscount(decimal total);

        string Description { get; }
    }

    public class NoDiscount : IDiscount
    {
        public string Description => "Без скидки";

        public decimal ApplyDiscount(decimal total)
        {
            return total;
        }
    }

    public class SalesCardDiscount(SalesCard salesCard) : IDiscount
    {
        private readonly SalesCard SalesCard = salesCard;

        public string Description => $"Скидочная карта #{SalesCard.Number} ({SalesCard.SalesAmount}%)";

        public decimal ApplyDiscount(decimal total)
        {
            decimal discount = total * SalesCard.SalesAmount / 100m;
            return Math.Max(0m, total - discount);
        }
    }

    public class CouponAmountDiscount(AmountCoupon coupon) : IDiscount
    {
        private readonly AmountCoupon Coupon = coupon;

        public string Description => $"Купон #{Coupon.Name} (-{Coupon.DiscountAmount} руб.)";

        public decimal ApplyDiscount(decimal total)
        {
            return Math.Max(0m, total - Coupon.DiscountAmount);
        }
    }

    public class CouponPercentDiscount(PercentCoupon coupon) : IDiscount
    {
        private readonly PercentCoupon Coupon = coupon;

        public string Description => $"Купон #{Coupon.Name} (-{Coupon.DiscountPercent}%)";

        public decimal ApplyDiscount(decimal total)
        {
            decimal discount = total * Coupon.DiscountPercent / 100m;
            return Math.Max(0m, total - discount);
        }
    }
}