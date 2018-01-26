using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AstroNET.QueueSchedule;
using DataKeeper.Engine.QueueSchedule;
using MongoDB.Bson;
using MongoDB.Driver;

namespace DataKeeper.Engine
{
    public class DBQueueEngine
    {
        protected static IMongoClient _client;
        protected static IMongoDatabase _database;

        public static void ConnectDB()
        {
            _client = DBEngine._client;
            _database = DBEngine._database;
        }

        public static AstroQueueImpl FindById(STATIONNAME stationName, String Id)
        {
            if (_database == null) return null;

            var collection = _database.GetCollection<AstroQueueImpl>("QUEUES");

            var query = collection.AsQueryable()
                 .Where(x => x.Id == Id).First();

            return query;
        }

        public static IQueryable<AstroQueueImpl> Find(QUEUE_STATUS queueStatus, SENDING_STATUS sendingStatus)
        {
            if (_database == null) return null;

            var collection = _database.GetCollection<AstroQueueImpl>("QUEUES");

            var query = collection.AsQueryable()
                .Where(e => e.QueueStatus.Where(
                    a => a.queueStatus == queueStatus && 
                    a.sendingStatus == sendingStatus && 
                    a.timeStamp == null)
                .Any()); // OK
                //.Where(e => e.QueueStatus.Any(t => t.status == TARGET_STATUS.WAITINGSERVER)); // OK
                //.Where(e => e.QueueStatus.Where(a => a.status == TARGET_STATUS.WAITINGSERVER).Any()); // OK                

            return query;
        }

        public static IQueryable<AstroQueueImpl> FindNE(QUEUE_STATUS queueStatus)
        {
            if (_database == null) return null;

            var collection = _database.GetCollection<AstroQueueImpl>("QUEUES");

            var query = collection.AsQueryable()
                .Where(e => e.QueueStatus.Where(
                    a => a.queueStatus == queueStatus)
                .Any()); // OK
                         //.Where(e => e.QueueStatus.Any(t => t.status == TARGET_STATUS.WAITINGSERVER)); // OK
                         //.Where(e => e.QueueStatus.Where(a => a.status == TARGET_STATUS.WAITINGSERVER).Any()); // OK                

            return query;
        }

        public static QueueStatus FindLastestStatus(String Id)
        {
            if (_database == null) return null;

            var collection = _database.GetCollection<AstroQueueImpl>("QUEUES");

            var query = collection.AsQueryable()
                .Where(x => x.Id == Id)
                .OrderByDescending(a => a.QueueStatus).First()
                .QueueStatus.OrderByDescending(x => x.timeStamp).First();     

            return query;
        }

        public static bool UpdateObject(AstroQueueImpl astroQueue)
        {
            if (_database == null)
                return false;

            try
            {
                var collection = _database.GetCollection<AstroQueueImpl>("QUEUES");
                var filter = Builders<AstroQueueImpl>.Filter.Eq("_id", ObjectId.Parse(astroQueue.Id));
                var Doc = collection.ReplaceOne(filter, astroQueue);
                return true;
            }
            catch { }

            return false;
        }
    }
}
