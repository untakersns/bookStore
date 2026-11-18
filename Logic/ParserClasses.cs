using bookStore.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace bookStore.Logic
{
    internal class FileLoadCatalog(string filePath)
    {
        private string FilePath { get; set; } = filePath;

        public List<Book> LoadAll()
        {
            List<Book> books = [];
            //чтение файла и запись в список книг

            if (!File.Exists(FilePath))
            {
                return books;
            }

            var lines = File.ReadAllLines(FilePath)
                .Where(line => !string.IsNullOrWhiteSpace(line));


            foreach (var line in lines)
            {
                string[] fields = line.Split(';');

                if (fields.Length < 4) continue;

                if (int.TryParse(fields[0].Trim(), out int id) && fields[1].Trim() != "" && fields[2].Trim() != "" && decimal.TryParse(fields[3], out decimal cost))
                {
                    books.Add(new Book(
                        id,
                        fields[1].Trim(),
                        fields[2].Trim(),
                        cost
                        ));
                }
                else continue;
            }

            return books;
        }
    }

    internal class FileCardStore(string filePath)
    {
        private string FilePath { get; set; } = filePath;
        public List<SalesCard> salesCards { get; set; } = [];

        public void FillCards()
        {
            var lines = File.ReadAllLines(FilePath)
                .Where(line => !string.IsNullOrWhiteSpace(line));

            foreach (var line in lines)
            {
                string[] fields = line.Split(';');

                if (fields.Length < 2) continue;

                if ((Regex.IsMatch(fields[0].Trim(), @"^\+375\(0[\d]{2}\)[\d]{3}-[\d]{2}-[\d]{2}$") ||
                    Regex.IsMatch(fields[0].Trim(), @"^8-\([\d]{3}\)-[\d]{3}-[\d]{2}-[\d]{2}$")) &&
                    int.TryParse(fields[1].Trim(), out int amountPercent))
                {
                    salesCards.Add(new SalesCard
                        (
                            fields[0].Trim(),
                            amountPercent
                        ));
                }
                else continue;
            }
        }

        public bool TryGetCards(string cardNum, out IDiscount discountStrategy)
        {
            if (salesCards.Count < 1 || !salesCards.Any(card => card.Number == cardNum))
            {
                discountStrategy = new NoDiscount();
                return false;
            }

            discountStrategy = new SalesCardDiscount(salesCards.FirstOrDefault(card => card.Number == cardNum));
            return true;
        }
    }

    internal class FileCouponStore(string filePath)
    {
        private string FilePath { get; set; } = filePath;
        public List<AmountCoupon> amountCoupons { get; set; } = [];
        public List<PercentCoupon> percentCoupons { get; set; } = [];

        public void FillCoupons()
        {
            var lines = File.ReadAllLines(FilePath)
                .Where(line => !string.IsNullOrWhiteSpace(line));

            foreach (var line in lines)
            {
                string[] fields = line.Split(';');

                if (fields.Length < 2) continue;

                if (fields[1].Trim().Contains('%'))
                {
                    percentCoupons.Add(new PercentCoupon(
                        fields[0].Trim(),
                        int.Parse(fields[1].Trim().Replace("%", ""))
                        ));
                }
                else if (int.TryParse(fields[1].Trim(), out int amount))
                {
                    amountCoupons.Add(new AmountCoupon(
                        fields[0].Trim(),
                        amount
                        ));
                }
                else continue;
            }
        }

        public bool TryFindCoupons(string name, out IDiscount discountStrategy)
        {
            if (percentCoupons.Any(coupon => coupon.Name == name))
            {
                discountStrategy = new CouponPercentDiscount(percentCoupons.FirstOrDefault(coupon => coupon.Name == name));
                return true;
            }
            if(amountCoupons.Any(coupon => coupon.Name == name))
            {
                discountStrategy = new CouponAmountDiscount(amountCoupons.FirstOrDefault(coupon => coupon.Name == name));
                return true;
            }
            
            discountStrategy = new NoDiscount();
            return false;
        }

        public int RemoveCoupon(IEnumerable<string> names)
        {
            HashSet<string> namesToRemove = new(
                names.Where(name => !string.IsNullOrWhiteSpace(name)),
                StringComparer.OrdinalIgnoreCase
                );

            if(namesToRemove.Count == 0)
            {
                return 0;
            }

            if (!File.Exists(FilePath))
            {
                return 0;
            }

            string[] lines = File.ReadAllLines(FilePath);

            //Оставляет все строки кроме использованных купонов
            string[] remainingLines = lines
                .Where(line =>
                {
                    int separatorIndex = line.IndexOf(';');

                    string couponName = (
                        separatorIndex >= 0 ? line[..separatorIndex] : line
                    ).Trim();

                    return !namesToRemove.Contains(couponName);
                }).ToArray();

            int removedCount = lines.Length - remainingLines.Length;

            if (removedCount == 0) return 0;

            //создание временного файла
            string tempPath = FilePath + ".tmp";

            try
            {
                File.WriteAllLines(tempPath, remainingLines);

                File.Move(tempPath, FilePath, overwrite: true);
            }
            catch
            {
                if (File.Exists(tempPath))
                {
                    File.Delete(tempPath);
                }
                return 0;
            }

            amountCoupons.RemoveAll(c => namesToRemove.Contains(c.Name));
            percentCoupons.RemoveAll(c => namesToRemove.Contains(c.Name));

            return removedCount;
        }
    }
}