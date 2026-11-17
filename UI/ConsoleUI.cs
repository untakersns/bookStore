using bookStore.Entities;
using bookStore.Logic;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace bookStore.UI
{
    internal class ConsoleUI(FileCouponStore fileCouponStore, List<Book> books, List<SalesCard> salesCards, WishList wishList)
    {
        private readonly FileCouponStore FileCouponStore = fileCouponStore;
        private readonly List<Book> Books = books;
        private WishList WWishList = wishList;
        private Delivery delivery = new(string.Empty, string.Empty, string.Empty);
        private Order order = new(new WishList(), new Delivery(string.Empty, string.Empty, string.Empty), new NoDiscountStrategy());
        private Order_Formatting order_Formatting = new(new Order(new WishList(), new Delivery(string.Empty, string.Empty, string.Empty), new NoDiscountStrategy()));

        public void Run()
        {
            while (true)
            {
                Console.WriteLine("Введите желаемую функцию:\n" +
                    "1. Добавить книгу в корзину.\n" +
                    "2. Удалить книгу из корзины.\n" +
                    "3. Вывести корзину\n" +
                    "4. Очистить корзину.\n" +
                    "5. Создать заказ.\n" +
                    "0. Выйти из цикла.\n");

                switch (int.Parse(Console.ReadLine()!))
                {
                    case 0:
                        Environment.Exit(0);
                        break;

                    case 1:
                        AddBookInWishList();
                        break;

                    case 2:
                        RemoveBookFromList();
                        break;

                    case 3:
                        Console.WriteLine(string.Join('\n', WWishList.wishList));
                        break;

                    case 4:
                        if (WWishList.wishList.Count > 0)
                        {
                            WWishList.wishList = WWishList.Clear();
                            Console.WriteLine("Корзина очищена.\n");
                        }
                        else
                        {
                            Console.WriteLine("Корзина пуста.\n");
                        }
                        break;

                    case 5:
                        Order orderInput = CreateOrder();
                        Console.WriteLine("Вы уверены что хотите создать заказ? (Да/Нет)");
                        if(Console.ReadLine() == "Да")
                        {
                            order = orderInput;
                            Console.WriteLine("Заказ успешно создан!\n" + order);
                        }
                        else
                        {
                            Console.WriteLine("Создание заказа отменено.");
                            break;
                        }
                        order_Formatting = new(order);
                        Console.WriteLine(order_Formatting.SchetFaktura());
                        order_Formatting.SchetFakturaLoadInFile();
                        break;

                    default:
                        continue;
                }
            }
        }

        private void AddBookInWishList()
        {
            while (true)
            {
                Console.WriteLine("Введите Id желаемой книги: (введите Exit чтобы выйти из цикла)");
                string num = Console.ReadLine()!;
                if (int.TryParse(num, out int n))
                {
                    if(Books.Contains(Books.FirstOrDefault(b => b.Id == n)))
                    {
                        WWishList.AddBook(Books, n);
                        Console.WriteLine("Книга успешно добавлена!\n");
                    }
                    else
                    {
                        Console.WriteLine("Книга не добавлена! Возможно ее не существует.\n");
                    }
                }
                else if(num == "Exit")
                {
                    break;
                }
                else
                {
                    Console.WriteLine("Введено не-число! Введите заново.\n");
                    continue;
                }
            }
        }

        private void RemoveBookFromList()
        {
            while (true)
            {
                Console.WriteLine("Введите Id желаемой книги: (нажмите Exit чтобы выйти из цикла)");
                string num = Console.ReadLine()!;
                if (int.TryParse(num, out int n))
                {
                    if(WWishList.wishList.Contains(WWishList.wishList.FirstOrDefault(b => b.Id == n)))
                    {
                        WWishList.RemoveBook(n);
                        Console.WriteLine("Книга удалена!\n");
                    }
                    else
                    {
                        Console.WriteLine("Книга не удалена! Возможно ее нет в корзине.\n");
                    }
                }
                else if (num == "Exit")
                {
                    break;
                }
                else
                {
                    Console.WriteLine("Введено не-число! Введите заново.\n");
                    continue;
                }
            }
        }

        private Delivery CreateDelivery()
        {
            Console.WriteLine("Введите адрес доставки:");
            string address = Console.ReadLine() ?? string.Empty;


            Console.WriteLine("Введите номер телефона:");
            string input = Console.ReadLine() ?? string.Empty;
            
            Console.WriteLine("Введите email:");
            string input2 = Console.ReadLine() ?? string.Empty;
            
            return new(address, input, input2);
        }

        private IDiscountStrategy? SelectDiscountStrategy(List<SalesCard> salesCards, List<AmountCoupon> amountCoupons, List<PercentCoupon> percentCoupons)
        {
            Console.WriteLine("\nВыберите тип скидки: ");
            Console.WriteLine("0. Без скидки");
            Console.WriteLine("1. Скидочная карта");
            Console.WriteLine("2. Купон на фиксированную сумму");
            Console.WriteLine("3. Купон на процент");

            if (!int.TryParse(Console.ReadLine(), out int choice))
            {
                return null;
            }

            switch (choice)
            {
                case 0:
                    return new NoDiscountStrategy();

                case 1:
                    Console.WriteLine("Введите номер телефона привязанный к карте:");
                    string cardName = Console.ReadLine()!;

                    if(cardName == null)
                    {
                        return new NoDiscountStrategy();
                    }

                    SalesCard? card = salesCards
                        .FirstOrDefault(c => c.Number == cardName);

                    if (card == null)
                    {
                        Console.WriteLine("Карта не найдена");
                        return new NoDiscountStrategy();
                    }

                    Console.WriteLine("Карта применена");
                    return new SalesCardDiscountStrategy(card);

                case 2:
                    string amountName = Console.ReadLine()!;
                    if (amountName == null)
                    {
                        return new NoDiscountStrategy();
                    }

                    AmountCoupon? amountCoupon = amountCoupons
                        .FirstOrDefault(c => c.Name == amountName);

                    if (amountCoupon == null)
                    {
                        return new NoDiscountStrategy();
                    }

                    return new CouponAmountDiscountStrategy(amountCoupon);

                case 3:
                    string percentName = Console.ReadLine()!;
                    if (percentName == null)
                    {
                        return new NoDiscountStrategy();
                    }

                    PercentCoupon? percentCoupon = percentCoupons
                        .FirstOrDefault(c => c.Name == percentName);

                    if (percentCoupon == null)
                    {
                        return new NoDiscountStrategy();
                    }

                    return new CouponPercentDiscountStrategy(percentCoupon);

                default:
                    return new NoDiscountStrategy();
            }
        }

        private Order CreateOrder()
        {
            delivery = CreateDelivery();
            return new(wishList, delivery, SelectDiscountStrategy(salesCards, FileCouponStore.amountCoupons, FileCouponStore.percentCoupons)!);
        }
    }
}
