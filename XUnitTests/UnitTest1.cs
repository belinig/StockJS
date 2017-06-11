using System;
using Xunit;
using StockQuote.Data;
namespace XUnitTests
{
    public class UnitTest1
    {
        [Fact]
        public void Test1()
        {
            StockQuote.Data.MongoDBDataAccess da = new MongoDBDataAccess();
            MongoDBDataAccess.Watchlist(null, "Oil");
        }
    }
}
