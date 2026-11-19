using bookStore.Entities;
using bookStore.Logic;
using bookStore.UI;

internal class Program
{
    const string booksPath = @"files\books.txt";
    const string salesCardsPath = @"files\salesCards.txt";
    const string couponPath = @"files\coupons.txt";

    private static void Main(string[] args)
    {
        FileLoadCatalog fileLoadCatalog = new(booksPath);
        FileCardStore fileCardStore = new(salesCardsPath);
        FileCouponStore fileCouponStore = new(couponPath);

        List<Book> books = fileLoadCatalog.LoadAll();
        fileCardStore.FillCards();
        fileCouponStore.FillCoupons();
        WishList wishList = new();

        ConsoleUI consoleUI = new(
                fileCouponStore,
                fileCardStore,
                books,
                wishList);

        consoleUI.Run();
    }
}