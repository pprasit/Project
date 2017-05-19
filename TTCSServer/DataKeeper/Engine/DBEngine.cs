using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;

namespace DataKeeper.Engine
{
    class DBEngine
    {
        protected static IMongoClient _client;
        protected static IMongoDatabase _database;

        public DBEngine()
        {
            _client = new MongoClient();
            _database = _client.GetDatabase("STATION_DATA");
        }

        public void insert(String StationName, String DeviceName, String FieldName, String Value, DateTime DataTimestamp)
        {
            var document = new BsonDocument
                    {
                        { "Values", Value },
                        { "Updated", DataTimestamp.ToString("yyyy-MM-dd HH:mm:ss") }
                    };

            Task TTask = Task.Run(async () =>
            {
                var collection = _database.GetCollection<BsonDocument>(StationName + "_"+ DeviceName +"_" + FieldName);
                await collection.InsertOneAsync(document);
            });
        }
    }
}
