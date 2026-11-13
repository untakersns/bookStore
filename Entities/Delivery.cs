using System.Text.RegularExpressions;

namespace bookStore.Entities
{
    public class Delivery(string address, string phoneNumber, string email)
    {
        private string Address { get; set; } = address;
        private string PhoneNumber { get; set; } = phoneNumber;
        private string Email { get; set; } = email;
        public decimal DeliveryCost { get; private set; } = 500.0m;

        public static string TryAddNumber(string input)
        {
            if (Regex.IsMatch(input, @"^\+375\(0[\d]{2}\)[\d]{3}-[\d]{2}-[\d]{2}$") ||
                Regex.IsMatch(input, @"^8-\([\d]{3}\)-[\d]{3}-[\d]{2}-[\d]{2}$"))
            {
                return input;
            }
            else
            {
                return string.Empty;
            }
        }

        public static string TryAddEmail(string input)
        {
            if (Regex.IsMatch(input, @"^[^\s@]+@[^\s@]+\.[^\s@]+$"))
            {
                return input;
            }
            else
            {
                return string.Empty;
            }
        }

        public static Delivery AddDelivery()
        {
            Console.WriteLine("Введите адрес доставки: ", Console.ForegroundColor = ConsoleColor.Yellow);
            Console.ResetColor();
            string address = Console.ReadLine()!;

            Console.WriteLine("Введите номер телефона: ", Console.ForegroundColor = ConsoleColor.Yellow);
            Console.ResetColor();
            string number = TryAddNumber(Console.ReadLine()!);

            Console.WriteLine("Введите адрес электронной почты: ", Console.ForegroundColor = ConsoleColor.Yellow);
            Console.ResetColor();
            string email = TryAddEmail(Console.ReadLine()!);

            return new(address, number, email);
        }

        public override string ToString()
        {
            return $"Адрес: {Address}, Номер телефона: {PhoneNumber}, Email: {Email}, Стоимость доставки: {DeliveryCost}";
        }
    }
}