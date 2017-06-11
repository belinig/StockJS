using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Driver;
using MongoDB.Bson;
using MongoDB.Driver.Linq;
using MongoDB.Driver.Core;
using MongoDB.Driver.GeoJsonObjectModel;
using StockQuote.Models;

namespace StockQuote.Data
{

    public class StocksDB
    {

        const string cStocksDatabaseName = "stocks";
        const string cConnection = "mongodb://localhost:27017";

        public string DatabaseName { get; set; }
        public string Connection { get; set; }
        public IMongoDatabase Database { get; set; }
        public MongoClient Client { get; set; }

        public StocksDB()
        {

            DatabaseName = cStocksDatabaseName;
            Connection = "mongodb://localhost:27017";
        }

        public virtual IMongoCollection<TConnectionType> GetCollection<TConnectionType>(string collectionName)
        {
            Client = new MongoClient("mongodb://localhost:27017");
            Database = Client.GetDatabase(DatabaseName);
            IMongoCollection<TConnectionType> collection = Database.GetCollection<TConnectionType>(collectionName);
            return collection;
        }
     }
}
