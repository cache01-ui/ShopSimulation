using System;
using System.Collections.Generic;
using System.Linq;

namespace ShopSimulation
{
    // Структура для итоговых результатів
    struct ShopResult
    {
        public int Profit;
        public int TopBuyerByItemsCard;
        public int TopBuyerByItemsCount;
        public int TopSpenderCard;
        public int TopSpenderAmount;
    }

    class Product
    {
        public string Name { get; }
        public int Price { get; }
        public int Quantity { get; set; }

        public Product(string name, int price, int quantity)
        {
            Name = name;
            Price = price;
            Quantity = quantity;
        }
    }

    class Customer
    {
        public int CardNumber { get; }
        public int Money { get; private set; }
        public int ItemsBought { get; private set; }
        public int MoneySpent { get; private set; }

        public Customer(int cardNumber, int money)
        {
            CardNumber = cardNumber;
            Money = money;
        }

        public void Pay(int price)
        {
            Money -= price;
            MoneySpent += price;
            ItemsBought++;
        }
    }

    class Shop
    {
        public List<Product> Products { get; } = new List<Product>();
        public int Profit { get; private set; } = 0;

        public Shop()
        {
            Products.Add(new Product("Хліб", 25, 30));
            Products.Add(new Product("Молоко", 40, 25));
            Products.Add(new Product("Сир", 180, 15));
            Products.Add(new Product("Ковбаса", 250, 10));
            Products.Add(new Product("Яблука (кг)", 60, 40));
            Products.Add(new Product("Кава", 350, 12));
            Products.Add(new Product("Шоколад", 80, 35));
            Products.Add(new Product("Сік", 55, 20));
            Products.Add(new Product("Риба", 400, 8));
            Products.Add(new Product("Олія", 120, 18));
        }

        // Товари, які є в наявності і які покупець може дозволити собі
        public List<Product> GetAvailableFor(int money)
        {
            return Products.Where(p => p.Quantity > 0 && p.Price <= money).ToList();
        }

        public bool Sell(Product product, Customer customer)
        {
            if (product.Quantity <= 0 || customer.Money < product.Price)
                return false;

            product.Quantity--;
            customer.Pay(product.Price);
            Profit += product.Price;
            return true;
        }
    }

    class Program
    {
        static Random rnd = new Random();

        static int ReadCustomersCount()
        {
            while (true)
            {
                Console.Write("Введіть кількість покупців: ");
                if (int.TryParse(Console.ReadLine(), out int n) && n > 0)
                    return n;
                Console.WriteLine("Помилка: введіть додатне ціле число.");
            }
        }

        static List<Customer> CreateCustomers(int count)
        {
            var customers = new List<Customer>();
            var usedCards = new HashSet<int>();

            for (int i = 0; i < count; i++)
            {
                int card;
                do
                {
                    card = rnd.Next(1, 1001); // 1..1000
                } while (usedCards.Contains(card) && usedCards.Count < 1000);
                usedCards.Add(card);

                int money = rnd.Next(1000, 10001); // 1000..10000
                customers.Add(new Customer(card, money));
                Console.WriteLine($"Покупець: картка №{card}, грошей: {money}");
            }
            return customers;
        }

        static void RunSimulation(Shop shop, List<Customer> customers)
        {
            // Поки хтось ще може щось купити
            bool someoneBought = true;
            while (someoneBought)
            {
                someoneBought = false;
                foreach (var customer in customers)
                {
                    var available = shop.GetAvailableFor(customer.Money);
                    if (available.Count == 0)
                        continue;

                    var product = available[rnd.Next(available.Count)];
                    if (shop.Sell(product, customer))
                        someoneBought = true;
                }
            }
        }

        static ShopResult CalculateResult(Shop shop, List<Customer> customers)
        {
            var topItems = customers.OrderByDescending(c => c.ItemsBought).First();
            var topSpender = customers.OrderByDescending(c => c.MoneySpent).First();

            return new ShopResult
            {
                Profit = shop.Profit,
                TopBuyerByItemsCard = topItems.CardNumber,
                TopBuyerByItemsCount = topItems.ItemsBought,
                TopSpenderCard = topSpender.CardNumber,
                TopSpenderAmount = topSpender.MoneySpent
            };
        }

        static void PrintResult(ShopResult r)
        {
            Console.WriteLine("\n===== РЕЗУЛЬТАТИ =====");
            Console.WriteLine($"Прибуток магазину: {r.Profit}");
            Console.WriteLine($"Найбільше товарів купив: картка №{r.TopBuyerByItemsCard} ({r.TopBuyerByItemsCount} шт.)");
            Console.WriteLine($"Найбільше грошей витратив: картка №{r.TopSpenderCard} ({r.TopSpenderAmount})");
        }

        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            int count = ReadCustomersCount();
            var shop = new Shop();
            var customers = CreateCustomers(count);

            Console.WriteLine("\nПокупці роблять покупки...");
            RunSimulation(shop, customers);

            PrintResult(CalculateResult(shop, customers));
        }
    }
}
