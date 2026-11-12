using System.Text.RegularExpressions;

string filePath = "books.txt";
string filePath2 = "salesCards.txt";
string filePath3 = "amountCoupons.txt";
string filePath4 = "percentCoupons.txt";


while (true)
{
    try
    {
        //чтение файла и запись в список книг
        List<Book> books = File.ReadLines(filePath)
            .Where(line => !string.IsNullOrEmpty(line))
            .Select(line =>
            {
                string[] fields = line.Split(';');

                return new Book
                (
                    int.Parse(fields[0].Trim()),
                    fields[1].Trim(),
                    fields[2].Trim(),
                    decimal.Parse(fields[3].Trim(), System.Globalization.CultureInfo.InvariantCulture)
                );
            }).ToList();

        //чтение скидочных карт
        List<SalesCard> salesCards = File.ReadLines(filePath2)
            .Where(line => !string.IsNullOrEmpty(line))
            .Select(line =>
            {
                string[] fields = line.Split(';');

                return new SalesCard
                (
                    int.Parse(fields[0].Trim()),
                    int.Parse(fields[1].Trim())
                );
            }).ToList();

        // список купонов с фиксированной скидкой
        List<AmountCoupon> amountCoupons = File.ReadAllLines(filePath3)
            .Where(line => !string.IsNullOrEmpty(line))
            .Select(line =>
            {
                string[] fields = line.Split(';');

                return new AmountCoupon
                (
                    int.Parse(fields[0].Trim()),
                    int.Parse(fields[1].Trim())
                );
            }).ToList();

        // список купонов с процентной скидкой
        List<PercentCoupon> percentCoupons = File.ReadAllLines(filePath4)
            .Where(line => !string.IsNullOrEmpty(line))
            .Select(line =>
            {
                string[] fields = line.Split(';');

                return new PercentCoupon
                (
                    int.Parse(fields[0].Trim()),
                    int.Parse(fields[1].Trim())
                );
            }).ToList();

        //вывод из списка
        Console.WriteLine("Список имеющихся книг:");
        foreach (var book in books)
        {
            Console.WriteLine(book);
        }

        WishList wishList = new();

        List<Order> orders = []; //хранит все заказы
        Order? currentOrder = null; //последний созданный заказ

        while (true)
        {
            Console.WriteLine("\nВведите желаемую функцию:" +
                "\n\t1. Добавить книги в корзину" +
                "\n\t2. Удалить книги из корзины" +
                "\n\t3. Вывести счет-фактуру" +
                "\n\t4. Создать заказ" +
                "\n\t5. Список заказов", Console.ForegroundColor = ConsoleColor.Yellow);
            Console.ResetColor();

            switch (int.Parse(Console.ReadLine()!))
            {
                case 1:
                    wishList.AddBook(books);
                    break;

                case 2:
                    wishList.RemoveBook(books);
                    break;

                case 3:
                    if (currentOrder == null)
                    {
                        Console.WriteLine("Сначала создайте заказ!");
                        break;
                    }

                    Console.WriteLine("\nСчёт-фактура", Console.ForegroundColor = ConsoleColor.Yellow);
                    Console.ResetColor();
                    Console.WriteLine("\nСписок товаров:");

                    foreach (Book book in currentOrder.WishList.wishList!)
                    {
                        Console.WriteLine(book);
                    }

                    Console.WriteLine("\nСтоимость книг: " +
                        $"{currentOrder.BooksCost} руб.");

                    Console.WriteLine("Применённая скидка: " +
                        currentOrder.DiscountDescription);

                    Console.WriteLine("Размер скидки: " +
                        $"{currentOrder.DiscountAmount} руб.");

                    Console.WriteLine("\nИнформация о доставке:");
                    Console.WriteLine(currentOrder.Delivery);

                    Console.WriteLine("\nИтого к оплате: " +
                        $"{currentOrder.Result()} руб.", Console.ForegroundColor = ConsoleColor.Yellow);
                    Console.ResetColor();
                    break;

                case 4:
                    if (wishList.wishList.Count == 0)
                    {
                        Console.WriteLine("Ваша корзина пуста!");
                        break;
                    }

                    // выбор скидки
                    IDiscountStrategy? discountStrategy = SelectDiscountStrategy(salesCards, amountCoupons, percentCoupons);

                    if (discountStrategy == null)
                    {
                        Console.WriteLine("Создание заказа отменено!");
                        break;
                    }

                    // создание заказа вместе с доставкой
                    Order order = new(wishList, Delivery.AddDelivery(), discountStrategy);

                    orders.Add(order);
                    currentOrder = order;

                    // создаём новую корзину для следующего заказа
                    wishList = new WishList();

                    Console.WriteLine("Заказ успешно создан!");
                    break;

                case 5:
                    if (orders.Count == 0)
                    {
                        Console.WriteLine("Заказов пока нет!");
                        break;
                    }

                    for (int i = 0; i < orders.Count; i++)
                    {
                        Console.WriteLine($"\nЗаказ #{i + 1}");

                        Console.WriteLine();

                        Console.WriteLine(orders[i].Delivery);

                        Console.WriteLine(
                            $"Итого: {orders[i].Result()} руб."
                        );
                    }

                    break;

                default:
                    Console.WriteLine("Неверный параметр ввода!", Console.ForegroundColor = ConsoleColor.Red);
                    break;
            }
        }
    }
    catch (FileNotFoundException)
    {
        Console.WriteLine("Файл не найден.", Console.BackgroundColor = ConsoleColor.Red);
        Console.ResetColor();
        Environment.Exit(0);
    }
    catch (FormatException)
    {
        Console.WriteLine("Неверный формат данных.", Console.BackgroundColor = ConsoleColor.Red);
        Console.ResetColor();
    }
    catch (IndexOutOfRangeException)
    {
        Console.WriteLine("В строке файла не хватает полей.", Console.BackgroundColor = ConsoleColor.Red);
        Console.ResetColor();
        Environment.Exit(0);
    }
}


//метод для возвращения выбранной стратегии скидки для последующего расчета
static IDiscountStrategy? SelectDiscountStrategy(List<SalesCard> salesCards, List<AmountCoupon> amountCoupons, List<PercentCoupon> percentCoupons)
{
    Console.WriteLine("\nВыберите тип скидки:", Console.ForegroundColor = ConsoleColor.Yellow);
    Console.ResetColor();
    Console.WriteLine("0. Без скидки");
    Console.WriteLine("1. Скидочная карта");
    Console.WriteLine("2. Купон на фиксированную сумму");
    Console.WriteLine("3. Купон на процент");

    if (!int.TryParse(Console.ReadLine(), out int choice))
    {
        Console.WriteLine("Неверный формат ввода!", Console.ForegroundColor = ConsoleColor.Red);
        Console.ResetColor();
        return null;
    }

    switch (choice)
    {
        case 0:
            return new NoDiscountStrategy();

        case 1:
            Console.WriteLine("Введите ID скидочной карты:");

            if (!int.TryParse(Console.ReadLine(), out int cardId))
            {
                Console.WriteLine("Неверный ID!");
                return null;
            }

            SalesCard? card = salesCards
                .FirstOrDefault(c => c.Id == cardId);

            if (card == null)
            {
                Console.WriteLine("Скидочная карта не найдена!");
                return null;
            }

            return new SalesCardDiscountStrategy(card);

        case 2:
            Console.WriteLine("Доступные купоны:");

            foreach (AmountCoupon coupon in amountCoupons)
            {
                Console.WriteLine(
                    $"ID: {coupon.Id}, скидка: {coupon.DiscountAmount} руб."
                );
            }

            Console.WriteLine("Введите ID купона:");

            if (!int.TryParse(Console.ReadLine(), out int amountId))
            {
                Console.WriteLine("Неверный ID!");
                return null;
            }

            AmountCoupon? amountCoupon = amountCoupons
                .FirstOrDefault(c => c.Id == amountId);

            if (amountCoupon == null)
            {
                Console.WriteLine("Купон не найден!");
                return null;
            }

            return new CouponAmountDiscountStrategy(amountCoupon);

        case 3:
            Console.WriteLine("Доступные купоны:");

            foreach (PercentCoupon coupon in percentCoupons)
            {
                Console.WriteLine(
                    $"ID: {coupon.Id}, скидка: {coupon.DiscountPercent}%"
                );
            }

            Console.WriteLine("Введите ID купона:");

            if (!int.TryParse(Console.ReadLine(), out int percentId))
            {
                Console.WriteLine("Неверный ID!");
                return null;
            }

            PercentCoupon? percentCoupon = percentCoupons
                .FirstOrDefault(c => c.Id == percentId);

            if (percentCoupon == null)
            {
                Console.WriteLine("Купон не найден!");
                return null;
            }

            return new CouponPercentDiscountStrategy(percentCoupon);

        default:
            Console.WriteLine("Неверный вариант выбора!" , Console.ForegroundColor = ConsoleColor.Red);
            Console.ResetColor();
            return null;
    }
}

public class Book(int id, string name, string author, decimal price)
{
    public int Id { get; set; } = id;
    public string Name { get; set; } = name;
    public string Author { get; set; } = author;
    public decimal Price { get; set; } = price;

    public override string ToString()
    {
        return $"{Id}. Название: {Name} Автор: {Author} Цена: {Price}";
    }
}

public class SalesCard(int id, int salesAmount)
{
    public int Id { get; set; } = id;
    public int SalesAmount { get; set; } = salesAmount;
}

public class WishList
{
    public List<Book>? wishList = [];

    public void AddBook(List<Book> books)
    {
        Console.WriteLine("Сформируйте корзину!");
        while (true)
        {
            Console.WriteLine("Введите id желаемой книги чтобы добавить её в корзину: (для выхода введите 0)", Console.ForegroundColor = ConsoleColor.Yellow);
            Console.ResetColor();
            int? bookId = int.Parse(Console.ReadLine()!);
            if (bookId == null || bookId == 0)
            {
                Console.WriteLine("Ваша корзина на данный момент: ", Console.ForegroundColor = ConsoleColor.Yellow);
                Console.ResetColor();
                foreach (Book book in wishList)
                {
                    Console.WriteLine("\t " + book);
                }
                break;
            }
            else if ((bookId == 0 || bookId == null) && wishList.Count == 0)
            {
                Console.WriteLine("Ваша корзина пуста! Добавьте пожалуйста книги.", Console.ForegroundColor = ConsoleColor.Red);
                Console.ResetColor();
                continue;
            }
            else
            {
                Book? existingBook = books.FirstOrDefault(book => book.Id == bookId);
                if (existingBook != null)
                {
                    wishList.Add(existingBook);
                }
                else
                {
                    Console.WriteLine("Книга не найдена!", Console.BackgroundColor = ConsoleColor.Red);
                    Console.ResetColor();
                }
            }
        }
    }

    public void RemoveBook(List<Book> books)
    {
        while (true)
        {
            Console.WriteLine("Введите id желаемой книги чтобы удалить её из корзины: (для выхода введите 0)", Console.ForegroundColor = ConsoleColor.Yellow);
            Console.ResetColor();
            int? bookId = int.Parse(Console.ReadLine()!);
            if (bookId == null || bookId == 0)
            {
                break;
            }
            else if (wishList.Count == 0)
            {
                Console.WriteLine("Ваша корзина пуста! Добавьте пожалуйста книги.", Console.ForegroundColor = ConsoleColor.Red);
                Console.ResetColor();
                break;
            }
            else
            {
                Book? existingBook = books.FirstOrDefault(book => book.Id == bookId);
                if (existingBook != null)
                {
                    wishList.Remove(existingBook);
                }
                else
                {
                    Console.WriteLine("Книга не найдена!", Console.BackgroundColor = ConsoleColor.Red);
                    Console.ResetColor();
                }
            }
        }
    }
}

public class Delivery(string address, string phoneNumber, string email)
{
    private string Address { get; set; } = address;
    private string PhoneNumber { get; set; } = phoneNumber;
    private string Email { get; set; } = email;
    public decimal DeliveryCost { get; private set; } = 500.0m;

    public static string AddNumber()
    {
        Regex isPhoneNumber = new(@"^\+375\(0[\d]{2}\)[\d]{3}-[\d]{2}-[\d]{2}$");
        Regex isPhoneNumber2 = new(@"^8-\([\d]{3}\)-[\d]{3}-[\d]{2}-[\d]{2}$");

        while (true)
        {
            string input = Console.ReadLine()!;
            if (isPhoneNumber.IsMatch(input) || isPhoneNumber2.IsMatch(input))
            {
                return input;
            }
            else
            {
                Console.WriteLine("Неверный формат ввода! Попробуйте еще раз.", Console.ForegroundColor = ConsoleColor.Red);
                Console.ResetColor();
                continue;
            }
        }
    }

    public static string AddEmail()
    {
        Regex isEmail = new(@"^[^\s@]+@[^\s@]+\.[^\s@]+$");

        while (true)
        {
            string input = Console.ReadLine()!;
            if (isEmail.IsMatch(input))
            {
                return input;
            }
            else
            {
                Console.WriteLine("Неверный формат ввода! Попробуйте еще раз.", Console.ForegroundColor = ConsoleColor.Red);
                Console.ResetColor();
                continue;
            }
        }
    }

    public static Delivery AddDelivery()
    {
        Console.WriteLine("Введите адрес доставки: ", Console.ForegroundColor = ConsoleColor.Yellow);
        Console.ResetColor();
        string address = Console.ReadLine()!;

        Console.WriteLine("Введите номер телефона: ", Console.ForegroundColor = ConsoleColor.Yellow);
        Console.ResetColor();
        string number = Delivery.AddNumber();

        Console.WriteLine("Введите адрес электронной почты: ", Console.ForegroundColor = ConsoleColor.Yellow);
        Console.ResetColor();
        string email = Delivery.AddEmail();

        return new(address, number, email);
    }

    public override string ToString()
    {
        return $"Адрес: {Address}, Номер телефона: {PhoneNumber}, Email: {Email}, Стоимость доставки: {DeliveryCost}";
    }
}

public abstract class Coupon(int id)
{
    public int Id { get; private set; } = id;
}

public class AmountCoupon(int id, decimal discountAmount) : Coupon(id)
{
    public decimal DiscountAmount { get; set; } = discountAmount;
}

public class PercentCoupon(int id, int discountPercent) : Coupon(id)
{
    public int DiscountPercent { get; set; } = discountPercent;
}

public interface IDiscountStrategy
{
    decimal ApplyDiscount(decimal total);

    string Description { get; }
}

public class NoDiscountStrategy : IDiscountStrategy
{
    public string Description => "Без скидки";

    public decimal ApplyDiscount(decimal total)
    {
        return total;
    }
}

public class SalesCardDiscountStrategy(SalesCard salesCard) : IDiscountStrategy
{
    private readonly SalesCard SalesCard = salesCard;

    public string Description => $"Скидочная карта #{SalesCard.Id} ({SalesCard.SalesAmount}%)";

    public decimal ApplyDiscount(decimal total)
    {
        decimal discount = total * SalesCard.SalesAmount / 100m;
        return total - discount;
    }
}

public class CouponAmountDiscountStrategy(AmountCoupon coupon) : IDiscountStrategy
{
    private readonly AmountCoupon Coupon = coupon;

    public string Description => $"Купон #{Coupon.Id} (-{Coupon.DiscountAmount} руб.)";

    public decimal ApplyDiscount(decimal total)
    {
        return Math.Max(0m, total - Coupon.DiscountAmount);
    }
}

public class CouponPercentDiscountStrategy(PercentCoupon coupon) : IDiscountStrategy
{
    private readonly PercentCoupon Coupon = coupon;

    public string Description => $"Купон #{Coupon.Id} (-{Coupon.DiscountPercent}%)";

    public decimal ApplyDiscount(decimal total)
    {
        decimal discount = total * Coupon.DiscountPercent / 100m;
        return total - discount;
    }
}

public class Order(WishList wishList, Delivery delivery, IDiscountStrategy discountStrategy)
{
    public WishList WishList { get; } = wishList;
    public Delivery Delivery { get; } = delivery;

    private readonly IDiscountStrategy DiscountStrategy = discountStrategy;

    //общая стоимость книг в корзине
    private static decimal CalculateCost(List<Book> wishList)
    {
        decimal fullPrice = 0.0m;

        foreach (var book in wishList)
        {
            fullPrice += book.Price;
        }

        return fullPrice;
    }

    //запись результата в отдельную переменную
    public decimal BooksCost => CalculateCost(WishList.wishList!);

    //размер скидки для отображения в счет-фактуре для данного заказа
    public decimal DiscountAmount => BooksCost - DiscountStrategy.ApplyDiscount(BooksCost);

    //тип примененной скидки в счет-фактуре
    public string DiscountDescription => DiscountStrategy.Description;

    //расчет итоговой цены после применения скидки с учетом стоимости доставки
    public decimal Result()
    {
        decimal discountedPrice = DiscountStrategy.ApplyDiscount(BooksCost);

        return discountedPrice + Delivery.DeliveryCost;
    }
}