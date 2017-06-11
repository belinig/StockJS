using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ut = Microsoft.VisualStudio.TestTools.UnitTesting;
using StockQuote.Data;
using StockQuote.Helpers;
using StockQuote.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace UnitTestProject1
{
    [TestClass]
    public class WatchlistApiTests
    {
        [TestMethod]
        public void DeserializeWatchlistTest()
        {
            string value = "{ Name: \"NewWachtlist\", Symbols: [\"far\", \"myr\"]}";
            Watchlist watchlist = Newtonsoft.Json.JsonConvert.DeserializeObject<Watchlist>(value);
        }


        [TestMethod]
        public void GetWatchlists()
        {
            List<string>  watchlists = MongoDBDataAccess.GetWatchlists(null, "igor.belin@hotmail.com");
        }
    }
}
