using bookStore.Entities;
using bookStore.Logic;

internal class Program
{
    private static void Main(string[] args)
    {
        WishList wishList = new();
        FileLoadCatalog fileLoadCatalog = new("books.txt");
        FileCardStore fileCardCatalog = new("salesCards.txt");
        FileCouponStore fileCouponStore = new("coupons.txt");

        fileCardCatalog.FillCards();
        fileCouponStore.FillCoupons();

        List<Book> books = fileLoadCatalog.LoadAll();
        wishList.wishList = [
                books[0],
                books[1],
                books[2],
                books[3]
            ];
        Delivery delivery = new("ул. Пушкина", "8-(029)-124-21-64", "kkk@gmail.com");

        fileCardCatalog.TryGetCards("8-(029)-124-21-64", out IDiscountStrategy discountStrategy);
        Order order = new(wishList, delivery, discountStrategy);
        Order_Formatting order_Formatting = new(order);
        Console.WriteLine(order_Formatting.SchetFaktura()!);
        order_Formatting.SchetFakturaLoadInFile();
    }
}