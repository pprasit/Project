using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using DataKeeper.Engine.Command;

namespace DataKeeper.Engine
{
    public enum STATIONNAME { NULL, TNO, AIRFORCE, SONGKLA, CHACHOENGSAO, ASTROPARK, NAKHONRATCHASIMA, CHINA, USA, AUSTRIA, ASTROSERVER }

    public enum DEVICENAME
    {
        NULL,
        ASTROPARK_SERVER, //Astro Server
        AIRFORCE_TS700MM, AIRFORCE_ASTROHEVENDOME, AIRFORCE_WEATHER, AIRFORCE_CCTV, AIRFORCE_IMAGING, AIRFORCE_LANOUTLET, AIRFORCE_GPS, AIRFORCE_SQM, AIRFORCE_SEEING, AIRFORCE_ALLSKY, AIRFORCE_CLOUDSENSOR, AIRFORCE_ASTROCLIENT, //Airforce base
        SONGKLA_TS700MM, SONGKLA_ASTROHEVENDOME, SONGKLA_WEATHER, SONGKLA_CCTV, SONGKLA_IMAGING, SONGKLA_LANOUTLET, SONGKLA_GPS, SONGKLA_SQM, SONGKLA_SEEING, SONGKLA_ALLSKY, SONGKLA_CLOUDSENSOR, SONGKLA_ASTROCLIENT, //SONGKLA
        CHACHOENGSAO_TS700MM, CHACHOENGSAO_ASTROHEVENDOME, CHACHOENGSAO_WEATHER, CHACHOENGSAO_CCTV, CHACHOENGSAO_IMAGING, CHACHOENGSAO_LANOUTLET, CHACHOENGSAO_GPS, CHACHOENGSAO_SQM, CHACHOENGSAO_SEEING, CHACHOENGSAO_ALLSKY, CHACHOENGSAO_CLOUDSENSOR, CHACHOENGSAO_ASTROCLIENT, //CHACHOENGSAO
        ASTROPARK_TS700MM, ASTROPARK_ASTROHEVENDOME, ASTROPARK_WEATHER, ASTROPARK_CCTV, ASTROPARK_IMAGING, ASTROPARK_LANOUTLET, ASTROPARK_GPS, ASTROPARK_SQM, ASTROPARK_SEEING, ASTROPARK_ALLSKY, ASTROPARK_CLOUDSENSOR, ASTROPARK_ASTROCLIENT, //ASTROPARK
        NAKHONRATCHASIMA_TS700MM, NAKHONRATCHASIMA_ASTROHEVENDOME, NAKHONRATCHASIMA_WEATHER, NAKHONRATCHASIMA_CCTV, NAKHONRATCHASIMA_IMAGING, NAKHONRATCHASIMA_LANOUTLET, NAKHONRATCHASIMA_GPS, NAKHONRATCHASIMA_SQM, NAKHONRATCHASIMA_SEEING, NAKHONRATCHASIMA_ALLSKY, NAKHONRATCHASIMA_CLOUDSENSOR, NAKHONRATCHASIMA_ASTROCLIENT, //NAKHONRATCHASIMA
        CHINA_TS700MM, CHINA_ASTROHEVENDOME, CHINA_WEATHER, CHINA_CCTV, CHINA_IMAGING, CHINA_LANOUTLET, CHINA_GPS, CHINA_SQM, CHINA_SEEING, CHINA_ALLSKY, CHINA_CLOUDSENSOR, CHINA_ASTROCLIENT, //CHINA
        USA_TS700MM, USA_ASTROHEVENDOME, USA_WEATHER, USA_CCTV, USA_IMAGING, USA_LANOUTLET, USA_GPS, USA_SQM, USA_SEEING, USA_ALLSKY, USA_CLOUDSENSOR, USA_ASTROCLIENT, //USA
        AUSTRIA_TS700MM, AUSTRIA_ASTROHEVENDOME, AUSTRIA_WEATHER, AUSTRIA_CCTV, AUSTRIA_IMAGING, AUSTRIA_LANOUTLET, AUSTRIA_GPS, AUSTRIA_SQM, AUSTRIA_SEEING, AUSTRIA_ALLSKY, AUSTRIA_CLOUDSENSOR, AUSTRIA_ASTROCLIENT //AUSTRIA

    }

    public enum DEVICECATEGORY
    {
        NULL,
        T24TS, T24DIO, T24GPS, T24TEMP, T24DOME, T24CAN, T24DIS, T24WSC, T244K, T24SI, T24MRES, T24I8000,  //2.4 Meter Telescope        
        TS700MM, TS500MM, TS1000MM, ASTROHEVENDOME, WEATHERSTATION, CCTV, IMAGING, LANOUTLET, GPS, SQM, SEEING, ALLSKY, CLOUDSENSOR, ASTROCLIENT,   //Reginal Observatory Telescope
        ASTROSERVER
    }

    public enum ReturnStatus { SUCESSFUL, FAILED, WORKING, ONCALLBACK }
    public enum DATAACTION { INSERT, UPDATE, DELETE, SYNCALL }

    public class DEVICEMAPPER
    {
        public DEVICENAME DeviceName { get; set; }
        public DEVICECATEGORY DeviceCategory { get; set; }
    }

    public class ACKNOWLEDGE
    {
        public String StationName { get; set; }
        public String DeviceName { get; set; }
        public String FieldName { get; set; }
        public String Value { get; set; }
        public String ReviceDateTime { get; set; }
    }

    public class OBSERVATIONSTRUCT
    {
        public Int64 ObservationID { get; set; }
        public Int64 ObservationName { get; set; }
        public String ExecutionTimeStart { get; set; }
        public DateTime? TimeStart { get; set; }
        public DateTime? TimeEnd { get; set; }
        public int? ScriptNumber { get; set; }
        public dynamic CommandName { get; set; }
        public Object[] Parameter { get; set; }
        public String Status { get; set; }
        public String Owner { get; set; }
    }

    public class INFORMATIONSTRUCT
    {
        public STATIONNAME StationName { get; set; }
        public DEVICECATEGORY DeviceCategory { get; set; }
        public DEVICENAME DeviceName { get; set; }
        public dynamic FieldName { get; set; }
        public Object Value { get; set; }
        public DateTime? UpdateTime { get; set; }
        public Boolean IsUpdated { get; set; }
        public ConcurrentDictionary<String, Object> ClientSubscribe { get; set; }
    }

    public class OUTPUTSTRUCT
    {
        public STATIONNAME StationName { get; set; }
        public DEVICECATEGORY DeviceCategory { get; set; }
        public String FieldName { get; set; }
        public String Value { get; set; }
        public String DataType { get; set; }
        public String UpdateTime { get; set; }
    }

    public class DeviceData
    {
        public DEVICECATEGORY DeviceName { get; set; }
        public dynamic FieldName { get; set; }
        public Object Value { get; set; }
        public DateTime UpdateTime { get; set; }
        public Boolean IsUpdated { get; set; }
    }

    public class ReturnKnowType
    {
        public ReturnStatus ReturnType { get; set; }
        public String ReturnMessage { get; set; }
        public DateTime ReturnDateTime { get; set; }
        public Object ReturnValue { get; set; }

        public static ReturnKnowType DefineReturn(ReturnStatus ReturnType, String ReturnMessage)
        {
            ReturnKnowType NewResult = new ReturnKnowType();
            NewResult.ReturnType = ReturnType;
            NewResult.ReturnMessage = ReturnMessage;
            NewResult.ReturnDateTime = DateTime.Now;
            NewResult.ReturnValue = null;

            PrintMessage(ReturnType, ReturnMessage, NewResult);
            return NewResult;
        }

        public static ReturnKnowType DefineReturn(ReturnStatus ReturnType, String ReturnMessage, Object ReturnValue)
        {
            ReturnKnowType NewResult = new ReturnKnowType();
            NewResult.ReturnType = ReturnType;
            NewResult.ReturnMessage = ReturnMessage;
            NewResult.ReturnDateTime = DateTime.Now;
            NewResult.ReturnValue = ReturnValue;

            PrintMessage(ReturnType, ReturnMessage, NewResult);
            return NewResult;
        }

        private static void PrintMessage(ReturnStatus ReturnType, String ReturnMessage, ReturnKnowType NewResult)
        {
            if (ReturnMessage != null && ReturnMessage != "")
            {
                LogType ThisType = LogType.MESSAGE;

                switch (ReturnType)
                {
                    case ReturnStatus.FAILED: ThisType = LogType.ERROR; break;
                    case ReturnStatus.ONCALLBACK: ThisType = LogType.COMMUNICATION; break;
                    case ReturnStatus.SUCESSFUL: ThisType = LogType.MESSAGE; break;
                    case ReturnStatus.WORKING: ThisType = LogType.MESSAGE; break;
                }

                TTCSLog.NewLogInformation(STATIONNAME.ASTROSERVER, NewResult.ReturnDateTime, ReturnMessage, ThisType, null);
            }
        }
    }

    public class ReturnLogInformation
    {
        public ReturnStatus ReturnType { get; set; }
        public String ReturnMessage { get; set; }
        public DateTime ReturnDateTime { get; set; }
        public List<InformationLogs> ReturnValue { get; set; }

        public static ReturnLogInformation DefineReturn(ReturnStatus ReturnType, String ReturnMessage)
        {
            ReturnLogInformation ThisReturn = new ReturnLogInformation();
            ThisReturn.ReturnType = ReturnType;
            ThisReturn.ReturnMessage = ReturnMessage;
            ThisReturn.ReturnDateTime = DateTime.Now;
            ThisReturn.ReturnValue = null;

            return ThisReturn;
        }

        public static ReturnLogInformation DefineReturn(ReturnStatus ReturnType, String ReturnMessage, List<InformationLogs> ReturnValue)
        {
            ReturnLogInformation ThisReturn = new ReturnLogInformation();
            ThisReturn.ReturnType = ReturnType;
            ThisReturn.ReturnMessage = ReturnMessage;
            ThisReturn.ReturnDateTime = DateTime.Now;
            ThisReturn.ReturnValue = ReturnValue;

            return ThisReturn;
        }
    }

    public class ReturnDeviceInformation
    {
        public ReturnStatus ReturnType { get; set; }
        public String ReturnMessage { get; set; }
        public DateTime ReturnDateTime { get; set; }
        public INFORMATIONSTRUCT ReturnValue { get; set; }

        public static ReturnDeviceInformation DefineReturn(ReturnStatus ReturnType, String ReturnMessage)
        {
            ReturnDeviceInformation ThisReturn = new ReturnDeviceInformation();
            ThisReturn.ReturnType = ReturnType;
            ThisReturn.ReturnMessage = ReturnMessage;
            ThisReturn.ReturnDateTime = DateTime.Now;
            ThisReturn.ReturnValue = null;

            PrintMessage(ReturnType, ReturnMessage, ThisReturn);
            return ThisReturn;
        }

        public static ReturnDeviceInformation DefineReturn(ReturnStatus ReturnType, String ReturnMessage, INFORMATIONSTRUCT ReturnValue)
        {
            ReturnDeviceInformation ThisReturn = new ReturnDeviceInformation();
            ThisReturn.ReturnType = ReturnType;
            ThisReturn.ReturnMessage = ReturnMessage;
            ThisReturn.ReturnDateTime = DateTime.Now;
            ThisReturn.ReturnValue = ReturnValue;

            PrintMessage(ReturnType, ReturnMessage, ThisReturn);
            return ThisReturn;
        }

        private static void PrintMessage(ReturnStatus ReturnType, String ReturnMessage, ReturnDeviceInformation NewResult)
        {
            if (ReturnMessage != null && ReturnMessage != "")
            {
                LogType ThisType = LogType.MESSAGE;

                switch (ReturnType)
                {
                    case ReturnStatus.FAILED: ThisType = LogType.ERROR; break;
                    case ReturnStatus.ONCALLBACK: ThisType = LogType.COMMUNICATION; break;
                    case ReturnStatus.SUCESSFUL: ThisType = LogType.MESSAGE; break;
                    case ReturnStatus.WORKING: ThisType = LogType.MESSAGE; break;
                }

                TTCSLog.NewLogInformation(STATIONNAME.ASTROSERVER, NewResult.ReturnDateTime, ReturnMessage, ThisType, null);
            }
        }
    }
}
