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

        public void insert_user_login(String StationName, String DeviceName, String FieldName, String Value, String State, DateTime AccessDate, DateTime DataTimestamp)
        {
            Task TTask = Task.Run(async () =>
            {
                var collection = _database.GetCollection<BsonDocument>(StationName + "_" + DeviceName + "_" + FieldName);
                var builder = Builders<BsonDocument>.Filter;
                var filter = builder.Eq("Values", Value) & builder.Eq("State", "LOGIN") & builder.Eq("LogoutDate", "");
                var update = Builders<BsonDocument>.Update.Set("State", State).Set("LogoutDate", AccessDate.ToString("yyyy-MM-dd HH:mm:ss")).Set("Updated", DataTimestamp.ToString("yyyy-MM-dd HH:mm:ss"));

                await collection.UpdateManyAsync(filter, update);
            });

            var document = new BsonDocument
                    {
                        { "Values", Value },
                        { "State", State },
                        { "LoginDate", AccessDate.ToString("yyyy-MM-dd HH:mm:ss") },
                        { "LogoutDate", "" },
                        { "Updated", DataTimestamp.ToString("yyyy-MM-dd HH:mm:ss") }
                    };

            Task TTask2 = Task.Run(async () =>
            {
                var collection = _database.GetCollection<BsonDocument>(StationName + "_" + DeviceName + "_" + FieldName);
                await collection.InsertOneAsync(document);
            });
        }

        public void insert_user_logout(String StationName, String DeviceName, String FieldName, String Value, String State, DateTime AccessDate, DateTime DataTimestamp)
        {
            Task TTask = Task.Run(async () =>
            {
                var collection = _database.GetCollection<BsonDocument>(StationName + "_" + DeviceName + "_" + FieldName);
                var builder = Builders<BsonDocument>.Filter;
                var filter = builder.Eq("Values", Value) & builder.Eq("State", "LOGIN") & builder.Eq("LogoutDate", "");
                var update = Builders<BsonDocument>.Update.Set("State", State).Set("LogoutDate", AccessDate.ToString("yyyy-MM-dd HH:mm:ss")).Set("Updated", DataTimestamp.ToString("yyyy-MM-dd HH:mm:ss"));
                
                await collection.UpdateManyAsync(filter, update);
            });
        }

        /*
                        if(FieldName.ToString().Equals("ASTROHEVEN_SHUTTERA_STATUS"))
                        {
                            db.shutter_a = true;
                        }
                        else if(FieldName.ToString().Equals("ASTROHEVEN_SHUTTERB_STATUS"))
                        {
                            db.shutter_b = true;
                        }
          
         */

        public void insert_dome_open(String StationName, String DeviceName, String FieldName, String Value, DateTime AccessDate, DateTime DataTimestamp)
        {
            Task TTask = Task.Run(async () =>
            {
                var dome_side = "";

                if(FieldName.ToString().Equals("ASTROHEVEN_SHUTTERA_STATUS"))
                {
                    dome_side = "A";
                }
                else if (FieldName.ToString().Equals("ASTROHEVEN_SHUTTERB_STATUS"))
                {
                    dome_side = "B";
                }

                if (dome_side != "")
                {
                    var collection = _database.GetCollection<BsonDocument>(StationName + "_" + DeviceName + "_TEMP");
                    var builder = Builders<BsonDocument>.Filter;
                    var filter = builder.Eq("SHUTTER", dome_side) & builder.Eq("State", "OPEN") & builder.Eq("CloseDate", "");
                    var cursor = collection.Find(filter);

                    var count = cursor.CountAsync();

                    if (count.Result <= 0) //NO OPENED State, Do insert to DB.
                    {
                        var document = new BsonDocument
                        {
                            { "SHUTTER", dome_side },
                            { "State", "OPEN" },
                            { "OpenDate", AccessDate.ToString("yyyy-MM-dd HH:mm:ss") },
                            { "CloseDate", "" },
                            { "Updated", DataTimestamp.ToString("yyyy-MM-dd HH:mm:ss") }
                        };

                        await collection.InsertOneAsync(document);
                    }

                    var collection3 = _database.GetCollection<BsonDocument>(StationName + "_" + DeviceName + "_TEMP");
                    var builder3 = Builders<BsonDocument>.Filter;
                    var filter3 = builder.Eq("State", "OPEN");
                    var cursor3 = collection3.Find(filter);

                    var count3 = cursor3.CountAsync();

                    if (count3.Result <= 0)
                    {
                        var collection2 = _database.GetCollection<BsonDocument>(StationName + "_" + DeviceName + "_STATE");
                        var builder2 = Builders<BsonDocument>.Filter;
                        var filter2 = builder2.Eq("State", "OPEN") & builder.Eq("CloseDate", "");
                        var cursor2 = collection2.Find(filter);

                        var count2 = cursor2.CountAsync();

                        if (count2.Result <= 0) //NO OPENED State, Do insert to DB.
                        {
                            var document = new BsonDocument
                            {
                                { "State", "OPEN" },
                                { "OpenDate", AccessDate.ToString("yyyy-MM-dd HH:mm:ss") },
                                { "CloseDate", "" },
                                { "Updated", DataTimestamp.ToString("yyyy-MM-dd HH:mm:ss") }
                            };

                            await collection.InsertOneAsync(document);
                        }
                    }
                }
            });
        }

        public void insert_dome_close(String StationName, String DeviceName, String FieldName, String Value, DateTime AccessDate, DateTime DataTimestamp)
        {
            Task TTask = Task.Run(async () =>
            {
                var dome_side = "";

                if (FieldName.ToString().Equals("ASTROHEVEN_SHUTTERA_STATUS"))
                {
                    dome_side = "A";
                }
                else if (FieldName.ToString().Equals("ASTROHEVEN_SHUTTERB_STATUS"))
                {
                    dome_side = "B";
                }

                if (dome_side != "")
                {
                    var collection = _database.GetCollection<BsonDocument>(StationName + "_" + DeviceName + "_TEMP");
                    var builder = Builders<BsonDocument>.Filter;
                    var filter = builder.Eq("SHUTTER", dome_side) & builder.Eq("State", "OPEN") & builder.Eq("CloseDate", "");
                    var update = Builders<BsonDocument>.Update.Set("State", "CLOSE").Set("ClosedDate", AccessDate.ToString("yyyy-MM-dd HH:mm:ss")).Set("Updated", DataTimestamp.ToString("yyyy-MM-dd HH:mm:ss"));

                    await collection.UpdateManyAsync(filter, update);

                    var collection3 = _database.GetCollection<BsonDocument>(StationName + "_" + DeviceName + "_TEMP");
                    var builder3 = Builders<BsonDocument>.Filter;
                    var filter3 = builder.Eq("State", "OPEN");
                    var cursor3 = collection3.Find(filter);

                    var count3 = cursor3.CountAsync();

                    if (count3.Result <= 0)
                    { 
                        var collection2 = _database.GetCollection<BsonDocument>(StationName + "_" + DeviceName + "_STATE");
                        var builder2 = Builders<BsonDocument>.Filter;
                        var filter2 = builder.Eq("State", "OPEN") & builder.Eq("CloseDate", "");
                        var update2 = Builders<BsonDocument>.Update.Set("State", "CLOSE").Set("ClosedDate", AccessDate.ToString("yyyy-MM-dd HH:mm:ss")).Set("Updated", DataTimestamp.ToString("yyyy-MM-dd HH:mm:ss"));

                        await collection.UpdateManyAsync(filter, update);
                    }
                }
            });
        }
    }
}
