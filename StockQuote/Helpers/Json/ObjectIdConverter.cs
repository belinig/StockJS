using MongoDB.Bson;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace StockQuote.Helpers.Json
{
     //[JsonConverter(typeof(ObjectIdConverter))]
     class ObjectIdConverter : JsonConverter
        {

            public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
            {
                serializer.Serialize(writer, value.ToString());

            }

            public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
            {
                ObjectId id = new ObjectId(reader.Value.ToString());
                return id;
                //throw new NotImplementedException();
            }

            public override bool CanConvert(Type objectType)
            {
                return typeof(ObjectId).IsAssignableFrom(objectType);
                //return true;
            }

                //
                // Summary:
                //     Gets a value indicating whether this Newtonsoft.Json.JsonConverter can read JSON.
                public override bool CanRead {
                    get { return true; }
                }
                //
                // Summary:
                //     Gets a value indicating whether this Newtonsoft.Json.JsonConverter can write
                //     JSON.
                public override bool CanWrite {
                    get { return true; }
                }


    }
}
