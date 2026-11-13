using System;
using System.Collections.Generic;
using System.Text;

namespace bookStore.Entities
{
    public class WishList
    {
        public List<Book> wishList { get; set; } = [];

        //добавить книгу в корзину
        public void AddBook(List<Book> books, int num)
        {
            if (books.Any(book => book.Id == num))
            {
                wishList.Add(books.FirstOrDefault(book => book.Id == num)!);
            }
            else
            {
                return;
            }
        }

        public Book? GetBook(List<Book> books, int num)
        {
            return books.FirstOrDefault(book => book.Id == num)!;
        }

        //удалить книгу из корзины
        public void RemoveBook(int num)
        {
            if(wishList.Any(book => book.Id == num))
            {
                wishList.Remove(wishList.FirstOrDefault(book => book.Id == num)!);
            }
            else
            {
                return;
            }
        }

        //итого в корзине
        public decimal CalculateWishlist()
        {
            decimal fullPrice = 0.0m;

            foreach (var book in wishList)
            {
                fullPrice += book.Price;
            }

            return fullPrice;
        }
    }
}
