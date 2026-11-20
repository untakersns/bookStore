using System;
using System.Collections.Generic;
using System.Linq;

namespace bookStore.Entities
{
    public class Order
    {
        public WishList WishList { get; }
        public Delivery Delivery { get; }

        public IDiscount DiscountStrategy { get; }

        private readonly decimal? salesCardPercent;
        private readonly decimal? couponPercent;
        private readonly List<decimal> amountDiscounts;
        private readonly string discountDescription;

        public Order(
            WishList wishList,
            Delivery delivery,
            IDiscount discountStrategy,
            decimal? salesCardPercent,
            decimal? couponPercent,
            IEnumerable<decimal> amountDiscounts,
            string discountDescription)
        {
            WishList = wishList;
            Delivery = delivery;
            DiscountStrategy = discountStrategy;

            this.salesCardPercent = salesCardPercent;
            this.couponPercent = couponPercent;
            this.amountDiscounts = amountDiscounts?.ToList() ?? [];

            this.discountDescription =
                string.IsNullOrWhiteSpace(discountDescription)
                    ? "Без скидки"
                    : discountDescription;
        }

        private static decimal CalculateCost(List<Book> wishList)
        {
            decimal fullPrice = 0m;

            foreach (Book book in wishList)
                fullPrice += book.Price;

            return fullPrice;
        }

        public decimal BooksCost =>
            CalculateCost(WishList.wishList!);

        // Применение всех выбранных скидок.
        private decimal CalculateDiscountedPrice()
        {
            return DiscountStrategy.ApplyDiscount(
                BooksCost,
                salesCardPercent,
                couponPercent,
                amountDiscounts);
        }

        public decimal DiscountAmount =>
            BooksCost - CalculateDiscountedPrice();

        public string DiscountDescription =>
            discountDescription;

        public decimal Result()
        {
            return CalculateDiscountedPrice() + Delivery.CalculateDeliveryCost();
        }

        public static decimal CalculateOrder(Order currentOrder)
        {
            if (currentOrder == null)
                return 0m;

            return currentOrder.Result();
        }

        public override string ToString()
        {
            return $"{string.Join('\n', WishList.wishList)} \n" +
                   $"{Delivery} \n{Result()} \n";
        }
    }
}
