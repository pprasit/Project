using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;

namespace DataKeeper.Engine
{
    public class DBScheduleEngine
    {
        protected static IMongoClient _client;
        protected static IMongoDatabase _database;

        public static void ConnectDB()
        {
            _client = new MongoClient("mongodb://192.168.2.215:27017");
            _database = _client.GetDatabase("STATION_DATA_V3");            
        }

        public static Boolean DropSchedule(STATIONNAME ScriptStationName)
        {
            if (_database == null)
                return false;

            _database.DropCollection(ScriptStationName + "_SCHEDULE");

            return true;
        }

        public static Boolean CancleFailSchedule(STATIONNAME StationName)
        {
            if (_database == null)
                return false;

            var collection = _database.GetCollection<BsonDocument>(StationName + "_SCHEDULE");
            var builder = Builders<BsonDocument>.Filter;
            var filter = builder.Eq("ScriptState", SCRIPTSTATE.EXECUTING.ToString());
            var update = Builders<BsonDocument>.Update.Set("ScriptState", SCRIPTSTATE.FAILED.ToString());

            collection.UpdateMany(filter, update);

            return true;
        }

        public static String InsertSchedule(ScriptStructureNew Script)
        {
            var collection = _database.GetCollection<BsonDocument>(Script.StationName + "_SCHEDULE");

            var document = new BsonDocument
            {
                { "ScriptID", Script.ScriptID },
                { "TargetID", Script.TargetID },
                { "BlockID", Script.BlockID },
                { "Life", Script.Life },
                { "StationName", Script.StationName },
                { "DeviceName", Script.DeviceName },
                { "CommandName", Script.CommandName },
                { "Parameters", String.Join(",", Script.Parameters) },
                { "ScriptState", Script.ScriptState },
                { "ExecutionTimeStart", Script.ExecutionTimeStart },
                { "ExecutionTimeEnd", Script.ExecutionTimeEnd },
                { "ActualTimeStart", "" },
                { "ActualTimeEnd", "" },
                { "Owner", Script.Owner },
                { "IsRead", true },
            };

            collection = _database.GetCollection<BsonDocument>(Script.StationName + "_SCHEDULE");
            collection.InsertOne(document);            

            return document["_id"].ToString();
        }

        public static bool IsFoundScheduleByTargetID(String StationName, String TargetID)
        {
            if (_database == null)
                return false;

            var collection = _database.GetCollection<BsonDocument>(StationName + "_SCHEDULE");
            var builder = Builders<BsonDocument>.Filter;
            var filter = builder.Eq("TargetID", TargetID);

            List<BsonDocument> value = collection.Find(filter).Project("{_id:1}").Limit(1).ToList();

            //var document = collection.Find(filter).Limit(1).ToList();
            //List<BsonDocument> value = document;

            if (value.Count > 0)
                return true;
            else
                return false;
        }

        public static Boolean UpdateFailSchedule(STATIONNAME StationName)
        {
            var collection = _database.GetCollection<BsonDocument>(StationName + "_SCHEDULE");
            var builder = Builders<BsonDocument>.Filter;
            var filter = builder.Eq("ScriptState", SCRIPTSTATE.EXECUTING.ToString());
            var update = Builders<BsonDocument>.Update.Set("ScriptState", SCRIPTSTATE.FAILED.ToString()).Set("IsRead", false);

            collection.UpdateMany(filter, update);
            return true;
        }

        public static Boolean UpdateSchedule(ScriptStructureNew Script)
        {
            if (_database == null)
                return false;            

            var collection = _database.GetCollection<BsonDocument>(Script.StationName + "_SCHEDULE");
            var builder = Builders<BsonDocument>.Filter;
            var filter = builder.Eq("StationName", Script.StationName) & builder.Eq("BlockID", Script.BlockID) & builder.Eq("ScriptID", Script.ScriptID);

            var IsRead = false;

            if (Script.ScriptState.ToString() == SCRIPTSTATE.SENDINGTOSTATION.ToString())
            {
                IsRead = true;
            }
            else
            {
                IsRead = false;
            }

            var update = Builders<BsonDocument>.Update.Set("ScriptState", Script.ScriptState).Set("IsRead", IsRead);

            if(Script.ScriptState == SCRIPTSTATE.EXECUTING.ToString())
            {
                update = Builders<BsonDocument>.Update.Set("ScriptState", Script.ScriptState).Set("IsRead", IsRead).Set("ActualTimeStart", Script.ActualTimeStart);
            }
            else if(Script.ScriptState == SCRIPTSTATE.EXECUTED.ToString())
            {
                update = Builders<BsonDocument>.Update.Set("ScriptState", Script.ScriptState).Set("IsRead", IsRead).Set("ActualTimeStart", Script.ActualTimeStart).Set("ActualTimeEnd", Script.ActualTimeEnd);
            }

            while (true)
            {
                try
                {
                    collection.UpdateMany(filter, update);
                    break;
                }
                catch (Exception e)
                {
                    Console.WriteLine("Mongo Connection Error !!");
                }

                Task.Delay(1000);
            }            

            return true;            
        }

        public static String GetId(ScriptStructureNew Script)
        {
            if (_database == null)
                return null;

            var collection = _database.GetCollection<BsonDocument>(Script.StationName + "_SCHEDULE");
            var builder = Builders<BsonDocument>.Filter;
            var filter = builder.Eq("ScriptID", Script.ScriptID) & builder.Eq("BlockID", Script.BlockID);

            List<BsonDocument> value = collection.Find(filter).Project("{_id:1}").Limit(1).ToList();
            
            //var document = collection.Find(filter).Limit(1).ToList();
            //List<BsonDocument> value = document;

            if (value.Count > 0)
                return value[0]["_id"].ToString();
            else
                return null;             
        }

        public static void InsertData(String DataId, STATIONNAME StationName, DEVICECATEGORY DeviceCategory, DEVICENAME DeviceName, String FieldName, Object Value, long DateTimeUTC)
        {
            if (_database == null)
                return;

            /*
            var document = new BsonDocument
            {
                { "DataId", DataId },
                { "DeviceCategory", DeviceCategory.ToString() },
                { "FieldName", FieldName },
                { "Value", Value.ToString() },
                { "DataType", Value.GetType().ToString() },
                { "DateTimeUTC", DateTimeUTC },
                { "Updated", new DateTime(DateTimeUTC) }
            };
            */

            var document = new BsonDocument
            {
                { "Value", Value.ToString() },
                { "DateTimeUTC", DateTimeUTC },
                { "Updated", new DateTime(DateTimeUTC) },
                { "ServerTimeStamp", new DateTime(DateTime.UtcNow.Ticks) }
            };

            Task DatabaseTask = Task.Run(() =>
            {
                for (int i=0; i<= 10; i++)
                {
                    try
                    {
                        var collection = _database.GetCollection<BsonDocument>(StationName.ToString() + "_" + FieldName);
                        collection.InsertOne(document);
                        break;
                    }
                    catch(Exception e)
                    {
                        Console.WriteLine("Mongo Connection Error !!");
                    }

                    Task.Delay(1000);
                }           
            });
        }

        public static void InsertFITSData(String TargetID, String BlockID, STATIONNAME StationName, String FileName, long DateTimeUTC, long ReceivedUTC)
        {
            if (_database == null)
                return;

            var document = new BsonDocument
            {
                { "TargetID", TargetID },
                { "BlockID", BlockID },
                { "StationName", StationName.ToString() },
                { "FileName", FileName },                
                { "DateTimeUTC", DateTimeUTC },
                { "ReceivedUTC", ReceivedUTC }
            };

            Task DatabaseTask = Task.Run(() =>
            {
                while (true)
                {
                    try
                    {
                        var collection = _database.GetCollection<BsonDocument>("FITS");
                        collection.InsertOne(document);
                        break;
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Mongo Connection Error !!");
                    }

                    Task.Delay(1000);
                }
            });
        }
    }
}
