using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace IntiveTest
{
    class Program
    {
        static void Main(string[] args)
        {
            Run(args).Wait();
        }

        static async Task Run(string[] args)
        {
            var csv = new OpenCSV("IntiveTest.transactions-v2.csv");
            var calcRate = new Rates();
            List<KeyValuePair<string, double>> rates = new List<KeyValuePair<string, double>>();
            foreach (var transaction in csv.GetTransactions())
            {
                var currencyRate = await calcRate.GetRate(transaction.BaseCurrency, transaction.CounterCurrency, transaction.ValueDate);
                var amount = (double)transaction.Amount * currencyRate;
                rates.Add(new KeyValuePair<string, double>(transaction.CounterCurrency, amount));
            }

            var totalVolume = from rate in rates
                      group rate by rate.Key into g
                      select new { Key = g.Key, Sum = g.Sum(x => x.Value) };

            Console.WriteLine("Total volume by currency:");
            foreach(var volume in totalVolume)
            {
                Console.WriteLine(String.Format("{0} - {1:0.00}", volume.Key, volume.Sum));
            }
            Console.ReadLine();
        }
    }
}
