using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Collections.Concurrent;
using DataKeeper.Engine;
using System.Web.Script.Serialization;
using System.Windows.Forms;
using System.Reflection;
using DataKeeper.Engine.Command;
using Fleck;
using System.Security.Cryptography.X509Certificates;

namespace DataKeeper.Interface
{
    public class SubscribeStructure
    {
        public STATIONNAME StationName { get; set; }
        public String DeviceCategoryStr { get; set; }
        public DEVICECATEGORY DeviceCategory { get; set; }
        public DEVICENAME DeviceName { get; set; }
        public dynamic FieldName { get; set; }
        public List<String> Filter { get; set; }
    }

    public class ResultStructure
    {
        public String StationName { get; set; }
        public String DeviceNameStr { get; set; }
        public String DeviceName { get; set; }
        public String FieldName { get; set; }
        public String Value { get; set; }
        public String DataType { get; set; }
        public String ParameterName { get; set; }
        public String InformationType { get; set; }
        public String ReturnMessage { get; set; }
        public String ReturnResult { get; set; }
        public String DataTimeStamp { get; set; }
    }

    public class WSConnection
    {
        public IWebSocketConnection WSClient { get; set; }
        public String IPAddress { get; set; }
        public int Port { get; set; }
        public DateTime ConnectedTime { get; set; }
        public Boolean IsLogin { get; set; }
        public Boolean IsConnected { get; set; }
        public Boolean BreakTimout { get; set; }
        public String UserName { get; set; }
        public String Password { get; set; }
        public List<String> Messages { get; set; }
        public ConcurrentDictionary<String, SubscribeStructure> SubscribeList { get; set; }

        public void CreateConnectionTimeout()
        {
            Task TTCSTask = Task.Run(() =>
            {
                int Timeout = 5;
                while (!BreakTimout)
                {
                    if (Timeout == 0)
                        WebSockets.RemoveClient(this);

                    Timeout--;
                    Thread.Sleep(1000);
                }
            });
        }
    }

    public class ConnectionHistory
    {
        public String IPAddress { get; set; }
        public int Counter { get; set; }
        public Boolean IsBlockingStart { get; set; }
    }

    public class WebSockets
    {
        private static ConcurrentDictionary<String, WSConnection> AllConnection = new ConcurrentDictionary<String, WSConnection>();
        private static DataGridView ClientGrid = null;
        private static DataGridView TTCSLogGrid = null;
        private static int VerifyConnectionTimout = 10000;
        private static Object ObjMainPage = null;
        private static List<UserTB> UserList = null;
        private static Int64 HeaderID = 0;
        private static ConcurrentDictionary<String, ConnectionHistory> ConnectionCounter = new ConcurrentDictionary<String, ConnectionHistory>();
        private static List<String> ConnectionBlockList = new List<string>();
        private static String SocketServerAddress = null;

        public static void CreateConnection(DataGridView ClientGrid, Object ObjMainPage, DataGridView TTCSLogGrid, String SocketServerAddress)
        {
            String IPAddressStr = TTCSHelper.GetLocalIPAddress();
            WebSockets.ClientGrid = ClientGrid;
            WebSockets.TTCSLogGrid = TTCSLogGrid;
            WebSockets.ObjMainPage = ObjMainPage;

            UserList = DatabaseSynchronization.GetAllUser();

            FleckLog.Level = LogLevel.Debug;
            AllConnection = new ConcurrentDictionary<String, WSConnection>();
            var server = new WebSocketServer(SocketServerAddress);
            String CerPath = "C:\\Users\\AstroNET\\AppData\\Roaming\\letsencrypt-win-simple\\httpsacme-v01.api.letsencrypt.org\\astronet.narit.or.th-all.pfx";

            server.Certificate = new X509Certificate2(CerPath);
            server.Start(socket =>
            {
                socket.OnOpen = () =>
                {
                    String Key = socket.ConnectionInfo.ClientIpAddress + socket.ConnectionInfo.ClientPort;

                    WSConnection ThisConnection = new WSConnection();
                    ThisConnection.WSClient = socket;
                    ThisConnection.IPAddress = socket.ConnectionInfo.ClientIpAddress;
                    ThisConnection.Port = socket.ConnectionInfo.ClientPort;
                    ThisConnection.ConnectedTime = DateTime.Now;
                    ThisConnection.IsLogin = false;
                    ThisConnection.IsConnected = true;
                    ThisConnection.UserName = null;
                    ThisConnection.Password = null;
                    ThisConnection.Messages = new List<String>();
                    ThisConnection.BreakTimout = false;
                    ThisConnection.CreateConnectionTimeout();
                    ThisConnection.SubscribeList = new ConcurrentDictionary<string, SubscribeStructure>();

                    if (!ConnectionLimit(ThisConnection.IPAddress))
                    {
                        AddConnectionHistory(ThisConnection.IPAddress);
                        OnConnectHandler(ThisConnection);
                        AllConnection.TryAdd(Key, ThisConnection);
                    }
                    else
                    {
                        BlockLoop(ThisConnection.IPAddress);
                        socket.Close();
                    }
                };
                socket.OnClose = () =>
                {
                    WSConnection ThisConnection = AllConnection.FirstOrDefault(Item => Item.Key == socket.ConnectionInfo.ClientIpAddress + socket.ConnectionInfo.ClientPort).Value;
                    RemoveClient(ThisConnection);
                };
                socket.OnMessage = message =>
                {
                    WSConnection ThisConnection = AllConnection.FirstOrDefault(Item => Item.Key == socket.ConnectionInfo.ClientIpAddress + socket.ConnectionInfo.ClientPort).Value;

                    if (ThisConnection != null)
                    {
                        OnMessageHandler(ThisConnection, message);

                        MethodInfo method = ObjMainPage.GetType().GetMethod("RelayMessageToMonitoring");
                        method.Invoke(ObjMainPage, new object[] { message });
                    }
                };
            });
        }

        private static void OnConnectHandler(WSConnection ThisConnection)
        {
            DisplayClient(ThisConnection.IPAddress, ThisConnection.Port, "Connected", DateTime.Now, 1);

            Task MessageTask = Task.Run(() =>
            {
                while (ThisConnection.IsConnected)
                {
                    SetDataGridEditRow(ThisConnection.IPAddress, ThisConnection.Port, ThisConnection.ConnectedTime, null, DateTime.Now);
                    Thread.Sleep(1000);
                }
            });
        }

        private static void OnCloseHanlder(String IPAddress, int Port)
        {
            //SetRemoveingToMonitoring(IPAddress, Port.ToString());
            SetDataGridRemoveRow(IPAddress, Port);
        }

        private static void OnMessageHandler(WSConnection ThisConnection, String Message)
        {
            if (Message == null || Message == "")
                return;

            if (Message.Contains("CloseConnection"))
            {
                ThisConnection.WSClient.Close();
            }
            else if (UserVerification(ThisConnection, Message))
            {
                ThisConnection.BreakTimout = true;
                String[] SplitedInformation = Message.Split(new char[] { ',' });
                SetDataGridEditRow(ThisConnection.IPAddress, ThisConnection.Port, ThisConnection.ConnectedTime, Message, DateTime.Now);

                RemoveConnectionHistory(ThisConnection.IPAddress);

                foreach (String CommandStr in SplitedInformation)
                {
                    String[] SplitedParameter = CommandStr.Split(new char[] { '&' });

                    if (SplitedParameter[0] == "Subscribe")
                    {
                        if (SplitedParameter.Count() == 4 || SplitedParameter.Count() == 5)
                            SubscribeHandler(ThisConnection, SplitedParameter, CommandStr);
                        else
                            ReturnMessage(ThisConnection.WSClient, "", "", "", "", "", "Subscribe", "Invalid format for subscribe method. The format for 'Subscribe' method is 'Subscribe&Station=???&DeviceName=???&CommandName=???'", "Subscribe_Error");
                    }
                    else if (SplitedParameter[0] == "Unsubscribe")
                        Unsubscribe(ThisConnection, SplitedParameter);
                    else if (SplitedParameter[0] == "Get")
                    {
                        if (SplitedParameter.Count() >= 3)
                            GetHandler(ThisConnection, SplitedParameter);
                        else
                            ReturnMessage(ThisConnection.WSClient, "", "", "", "", "", "Subscribe", "Invalid format for get method. The format for 'Subscribe' method is 'Get&Station=???&DeviceName=???&CommandName=???'", "Subscribe_Error");
                    }
                    else if (SplitedParameter[0] == "Set")
                    {
                        if (SplitedParameter.Count() >= 4)
                            SetHandler(ThisConnection, SplitedParameter);
                        else
                            ReturnMessage(ThisConnection.WSClient, "", "", "", "", "", "Set", "Invalid format for set method. The format for 'Set' method is 'Set&Station=???&DeviceName=???&CommandName=???&ParameterName....'", "Set_Error");
                    }
                    else if (SplitedParameter[0] == "Script")
                        ScriptHandler(ThisConnection, SplitedParameter);
                }
            }
            else
                ThisConnection.WSClient.Close();
        }

        private static void BlockLoop(String IPAddress)
        {
            ConnectionHistory ThisConnection = ConnectionCounter.FirstOrDefault(Item => Item.Value.IPAddress == IPAddress && Item.Value.Counter == 10 && !Item.Value.IsBlockingStart).Value;
            if (ThisConnection == null)
                return;

            Task BlockLoops = Task.Run(() =>
            {
                int TimeCounter = 0;
                while (true)
                {
                    if (TimeCounter <= 60)
                    {
                        ThisConnection.IsBlockingStart = true;
                        TimeCounter++;
                    }
                    else
                    {
                        RemoveBlockConnection();
                        break;
                    }

                    Thread.Sleep(1000);
                }
            });
        }

        private static Boolean ConnectionLimit(String IPAddress)
        {
            ConnectionHistory ThisConnection = ConnectionCounter.FirstOrDefault(Item => Item.Value.IPAddress == IPAddress).Value;
            if (ThisConnection != null && ThisConnection.Counter >= 10)
                return true;

            return false;
        }

        private static void AddConnectionHistory(String IPAddress)
        {
            if (ConnectionCounter.FirstOrDefault(Item => Item.Value.IPAddress == IPAddress).Value == null)
            {
                ConnectionHistory CreateHistory = new ConnectionHistory();
                CreateHistory.IPAddress = IPAddress;
                CreateHistory.IsBlockingStart = false;
                CreateHistory.Counter = 1;

                ConnectionCounter.TryAdd(IPAddress, CreateHistory);
            }
        }

        private static void RemoveBlockConnection()
        {
            List<ConnectionHistory> ConnectionList = ConnectionCounter.Values.Where(Item => Item.Counter == 10).ToList();

            foreach (ConnectionHistory ThisConnection in ConnectionList)
            {
                ConnectionHistory TempHistory;
                ConnectionCounter.TryRemove(ThisConnection.IPAddress, out TempHistory);
            }
        }

        private static void RemoveConnectionHistory(String IPAddress)
        {
            List<ConnectionHistory> IPAddressHistoryList = ConnectionCounter.Values.Where(Item => Item.IPAddress == IPAddress).ToList();

            foreach (ConnectionHistory ThisConnection in IPAddressHistoryList)
            {
                ConnectionHistory TempHistory;
                ConnectionCounter.TryRemove(ThisConnection.IPAddress, out TempHistory);
            }
        }

        public static void RemoveClient(WSConnection ThisConnection)
        {
            if (ThisConnection == null)
                return;

            OnCloseHanlder(ThisConnection.IPAddress, ThisConnection.Port);
            ThisConnection.IsConnected = false;
            AllConnection.TryRemove(ThisConnection.IPAddress + ThisConnection.Port.ToString(), out ThisConnection);
        }

        public static List<String> GetMonitoringInformation(String IPAddress, String Port)
        {
            WSConnection ThisConnection = AllConnection.FirstOrDefault(Item => Item.Key == IPAddress + Port).Value;
            return ThisConnection.Messages;
        }

        #region Get Handler

        private static void GetHandler(WSConnection ThisConnection, String[] SplitedCommand)
        {
            STATIONNAME ThisStation = STATIONNAME.NULL;
            DEVICECATEGORY ThisDeviceCategory = DEVICECATEGORY.NULL;
            DEVICENAME ThisDeviceName = DEVICENAME.NULL;
            dynamic ThisFieldName = null;
            List<String> ThisParameter = new List<String>();

            ResultStructure ThisResult = new ResultStructure();
            ThisResult.StationName = "null";
            ThisResult.DeviceName = "null";
            ThisResult.FieldName = "null";
            ThisResult.Value = "null";
            ThisResult.DataType = "null";
            ThisResult.ParameterName = "null";
            ThisResult.InformationType = "Get";
            ThisResult.ReturnMessage = "null";
            ThisResult.ReturnResult = "Get_Error";
            ThisResult.DataTimeStamp = DateTime.Now.ToString();

            foreach (String ThisStr in SplitedCommand)
            {
                if (ThisStr != "Get")
                {
                    String[] SplitedStr = ThisStr.Split(new char[] { '=' });
                    String FieldName = SplitedStr[0];
                    String Value = ThisStr.Split(new char[] { '=' })[1];

                    if (FieldName == "Station")
                        ThisStation = TTCSHelper.StationStrConveter(Value);
                    else if (FieldName == "DeviceName")
                    {
                        ThisDeviceName = TTCSHelper.DeviceNameStrConverter(Value);
                        ThisDeviceCategory = TTCSHelper.ConvertDeviceNameToDeviceCategory(ThisStation, ThisDeviceName);
                    }
                    else if (FieldName == "FieldName")
                        ThisFieldName = TTCSHelper.InformationNameConveter(ThisDeviceCategory, Value);
                    else if (FieldName == "Parameter")
                        ThisParameter.Add(Value);
                }
            }

            if (ThisStation != STATIONNAME.NULL && ThisDeviceCategory != DEVICECATEGORY.NULL && ThisFieldName != null)
            {
                OUTPUTSTRUCT ThisInformation = null;
                if (ThisParameter.Count > 0)
                    ThisInformation = AstroData.GetInformation(ThisStation, ThisDeviceName, ThisFieldName);  //Handler parameter is need to imprement
                else
                    ThisInformation = AstroData.GetInformation(ThisStation, ThisDeviceName, ThisFieldName);

                if (ThisInformation != null)
                {
                    ThisResult.StationName = ThisStation.ToString();
                    ThisResult.DeviceName = ThisDeviceName.ToString();
                    ThisResult.DataType = ThisInformation.DataType;
                    ThisResult.FieldName = ThisInformation.FieldName;
                    ThisResult.Value = ThisInformation.Value;
                    ThisResult.ParameterName = ThisParameter.Count > 0 ? String.Join(",", ThisParameter) : "null";
                    ThisResult.ReturnResult = "Get_Successful";
                }
                else
                {
                    ThisResult.ReturnResult = "Get_Error";
                    ThisResult.ReturnMessage = "Can not get field name '" + ThisFieldName.ToString() + "'. Because The Field name could not be found in information pool.";
                }
            }
            else
            {
                ThisResult.ReturnResult = "Get_Error";

                if (ThisStation == STATIONNAME.NULL)
                    ThisResult.ReturnMessage = "Can not get field name '" + ThisFieldName.ToString() + "'. Because invalid Station name.";
                else if (ThisDeviceCategory == DEVICECATEGORY.NULL)
                    ThisResult.ReturnMessage = "Can not get field name '" + ThisFieldName.ToString() + "'. Because invalid Device name.";
                else if (ThisFieldName == null)
                    ThisResult.ReturnMessage = "Can not get field name '" + ThisFieldName.ToString() + "'. Because invalid Field name.";
            }

            var ReturningJson = new JavaScriptSerializer().Serialize(ThisResult);
            SendMessage(ThisConnection.WSClient, ReturningJson);
        }

        #endregion

        #region Script

        private static void ScriptHandler(WSConnection ThisConnection, String[] SplitedParameter)
        {
            List<String> ParameterStrs = new List<String>();

            ScriptStructure ThisScript = new ScriptStructure();
            ThisScript.BlockName = null;
            ThisScript.StationName = STATIONNAME.NULL;
            ThisScript.ExecutionTimeStart = null;
            ThisScript.ExecutionTimeEnd = null;
            ThisScript.BlockID = null;
            ThisScript.ExecutionNumber = null;
            ThisScript.DeviceName = DEVICENAME.NULL;
            ThisScript.DeviceCategory = DEVICECATEGORY.NULL;
            ThisScript.CommandName = null;
            ThisScript.Owner = null;
            ThisScript.DelayTime = null;
            ThisScript.Parameter = new List<Object>();
            ThisScript.ScriptState = SCRIPTSTATE.WAITINGSERVER;

            foreach (String ThisStr in SplitedParameter)
            {
                if (ThisStr != "Script")
                {
                    String[] SplitedStr = ThisStr.Split(new char[] { '=' });
                    String FieldName = SplitedStr[0];
                    String Value = ThisStr.Split(new char[] { '=' })[1];

                    if (SplitedStr.Count() <= 1)
                    {
                        ReturnScriptResult(ThisConnection, ThisScript.BlockName, "", ThisScript.ExecutionNumber.ToString(), "", "Invalid script structure at -> " + ThisStr, "Script_Error");
                        return;
                    }

                    if (FieldName == "BlockName")
                    {
                        ThisScript.BlockName = Value;
                    }
                    else if (FieldName == "ExecutionTimeStart")
                    {
                        DateTime ExecutionTimeStartTemp;
                        if (DateTime.TryParse(Value, out ExecutionTimeStartTemp))
                            ThisScript.ExecutionTimeStart = ExecutionTimeStartTemp;
                        else
                        {
                            ReturnScriptResult(ThisConnection, ThisScript.BlockName, "", ThisScript.ExecutionNumber.ToString(), "", "Invalid datetime format in script at -> " + ThisStr, "Script_Error");
                            return;
                        }
                    }
                    else if (FieldName == "ExecutionTimeEnd")
                    {
                        DateTime ExecutionTimeEndTemp;
                        if (DateTime.TryParse(Value, out ExecutionTimeEndTemp))
                            ThisScript.ExecutionTimeEnd = ExecutionTimeEndTemp;
                        else
                        {
                            ReturnScriptResult(ThisConnection, ThisScript.BlockName, "", ThisScript.ExecutionNumber.ToString(), "", "Invalid datetime format in script at -> " + ThisStr, "Script_Error");
                            return;
                        }
                    }
                    else if (FieldName == "StationName")
                    {
                        ThisScript.StationName = TTCSHelper.StationStrConveter(Value);
                    }
                    else if (FieldName == "CommandCounter")
                    {
                        int CommandCounterTemp;
                        if (int.TryParse(Value, out CommandCounterTemp))
                            ThisScript.CommandCounter = CommandCounterTemp;
                        else
                        {
                            ReturnScriptResult(ThisConnection, ThisScript.BlockName, "", ThisScript.ExecutionNumber.ToString(), "", "Invalid command counter see -> " + ThisStr, "Script_Error");
                            return;
                        }
                    }
                    else if (FieldName == "DeviceName")
                    {
                        ThisScript.DeviceName = GetDeviceNameFromStr(Value);
                        ThisScript.DeviceCategory = AstroData.GetDeviceCategoryByDeviceName(ThisScript.StationName, ThisScript.DeviceName);
                    }
                    else if (FieldName == "CommandName")
                    {
                        ThisScript.CommandName = Value;
                    }
                    else if (FieldName == "ExecutionNumber")
                    {
                        int ExecutionNumberTemp;
                        if (int.TryParse(Value, out ExecutionNumberTemp))
                            ThisScript.ExecutionNumber = ExecutionNumberTemp;
                        else
                        {
                            ReturnScriptResult(ThisConnection, ThisScript.BlockName, "", ThisScript.ExecutionNumber.ToString(), "", "Invalid executeion number see -> " + ThisStr, "Script_Error");
                            return;
                        }
                    }
                    else if (FieldName == "Owner")
                    {
                        ThisScript.Owner = Value;
                    }
                    else if (FieldName == "DelayTime")
                    {
                        Double DelayTimeTemp = 2;
                        if (Double.TryParse(Value, out DelayTimeTemp))
                            ThisScript.DelayTime = DelayTimeTemp;
                        else
                        {
                            ReturnScriptResult(ThisConnection, ThisScript.BlockName, "", ThisScript.ExecutionNumber.ToString(), "", "Invalid delay time see -> " + ThisStr, "Script_Error");
                            return;
                        }
                    }
                    else if (FieldName == "ParameterName")
                    {
                        ParameterStrs.Add(Value);
                    }
                }
            }

            ReturnKnowType CommandResult = CommandDefinition.VerifyCommand(ThisScript.StationName, ThisScript.DeviceCategory, ThisScript.CommandName, ParameterStrs.ToArray());

            if (CommandResult.ReturnType == ReturnStatus.FAILED)
                ReturnScriptResult(ThisConnection, ThisScript.BlockName, "", ThisScript.ExecutionNumber.ToString(), ThisScript.CommandName.ToString(), "Invalid command name in script. ", "Script_Error");
            else
            {
                dynamic CommandName = TTCSHelper.CommandNameConverter(ThisScript.DeviceCategory, ThisScript.CommandName);
                Object[] Values = CommandDefinition.ValueConvertion(ParameterStrs.ToArray());
                ThisScript.Parameter = Values.ToList();

                ScriptManager.NewScriptFromSocket(ThisScript, ThisConnection);
            }
        }

        public static void ReturnScriptResult(WSConnection ThisConnection, String BlockName, String BlockID, String ExecutionNumber, String CommandName, String Message, String ScriptState)
        {
            ResultStructure ThisResult = new ResultStructure();
            ThisResult.StationName = BlockName + "_" + ExecutionNumber;
            ThisResult.DeviceName = BlockID;
            ThisResult.FieldName = CommandName;
            ThisResult.Value = "null";
            ThisResult.DataType = "null";
            ThisResult.ParameterName = "null";
            ThisResult.ReturnMessage = Message;
            ThisResult.InformationType = "Script";
            ThisResult.ReturnResult = ScriptState;
            ThisResult.DataTimeStamp = DateTime.Now.ToString();

            var ReturningJson = new JavaScriptSerializer().Serialize(ThisResult);
            SendMessage(ThisConnection.WSClient, ReturningJson);
        }

        #endregion

        #region Authentication Handler

        private static void GetLoginInfo(String Message, out String UserName, out String Password)
        {
            String FinalUserName = null;
            String FinalPassword = null;

            if (Message.Contains("Authentication"))
            {
                try
                {
                    String[] LoginPart = Message.Split(new char[] { ',' });

                    if (LoginPart.Count() > 0)
                    {
                        String[] LoginInfo = LoginPart[0].Split(new char[] { '&' });

                        for (int i = 1; i < LoginInfo.Count(); i++)
                        {
                            String[] FinalSplit = LoginInfo[i].Split(new char[] { '=' });

                            if (FinalSplit[0].ToLower() == "username")
                                FinalUserName = FinalSplit[1];
                            else if (FinalSplit[0].ToLower() == "password")
                                FinalPassword = FinalSplit[1];
                        }
                    }
                }
                catch
                {
                    FinalUserName = null;
                    FinalPassword = null;
                }
            }

            UserName = FinalUserName;
            Password = FinalPassword;
        }

        private static Boolean UserVerification(WSConnection ThisConnection, String Message)
        {
            if (!ThisConnection.IsLogin)
            {
                String UserName = null;
                String Password = null;
                GetLoginInfo(Message, out UserName, out Password);

                if (UserName == null || Password == null)
                {
                    ReturnMessage(ThisConnection.WSClient, "", "", "", "", "", "Authentication", "Invalid Username or Password. Username or password can not empty.", "Authentication_Error");
                    return false;
                }
                else
                {
                    UserTB ThisUser = UserList.FirstOrDefault(Item => Item.UserLoginName == UserName && Item.UserLoginPassword == Password);
                    if (ThisUser != null)
                    {
                        ThisConnection.IsLogin = true;
                        ThisConnection.UserName = UserName;
                        ThisConnection.Password = Password;

                        ReturnMessage(ThisConnection.WSClient, "", "", "", "", "", "Authentication", "Login successful.", "Authentication_Successful");
                        return true;
                    }
                }
            }
            else if (ThisConnection.IsLogin)
                return true;

            ReturnMessage(ThisConnection.WSClient, "", "", "", "", "", "Authentication", "Invalid Username or Password. Please check.", "Authentication_Error");
            return false;
        }

        #endregion

        #region Subscribe Handler

        private static void Unsubscribe(WSConnection ThisConnection, String[] SplitedCommand)
        {
            STATIONNAME ThisStation = STATIONNAME.NULL;
            DEVICECATEGORY ThisDeviceCategory = DEVICECATEGORY.NULL;
            DEVICENAME ThisDeviceName = DEVICENAME.NULL;
            dynamic ThisFieldName = null;

            String ThisStationNameStr = null;
            String ThisDeviceNameStr = null;
            String ThisFieldNameStr = null;

            ThisStationNameStr = SplitedCommand[1].Split(new char[] { '=' })[1];
            ThisStation = TTCSHelper.StationStrConveter(ThisStationNameStr);
            ThisDeviceNameStr = SplitedCommand[2].Split(new char[] { '=' })[1];

            ThisDeviceName = GetDeviceNameFromStr(ThisDeviceNameStr);
            ThisDeviceCategory = AstroData.GetDeviceCategoryByDeviceName(ThisStation, ThisDeviceName);
            ThisFieldNameStr = SplitedCommand[3].Split(new char[] { '=' })[1];
            ThisFieldName = TTCSHelper.InformationNameConveter(ThisDeviceCategory, ThisFieldNameStr);

            ResultStructure ThisResult = new ResultStructure();
            ThisResult.StationName = "null";
            ThisResult.DeviceName = "null";
            ThisResult.FieldName = "null";
            ThisResult.Value = "null";
            ThisResult.DataType = "null";
            ThisResult.ParameterName = "null";
            ThisResult.ReturnMessage = "null";
            ThisResult.InformationType = "Unsubscribe";
            ThisResult.ReturnResult = "Unsubscribe_Error";
            ThisResult.ReturnMessage = "Can not Unsubscribe information.";
            ThisResult.DataTimeStamp = DateTime.Now.ToString();

            if (ThisStation != STATIONNAME.NULL && ThisDeviceCategory != DEVICECATEGORY.NULL && ThisFieldName != null)
            {
                String Key = ThisStation.ToString() + ":" + ThisDeviceNameStr.ToString() + ":" + ThisFieldName + ":" + ThisConnection.IPAddress + ":" + ThisConnection.Port.ToString();
                SubscribeStructure TempSubscribe = null;
                ThisConnection.SubscribeList.TryRemove(Key, out TempSubscribe);

                ThisResult.StationName = ThisStationNameStr;
                ThisResult.DeviceName = ThisDeviceNameStr;
                ThisResult.FieldName = ThisFieldNameStr;
                ThisResult.ReturnMessage = "Unsubscribe field name '" + ThisFieldName + "' successful";
                ThisResult.ReturnResult = "Unsubscribe_Successful";
            }
            else if (ThisStation == STATIONNAME.NULL)
                ThisResult.ReturnMessage = "Can not Unsubscribe field name '" + ThisFieldName + "'. Because invalid StationName.";
            else if (ThisDeviceCategory == DEVICECATEGORY.NULL)
            {
                ThisResult = new ResultStructure();
                ThisResult.StationName = ThisStationNameStr;
                ThisResult.DeviceName = ThisDeviceNameStr;
                ThisResult.ReturnMessage = "Can not Unsubscribe field name '" + ThisFieldName + "'. Because invalid DeviceName.";
            }
            else if (ThisFieldName == null)
            {
                ThisResult = new ResultStructure();
                ThisResult.StationName = ThisStationNameStr;
                ThisResult.DeviceName = ThisDeviceNameStr;
                ThisResult.FieldName = ThisFieldNameStr;
                ThisResult.ReturnMessage = "Can not Unsubscribe field name '" + ThisFieldName + "'. Because invalid FieldName.";
            }

            var json = new JavaScriptSerializer().Serialize(ThisResult);
            SendMessage(ThisConnection.WSClient, json);
            //SetReturningToMonitoring(TotalByte, ThisStation == STATIONNAME.NULL ? "Error" : ThisStation.ToString() + " - " + (ThisDeviceCategory == DEVICECATEGORY.NULL ? "Error" : ThisDeviceNameStr.ToString()) + " - " +
            //    ThisFieldName == null ? "Error" : ThisFieldName.ToString(), ThisResult.Value.ToString(), ConditionCase);
        }

        private static void SubscribeHandler(WSConnection ThisConnection, String[] SplitedCommand, String CommandStr)
        {
            STATIONNAME ThisStation = STATIONNAME.NULL;
            DEVICECATEGORY ThisDeviceCategory = DEVICECATEGORY.NULL;
            DEVICENAME ThisDeviceName = DEVICENAME.NULL;
            dynamic ThisFieldName = null;
            Boolean ThisReturningState = false;
            List<String> ThisFilter = new List<String>();

            foreach (String ThisStr in SplitedCommand)
            {
                if (ThisStr != "Subscribe")
                {
                    String[] SplitedStr = ThisStr.Split(new char[] { '=' });
                    String FieldName = SplitedStr[0];
                    String Value = ThisStr.Split(new char[] { '=' })[1];

                    if (FieldName == "Station")
                        ThisStation = TTCSHelper.StationStrConveter(Value);
                    else if (FieldName == "DeviceName")
                    {
                        ThisDeviceName = TTCSHelper.DeviceNameStrConverter(Value);
                        ThisDeviceCategory = TTCSHelper.ConvertDeviceNameToDeviceCategory(ThisStation, ThisDeviceName);
                    }
                    else if (FieldName == "FieldName")
                        ThisFieldName = TTCSHelper.InformationNameConveter(ThisDeviceCategory, Value);
                    else if (FieldName == "ReturningState")
                        Boolean.TryParse(Value, out ThisReturningState);
                    else if (FieldName.Contains("Filter"))
                        ThisFilter.Add(Value);
                }
            }

            ResultStructure ThisResult = new ResultStructure();
            ThisResult.StationName = "null";
            ThisResult.DeviceName = "null";
            ThisResult.FieldName = "null";
            ThisResult.Value = "null";
            ThisResult.DataType = "null";
            ThisResult.ParameterName = "null";
            ThisResult.InformationType = "Subscribe";
            ThisResult.ReturnMessage = "null";
            ThisResult.ReturnResult = "Subscribe_Error";
            ThisResult.DataTimeStamp = DateTime.Now.ToString();

            if (ThisStation != STATIONNAME.NULL && ThisDeviceCategory != DEVICECATEGORY.NULL && ThisFieldName != null)
            {
                String Key = ThisStation.ToString() + ":" + ThisDeviceName.ToString() + ":" + ThisFieldName.ToString() + ":" + ThisConnection.IPAddress + ":" + ThisConnection.Port.ToString();
                SubscribeStructure ThisSubscribe = new SubscribeStructure();
                ThisSubscribe.StationName = ThisStation;
                ThisSubscribe.DeviceCategoryStr = ThisDeviceCategory.ToString();
                ThisSubscribe.DeviceCategory = ThisDeviceCategory;
                ThisSubscribe.DeviceName = ThisDeviceName;
                ThisSubscribe.FieldName = ThisFieldName;
                ThisSubscribe.Filter = ThisFilter;

                ThisConnection.SubscribeList.TryAdd(Key, ThisSubscribe);

                INFORMATIONSTRUCT ThisInformation = AstroData.GetInformationObject(ThisSubscribe.StationName, ThisSubscribe.DeviceName, ThisSubscribe.FieldName);
                ThisResult.StationName = ThisSubscribe.StationName.ToString();
                ThisResult.DeviceName = ThisSubscribe.DeviceName.ToString();
                ThisResult.FieldName = ThisSubscribe.FieldName.ToString();

                if (ThisInformation != null)
                {
                    if (ThisSubscribe.FieldName.ToString() == "ASTROSERVER_ALLSCRIPTBLOCK")
                    {
                        var jsonSerialiser = new JavaScriptSerializer();
                        ScriptTB[] AllScript = (ScriptTB[])ThisInformation.Value;
                        var ValueJson = jsonSerialiser.Serialize(AllScript);

                        ThisResult.Value = ValueJson;
                    }
                    else
                    {
                        if (ThisInformation.Value != null)
                            ThisResult.Value = ThisInformation.Value.ToString();
                        else
                            ThisResult.Value = "";
                    }

                    ThisResult.DataType = ThisInformation.Value == null ? "null" : ThisInformation.Value.GetType().ToString().Replace("System.", "");
                    ThisResult.DataTimeStamp = ThisInformation.UpdateTime.ToString();
                    ThisResult.ReturnResult = "Subscribe_Data";
                }
                else
                {
                    ThisResult.ReturnMessage = "Can not subscribe field name '" + ThisFieldName == null ? "null" : ThisFieldName.ToString() + "'. Because invalid FieldName";
                    ThisResult.ReturnResult = "Subscribe_Error";
                }
            }
            else if (ThisStation == STATIONNAME.NULL)
                ThisResult.ReturnMessage = "Can not subscribe field name '" + ThisFieldName == null ? "null" : ThisFieldName.ToString() + "'. Because invalid StationName.";
            else if (ThisDeviceCategory == DEVICECATEGORY.NULL)
                ThisResult.ReturnMessage = "Can not subscribe field name '" + ThisFieldName == null ? "null" : ThisFieldName.ToString() + "'. Because invalid DeviceName.";
            else if (ThisFieldName == null)
                ThisResult.ReturnMessage = "Can not subscribe field name '" + ThisFieldName == null ? "null" : ThisFieldName.ToString() + "'. Because invalid FieldName.";

            if (ThisReturningState)
            {
                ResultStructure ReturningResult = new ResultStructure();
                ReturningResult.StationName = ThisStation.ToString();
                ReturningResult.DeviceName = ThisDeviceName.ToString();
                ReturningResult.FieldName = ThisFieldName.ToString();
                ReturningResult.Value = CommandStr;
                ReturningResult.DataType = "null";
                ReturningResult.ParameterName = "null";
                ReturningResult.InformationType = "Subscribe";
                ReturningResult.ReturnMessage = "null";
                ReturningResult.ReturnResult = "Subscribe_Successful";
                ReturningResult.DataTimeStamp = DateTime.Now.ToString();

                var ReturningJson = new JavaScriptSerializer().Serialize(ReturningResult);
                SendMessage(ThisConnection.WSClient, ReturningJson);
            }

            var json = new JavaScriptSerializer().Serialize(ThisResult);
            SendMessage(ThisConnection.WSClient, json);
            //SetReturningToMonitoring(TotalByte, ThisStation == STATIONNAME.NULL ? "Error" : ThisStation.ToString() + " - " + (ThisDeviceCategory == DEVICECATEGORY.NULL ? "Error" : ThisDeviceNameStr.ToString()) + " - " +
            //    ThisFieldName == null ? "Error" : ThisFieldName.ToString(), ThisResult.Value.ToString(), ConditionCase);
        }

        private static DEVICENAME GetDeviceNameFromStr(String DeviceNameStr)
        {
            List<DEVICENAME> ListOfDevicename = Enum.GetValues(typeof(DEVICENAME)).Cast<DEVICENAME>().ToList();
            DEVICENAME ThisDeviceName = ListOfDevicename.FirstOrDefault(Item => Item.ToString() == DeviceNameStr);
            return ThisDeviceName;
        }

        #endregion

        #region Set Handler

        private static void SetHandler(WSConnection ThisConnection, String[] SplitedCommand)
        {
            STATIONNAME ThisStation = STATIONNAME.NULL;
            DEVICECATEGORY ThisDeviceCategory = DEVICECATEGORY.NULL;
            DEVICENAME ThisDeviceName = DEVICENAME.NULL;
            String ThisCommand = null;
            List<String> Parameter = new List<String>();

            String StationNameStr = SplitedCommand[1].Split(new char[] { '=' })[1];
            String DeviceNameStr = SplitedCommand[2].Split(new char[] { '=' })[1];

            ThisStation = TTCSHelper.StationStrConveter(StationNameStr);
            ThisDeviceName = TTCSHelper.DeviceNameStrConverter(DeviceNameStr);
            ThisDeviceCategory = TTCSHelper.ConvertDeviceNameToDeviceCategory(ThisStation, ThisDeviceName);
            ThisCommand = SplitedCommand[3].Split(new char[] { '=' })[1];

            for (int i = 4; i < SplitedCommand.Count(); i++)
                Parameter.Add(SplitedCommand[i].Split(new char[] { '=' })[1]);

            if (ThisStation != STATIONNAME.NULL && ThisDeviceCategory != DEVICECATEGORY.NULL && ThisCommand != null)
            {
                ReturnKnowType CommandResult = CommandDefinition.VerifyCommand(ThisStation, ThisDeviceCategory, ThisCommand, Parameter.ToArray());

                if (CommandResult.ReturnType == ReturnStatus.FAILED)
                    ReturnMessage(ThisConnection.WSClient, "", "", ThisCommand, "", "", "Set", CommandResult.ReturnMessage, "Set_Error");
                else
                {
                    dynamic CommandName = TTCSHelper.CommandNameConverter(ThisDeviceCategory, ThisCommand);
                    Object[] Values = CommandDefinition.ValueConvertion(Parameter.ToArray());
                    AstroData.SetCommandHandler(ThisStation, ThisDeviceCategory, ThisDeviceName, CommandName, Values, DateTime.Now);

                    String MessageLog = "Relay Command to Station : " + ThisStation.ToString() + ", Device : " + ThisDeviceName.ToString() + ", Category : " + ThisDeviceCategory.ToString() +
                        ", Command : " + ThisCommand.ToString();
                    SetDataGridAddRowLog(DateTime.Now.ToString(), MessageLog, "CMD", String.Join(", ", Values), ThisStation.ToString(), "");
                    ReturnMessage(ThisConnection.WSClient, "", "", ThisCommand, "", "", "Set", "Commnad name -> '" + ThisCommand + "' had been verified.", "Set_Successful");
                }
            }
            else if (ThisStation == STATIONNAME.NULL)
                ReturnMessage(ThisConnection.WSClient, "", "", ThisCommand, "", "", "Set", "Invalid station name -> '" + StationNameStr + "' for command name '" + ThisCommand + "' type = Set", "Set_Error");
            else if (ThisDeviceCategory == DEVICECATEGORY.NULL)
                ReturnMessage(ThisConnection.WSClient, "", "", ThisCommand, "", "", "Set", "Invalid device name -> '" + DeviceNameStr + "' for command name '" + ThisCommand + "' type = Set", "Set_Error");
        }

        #endregion

        #region Monitoring

        private static void DisplayClient(String IPAddress, int Port, String Message, DateTime LastestRecive, int EventState)
        {
            if (EventState == 1)
                SetDataGridAddRow(IPAddress, Port, Message, DateTime.Now);
            else if (EventState == 2)
                SetDataGridRemoveRow(IPAddress, Port);
            else
                SetDataGridEditRow(IPAddress, Port, null, Message, LastestRecive);
        }

        //private static Boolean ActiveClientMonitoring(String IPAddress, String Port)
        //{
        //    MethodInfo method = ObjMonitoring.GetType().GetMethod("ClientChecker");
        //    object result = method.Invoke(ObjMonitoring, new object[] { IPAddress, Port });
        //    return (Boolean)result;
        //}

        //private static void SetLastestCommandToMonitoring(String[] LastestCommand, String LastestRecive)
        //{
        //    MethodInfo method = ObjMonitoring.GetType().GetMethod("ReciveCommandSetter");
        //    object result = method.Invoke(ObjMonitoring, new object[] { LastestCommand, LastestRecive });
        //}

        //private static void SetOnlineToMonitoring(String IPAddress, String Port)
        //{
        //    MethodInfo method = ObjMonitoring.GetType().GetMethod("OnlineTimeSetter");
        //    object result = method.Invoke(ObjMonitoring, new object[] { });
        //}

        //private static void SetReturningToMonitoring(String TotalByte, String CommandName, String DataMessage, String ConditionCase)
        //{
        //    MethodInfo method = ObjMonitoring.GetType().GetMethod("ReturningMessageSetter");
        //    method.Invoke(ObjMonitoring, new object[] { TotalByte, CommandName, DataMessage, ConditionCase });
        //}

        //private static void SetRemoveingToMonitoring(String IPAddress, String Port)
        //{
        //    MethodInfo method = ObjMonitoring.GetType().GetMethod("RemoveReturningInfo");
        //    method.Invoke(ObjMonitoring, new object[] { IPAddress, Port });
        //}

        #endregion

        #region Thread Handler

        //--------------------------------------------------------------------------------------Thread DataGrid Edit Row------------------------------------------------------------------------------------

        private static void SetDataGridEditRow(String IPAddress, int Port, DateTime? StartTime, String Message, DateTime LastestRecive)
        {
            if (ClientGrid.InvokeRequired)
            {
                ClientGrid.Invoke((Action)(() => ActionDataGridEditRow(IPAddress, Port, StartTime, Message, LastestRecive)));
                return;
            }

            ActionDataGridEditRow(IPAddress, Port, StartTime, Message, LastestRecive);
        }

        private static void ActionDataGridEditRow(String IPAddress, int Port, DateTime? StartTime, String Message, DateTime LastestRecive)
        {
            for (int i = 0; i < ClientGrid.RowCount; i++)
            {
                if (ClientGrid[1, i].Value.ToString() == IPAddress && ClientGrid[2, i].Value.ToString() == Port.ToString())
                {
                    if (StartTime != null)
                    {
                        TimeSpan Span = DateTime.Now - StartTime.Value;
                        ClientGrid[3, i].Value = Span.ToString(@"dd\.hh\:mm\:ss");
                    }

                    if (Message != null)
                    {
                        ClientGrid[4, i].Value = Message;
                        ClientGrid[5, i].Value = LastestRecive.ToString();
                    }
                    break;
                }
            }
        }

        //--------------------------------------------------------------------------------------Thread DataGrid Remove Row------------------------------------------------------------------------------------

        private static void SetDataGridRemoveRow(String IPAddress, int Port)
        {
            if (ClientGrid.InvokeRequired)
            {
                ClientGrid.Invoke((Action)(() => ActionDataGridRemoveRow(IPAddress, Port)));
                return;
            }

            ActionDataGridRemoveRow(IPAddress, Port);
        }

        private static void ActionDataGridRemoveRow(String IPAddress, int Port)
        {
            for (int i = 0; i < ClientGrid.RowCount; i++)
            {
                if (ClientGrid[1, i].Value.ToString() == IPAddress && ClientGrid[2, i].Value.ToString() == Port.ToString())
                {
                    ClientGrid.Rows.RemoveAt(i);
                    break;
                }
            }

            for (int i = 0; i < ClientGrid.RowCount; i++)
            {
                ClientGrid[0, i].Value = i + 1;
            }
        }

        //--------------------------------------------------------------------------------------Thread DataGrid Add Row Log------------------------------------------------------------------------------------

        private static void SetDataGridAddRowLog(String LogDate, String Message, String LogDataType, String Value, String Reporter, String UserID)
        {
            if (ClientGrid.InvokeRequired)
            {
                ClientGrid.Invoke((Action)(() => ActionDataGridAddRowLog(LogDate, Message, LogDataType, Value, Reporter, UserID)));
                return;
            }

            ActionDataGridAddRowLog(LogDate, Message, LogDataType, Value, Reporter, UserID);
        }

        private static void ActionDataGridAddRowLog(String LogDate, String Message, String LogDataType, String Value, String Reporter, String UserID)
        {
            TTCSLogGrid.Rows.Add(LogDate, Message, LogDataType, Value, Reporter, UserID);
        }

        //--------------------------------------------------------------------------------------Thread DataGrid Add Row------------------------------------------------------------------------------------

        private static void SetDataGridAddRow(String IPAddress, int Port, String Message, DateTime OnlineTime)
        {
            if (ClientGrid.InvokeRequired)
            {
                ClientGrid.Invoke((Action)(() => ActionDataGridAddRow(IPAddress, Port, Message, OnlineTime)));
                return;
            }

            ActionDataGridAddRow(IPAddress, Port, Message, OnlineTime);
        }

        private static void ActionDataGridAddRow(String IPAddress, int Port, String Message, DateTime OnlineTime)
        {
            TimeSpan Span = DateTime.Now - OnlineTime;
            ClientGrid.Rows.Add(ClientGrid.RowCount + 1, IPAddress, Port.ToString(), Span.ToString(@"dd\.hh\:mm\:ss"), Message);
        }

        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

        #endregion

        private static void SendMessage(IWebSocketConnection Client, String Message)
        {
            Client.Send(Message);
        }

        private static void ReturnMessage(IWebSocketConnection Client, String StationName, String DeviceName, String FieldName, String Value, String DataType, String InformationType, String ReturnMessage, String ReturnResult)
        {
            ResultStructure ThisReturnStructure = new ResultStructure();
            ThisReturnStructure.StationName = StationName;
            ThisReturnStructure.DeviceName = DeviceName;
            ThisReturnStructure.FieldName = FieldName;
            ThisReturnStructure.Value = Value;
            ThisReturnStructure.DataType = DataType;
            ThisReturnStructure.ParameterName = "null";
            ThisReturnStructure.InformationType = InformationType;
            ThisReturnStructure.ReturnMessage = ReturnMessage;
            ThisReturnStructure.ReturnResult = ReturnResult;
            ThisReturnStructure.DataTimeStamp = DateTime.Now.ToString();

            JavaScriptSerializer Serializer = new JavaScriptSerializer();
            Serializer.MaxJsonLength = Int32.MaxValue;
            var json = Serializer.Serialize(ThisReturnStructure);

            Client.Send(json);
            //SetReturningToMonitoring(TotalByte, CommandName, Value, ConditionCase);
        }

        public static void ReturnWebSubscribe(STATIONNAME StationName, DEVICENAME DeviceName, String FieldName, Object Value, DateTime DataTimeStamp)
        {
            foreach (KeyValuePair<String, WSConnection> ThisConnection in AllConnection)
            {
                SubscribeStructure ThisSub = ThisConnection.Value.SubscribeList.FirstOrDefault(Item => Item.Value.StationName == StationName && Item.Value.DeviceName == DeviceName && Item.Value.FieldName.ToString() == FieldName).Value;
                if (ThisSub != null)
                {
                    ResultStructure ThisResult = new ResultStructure();
                    ThisResult.StationName = StationName.ToString();
                    ThisResult.DeviceName = DeviceName.ToString();
                    ThisResult.FieldName = FieldName;

                    if (FieldName == "IMAGING_PREVIEW_BASE64")
                    {
                        if (Value.GetType() == typeof(Byte[]))
                            ThisResult.Value = Convert.ToBase64String((byte[])Value);
                        else
                            ThisResult.Value = Value.ToString();
                    }
                    else
                    {
                        if (Value.GetType() == typeof(Byte[]))
                            ThisResult.Value = Convert.ToBase64String((byte[])Value);
                        else
                            ThisResult.Value = Value.ToString();
                    }

                    ThisResult.DataType = Value.GetType().ToString().Replace("System.", "");
                    ThisResult.DataTimeStamp = DataTimeStamp.ToString();

                    JavaScriptSerializer Serializer = new JavaScriptSerializer();
                    Serializer.MaxJsonLength = Int32.MaxValue;
                    var json = Serializer.Serialize(ThisResult);

                    SendMessage(ThisConnection.Value.WSClient, json);
                    //SetReturningToMonitoring(TotalByte, StationName + " - " + DeviceName + " - " + FieldName, Value.ToString(), ConditionCase);
                }
            }
        }

        private static WSConnection GetClient(String IPAddress, int Port)
        {
            if (AllConnection != null && AllConnection.Count > 0)
            {
                String Key = IPAddress + ":" + Port.ToString();
                return AllConnection.FirstOrDefault(Item => Item.Key == Key).Value;
            }
            return null;
        }
    }
}
