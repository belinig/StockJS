/*
    Jarloo
    http://jarloo.com
 
    This work is licensed under a Creative Commons Attribution-ShareAlike 3.0 Unported License  
    http://creativecommons.org/licenses/by-sa/3.0/     

*/
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Xml.Linq;
using Jarloo.CardStock.Models;
using System.Net.Http;
using System.Net;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace Jarloo.CardStock.Helpers
{
    public class YahooStockEngine
    {
        private const string BASE_URL = "http://query.yahooapis.com/v1/public/yql?q=select%20*%20from%20yahoo.finance.quotes%20where%20symbol%20in%20({0})&format=json&env=store%3A%2F%2Fdatatables.org%2Falltableswithkeys";

        public static void Fetch(ObservableCollection<Quote> quotes)
        {
            string symbolList = String.Join("%2C", quotes.Select(w => "%22" + w.Symbol + "%22").ToArray());
            string url = string.Format(BASE_URL,symbolList);
            string response = Execute(url);
            XDocument doc = JsonConvert.DeserializeXNode(response);
            Parse(quotes,doc);
        }

        /*
        https://query.yahooapis.com/v1/public/yql?q=select%20*%20from%20yahoo.finance.historicaldata%20where%20symbol%20in%20(%22far.ax%22)%20and%20startDate%20=%20%222017-02-03%22%20and%20endDate%20=%20%222017-02-03%22&format=json&env=store%3A%2F%2Fdatatables.org%2Falltableswithkeys&callback=
        */

        private const string HISTORICAL_BASE_URL = "https://query.yahooapis.com/v1/public/yql?q=select%20*%20from%20yahoo.finance.historicaldata%20where%20symbol%20in%20({0})%20and%20startDate%20=%20%22{1}%22%20and%20endDate%20=%20%22{2}%22&format=json&env=store%3A%2F%2Fdatatables.org%2Falltableswithkeys";
        private const string cDateOnlyUS = "yyyy-MM-dd";
        public static void FetchHistorical(ObservableCollection<Quote> quotes, DateTime date)
        {
            string symbolList = String.Join("%2C", quotes.Select(w => "%22" + w.Symbol + "%22").ToArray());
            string dateOnlyUS = date.ToString(cDateOnlyUS);
            string url = string.Format(HISTORICAL_BASE_URL, symbolList, dateOnlyUS, dateOnlyUS);

            string response = Execute(url);
            
            XDocument doc = JsonConvert.DeserializeXNode(response);
            ParseHistorical(quotes, doc);
        }

        private static string Execute(string URL)
        {
            string response = null;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(URL);
            request.Method = "GET";
            request.ContentType = "application/json";
            //request.ContentLength = 0;

            try
            {
                //WebResponse webResponse = request.GetResponse();
                HttpWebResponse webResponse = Task.Factory.FromAsync(request.BeginGetResponse(null, null), result =>
                {
                    return (HttpWebResponse)request.EndGetResponse(result);
                }).Result;

                Stream webStream = webResponse.GetResponseStream();
                StreamReader responseReader = new StreamReader(webStream);
                response = responseReader.ReadToEnd();
                Console.Out.WriteLine(response);
                responseReader.Dispose();
            }
            catch (Exception e)
            {
                Console.Out.WriteLine("-----------------");
                Console.Out.WriteLine(e.Message);
            }
            return response;

        }

        private static void Parse(ObservableCollection<Quote> quotes, XDocument doc)
        {
            XElement results = doc.Root.Element("results");

            foreach (Quote quote in quotes)
            {
                XElement q = results.Elements("quote").First(w => w.Element("symbol").Value == quote.Symbol);

                quote.Ask = GetDecimal(q.Element("Ask").Value);
                quote.Bid = GetDecimal(q.Element("Bid").Value);
                quote.AverageDailyVolume = GetDecimal(q.Element("AverageDailyVolume").Value);
                quote.BookValue = GetDecimal(q.Element("BookValue").Value);
                quote.Change = GetDecimal(q.Element("Change").Value);
                quote.DividendShare = GetDecimal(q.Element("DividendShare").Value);
                quote.LastTradeDate = GetDateTime(q.Element("LastTradeDate").Value + " " + q.Element("LastTradeTime").Value);
                quote.EarningsShare = GetDecimal(q.Element("EarningsShare").Value);
                quote.EpsEstimateCurrentYear = GetDecimal(q.Element("EPSEstimateCurrentYear").Value);
                quote.EpsEstimateNextYear = GetDecimal(q.Element("EPSEstimateNextYear").Value);
                quote.EpsEstimateNextQuarter = GetDecimal(q.Element("EPSEstimateNextQuarter").Value);
                quote.DailyLow = GetDecimal(q.Element("DaysLow").Value);
                quote.DailyHigh = GetDecimal(q.Element("DaysHigh").Value);
                quote.YearlyLow = GetDecimal(q.Element("YearLow").Value);
                quote.YearlyHigh = GetDecimal(q.Element("YearHigh").Value);
                quote.MarketCapitalization = GetDecimal(q.Element("MarketCapitalization").Value);
                quote.Ebitda = GetDecimal(q.Element("EBITDA").Value);
                quote.ChangeFromYearLow = GetDecimal(q.Element("ChangeFromYearLow").Value);
                quote.PercentChangeFromYearLow = GetDecimal(q.Element("PercentChangeFromYearLow").Value);
                quote.ChangeFromYearHigh = GetDecimal(q.Element("ChangeFromYearHigh").Value);
                quote.LastTradePrice = GetDecimal(q.Element("LastTradePriceOnly").Value);
                quote.PercentChangeFromYearHigh = GetDecimal(q.Element("PercebtChangeFromYearHigh").Value); //missspelling in yahoo for field name
                quote.FiftyDayMovingAverage = GetDecimal(q.Element("FiftydayMovingAverage").Value);
                quote.TwoHunderedDayMovingAverage = GetDecimal(q.Element("TwoHundreddayMovingAverage").Value);
                quote.ChangeFromTwoHundredDayMovingAverage = GetDecimal(q.Element("ChangeFromTwoHundreddayMovingAverage").Value);
                quote.PercentChangeFromTwoHundredDayMovingAverage = GetDecimal(q.Element("PercentChangeFromTwoHundreddayMovingAverage").Value);
                quote.PercentChangeFromFiftyDayMovingAverage = GetDecimal(q.Element("PercentChangeFromFiftydayMovingAverage").Value);
                quote.Name = q.Element("Name").Value;
                quote.Open = GetDecimal(q.Element("Open").Value);
                quote.PreviousClose = GetDecimal(q.Element("PreviousClose").Value);
                quote.ChangeInPercent = GetDecimal(q.Element("ChangeinPercent").Value);
                quote.PriceSales = GetDecimal(q.Element("PriceSales").Value);
                quote.PriceBook = GetDecimal(q.Element("PriceBook").Value);
                quote.ExDividendDate = GetDateTime(q.Element("ExDividendDate").Value);
                quote.PeRatio = GetDecimal(q.Element("PERatio").Value);
                quote.DividendPayDate = GetDateTime(q.Element("DividendPayDate").Value);
                quote.PegRatio = GetDecimal(q.Element("PEGRatio").Value);
                quote.PriceEpsEstimateCurrentYear = GetDecimal(q.Element("PriceEPSEstimateCurrentYear").Value);
                quote.PriceEpsEstimateNextYear = GetDecimal(q.Element("PriceEPSEstimateNextYear").Value);
                quote.ShortRatio = GetDecimal(q.Element("ShortRatio").Value);
                quote.OneYearPriceTarget = GetDecimal(q.Element("OneyrTargetPrice").Value);
                quote.Volume = GetDecimal(q.Element("Volume").Value);
                quote.StockExchange = q.Element("StockExchange").Value;

                quote.LastUpdate = DateTime.Now;
            }
        }


        private static void ParseHistorical(ObservableCollection<Quote> quotes, XDocument doc)
        {
            XElement results = doc.Root.Element("results");

            foreach (Quote quote in quotes)
            {
                //XElement q = results.Elements("quote").First(w => w.Element("Symbol").Value == quote.Symbol);
                XElement q = null;
                foreach (XElement xl in results.Elements("quote"))
                {
                    if (xl.Element("Symbol").Value == quote.Symbol && xl.Element("Date") != null)
                    {
                        q = xl;
                        break;
                    }
                }

                if (q != null)
                {
                    quote.LastTradeDate = GetDateTime(q.Element("Date").Value);
                    quote.Open = GetDecimal(q.Element("Open").Value);
                    quote.DailyLow = GetDecimal(q.Element("Low").Value);
                    quote.DailyHigh = GetDecimal(q.Element("High").Value);
                    quote.LastTradePrice = GetDecimal(q.Element("Close").Value);
                    quote.Volume = GetDecimal(q.Element("Volume").Value);
                    quote.AdjClose = GetDecimal(q.Element("Adj_Close").Value);
                    quote.LastUpdate = DateTime.Now;
                }
            }
        }

        private static decimal? GetDecimal(string input)
        {
            if (input == null) return null;

            input = input.Replace("%", "");

            decimal value;

            if (Decimal.TryParse(input, out value)) return value;
            return null;
        }

        private static DateTime? GetDateTime(string input)
        {
            if (input == null) return null;

            DateTime value;

            if (DateTime.TryParse(input, out value)) return value;
            return null;
        }
    }
}