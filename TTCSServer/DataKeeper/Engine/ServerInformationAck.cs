using DataKeeper.Engine.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DataKeeper.Engine
{
    public static class ServerInformationAck
    {
        public static void ReturnNTPAckToStation(STATIONNAME StationName, DEVICENAME DeviceName, String FieldName, DateTime TimeStamp, Object ServerCallBackObject)
        {
            Task CallBackTask = Task.Run(() =>
            {
                try
                {
                    MethodInfo MInfo = ServerCallBackObject.GetType().GetMethod("OnInformationSync");
                    MInfo.Invoke(ServerCallBackObject, new Object[] { StationName, DeviceName, FieldName, TimeStamp });
                }
                catch (Exception e)
                {

                }
            });
        }
    }
}
