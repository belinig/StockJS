using System;
using System.ComponentModel;
using MongoDB.Bson;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Jarloo.CardStock.Models;

namespace StockQuote.Models
{
    public class WatchlistQuote
    {
        public Watchlist Watchlist { get; set; }
        public ObservableCollection<Quote> Quotes { get; set; }
    }

}