using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.IO;

namespace IntiveTest
{
    public class RateData
    {
        public string Base { get; set; }
        public DateTime Date { get; set; }
        public Dictionary<string, double> Rates { get; set; }
    }

    public class Rates
    {
        private static List<RateData> _instance;
        private static List<RateData> RateData
        {
            get
            {
                if(_instance == null)
                {
                    _instance = new List<RateData>();
                }

                return _instance;
            }
        }

        public async Task<double> GetRate(string currency, string counterCurrency, DateTime date)
        {
            var rate = GetRateFromCache(currency, date);
            if (rate == null)
            {
                rate = await GetRateFromServer(currency, date);
                RateData.Add(rate);
            }
            if(rate != null)
            {
                return rate.Rates.SingleOrDefault(r => r.Key == counterCurrency).Value;
            }
            else
            {
                throw new Exception("Currency not found.");
            }
        }

        private RateData GetRateFromCache(string currency, DateTime date)
        {
            return RateData.Where(r => r.Base == currency && r.Date == date).FirstOrDefault();
        }

        private async Task<RateData> GetRateFromServer(string currency, DateTime date)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://api.fixer.io/");
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                var response = await client.GetAsync(String.Format("{0:yyyy-MM-dd}?base={1}", date, currency));
                if(response.IsSuccessStatusCode)
                {
                    var serializer = new JsonSerializer();
                    using (var stream = await response.Content.ReadAsStreamAsync())
                    {
                        using (var reader = new StreamReader(stream))
                        {
                            using (var jsonReader = new JsonTextReader(reader))
                            {
                                return serializer.Deserialize<RateData>(jsonReader);
                            }
                        }
                    }
                }
                else
                {
                    return null;
                }
            }
        }
    }
}
