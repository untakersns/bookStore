using System;
using System.Collections.Generic;
using System.Text;

namespace bookStore.Entities
{
    public class SalesCard
    {
        public int Id { get; set; }
        public int SalesAmount { get; set; }

        public SalesCard(int id, int salesAmount)
        {
            Id = id;

            if(salesAmount > 100)
            {
                salesAmount = 99;
            }

            SalesAmount = salesAmount <= 0 ? 1 : salesAmount;
        }
    }
}
