using DataKeeper.Engine;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;

namespace AstroNET.QueueSchedule
{
    public enum QUEUE_STATUS
    {
        WAITINGSERVER = 0,
        WAITINGSTATION = 1,
        INQUEUE = 2,
        EXECUTING = 3,
        CANCELED = 4,
        EXECUTED = 5,
        ENDTIMEPASSED = 6,
        FAILED = 7,
        UNSAFE = 8
    }

    public enum SENDING_STATUS
    {
        IDLE = 0,
        SENDING = 1,
        COMPLETED = 2,
        FAILED = 3
    }

    public class QueueStatus
    {
        public QUEUE_STATUS queueStatus;
        public SENDING_STATUS sendingStatus;

        [BsonIgnoreIfNullAttribute]
        public DateTime? timeStamp;

        [BsonIgnoreIfNullAttribute]
        public String message;

        public QueueStatus()
        {
        }

        public QueueStatus(QUEUE_STATUS queueStatus, SENDING_STATUS sendingStatus, DateTime? timeStamp = null, String message = null)
        {
            this.queueStatus = queueStatus;
            this.sendingStatus = sendingStatus;
            this.timeStamp = timeStamp;
            this.message = message;
        }
    }

    public class AstroQueue
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        public int sequenceNo;

        public User User { get; set; }
        public Target Target { get; set; }

        public static List<AstroQueue> Clone(List<AstroQueueImpl> astroQueues)
        {
            List<AstroQueue> astroQueueTemps = new List<AstroQueue>();
            AstroQueue temp = null;

            astroQueues.ForEach(q => {
                temp = new AstroQueue();
                temp.Id = q.Id;
                temp.User = q.User;
                temp.Target = q.Target;
                temp.Target.exposedHistory.Clear();
                astroQueueTemps.Add(temp);
            });

            return astroQueueTemps;
        }        
    }

    public class AstroQueueImpl : AstroQueue
    {
        public List<QueueStatus> QueueStatus = new List<QueueStatus>();

        public bool Save()
        {
            return DBQueueEngine.UpdateObject(this);
        }
    }
}
