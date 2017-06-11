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
    public class ASXListedCompaniesTests
    {
        [TestMethod]
        public void UpdateASXListedCompaniesBatchDates()
        {
            MongoDBDataAccess.UpdateASXListedCompaniesBatchDates();

        }

        [TestMethod]
        public void GetASXListedCompaniesBatchRecord()
        {
            var batchRecord = MongoDBDataAccess.GetASXListedCompaniesBatchRecord(null);

        }

        [TestMethod]
        [DeploymentItem(@"UnitTestProject1\testdata\ASXListedCompanies.csv", "testdata")]
        public void TestASXListedCompaniesBatch()
        {
            List<ASXListedCompany> stocks;
            string testData = System.IO.File.ReadAllText(@"testdata\ASXListedCompanies.csv");
            ASXStockEngine.Parse(out stocks, testData);
            MongoDBDataAccess.ProcessASXListedCompaniesBatch(stocks);

        }


        [TestMethod]
        public void TestStartUpdateASXListedCompaniesBatch()
        {
            ASXListedCompanyBatch batch;
            var result = MongoDBDataAccess.StartUpdateASXListedCompaniesBatch(null, out batch);

        }

        [TestMethod]
        public void TestUpdateCacheAsynch()
        {
            ut.Logging.Logger.LogMessage("TestUpdateCacheAsynch started ...");

            Task t = Task.Run(() =>
                    {
                        var batch = MongoDBDataAccess.GetASXListedCompaniesBatchRecord(null);
                        ut.Logging.Logger.LogMessage("batch last update {0} before update", batch.Dates.LastUpdatedDate);
                        ASXStockEngine.UpdateCache();
                    }).ContinueWith(
                    (x) => {
                        var batch = MongoDBDataAccess.GetASXListedCompaniesBatchRecord(null);
                        ut.Logging.Logger.LogMessage("Cache updated {0}", batch.Dates.LastUpdatedDate);
                    }
                    );
            ut.Logging.Logger.LogMessage("TestUpdateCacheAsynch complete");
            t.Wait();
        }
    }
}
