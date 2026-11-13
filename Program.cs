using bookStore.Entities;

internal class Program
{
    private static void Main(string[] args)
    {
        WishList wishList = new();
        List<Book> books = [
                new Book(1, "Математика 5 Класс", "Иванова", 200),
                new Book(2, "Русский Язык 7 Класс", "Путин", 300),
                new Book(3, "Война и мир", "Толстой", 500),
                new Book(4, "Отцы и дети", "Тургенев", 200)
            ];
        List<SalesCard> salesCards = [
                new SalesCard(1, 10),
                new SalesCard(2, 150)
            ];
        List<AmountCoupon> amountCoupons = [
                new AmountCoupon(1, 100),
                new AmountCoupon(2, 500000000)
            ];
        List<PercentCoupon> percentCoupons = [
                new PercentCoupon(1, 5),
                new PercentCoupon(2, 150)
            ];
        wishList.wishList = [
                wishList.GetBook(books, 1),
                wishList.GetBook(books, 2),
                wishList.GetBook(books, 3),
                wishList.GetBook(books, 4)
            ];
        Delivery delivery = new("ул. Пушкина", Delivery.TryAddNumber(Console.ReadLine()!), Delivery.TryAddEmail(Console.ReadLine()!));
        Delivery wrongDelivery = new("ул. Пушкина", Delivery.TryAddNumber(Console.ReadLine()!), Delivery.TryAddEmail(Console.ReadLine()!));

        Order order = new(wishList, delivery, new CouponAmountDiscountStrategy(amountCoupons[0]));
        Order order1 = new(wishList, wrongDelivery, new CouponAmountDiscountStrategy(amountCoupons[1]));

        List<Order> orders = [order, order1];
        Console.WriteLine(string.Join('\n', orders));
    }
}