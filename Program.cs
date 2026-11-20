using bookStore.Entities;
using bookStore.Logic;
using bookStore.UI;

internal class Program
{
    const string booksPath = @"files\books.txt";
    const string salesCardsPath = @"files\salesCards.txt";
    const string couponPath = @"files\coupons.txt";
    const decimal pickupCost = 0m;
    const decimal courierCost = 10m;
    const string pickupPoint = "ул. Пушкина, д. Колотушкина, 13";
    private static void Main(string[] args)
    {
        FileLoadCatalog fileLoadCatalog = new(booksPath);
        FileCardStore fileCardStore = new(salesCardsPath);
        FileCouponStore fileCouponStore = new(couponPath);

        List<Book> books = fileLoadCatalog.LoadAll();
        WishList wishList = new();
        fileCardStore.FillCards();
        fileCouponStore.FillCoupons();
        
        ConsoleUI consoleUI = new(
                fileCouponStore,
                fileCardStore,
                books,
                wishList,
                pickupCost,
                pickupPoint,
                courierCost);

        consoleUI.Run();
    }
}