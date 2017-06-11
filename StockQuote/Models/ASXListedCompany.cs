using System;
using System.ComponentModel;
using MongoDB.Bson;

namespace StockQuote.Models
{
    public class ASXListedCompany : ASXListedCompanyBatchRecord
    {
        public ObjectId Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string IndustryGroup { get; set; }
    }

    public class Dates
    {
        [MongoDB.Bson.Serialization.Attributes.BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime? CreatedDate { get; set; }
        [MongoDB.Bson.Serialization.Attributes.BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime? LastUpdatedDate { get; set; }
        [MongoDB.Bson.Serialization.Attributes.BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime? UpdateStartedDate { get; set; }
    }

    public class ASXListedCompanyBatch : ASXListedCompanyBatchRecord
    {
        public const string cBatchName = "ASXListedCompanyBatch";
        [MongoDB.Bson.Serialization.Attributes.BsonId]
        public string BatchName { get; set; }
    }

    public class ASXListedCompanyBatchRecord
    {
        public Dates Dates { get; set; }
    }

}