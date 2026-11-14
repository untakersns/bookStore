using bookStore;
using bookStore.Entities;

internal class Program
{
    private static void Main(string[] args)
    {
        WishList wishList = new();
        FileLoadCatalog fileLoadCatalog = new("books.txt");
        FileCardStore fileCardCatalog = new("salesCards.txt");
        FileCouponStore fileCouponStore = new("coupons.txt");

        Console.WriteLine("Книги");
        Console.WriteLine(string.Join('\n', fileLoadCatalog.LoadAll()));
        Console.WriteLine("Карты");
        fileCardCatalog.FillCards();
        Console.WriteLine(string.Join('\n', fileCardCatalog.TryGetCards("8-(029)-124-21-64", out IDiscountStrategy strategy)));
        Console.WriteLine("Купоны");
        fileCouponStore.FillCoupons();
        Console.WriteLine(string.Join('\n', fileCouponStore.TryFindCoupons("", out IDiscountStrategy strategy1)));
        Console.WriteLine(strategy1.ToString());
        fileCouponStore.RemoveCoupon("Coup1");
        Console.WriteLine(string.Join('\n', fileCouponStore.TryFindCoupons("Coup1", out IDiscountStrategy strategy11)));
        Console.WriteLine(string.Join('\n', fileCouponStore.TryFindCoupons("Coup2", out IDiscountStrategy strategy2)));
        Console.WriteLine(strategy2.ToString());
        fileCouponStore.RemoveCoupon("Coup2");
        Console.WriteLine(string.Join('\n', fileCouponStore.TryFindCoupons("Coup2", out IDiscountStrategy strategy22)));
        Console.WriteLine(string.Join('\n', fileCouponStore.amountCoupons));
        Console.WriteLine(string.Join('\n', fileCouponStore.percentCoupons));
    }
}