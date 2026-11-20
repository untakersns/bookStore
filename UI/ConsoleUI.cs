
using bookStore.Entities;
using bookStore.Logic;
using System;
using System.Collections.Generic;
using System.Linq;

namespace bookStore.UI
{
    internal class ConsoleUI(
        FileCouponStore fileCouponStore,
        FileCardStore fileCardStore,
        List<Book> books,
        WishList wishList)
    {
        private readonly FileCouponStore FileCouponStore = fileCouponStore;
        private readonly FileCardStore FileCardStore = fileCardStore;
        private readonly List<Book> Books = books;

        private WishList WWishList = wishList;
        private Delivery delivery = new PickupDelivery(string.Empty, string.Empty, string.Empty);

        private List<string> SelectedCouponNames = [];

        public void Run()
        {
            while (true)
            {
                Console.WriteLine(
                    "Введите желаемую функцию:\n" +
                    "1. Добавить книгу в корзину.\n" +
                    "2. Удалить книгу из корзины.\n" +
                    "3. Вывести корзину\n" +
                    "4. Очистить корзину.\n" +
                    "5. Создать заказ.\n" +
                    "0. Выйти из цикла.\n");

                if (!int.TryParse(Console.ReadLine(), out int choice))
                {
                    Console.WriteLine("Неправильный ввод.\n");
                    continue;
                }

                switch (choice)
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

                        Console.WriteLine(
                            "Вы уверены, что хотите создать заказ? (Да/Нет)");

                        if (string.Equals(
                            Console.ReadLine()?.Trim(),
                            "Да",
                            StringComparison.OrdinalIgnoreCase))
                        {
                            Console.WriteLine(
                                "Заказ успешно создан!\n" + orderInput);

                            if (SelectedCouponNames.Count > 0)
                            {
                                int removedCount =
                                    FileCouponStore.RemoveCoupon(SelectedCouponNames);

                                Console.WriteLine($"Удалено {removedCount} использованных купонов.");

                                SelectedCouponNames.Clear();
                            }

                            Order_Formatting orderFormatting = new(orderInput);

                            Console.WriteLine(orderFormatting.SchetFaktura());
                            orderFormatting.SchetFakturaLoadInFile();
                        }
                        else
                        {
                            Console.WriteLine("Создание заказа отменено.");

                            SelectedCouponNames.Clear();
                        }

                        break;

                    default:
                        Console.WriteLine("Такого пункта меню нет.\n");
                        break;
                }
            }
        }

        private void AddBookInWishList()
        {
            while (true)
            {
                Console.WriteLine(
                    "Введите Id желаемой книги: (введите Exit чтобы выйти из цикла)");

                string num = Console.ReadLine() ?? string.Empty;

                if (int.TryParse(num, out int n))
                {
                    if (Books.Any(b => b.Id == n))
                    {
                        WWishList.AddBook(Books, n);
                        Console.WriteLine("Книга успешно добавлена!\n");
                    }
                    else
                    {
                        Console.WriteLine(
                            "Книга не добавлена! Возможно, ее не существует.\n");
                    }
                }
                else if (string.Equals(
                    num,
                    "Exit",
                    StringComparison.OrdinalIgnoreCase))
                {
                    break;
                }
                else
                {
                    Console.WriteLine("Введено не-число! Введите заново.\n");
                }
            }
        }

        private void RemoveBookFromList()
        {
            while (true)
            {
                Console.WriteLine("Введите Id желаемой книги: (нажмите Exit чтобы выйти из цикла)");

                string num = Console.ReadLine() ?? string.Empty;

                if (int.TryParse(num, out int n))
                {
                    if (WWishList.wishList.Any(b => b.Id == n))
                    {
                        WWishList.RemoveBook(n);
                        Console.WriteLine("Книга удалена!\n");
                    }
                    else
                    {
                        Console.WriteLine(
                            "Книга не удалена! Возможно, ее нет в корзине.\n");
                    }
                }
                else if (string.Equals(
                    num,
                    "Exit",
                    StringComparison.OrdinalIgnoreCase))
                {
                    break;
                }
                else
                {
                    Console.WriteLine("Введено не-число! Введите заново.\n");
                }
            }
        }

        private Delivery CreateDelivery()
        {
            Console.WriteLine("Введите адрес доставки:");
            string address = Console.ReadLine() ?? string.Empty;

            Console.WriteLine("Введите номер телефона:");
            string phoneNumber = Delivery.TryAddNumber(Console.ReadLine()!) ?? string.Empty;

            Console.WriteLine("Введите email:");
            string email = Delivery.TryAddEmail(Console.ReadLine()!) ?? string.Empty;

            bool isMinsk = address.Contains("Минск", StringComparison.OrdinalIgnoreCase);

            while (true)
            {
                Console.WriteLine("\nВыберите способ доставки:");
                Console.WriteLine("1. Самовывоз");
                if (isMinsk)
                {
                    Console.WriteLine("2. Курьер по Минску");
                    Console.WriteLine("3. Почта");
                }
                else
                {
                    Console.WriteLine("2. Почта");
                }
                if(!int.TryParse(Console.ReadLine()!, out int choice))
                {
                    Console.WriteLine("Неправильный ввод!");
                    continue;
                }

                switch (choice)
                {
                    case 1:
                        return new PickupDelivery(address, phoneNumber, email);

                    case 2 when isMinsk:
                        return new CourierDelivery(address, phoneNumber, email);
                    case 2 when !isMinsk:
                        return new PostDelivery(address, phoneNumber, email, WWishList.wishList.Count);

                    case 3 when isMinsk:
                        return new PostDelivery(address, phoneNumber, email, WWishList.wishList.Count);

                    default:
                        Console.WriteLine("Такого пункта нет");
                        break;
                }
            }
        }

        private void SelectDiscount(DiscountBuilder builder, DiscountFabric discountFabric, string phoneNumber)
        {
            while (true)
            {
                Console.WriteLine("\nВыберите действие со скидками:");
                Console.WriteLine("0. Закончить выбор скидок");
                Console.WriteLine("1. Добавить скидочную карту");
                Console.WriteLine("2. Добавить купон");
                Console.WriteLine("3. Очистить все скидки");

                if (!int.TryParse(Console.ReadLine(), out int choice))
                {
                    Console.WriteLine("Неправильный ввод.");
                    continue;
                }

                switch (choice)
                {
                    case 0:
                        return;

                    case 1:
                        // Номер телефона уже получен при создании доставки.
                        SalesCard? card =
                            FileCardStore.FindCard(phoneNumber);
                        
                        if (card == null)
                        {
                            Console.WriteLine(
                                "Скидочная карта для указанного номера телефона не найдена.");
                            break;
                        }
                        
                        SalesCard? previousCard = builder.SetSalesCard(card);
                        
                        if (previousCard == null)
                        {
                            Console.WriteLine(
                                "Скидка по карте успешно добавлена.");
                        }
                        else if (previousCard.Number == card.Number)
                        {
                            Console.WriteLine("Эта карта уже выбрана!");
                        }
                        else
                        {
                            Console.WriteLine(
                                $"Карта {previousCard.Number} заменена картой {card.Number}.");
                        }
                        
                        break;


                    case 2:
                         Console.WriteLine("Введите название купона:");

                         string couponName =
                             Console.ReadLine() ?? string.Empty;

                         discountFabric.TryAddCoupon(
                             couponName,
                             out _,
                             out string message);

                         Console.WriteLine(message);

                         break;

                    case 3:
                        builder.Clear();
                        Console.WriteLine("Все скидки очищены.");
                        break;

                    default:
                        Console.WriteLine("Такого пункта меню нет.");
                        break;
                }
            }
        }

        private Order CreateOrder()
        {
            delivery = CreateDelivery();

            DiscountBuilder builder = new();

            DiscountFabric discountFabric = new(
                builder,
                FileCouponStore);

            SelectDiscount(builder, discountFabric, delivery.PhoneNumber);

            var discountValues = builder.GetDiscountValues();

            SelectedCouponNames = discountFabric.GetCouponNames();

            string discountDescription =
                builder.GetDiscountDescription();

            return new Order(
                WWishList,
                delivery,
                builder.Build(),
                discountValues.SalesCardPercent,
                discountValues.CouponPercent,
                discountValues.AmountDiscounts,
                discountDescription
            );
        }
    }
}
