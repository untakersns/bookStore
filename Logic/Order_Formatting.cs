using bookStore.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace bookStore.Logic
{
    internal class Order_Formatting(Order order)
    {
        private Order Order { get; set; } = order;

        public string SchetFaktura()
        {
            bool isHavingDuplicates = Order.WishList.wishList.Count() != Order.WishList.wishList.Distinct().Count();
            List<string> books = [];

            foreach (Book book in Order.WishList.wishList.Distinct())
            {
                if (isHavingDuplicates)
                {
                    books.Add($"{book.Name} | {Order.WishList.wishList.Count(b => b.Id == book.Id)} шт | {book.Price * Order.WishList.wishList.Count(b => b.Id == book.Id)} руб");
                }
                else
                {
                    books.Add($"{book.Name} | {book.Price} руб");
                }
            }
            return "Информация о доставке: \n\n" +
                $"{string.Join('\n', books)}\n" +
                $"Итого: {Order.WishList.CalculateWishlist().ToString()}\n" +
                $"Скидка {Order.DiscountDescription} {Order.DiscountAmount}\n" + 
                $"Итого к оплате {Order.Result().ToString()}";
        }

        public bool SchetFakturaLoadInFile(string path = "SchetFaktura.txt")
        {
            if (File.Exists(path))
            {
                File.WriteAllText(path, SchetFaktura());
                return true;
            }
            return false;
        }
    }
}
