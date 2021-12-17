using System;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;

namespace EconomyChecker
{
    class Program
    {
        static void Main(string[] args)
        {
            Program program = new Program();
            Console.ReadLine();
        }

        private string USD;
        private string EUR;
        private string GBP;

        private Dictionary<Type, Currency> data = new Dictionary<Type, Currency>();

        public Program()
        {
            while(true)
            {
                getData();
                Console.Clear();

                var USD = this.data[Type.USD];
                var EUR = this.data[Type.EUR];
                var GBP = this.data[Type.GBP];

                if (USD.before == null || EUR.before == null || GBP.before == null) continue;

                var diffUSD = Double.Parse(USD.after) > Double.Parse(USD.before) ? Double.Parse(USD.after) - Double.Parse(USD.before) : Double.Parse(USD.before) - Double.Parse(USD.after);
                var diffEUR = Double.Parse(EUR.after) > Double.Parse(EUR.before) ? Double.Parse(EUR.after) - Double.Parse(EUR.before) : Double.Parse(EUR.before) - Double.Parse(EUR.after);
                var diffGBP = Double.Parse(GBP.after) > Double.Parse(GBP.before) ? Double.Parse(GBP.after) - Double.Parse(GBP.before) : Double.Parse(GBP.before) - Double.Parse(GBP.after);

                Console.WriteLine("Dolar: {0} -> {1} " + (Double.Parse(USD.after) > Double.Parse(USD.before) ? "Artış" : "Düşüş") + ": " + Math.Round((Double)diffUSD, 5),
                    USD.before, USD.after);
                Console.WriteLine("Euro: {0} -> {1} " + (Double.Parse(EUR.after) > Double.Parse(EUR.before) ? "Artış" : "Düşüş") + ": " + Math.Round((Double)diffEUR, 5),
                    EUR.before, EUR.after);
                Console.WriteLine("Sterlin: {0} -> {1} " + (Double.Parse(GBP.after) > Double.Parse(GBP.before) ? "Artış" : "Düşüş") + ": " + Math.Round((Double)diffGBP, 5),
                    GBP.before, GBP.after);

                Thread.Sleep(1000);
            }
        }

        public void getData()
        {
            HttpClient client = new HttpClient();
            try
            {
                var response = client.GetAsync("https://dolar.tlkur.com/refresh/doviz.php").
                Result.Content.ReadAsStringAsync().Result;
                JObject jsonObject = JObject.Parse(response);

                setData(jsonObject);
            } catch
            {
                Console.WriteLine("HTTPS bağlantısı sağlanamadı, tekrar deneniyor.");
                return;
            }
        }

        public void setData(JObject jsonObject)
        {
            var afterUSD = jsonObject["USDTRY"].ToString();
            var afterEUR = jsonObject["EURTRY"].ToString();
            var afterGBP = jsonObject["GBPTRY"].ToString();
            if (data.Count == 0)
            {
                this.data.Add(Type.USD, new Currency(this.USD, afterUSD));
                this.data.Add(Type.EUR, new Currency(this.EUR, afterEUR));
                this.data.Add(Type.GBP, new Currency(this.GBP, afterGBP));
            } else
            {
                if (afterUSD != this.USD) this.data[Type.USD] = new Currency(this.USD, afterUSD);
                if (afterEUR != this.EUR) this.data[Type.EUR] = new Currency(this.EUR, afterEUR);
                if (afterGBP != this.GBP) this.data[Type.GBP] = new Currency(this.GBP, afterGBP);
            }

            this.USD = afterUSD;
            this.EUR = afterEUR;
            this.GBP = afterGBP;
        }

        public class Currency
        {
            public string before;
            public string after;

            public Currency(string before, string after)
            {
                this.before = before;
                this.after = after;
            }
        }

        public enum Type
        {
            USD,
            EUR,
            GBP
        }
    }
}
