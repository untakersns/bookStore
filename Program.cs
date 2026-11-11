using System.Net;
using System.Text.RegularExpressions;

string filePath = "books.txt";

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

        //вывод из списка
        Console.WriteLine("Список имеющихся книг:");
        foreach (var book in books)
        {
            Console.WriteLine(book);
        }

        WishList wishList = new();

        List<Delivery> deliveryList = [];

        while (true)
        {
            Console.WriteLine("\nВведите желаемую функцию:" +
                "\n\t1. Добавить книги в корзину" +
                "\n\t2. Удалить книги из корзины" +
                "\n\t3. Вывести счет-фактуру" +
                "\n\t4. Добавить доставку" +
                "\n\t5. Список доставок", Console.ForegroundColor = ConsoleColor.Yellow);
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
                    wishList.CalculateResult();
                    break;

                case 4:
                    if(wishList.wishList.Count == 0)
                    {
                        Console.WriteLine("Ваша корзина пуста!", Console.ForegroundColor = ConsoleColor.Red);
                        Console.ResetColor();
                        break;
                    }
                    
                    Console.WriteLine("Введите адрес доставки: ", Console.ForegroundColor = ConsoleColor.Yellow);
                    Console.ResetColor();
                    string address = Console.ReadLine()!;

                    Console.WriteLine("Введите номер телефона: ", Console.ForegroundColor = ConsoleColor.Yellow);
                    Console.ResetColor();
                    string number = Delivery.AddNumber();

                    Console.WriteLine("Введите адрес электронной почты: ", Console.ForegroundColor = ConsoleColor.Yellow);
                    Console.ResetColor();
                    string email = Delivery.AddEmail();

                    deliveryList.Add(new Delivery(address, number, email));

                    Console.WriteLine("Запись успешно добавлена!");
                    break;

                case 5:
                    foreach (Delivery delivery in deliveryList)
                    {
                        Console.WriteLine(delivery); 
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

decimal CalculateCost(List<Book> wishList)
{
    decimal fullPrice = 0.0m;
    foreach (var book in wishList)
    {
        fullPrice += book.Price;
    }
    return fullPrice;
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

    private List<SalesCard>? salesCards = [];

    private readonly string filePath2 = "salesCards.txt";
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
                foreach(Book book in wishList)
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

    public void CalculateResult()
    {
        if(wishList.Count == 0)
        {
            Console.WriteLine("Ваша корзина пуста! Добавьте книги в корзину.", Console.ForegroundColor = ConsoleColor.Red);
            Console.ResetColor();
            return;
        }

        
        Console.WriteLine("Счет фактура", Console.ForegroundColor = ConsoleColor.Yellow);
        Console.ResetColor();
        Console.WriteLine("Имеется ли у вас скидочная карта? (0 - нет; 1 - да)");
        switch (int.Parse(Console.ReadLine()!))
        {
            case 0:
                //вывод счет-фактуры без скидок
                Console.WriteLine("Стоимость книг:");
                foreach (var book in wishList)
                {
                    Console.WriteLine(book);
                }
                Console.Write("Итого: ");
                decimal fullPrice = 0.0m;
                foreach (var book in wishList)
                {
                    fullPrice += book.Price;
                }
                Console.WriteLine($"{fullPrice} руб.");
                break;

            //case 1:
            //    //чтение и ввод в список скидочных карт
            //    salesCards = File.ReadLines(filePath2)
            //        .Where(line => !string.IsNullOrEmpty(line))
            //        .Select(line =>
            //        {
            //            string[] fields = line.Split(';');

            //            return new SalesCard
            //            (
            //                int.Parse(fields[0].Trim()),
            //                int.Parse(fields[1].Trim())
            //            );
            //        }).ToList();

            //    Console.WriteLine("Введите id желаемой скидочной карты: (введите 0 при отсутствии карты)", Console.ForegroundColor = ConsoleColor.Yellow);
            //    Console.ResetColor();
            //    int cardId = int.Parse(Console.ReadLine()!);
            //    if (cardId == 0)
            //    {
            //        break;
            //    }
            //    SalesCard? existingCard = salesCards.FirstOrDefault(card => card.Id == cardId);
            //    if (existingCard == null)
            //    {
            //        Console.WriteLine("Скидочная карта не найдена!", Console.BackgroundColor = ConsoleColor.Red);
            //        Console.ResetColor();
            //    }
            //    else
            //    {
            //        //счет-фактура со скидкой
            //        Console.WriteLine("Счет фактура с учетом скидочной карты", Console.ForegroundColor = ConsoleColor.Yellow);
            //        Console.ResetColor();
            //        Console.WriteLine("Стоимость книг:");
            //        foreach (var book in wishList)
            //        {
            //            Console.WriteLine(book);
            //        }
            //        Console.Write("Итого: ");
            //        decimal fullPriceWithSale = 0.0m;
            //        foreach (var book in wishList)
            //        {
            //            fullPriceWithSale += book.Price;
            //        }
            //        Console.WriteLine($"{fullPriceWithSale - (fullPriceWithSale * existingCard.SalesAmount / 100.0m)} руб. (Скидка {existingCard.SalesAmount}%)");
            //        break;
            //    }
            //    break;

            default:
                Console.WriteLine("Неверный параметр ввода!", Console.BackgroundColor = ConsoleColor.Red);
                Console.ResetColor();
                break;
        }
    }
}

public class Delivery(string address, string phoneNumber, string email)
{
    public string Address { get; set; } = address;
    public string PhoneNumber { get; set; } = phoneNumber;
    public string Email { get; set; } = email;
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

    public override string ToString()
    {
        return $"Адрес: {Address}, Номер телефона: {PhoneNumber}, Email: {Email}, Стоимость доставки: {DeliveryCost}";
    }
}