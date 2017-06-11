using System;
using System.ComponentModel;
using MongoDB.Bson;
using System.Collections.Generic;

namespace StockQuote.Models
{
    public class User
    {
        public ObjectId Id { get; set; }
        public string Login { get; set; }
        [MongoDB.Bson.Serialization.Attributes.BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime? CreatedDate { get; set; }
        [MongoDB.Bson.Serialization.Attributes.BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime? LastAccessDate { get; set; }
    }

}