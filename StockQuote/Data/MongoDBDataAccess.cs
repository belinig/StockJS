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
    public class MongoDBDataAccess
    {
        public static List<BsonDocument> Connect()
        {

            string databaseName = "stocks";
            string collectionName = "watchlists";
            MongoClient client = new MongoClient("mongodb://localhost:27017");
            var database = client.GetDatabase(databaseName);
            IMongoCollection<BsonDocument> collection = database.GetCollection<BsonDocument>(collectionName);
            var filter = Builders<BsonDocument>.Filter.Empty;
            var docs = collection.Find<BsonDocument>(filter).ToList();
            foreach (var doc in docs)
            {
                System.Diagnostics.Debug.WriteLine(doc["watchlist"]);
            }
            return docs;
        }

        public static BsonDocument BsonWatchlist(string login, string name)
        {
            User user = null;
            string collectionName = "watchlists";
            StocksDB stocksDB = new StocksDB();
            user = GetUserRecord(stocksDB, login);

            IMongoCollection<BsonDocument> collection = stocksDB.GetCollection<BsonDocument>(collectionName);
            var filter = Builders<BsonDocument>.Filter.Empty;
            var docs = collection.Find<BsonDocument>(filter).ToList();
            foreach (var doc in docs)
            {
                if (doc.Contains("Name"))
                {
                    System.Diagnostics.Debug.WriteLine(doc["Name"]);
                    if (doc["Name"].AsString.ToUpper() == name.ToUpper())
                    {
                        return doc;
                    }
                }
            }
            return null;
        }

        public static Watchlist Watchlist(string login, string name)
        {
            User user = null;
            string collectionName = "watchlists";
            StocksDB stocksDB = new StocksDB();
            user = GetUserRecord(stocksDB, login);

             return GetWatchlistRecord(stocksDB, user, name);
        }

        #region ASXListedCompanies
        //public static BsonDocument GetASXListedCompaniesDates()
        //{

        //    string databaseName = "stocks";
        //    string collectionName = "ASXListedCompanies";
        //    MongoClient client = new MongoClient("mongodb://localhost:27017");
        //    var database = client.GetDatabase(databaseName);
        //    var collection = database.GetCollection<BsonDocument>(collectionName);
        //    var query_where2 = from a in collection.AsQueryable<BsonDocument>()
        //                        where a..Name.Contains("Contoso")
        //                        where a.Address1_City == "Redmond"
        //                        select a;
        //    foreach (var doc in docs)
        //    {
        //        System.Diagnostics.Debug.WriteLine(doc["watchlist"]);
        //        if (doc["watchlist"].AsString.ToUpper() == name.ToUpper())
        //        {
        //            return doc;
        //        }
        //    }
        //    return null;
        //}


        public static void UpdateASXListedCompaniesBatchDates()
        {
            StocksDB stocksDB = new StocksDB();
            ASXListedCompanyBatch batch = GetASXListedCompaniesBatchRecord(stocksDB);
            if (batch == null)
            {
                batch = new ASXListedCompanyBatch();
                //batch.Id = ObjectId.GenerateNewId();
            }

            DateTime? updateStartedDateCurrent = batch.Dates.UpdateStartedDate;
            batch.Dates.UpdateStartedDate = DateTime.Now;


            string databaseName = "stocks";
            string collectionName = "ASXListedCompanies";
            MongoClient client = new MongoClient("mongodb://localhost:27017");
            var database = client.GetDatabase(databaseName);
            var collection = database.GetCollection<ASXListedCompanyBatch>(collectionName);
            //var filter = Builders<ASXListedCompanyBatch>.Filter.And(Builders<ASXListedCompanyBatch>.Filter.Eq("Id", batch.Id),
            //                                                        Builders<ASXListedCompanyBatch>.Filter.Eq("Id", batch.Id));
            //ReplaceResult replaceResult = collection.ReplaceOne(filter, batch, new UpdateOptions { IsUpsert = true });
            //var insert = new UpdateO
            //using(stocksDB.Client..)
            //collection.BulkWrite
        }

        public static void ProcessASXListedCompaniesBatch(List<ASXListedCompany> stocks)
        {
            StocksDB stocksDB = new StocksDB();
            ASXListedCompanyBatch batch;
            bool batchStated = StartUpdateASXListedCompaniesBatch(stocksDB, out batch);
            if (!batchStated) return;

            foreach(ASXListedCompany company in stocks)
            {
                ASXListedCompany companyRecord = FindCompanyRecord(stocksDB, company);
                if (companyRecord != null)
                {
                    companyRecord.Dates.UpdateStartedDate = batch.Dates.UpdateStartedDate;
                }
                else
                {
                    companyRecord = company;
                    companyRecord.Id = ObjectId.GenerateNewId();
                    companyRecord.Dates = new Dates();
                    companyRecord.Dates.CreatedDate = batch.Dates.UpdateStartedDate;
                    companyRecord.Dates.UpdateStartedDate = companyRecord.Dates.CreatedDate;
                }

                SaveCompanyRecord(stocksDB, companyRecord);
            }

            MarkBatchCompleted(stocksDB, batch);

        }

        public static bool MarkBatchCompleted(StocksDB stocksDB, ASXListedCompanyBatch batch)
        {
            if (stocksDB is null) stocksDB = new StocksDB();
            string collectionName = "ASXListedCompanies";
            var collection = stocksDB.GetCollection<ASXListedCompanyBatchRecord>(collectionName);
            var filter = Builders<ASXListedCompanyBatchRecord>.Filter.Eq("Dates.UpdateStartedDate", batch.Dates.UpdateStartedDate);
            var update = Builders<ASXListedCompanyBatchRecord>.Update.CurrentDate("Dates.LastUpdatedDate");
            try
            {
                UpdateResult replaceResult = collection.UpdateMany(filter, update);
            }
            catch (Exception e)
            {
                Console.Write(e);
                return false;
            }
            return true;
        }


        public static bool SaveCompanyRecord(StocksDB stocksDB, ASXListedCompany company)
        {
            if (stocksDB is null) stocksDB = new StocksDB();
            string collectionName = "ASXListedCompanies";
            var collection = stocksDB.GetCollection<ASXListedCompany>(collectionName);
            var filter = Builders<ASXListedCompany>.Filter.Eq("Id", company.Id);
            try
            {
                ReplaceOneResult replaceResult = collection.ReplaceOne(filter, company, new UpdateOptions { IsUpsert = true });
            }
            catch (Exception e)
            {
                Console.Write(e);
                return false;
            }
            return true;
        }

        public static ASXListedCompany FindCompanyRecord(StocksDB stocksDB, ASXListedCompany company )
        {
            ASXListedCompany companyRecord = null;
            if (stocksDB is null) stocksDB = new StocksDB();
            string collectionName = "ASXListedCompanies";
            var collection = stocksDB.GetCollection<ASXListedCompany>(collectionName);
            var filter =  Builders<ASXListedCompany>.Filter.And(
                   Builders<ASXListedCompany>.Filter.Eq("Code", company.Code),
                   Builders<ASXListedCompany>.Filter.Eq("Name", company.Name),
                   Builders<ASXListedCompany>.Filter.Eq("IndustryGroup", company.IndustryGroup));
            try
            {
                companyRecord = collection.Find<ASXListedCompany>(filter).First();
            }
            catch (Exception e)
            {
                Console.Write(e);
            }
            return companyRecord;
        }

        public static List<ASXListedCompany> FindCompaniesByCode(string match, int top=0)
        {
            List<ASXListedCompany> companies = null;
            StocksDB stocksDB = new StocksDB();
            string collectionName = "ASXListedCompanies";
            var collection = stocksDB.GetCollection<ASXListedCompany>(collectionName);
            string regex = string.Concat("/", match, "/i");
            var filter = Builders<ASXListedCompany>.Filter.Regex("Code", regex);
            try
            {
                 companies = collection.Find<ASXListedCompany>(filter).Limit(top).ToList<ASXListedCompany>();

            }
            catch (Exception e)
            {
                Console.Write(e);
            }
            return companies;
        }

        public static List<ASXListedCompany> FindCompaniesByDescription(string match, int top=0)
        {
            List<ASXListedCompany> companies = null;
            StocksDB stocksDB = new StocksDB();
            string collectionName = "ASXListedCompanies";
            var collection = stocksDB.GetCollection<ASXListedCompany>(collectionName);
            string regex = string.Concat("/", match, "/i");
            var filter = Builders<ASXListedCompany>.Filter.Regex("Code", regex);
            try
            {
                companies = collection.Find<ASXListedCompany>(filter).Limit(top).ToList<ASXListedCompany>();
            }
            catch (Exception e)
            {
                Console.Write(e);
            }
            return companies;
        }


        public static ASXListedCompanyBatch GetASXListedCompaniesBatchRecord(StocksDB stocksDB)
        {
            ASXListedCompanyBatch batch = null;
            if (stocksDB is null) stocksDB = new StocksDB();
            string collectionName = "ASXListedCompanies";
            var collection = stocksDB.GetCollection<ASXListedCompanyBatch>(collectionName);
            var filter = Builders<ASXListedCompanyBatch>.Filter.Eq("BatchName", ASXListedCompanyBatch.cBatchName);
            try
            {
                 batch = collection.Find<ASXListedCompanyBatch>(filter).First();
            }
            catch (Exception e)
            {
                Console.Write(e);
            }
            return batch;
        }

        public static bool StartUpdateASXListedCompaniesBatch(StocksDB stocksDB, out ASXListedCompanyBatch batch)
        {
            batch = null;
            string collectionName = "ASXListedCompanies";
            if (stocksDB is null) stocksDB = new StocksDB();
            batch = GetASXListedCompaniesBatchRecord(stocksDB);
            var collection = stocksDB.GetCollection<ASXListedCompanyBatch>(collectionName);
            if (batch == null)
            {
                batch = new ASXListedCompanyBatch();
                batch.BatchName = ASXListedCompanyBatch.cBatchName;
                batch.Dates = new Dates();
                batch.Dates.CreatedDate = DateTime.Now;
                batch.Dates.UpdateStartedDate = batch.Dates.CreatedDate;
                var filter = Builders<ASXListedCompanyBatch>.Filter.Eq("BatchName", ASXListedCompanyBatch.cBatchName);

                try
                {
                    collection.InsertOne(batch);
                }
                catch (Exception e)
                {
                    Console.Write(e);
                    return false;
                }

            }
            else
            {
                DateTime updateStartedDate = DateTime.Now;

                //Check for in progress or run away updates
                if (!batch.Dates.LastUpdatedDate.HasValue ||
                    (batch.Dates.UpdateStartedDate > batch.Dates.LastUpdatedDate &&
                     batch.Dates.UpdateStartedDate.Value.AddHours(1) > updateStartedDate)
                   )
                    return false;

                var filter = Builders<ASXListedCompanyBatch>.Filter.And(
                    Builders<ASXListedCompanyBatch>.Filter.Eq("BatchName", ASXListedCompanyBatch.cBatchName),
                    Builders<ASXListedCompanyBatch>.Filter.Eq("Dates.LastUpdatedDate", batch.Dates.LastUpdatedDate),
                    Builders<ASXListedCompanyBatch>.Filter.Eq("Dates.UpdateStartedDate", batch.Dates.UpdateStartedDate));
                try
                {
                    batch.Dates.UpdateStartedDate = updateStartedDate;
                    ReplaceOneResult replaceResult = collection.ReplaceOne(filter, batch);
                    return replaceResult.ModifiedCount == 1;
                }
                catch (Exception e)
                {
                    Console.Write(e);
                    return false;
                }
            }

            return true;
        }
        #endregion ASXListedCompanies

        #region Watchlists
        public static User GetUserRecord(StocksDB stocksDB, string login)
        {
            User user = null;
            if (stocksDB is null) stocksDB = new StocksDB();
            string collectionName = "users";
            var collection = stocksDB.GetCollection<User>(collectionName);
            var filter = Builders<User>.Filter.Eq("Login", login);
            try
            {
                user = collection.Find<User>(filter).First();
            }
            catch (Exception e)
            {
                Console.Write(e);
            }
            return user;
        }

        public static User GetOrCreateUser(StocksDB stocksDB, string login)
        {
            User user = null;
            string collectionName = "users";
            if (stocksDB is null) stocksDB = new StocksDB();
            user = GetUserRecord(stocksDB, login);
            var collection = stocksDB.GetCollection<User>(collectionName);
            if (user == null)
            {
                user = new User();
                user.Id = ObjectId.GenerateNewId();
                user.Login = login;
                user.CreatedDate = DateTime.Now;
                user.LastAccessDate = user.CreatedDate;
                
                try
                {
                    collection.InsertOne(user);
                }
                catch(Exception e)
                {
                    Console.Write(e);
                    return null;
                }

            }
            else
            {
                DateTime lastAccessDate = DateTime.Now;

                var filter = Builders<User>.Filter.And(
                    Builders<User>.Filter.Eq("Id", user.Id),
                    Builders<User>.Filter.Lt("LastAccessDate", lastAccessDate));
                try
                {
                    user.LastAccessDate = lastAccessDate;
                    ReplaceOneResult replaceResult = collection.ReplaceOne(filter, user);
                }
                catch (Exception e)
                {
                    Console.Write(e);
                    return null;
                }
            }

            return user;
        }

        public static Watchlist GetWatchlistRecord(StocksDB stocksDB, User user, string watchlistName)
        {
            Watchlist watchlist = null;
            if (stocksDB is null) stocksDB = new StocksDB();
            string collectionName = "watchlists";
            var collection = stocksDB.GetCollection<Watchlist>(collectionName);
            var filter = Builders<Watchlist>.Filter.And(
                    Builders<Watchlist>.Filter.Eq("Name", watchlistName),
                    Builders<Watchlist>.Filter.Eq("UserId", user.Id));
            try
            {
                watchlist = collection.Find<Watchlist>(filter).First();
            }
            catch (Exception e)
            {
                Console.Write(e);
            }
            return watchlist;
        }

        public static Watchlist GetWatchlistRecord(StocksDB stocksDB, User user, ObjectId id)
        {
            Watchlist watchlist = null;
            if (stocksDB is null) stocksDB = new StocksDB();
            string collectionName = "watchlists";
            var collection = stocksDB.GetCollection<Watchlist>(collectionName);
            var filter = Builders<Watchlist>.Filter.Eq("Id", id);
            try
            {
                watchlist = collection.Find<Watchlist>(filter).First();
            }
            catch (Exception e)
            {
                Console.Write(e);
            }
            return watchlist;
        }

        public static bool SaveWatchlist(StocksDB stocksDB, string login, Watchlist watchlist)
        {
            User user = null;
            string collectionName = "watchlists";
            if (stocksDB is null) stocksDB = new StocksDB();
            user = GetOrCreateUser(stocksDB, login);
            if (user == null) throw new UserException("Unable to access user:"+login);

            watchlist.UserId = user.Id;
            watchlist.LastAccessDate = user.LastAccessDate;

            Watchlist watchlistRecord = null;
            if (watchlist.Id != null)
                watchlistRecord = GetWatchlistRecord(stocksDB, user, watchlist.Id);

            if (watchlistRecord != null)
            {
                watchlist.CreatedDate = watchlistRecord.CreatedDate;
            }
            else
            {
                watchlist.Id = ObjectId.GenerateNewId();
                watchlist.CreatedDate = user.LastAccessDate;
            }

            watchlist.LastAccessDate = user.LastAccessDate;

            var collection = stocksDB.GetCollection<Watchlist>(collectionName);

            DateTime lastAccessDate = DateTime.Now;

            var filter = Builders<Watchlist>.Filter.Eq("Id", watchlist.Id);
            try
            {
                user.LastAccessDate = lastAccessDate;
                ReplaceOneResult replaceResult = collection.ReplaceOne(filter, watchlist, new UpdateOptions { IsUpsert = true });
                if (replaceResult.ModifiedCount != 1 && !(replaceResult.IsAcknowledged && replaceResult.UpsertedId != null))
                    return false;
            }
            catch (Exception e)
            {
                Console.Write(e);
                throw new WatchlistException("Failed to save watchlist:" + watchlist.Name + ":" + e.Message);
            }

            return true;
        }

        public static List<string> GetWatchlists(StocksDB stocksDB, string login)
        {
            User user = null;
            string collectionName = "watchlists";
            if (stocksDB is null) stocksDB = new StocksDB();
            user = GetOrCreateUser(stocksDB, login);
            var collection = stocksDB.GetCollection<Watchlist>(collectionName);
            List<string> watchlists = null;


            var filter = Builders<Watchlist>.Filter.Eq("UserId", user?.Id);
            var fields = Builders<Watchlist>.Projection.Include(p => p.Name);
            try
            {
                watchlists = (from p in
                                collection.Find(filter).Project<Watchlist>(fields).ToList().AsQueryable()
                                select p.Name).ToList<string>();
            }
            catch (Exception e)
            {
                Console.Write(e);
            }
            return watchlists;
        }


        #endregion Watchlists
    }
}
