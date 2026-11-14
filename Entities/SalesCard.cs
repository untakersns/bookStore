using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace bookStore.Entities
{
    public class SalesCard
    {
        public string Number { get; set; }
        public int SalesAmount { get; set; }

        public SalesCard(string number, int salesAmount)
        {
            Number = number;

            if(salesAmount > 100)
            {
                salesAmount = 99;
            }

            SalesAmount = salesAmount <= 0 ? 1 : salesAmount;
        }

        public override string ToString()
        {
            return $"{Number}: {SalesAmount}%";
        }
    }
}
