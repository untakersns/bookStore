using System;
using System.Collections.Generic;
using System.Text;

namespace bookStore.Entities
{
    public class Order(WishList wishList, Delivery delivery, IDiscountStrategy discountStrategy)
    {
        public WishList WishList { get; } = wishList;
        public Delivery Delivery { get; } = delivery;

        private readonly IDiscountStrategy DiscountStrategy = discountStrategy;

        //общая стоимость книг в корзине
        private static decimal CalculateCost(List<Book> wishList)
        {
            decimal fullPrice = 0.0m;

            foreach (var book in wishList)
            {
                fullPrice += book.Price;
            }

            return fullPrice;
        }

        //запись результата в отдельную переменную
        public decimal BooksCost => CalculateCost(WishList.wishList!);

        //размер скидки для отображения в счет-фактуре для данного заказа
        public decimal DiscountAmount => BooksCost - DiscountStrategy.ApplyDiscount(BooksCost);

        //тип примененной скидки в счет-фактуре
        public string DiscountDescription => DiscountStrategy.Description;

        //расчет итоговой цены после применения скидки с учетом стоимости доставки
        public decimal Result()
        {
            decimal discountedPrice = DiscountStrategy.ApplyDiscount(BooksCost);

            return discountedPrice + Delivery.DeliveryCost;
        }

        public static decimal CalculateOrder(Order currentOrder)
        {
            if (currentOrder == null)
            {
                return 0.0m;
            }

            return currentOrder.Result();
        }

        public static Order CreateOrder(WishList wishList, 
            List<SalesCard> salesCards, 
            List<AmountCoupon> amountCoupons, 
            List<PercentCoupon> percentCoupons,
            List<Order> orders,
            Order currentOrder)
        {
            // выбор скидки
            IDiscountStrategy? discountStrategy = DiscountSelector.SelectDiscountStrategy(salesCards, amountCoupons, percentCoupons);

            if (discountStrategy == null)
            {
                return new Order(wishList, Delivery.AddDelivery(), new NoDiscountStrategy());
            }

            if (wishList.wishList.Count == 0)
            {
                wishList.wishList.Add(new Book(0, "", "", 0.0m));
                return new Order(wishList, Delivery.AddDelivery(), discountStrategy);
            }
            // создание заказа вместе с доставкой
            Order order = new(wishList, Delivery.AddDelivery(), discountStrategy);

            orders.Add(order);
            currentOrder = order;
            wishList = new WishList();
            return order;
        }

        public override string ToString()
        {
            return $"{string.Join('\n', WishList.wishList)} \n{Delivery} \n{Result()} \n";
        }
    }
}
