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
        private Order order = new(new WishList(), new Delivery(string.Empty, string.Empty, string.Empty), new NoDiscount());
        private Order_Formatting order_Formatting = new(new Order(new WishList(), new Delivery(string.Empty, string.Empty, string.Empty), new NoDiscount()));
        private List<string> SelectedCouponNames = [];

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

                        if(string.Equals(
                            Console.ReadLine()?.Trim(),
                            "Да",
                            StringComparison.OrdinalIgnoreCase))
                        {
                            order = orderInput;

                            Console.WriteLine("Заказ успешно создан!\n" + order);

                            if (SelectedCouponNames.Count > 0)
                            {
                                try
                                {
                                    int removedCount = FileCouponStore.RemoveCoupon(SelectedCouponNames);
                                    Console.WriteLine($"Удалено {removedCount} использованных купонов.");
                                }
                                catch
                                {
                                    return;
                                }

                                SelectedCouponNames.Clear();
                            }
                        }
                        else
                        {
                            Console.WriteLine("Создание заказа отменено.");

                            SelectedCouponNames.Clear();
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

        private void SelectDiscount(DiscountBuilder builder,
            List<SalesCard> salesCards,
            List<AmountCoupon> amountCoupons,
            List<PercentCoupon> percentCoupons)
        {
            while (true)
            {
                Console.WriteLine("\nВыберите тип скидки: ");
                Console.WriteLine("0. Без скидки");
                Console.WriteLine("1. Скидочная карта");
                Console.WriteLine("2. Купон на фиксированную сумму");
                Console.WriteLine("3. Купон на процент");
                Console.WriteLine("4. Очистить все скидки");
                Console.WriteLine("5. Закончить выбор скидок");

                if (!int.TryParse(Console.ReadLine(), out int choice))
                {
                    Console.WriteLine("Непрвильный ввод.");
                    continue;
                }

                switch (choice)
                {
                    case 0:
                        return;

                    case 1:
                        Console.WriteLine("Введите номер телефона привязанный к карте:");

                        string? cardNum = Console.ReadLine()?.Trim();

                        SalesCard? card = salesCards.FirstOrDefault(c =>
                            string.Equals(
                                c.Number,
                                cardNum,
                                StringComparison.OrdinalIgnoreCase));

                        if (card == null)
                        {
                            Console.WriteLine("Карта не найдена");
                            break;
                        }

                        SalesCard? previousCard = builder.SetSalesCard(card);

                        if(previousCard == null)
                        {
                            Console.WriteLine("Скидка по карте успешно применена.");

                        }
                        else if (previousCard.Number == card.Number)
                        {
                            Console.WriteLine("Эта карта уже выбрана!");
                        }
                        else
                        {
                            Console.WriteLine($"Карта {previousCard.Number} заменена картой {card.Number}.");
                        }
                        break;

                    case 2:
                        Console.WriteLine("Введите название купона на фиксированную сумму:");

                        string? amountName = Console.ReadLine()?.Trim();

                        AmountCoupon? amountCoupon = amountCoupons.FirstOrDefault(c =>
                            string.Equals(
                                c.Name,
                                amountName,
                                StringComparison.OrdinalIgnoreCase));

                        if (amountCoupon == null)
                        {
                            Console.WriteLine("Купон не найден.");
                            break;
                        }

                        if (builder.AddAmountCoupon(amountCoupon))
                        {
                            Console.WriteLine($"Купон {amountCoupon.Name} добавлен.");
                        }
                        else
                        {
                            Console.WriteLine("Этот купон уже добавлен.");
                        }

                        break;

                    case 3:
                        Console.WriteLine("Введите название процентного купона:");

                        string? percentName = Console.ReadLine()?.Trim();

                        PercentCoupon? percentCoupon = percentCoupons.FirstOrDefault(c =>
                            string.Equals(
                                c.Name,
                                percentName,
                                StringComparison.OrdinalIgnoreCase));

                        if (percentCoupon == null)
                        {
                            Console.WriteLine("Купон не найден.");
                            break;
                        }

                        bool wasAdded = builder.SetPercentCoupon(
                            percentCoupon,
                            out PercentCoupon? replacedCoupon);

                        if (!wasAdded)
                        {
                            Console.WriteLine("Этот процентный купон уже выбран.");
                        }
                        else if (replacedCoupon != null)
                        {
                            Console.WriteLine(
                                $"Процентный купон {replacedCoupon.Name} заменён купоном {percentCoupon.Name}.");
                        }
                        else
                        {
                            Console.WriteLine(
                                $"Процентный купон {percentCoupon.Name} добавлен.");
                        }

                        break;

                    case 4:
                        builder.Clear();
                        Console.WriteLine("Все скидки очищены.");
                        break;

                    case 5:
                        return;

                    default:
                        Console.WriteLine("Такого пункта меню нет.");
                        break;
                }
            }
        }

        private Order CreateOrder()
        {
            delivery = CreateDelivery();
            CompositeDiscount composite = new();
            DiscountBuilder builder = new(composite);

            SelectDiscount(builder,
                salesCards,
                FileCouponStore.amountCoupons,
                FileCouponStore.percentCoupons);

            SelectedCouponNames = builder.GetUsedCouponNames();

            return new Order(
                WWishList,
                delivery,
                builder.Build()
                );
        }
    }
}