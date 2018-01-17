using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AstroNET.QueueSchedule
{ 
    public class RaDecDeg
    {
        public double RaDeg;
        public double DecDeg;
    }

    [BsonIgnoreExtraElements]
    public class StarCatalog
    {
        [BsonElement("SeqNum")]
        public int SeqNum;

        [BsonElement("RaDecDeg")]
        public RaDecDeg RaDecDeg;

        [BsonElement("Mag")]
        public double Mag;
    }
}
