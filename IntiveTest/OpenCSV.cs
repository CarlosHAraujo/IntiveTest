using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;

namespace IntiveTest
{
    public class OpenCSV
    {
        private string _file;

        public OpenCSV(string file)
        {
            _file = file;
        }

        public IEnumerable<Transaction> GetTransactions()
        {
            using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(_file))
            {
                using (var reader = new StreamReader(stream))
                {
                    while(!reader.EndOfStream)
                    {
                        var line = reader.ReadLine();
                        var values = line.Split(',');
                        yield return new Transaction()
                        {
                            TradeDate = DateTime.Parse(values[0]),
                            BaseCurrency = values[1],
                            CounterCurrency = values[2],
                            Amount = decimal.Parse(values[3], new CultureInfo("en-US")),
                            ValueDate = DateTime.Parse(values[4])
                        };
                    }
                }
            }
        }
    }
}
