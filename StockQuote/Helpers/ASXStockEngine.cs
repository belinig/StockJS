using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Xml.Linq;
using Jarloo.CardStock.Models;
using StockQuote.Models;
using System.Net.Http;
using System.Net;
using System.IO;
using System.Text;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Threading.Tasks;
using StockQuote.ExtensionMethods;
using StockQuote.Data;


namespace StockQuote.Helpers
{
    public class ASXStockEngine
    {
        private const string BASE_URL = "http://www.asx.com.au/asx/research/ASXListedCompanies.csv";
        private const string OPTIONS_BASE_URL = "http://www.asx.com.au/data/ASXCLDerivativesMasterList.csv";

        public static DateTime? GetLastCacheUpdateDate()
        {
            DateTime? lastUpdatedDate = null;
            var batch = MongoDBDataAccess.GetASXListedCompaniesBatchRecord(null);
            if (batch != null && batch.Dates != null)
            {

                lastUpdatedDate = batch.Dates.LastUpdatedDate;
            }
            return lastUpdatedDate;
        }

        public static void UpdateCache()
        {
            List<ASXListedCompany> stocks = Fetch();
            MongoDBDataAccess.ProcessASXListedCompaniesBatch(stocks);
        }



        public static List<ASXListedCompany> Fetch()
        {
            List < ASXListedCompany > stocks  = new List<ASXListedCompany>();
            string url = BASE_URL;

            string response = GetStocks(url);

            Parse(out stocks, response);
            return stocks;
        }

        public static List<ASXListedCompany> Fetch(bool cached=true)
        {
            List<ASXListedCompany> stocks = new List<ASXListedCompany>();
            string url = BASE_URL;

            string response = GetStocks(url);

            Parse(out stocks, response);
            return stocks;
        }

        private static string GetStocks(string URL)
        {
            string response = null;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(URL);
            request.Method = "GET";
            //request.ContentType = "application/json";
            request.ContentLength(0);

            try
            {
                WebResponse webResponse = request.GetResponse();

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

        public static void Parse(out List<ASXListedCompany> stocks, string csvfile)
        {
            stocks = new List<ASXListedCompany>();
            StringReader reader = new StringReader(csvfile);
            string firstLine = reader.ReadLine();
            string headerLine = null;
            if (firstLine == null || !firstLine.StartsWith("ASX listed companies as at") || 
                reader.ReadLine() == null || (headerLine = reader.ReadLine()) == null ||
                !headerLine.Equals("Company name,ASX code,GICS industry group"))
                return;
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                ASXListedCompany c = new ASXListedCompany();
                string[] strStock = line.Split(',');
                c.Name = strStock[0].Trim('\"');
                c.Code = strStock[1].Trim('\"');
                c.IndustryGroup = strStock[2].Trim('\"');
                stocks.Add(c);
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