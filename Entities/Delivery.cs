using System.Text.RegularExpressions;

namespace bookStore.Entities
{
    public abstract class Delivery(string address, string phoneNumber, string email)
    {
        private string Address { get; set; } = address;
        public string PhoneNumber { get; } = phoneNumber;
        private string Email { get; set; } = email;
        public decimal DeliveryCost => CalculateDeliveryCost();
        public abstract decimal CalculateDeliveryCost();
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
        
        public override string ToString()
        {
            return $"Адрес: {Address}, Номер телефона: {PhoneNumber}, Email: {Email}, Стоимость доставки: {DeliveryCost}";
        }
    }

    public class PickupDelivery(decimal cost, string address, string phoneNumber, string email)
        : Delivery(address, phoneNumber, email)
    {
        public override decimal CalculateDeliveryCost() => cost;
    }
    public class CourierDelivery(decimal cost, string address, string phoneNumber, string email)
        : Delivery(address, phoneNumber, email)
    {
        public static bool IsMinsk(string address) => address.Contains("Минск", StringComparison.OrdinalIgnoreCase);
        public override decimal CalculateDeliveryCost() => cost;
    }
    public class PostDelivery(string address, string phoneNumber, string email, int booksCount)
        : Delivery(address, phoneNumber, email)
    {
        public override decimal CalculateDeliveryCost() => 5m + 2m * booksCount;
    }
}