using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Jarloo.CardStock.Models;
using System.Collections.ObjectModel;
using Jarloo.CardStock.Helpers;
using MongoDB.Bson;
using StockQuote.Models;
using Microsoft.AspNetCore.Authorization;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace StockQuote.Controllers
{
    [Authorize]
    public class QuoteController : Controller
    {
        // GET: /<controller>/
        //public IActionResult QuoteFar()
        //{
        //    string ticker = "FAR";
        //    ObservableCollection<Quote>  Quotes = new ObservableCollection<Quote>();

        //    //Some example tickers
        //    Quotes.Add(new Quote(ticker + ".AX"));

        //    YahooStockEngine.Fetch(Quotes);

        //    var quote = Quotes[0];
        //    return View(quote);
        //}

        // GET: /<controller>/
        public IActionResult Quote(string ticker)
        {
            ObservableCollection<Quote> Quotes = new ObservableCollection<Quote>();

            //Some example tickers
            Quotes.Add(new Quote(ticker + ".AX"));

            YahooStockEngine.Fetch(Quotes);

            var quote = Quotes[0];
            return new JsonResult(quote.ToJson());
        }

        //public IActionResult Watchlist(string watchlist)
        //{
        //        ObservableCollection<Quote> Quotes = new ObservableCollection<Quote>();

        //    BsonDocument doc = StockQuote.Data.MongoDBDataAccess.Watchlist(watchlist);
        //    //Some example tickers
        //    foreach (string ticker in doc["codes"].AsBsonArray.Select(p => p.AsString).ToArray())
        //    {
        //        Quotes.Add(new Quote(ticker + ".AX"));
        //    }

        //    YahooStockEngine.Fetch(Quotes);
        //    return View(Quotes.AsEnumerable());
        //}

        public IActionResult Watchlist(string watchlistname, DateTime? date)
        {
            ObservableCollection<Quote> Quotes = new ObservableCollection<Quote>();

            Watchlist watchlist = StockQuote.Data.MongoDBDataAccess.Watchlist(User?.Identity.Name, watchlistname);
            //Some example tickers
            foreach (string ticker in watchlist.Symbols)
            {
                Quotes.Add(new Quote(ticker + ".AX"));
            }

            if (!date.HasValue)
                YahooStockEngine.Fetch(Quotes);
            else
                YahooStockEngine.FetchHistorical(Quotes, date.Value);

            WatchlistViewModel model = new WatchlistViewModel();
            model.Quotes = Quotes.AsEnumerable();
            model.Name = watchlistname;
            model.Date = date ?? DateTime.Now;
            return View(model);
        }

        public IActionResult Watch()
        {

            return View();
        }

        public IActionResult QuoteTabs()
        {

            return View();
        }

    }
}
