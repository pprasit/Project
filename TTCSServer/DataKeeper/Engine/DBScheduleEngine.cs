﻿using System;
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
            _database = _client.GetDatabase("STATION_DATA_V2");            
        }

        public static Boolean DropSchedule(STATIONNAME ScriptStationName)
        {
            if (_database == null)
                return false;

            _database.DropCollection(ScriptStationName + "_SCHEDULE");

            return true;
        }

        public static String InsertSchedule(ScriptStructureNew Script)
        {
            if (_database == null)
                return null;

            var collection = _database.GetCollection<BsonDocument>(Script.StationName + "_SCHEDULE");
            var builder = Builders<BsonDocument>.Filter;
            var filter = builder.Eq("ScriptState", SCRIPTSTATE.EXECUTING.ToString());
            var update = Builders<BsonDocument>.Update.Set("ScriptState", SCRIPTSTATE.FAILED.ToString());

            collection.UpdateMany(filter, update);

            var document = new BsonDocument
            {
                { "ScriptID", Script.ScriptID },
                { "BlockID", Script.BlockID },
                { "Life", Script.Life },
                { "StationName", Script.StationName },
                { "DeviceName", Script.DeviceName },
                { "CommandName", Script.CommandName },
                { "Parameters", String.Join(",", Script.Parameters) },
                { "ScriptState", Script.ScriptState },
                { "ExecutionTimeStart", Script.ExecutionTimeStart },
                { "ExecutionTimeEnd", Script.ExecutionTimeEnd },
                { "Owner", Script.Owner },
                { "MustResent", Script.MustResent }
            };

            collection = _database.GetCollection<BsonDocument>(Script.StationName + "_SCHEDULE");
            collection.InsertOne(document);            

            return document["_id"].ToString();
        }

        public static Boolean UpdateSchedule(ScriptStructureNew Script)
        {
            if (_database == null)
                return false;

            var collection = _database.GetCollection<BsonDocument>(Script.StationName + "_SCHEDULE");
            var builder = Builders<BsonDocument>.Filter;
            var filter = builder.Eq("ScriptState", SCRIPTSTATE.EXECUTING.ToString());
            var update = Builders<BsonDocument>.Update.Set("ScriptState", SCRIPTSTATE.FAILED.ToString());

            collection.UpdateMany(filter, update);

            collection = _database.GetCollection<BsonDocument>(Script.StationName + "_SCHEDULE");
            builder = Builders<BsonDocument>.Filter;
            filter = builder.Eq("StationName", Script.StationName) & builder.Eq("BlockID", Script.BlockID) & builder.Eq("ScriptID", Script.ScriptID);
            update = Builders<BsonDocument>.Update.Set("ScriptState", Script.ScriptState).Set("MustResent", Script.MustResent);

            collection.UpdateMany(filter, update);

            return true;            
        }

        public static String GetId(ScriptStructureNew Script)
        {
            if (_database == null)
                return null;

            var collection = _database.GetCollection<BsonDocument>(Script.StationName + "_SCHEDULE");
            var builder = Builders<BsonDocument>.Filter;
            var filter = builder.Eq("ScriptID", Script.ScriptID) & builder.Eq("BlockID", Script.BlockID);

            var document = collection.Find(filter).Limit(1).ToList();
            List<BsonDocument> value = document;
            if (value.Count > 0)
                return value[0]["_id"].ToString();
            else
                return null;             
        }

        public static void InsertData(String DataId, STATIONNAME StationName, DEVICECATEGORY DeviceCategory, DEVICENAME DeviceName, String FieldName, Object Value, long DateTimeUTC)
        {
            if (_database == null)
                return;

            var document = new BsonDocument
            {
                { "DataId", DataId },
                { "DeviceCategory", DeviceCategory.ToString() },
                { "DeviceName", DeviceName.ToString() },
                { "FieldName", FieldName },
                { "Value", Value.ToString() },
                { "DataType", Value.GetType().ToString() },
                { "DateTimeUTC", DateTimeUTC }
            };

            Task DatabaseTask = Task.Run(() =>
            {
                var collection = _database.GetCollection<BsonDocument>(StationName.ToString() + "_" + DeviceCategory.ToString());
                collection.InsertOne(document);
            });
        }

        public static void InsertFITSData(String BlockID, STATIONNAME StationName, String FileName, long DateTimeUTC, long ReceivedUTC)
        {
            if (_database == null)
                return;

            var document = new BsonDocument
            {
                { "BlockID", BlockID },
                { "StationName", StationName.ToString() },
                { "FileName", FileName },                
                { "DateTimeUTC", DateTimeUTC },
                { "ReceivedUTC", ReceivedUTC }
            };

            Task DatabaseTask = Task.Run(() =>
            {
                var collection = _database.GetCollection<BsonDocument>("FITS");
                collection.InsertOne(document);
            });
        }
    }
}