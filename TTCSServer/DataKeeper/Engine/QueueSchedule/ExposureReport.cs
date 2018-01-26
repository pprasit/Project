using AstroNET.QueueSchedule;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataKeeper.Engine.QueueSchedule
{
    public class ExposureReport
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string AstroQueueId { get; set; }

        public ExposedHistory exposedHistory;

        public SENDING_STATUS sendingStatus;

        public ExposureReport(string AstroQueueId, ExposedHistory exposedHistory, SENDING_STATUS sendingStatus)
        {
            this.AstroQueueId = AstroQueueId;
            this.exposedHistory = exposedHistory;
            this.sendingStatus = sendingStatus;
        }
    }
}
