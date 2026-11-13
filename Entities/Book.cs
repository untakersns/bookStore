using System;
using System.Collections.Generic;
using System.Text;

namespace bookStore.Entities
{
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
}
