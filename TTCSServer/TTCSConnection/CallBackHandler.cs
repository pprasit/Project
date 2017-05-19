using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using DataKeeper.Engine;

namespace TTCSConnection
{
    public class SiteConnection
    {
        public ServerCallBack SiteCallBack { get; set; }
        public STATIONNAME StationName { get; set; }
        public String SiteSessionID { get; set; }
    }    

    public class InterfaceConnection
    {
        public ServerCallBack SiteCallBack { get; set; }
        public String InterfaceName { get; set; }
        public String InterfaceSessionID { get; set; }
    }

    public static class CallBackHandler
    {
        public static List<InterfaceConnection> InterfaceConnectionList = new List<InterfaceConnection>();
        public static List<SiteConnection> SiteConnectionList = new List<SiteConnection>();

        #region Site Connection

        public static void AddSiteConnection(STATIONNAME StationName, String SiteSessionID, ServerCallBack SiteCallBack)
        {
            SiteConnection NewSiteConnection = new SiteConnection();
            NewSiteConnection.StationName = StationName;
            NewSiteConnection.SiteSessionID = SiteSessionID;
            NewSiteConnection.SiteCallBack = SiteCallBack;

            SiteConnectionList.Add(NewSiteConnection);
        }

        public static ReturnKnowType RemoveSiteConnection(String SessionID)
        {
            try
            {
                SiteConnectionList.RemoveAll(Item => Item.SiteSessionID == SessionID);
                return ReturnKnowType.DefineReturn(ReturnStatus.SUCESSFUL, null);
            }
            catch (Exception e)
            {
                return ReturnKnowType.DefineReturn(ReturnStatus.FAILED, "Failed to remove site connection see. (" + e.Message + ")");
            }
        }

        #endregion

        public static ReturnKnowType AddInterfaceConnection(String InterfaceName, String InterfaceSessionID, ServerCallBack SiteCallBack)
        {
            try
            {
                InterfaceConnection NewInterfaceConnection = new InterfaceConnection();
                NewInterfaceConnection.InterfaceName = InterfaceName;
                NewInterfaceConnection.InterfaceSessionID = InterfaceSessionID;
                NewInterfaceConnection.SiteCallBack = SiteCallBack;

                InterfaceConnectionList.Add(NewInterfaceConnection);

                return ReturnKnowType.DefineReturn(ReturnStatus.SUCESSFUL, null);
            }
            catch (Exception e)
            {
                return ReturnKnowType.DefineReturn(ReturnStatus.FAILED, "Failed to add interface connection see. (" + e.Message + ")");
            }
        }

        public static ReturnKnowType RemoveInterfaceConnection(String InterfaceSessionID)
        {
            try
            {
                InterfaceConnectionList.RemoveAll(Item => Item.InterfaceSessionID == InterfaceSessionID);
                return ReturnKnowType.DefineReturn(ReturnStatus.SUCESSFUL, null);
            }
            catch (Exception e)
            {
                return ReturnKnowType.DefineReturn(ReturnStatus.FAILED, "Failed to remove interface connection see. (" + e.Message + ")");
            }
        }
    }
}
