using DataKeeper.Engine;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;

namespace AstroNET.QueueSchedule
{
    public enum FILTER_MODE
    {
        SORT = 1, //R R R G G G B B B
        SEQUENCE = 2, // R G B   R G B
        SWITCH = 3 // R        R        R       G      G     G
    }

    public enum OBJECT_TYPE
    {
        DEEPSKY = 1,
        PLANET = 2
    }

    public class Target
    {
        public STATIONNAME StationName;
        public String RA;
        public String DEC;

        public String name;
        public OBJECT_TYPE objectType;

        public String code;

        public double cadentInterval;

        public FILTER_MODE filterMode;
        public List<ExposureInfo> exposureInfo = new List<ExposureInfo>();

        public bool moonAvoid;
        public bool brightnessCheck;
        public bool autoTimeExposure;

        //value of start expose
        public double? maxAirmass;

        public DateTime? airmassDateStart;
        public DateTime? airmassDateEnd;

        public double? dither;

        public bool ignorePA;
        public double? commandPA;

        public List<ExposedHistory> exposedHistory = new List<ExposedHistory>();
    }
}
