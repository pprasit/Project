using DataKeeper.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TTCSConnection
{
    public class SubscribeStructure
    {
        public enum SubscribeType { Site, Device, CommandName, Error }
        public SubscribeType Type { get; set; }
        public STATIONNAME StationName { get; set; }
        public DEVICECATEGORY DeviceName { get; set; }
        public String CommandName { get; set; }
        public List<ClientSubscription> ClientList { get; set; }
    }

    public class ClientSubscription
    {
        public String ClientSessionID { get; set; }
        public ServerCallBack ClientCallBack { get; set; }
    }

    public static class SubscribeHandler
    {
        private static List<SubscribeStructure> SubscribeList = new List<SubscribeStructure>();

        public static ReturnKnowType SubscribeByCommmanName(ServerCallBack ClientCallBack, String ClientSessionID, STATIONNAME StationName, DEVICECATEGORY DeviceName, String CommandName)
        {
            try
            {
                SubscribeStructure.SubscribeType SubscribeType = GetSubscribeType(StationName, DeviceName, CommandName);
                DuplicationSubscribeChecking(SubscribeType, ClientSessionID, StationName, DeviceName, CommandName);

                SubscribeStructure ThisSubscribe = SubscribeList.FirstOrDefault(Item => Item.StationName == StationName && Item.DeviceName == DeviceName && Item.CommandName == CommandName);

                if (ThisSubscribe == null)
                {
                    ClientSubscription ThisClient = new ClientSubscription();
                    ThisClient.ClientCallBack = ClientCallBack;
                    ThisClient.ClientSessionID = ClientSessionID;

                    ThisSubscribe = new SubscribeStructure();
                    ThisSubscribe.StationName = StationName;
                    ThisSubscribe.DeviceName = DeviceName;
                    ThisSubscribe.CommandName = CommandName;
                    ThisSubscribe.Type = SubscribeType;

                    ThisSubscribe.ClientList = new List<ClientSubscription>();
                    ThisSubscribe.ClientList.Add(ThisClient);
                }
                else
                {
                    ClientSubscription ThisClient = ThisSubscribe.ClientList.FirstOrDefault(Item => Item.ClientSessionID == ClientSessionID);

                    if (ThisClient == null)
                    {
                        ThisClient = new ClientSubscription();
                        ThisClient.ClientCallBack = ClientCallBack;
                        ThisClient.ClientSessionID = ClientSessionID;

                        ThisSubscribe.ClientList.Add(ThisClient);
                    }
                    else
                        return ReturnKnowType.DefineReturn(ReturnStatus.SUCESSFUL, "Your request is already exist in subscribe list. Please check.");
                }

                return ReturnKnowType.DefineReturn(ReturnStatus.SUCESSFUL, null);
            }
            catch(Exception e)
            {
                return ReturnKnowType.DefineReturn(ReturnStatus.FAILED, "Failed to subscribe information see. (" + e.Message + ")");
            }
        }

        public static void ReturnSubscribe(STATIONNAME _StationName, DEVICECATEGORY _DeviceName, String _CommandName, Object _Value, DateTime _UpdateTime)
        {
            SubscribeStructure ThisSubscribe = SubscribeList.FirstOrDefault(Item => Item.StationName == _StationName && Item.DeviceName == _DeviceName && Item.CommandName == _CommandName);

            for(int i = 0; i < SubscribeList.Count; i++)
            {
                //if(SubscribeList[i].StationName == _StationName && SubscribeList[i].DeviceName == DeviceName.NULL && SubscribeList[i].CommandName == null)
            }
        }

        private static void DuplicationSubscribeChecking(SubscribeStructure.SubscribeType Type, String ClientSessionID, STATIONNAME StationName, DEVICECATEGORY DeviceName, String CommandName)
        {
            if (Type == SubscribeStructure.SubscribeType.Device)
            {
                List<SubscribeStructure> ThisSubscribe = SubscribeList.Where(Item => Item.StationName == StationName && Item.DeviceName == DeviceName).ToList();

                for (int i = 0; i < ThisSubscribe.Count; i++)
                    ThisSubscribe[i].ClientList.RemoveAll(Item => Item.ClientSessionID == ClientSessionID);
            }
            else if (Type == SubscribeStructure.SubscribeType.Site)
            {
                List<SubscribeStructure> ThisSubscribe = SubscribeList.Where(Item => Item.StationName == StationName).ToList();

                for (int i = 0; i < ThisSubscribe.Count; i++)
                    ThisSubscribe[i].ClientList.RemoveAll(Item => Item.ClientSessionID == ClientSessionID);
            }
        }

        private static SubscribeStructure.SubscribeType GetSubscribeType(STATIONNAME _StationName, DEVICECATEGORY _DeviceName, String _CommandName)
        {
            if (_StationName != STATIONNAME.NULL && _DeviceName != DEVICECATEGORY.NULL && _CommandName != null)
                return SubscribeStructure.SubscribeType.CommandName;
            else if(_StationName != STATIONNAME.NULL && _DeviceName != DEVICECATEGORY.NULL && _CommandName == null)
                return SubscribeStructure.SubscribeType.Device;
            else if (_StationName != STATIONNAME.NULL && _DeviceName == DEVICECATEGORY.NULL && _CommandName == null)
                return SubscribeStructure.SubscribeType.Site;
            else
                return SubscribeStructure.SubscribeType.Error;
        }
    }
}
