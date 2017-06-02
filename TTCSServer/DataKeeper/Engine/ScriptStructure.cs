using DataKeeper.Engine.Command;
using DataKeeper.Interface;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace DataKeeper.Engine
{
    public enum SCRIPTSTATE { WAITINGSERVER, WAITINGSTATION, SENDINGTOSTATION, EXECUTING, CANCELED, EXECUTED, ENDTIMEPASSED, UPDATED }

    public class ScriptBuffer
    {
        public WSConnection WSConnection { get; set; }
        public ScriptTB Script { get; set; }
    }

    public class ScriptStructure
    {
        public String BlockID { get; set; }
        public String BlockName { get; set; }
        public STATIONNAME StationName { get; set; }
        public DateTime? ExecutionTimeStart { get; set; }
        public DateTime? ExecutionTimeEnd { get; set; }
        public int CommandCounter { get; set; }
        public int? ExecutionNumber { get; set; }
        public DEVICENAME DeviceName { get; set; }
        public DEVICECATEGORY DeviceCategory { get; set; }
        public dynamic CommandName { get; set; }
        public String Owner { get; set; }
        public Double? DelayTime { get; set; }
        public List<Object> Parameter { get; set; }
        public SCRIPTSTATE ScriptState { get; set; }
    }

    public static class ScriptManager
    {
        public static Double ScriptLifeTimeValue = 2.0;

        private static ConcurrentQueue<ScriptBuffer> NewScriptBuffer = null;
        private static ConcurrentDictionary<String, ScriptTB> ScriptDBBuffer = null;

        private static Object ScriptMonitoring = null;

        public static void CreateScriptPool()
        {
            NewScriptBuffer = new ConcurrentQueue<ScriptBuffer>();
            ScriptDBBuffer = new ConcurrentDictionary<String, ScriptTB>();
            LoadScriptFromDB();
            ReadScriptFromBuffer();
        }

        public static void SetMonitoringObject(Object ScriptMonitoring)
        {
            ScriptManager.ScriptMonitoring = ScriptMonitoring;
        }

        public static void ScriptInformationIdentification(STATIONNAME StationName, DEVICENAME DeviceName, ASTROCLIENT FieldName, Object Value, DateTime DataTimestamp)
        {
            switch (FieldName)
            {
                case ASTROCLIENT.ASTROCLIENT_LASTESTEXECTIONCOMMAND: break;
                case ASTROCLIENT.ASTROCLIENT_LASTESTEXECUTIONSCRIPT_STATUS:
                    {
                        SendScriptToHTTP();
                        UpdateExecutionState(StationName, Value); break;
                    }
                case ASTROCLIENT.ASTROCLIENT_LASTESTSCRIPT_RECIVED:
                    SetStationStateToWaitingStation(Value);
                    break;
            }
        }

        private static void SendScriptToHTTP()
        {
            try
            {
                WebRequest request = WebRequest.Create("http://www.contoso.com/default.html");
                request.Credentials = CredentialCache.DefaultCredentials;
                WebResponse response = request.GetResponse();

                Console.WriteLine(((HttpWebResponse)response).StatusDescription);
            }
            catch { }
        }

        private static void UpdateExecutionState(STATIONNAME StationName, Object Value)
        {
            String[] ValueArr = Value.ToString().Split(new char[] { ',' });
            String BlockID = ValueArr[0];
            int ExecutionNumber = Convert.ToInt32(ValueArr[1]);
            String ScriptState = ValueArr[2];

            ScriptTB ScriptDB = ScriptDBBuffer.FirstOrDefault(Item => Item.Value.BlockID == BlockID && Item.Value.ExecutionNumber == ExecutionNumber).Value;

            if (ScriptDB == null)
                return;

            ScriptDB.ScriptState = ScriptState;

            DatabaseSynchronization.ScriptSaveChange(true);
            UpdateScriptToMonitoring(ScriptDB);
        }

        private static void SetStationStateToWaitingStation(Object ScriptBlockID)
        {
            String BlockID = ScriptBlockID.ToString();
            List<KeyValuePair<String, ScriptTB>> ScriptList = ScriptDBBuffer.Where(Item => Item.Value.BlockID == BlockID).ToList();

            foreach (KeyValuePair<String, ScriptTB> ScriptNode in ScriptList)
            {
                ScriptNode.Value.ScriptState = SCRIPTSTATE.WAITINGSTATION.ToString();
                UpdateScriptToMonitoring(ScriptNode.Value);
            }

            DatabaseSynchronization.ScriptSaveChange(true);
        }

        public static void NewScriptFromSocket(ScriptStructure NewScriptStructure, WSConnection ScriptConnection)
        {
            ScriptBuffer NewBuffer = new ScriptBuffer();
            NewBuffer.Script = new ScriptTB();
            NewBuffer.Script.BlockID = NewScriptStructure.BlockID;
            NewBuffer.Script.BlockName = NewScriptStructure.BlockName;
            NewBuffer.Script.CommandCounter = NewScriptStructure.CommandCounter;
            NewBuffer.Script.CommandName = NewScriptStructure.CommandName;
            NewBuffer.Script.DelayTime = (int)NewScriptStructure.DelayTime;
            NewBuffer.Script.DeviceCategory = NewScriptStructure.DeviceCategory.ToString();
            NewBuffer.Script.DeviceName = NewScriptStructure.DeviceName.ToString();
            NewBuffer.Script.ExecutionNumber = NewScriptStructure.ExecutionNumber.Value;
            NewBuffer.Script.ExecutionTimeEnd = NewScriptStructure.ExecutionTimeEnd;
            NewBuffer.Script.ExecutionTimeStart = NewScriptStructure.ExecutionTimeStart;
            NewBuffer.Script.Owner = NewScriptStructure.Owner;
            NewBuffer.Script.Parameter = String.Join(",", NewScriptStructure.Parameter);
            NewBuffer.Script.ScriptState = NewScriptStructure.ScriptState.ToString();
            NewBuffer.Script.StationName = NewScriptStructure.StationName.ToString();
            NewBuffer.WSConnection = ScriptConnection;

            NewScriptBuffer.Enqueue(NewBuffer);
        }

        public static void LoadScriptFromDB()
        {
            List<ScriptTB> ScriptList = DatabaseSynchronization.GetScript();

            foreach (ScriptTB ThisScript in ScriptList)
                ScriptDBBuffer.TryAdd(ThisScript.BlockID + ThisScript.ExecutionNumber, ThisScript);
        }

        public static void RemoveAllScriptDB()
        {
            ScriptDBBuffer.Clear();
            DatabaseSynchronization.DeleteAllScript();
            DatabaseSynchronization.ScriptSaveChange(true);
        }

        public static Boolean RemoveScript(String BlockID, int ExecutionNumber)
        {
            try
            {
                ScriptTB ThisBuffer = ScriptDBBuffer.FirstOrDefault(Item => Item.Value.BlockID == BlockID && Item.Value.ExecutionNumber == ExecutionNumber).Value;
                ScriptDBBuffer.TryRemove(ThisBuffer.BlockID + ThisBuffer.ExecutionNumber, out ThisBuffer);
                DatabaseSynchronization.DeleteScript(ScriptDBBuffer.FirstOrDefault(Item => Item.Value.BlockID == BlockID && Item.Value.ExecutionNumber == ExecutionNumber).Value);
                DatabaseSynchronization.ScriptSaveChange(true);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public static List<ScriptTB> GetScriptList()
        {
            return ScriptDBBuffer.Values.ToList();
        }

        private static void ReadScriptFromBuffer()
        {
            Task TTCSTask = Task.Run(() =>
            {
                while (true)
                {
                    ScriptBuffer ThisBuffer = null;
                    if (NewScriptBuffer.TryDequeue(out ThisBuffer))
                    {
                        ScriptTB ExistingScript = ScriptDBBuffer.FirstOrDefault(Item => Item.Value.BlockName == ThisBuffer.Script.BlockName && Item.Value.ScriptState != "EXECUTED").Value;
                        if (ExistingScript == null)  //New block name
                        {
                            ThisBuffer.Script.BlockID = TTCSHelper.GenNewID();
                            ThisBuffer.Script.ScriptState = "CREATED";
                            ScriptDBBuffer.TryAdd(ThisBuffer.Script.BlockID + ThisBuffer.Script.ExecutionNumber, ThisBuffer.Script);

                            DatabaseSynchronization.InsertScript(ThisBuffer.Script);
                            AddScriptToMonitoring(ThisBuffer.Script);
                        }
                        else if (ExistingScript.ScriptState != "EXECUTING") //Existing block name
                        {
                            ScriptTB ExistingBlockScript = ScriptDBBuffer.FirstOrDefault(Item => Item.Value.BlockName == ThisBuffer.Script.BlockName && Item.Value.ExecutionNumber == ThisBuffer.Script.ExecutionNumber && Item.Value.ScriptState != "EXECUTED").Value;

                            if (ExistingBlockScript == null)  //Add new script to the same BlockName
                            {
                                ExistingBlockScript = ThisBuffer.Script;
                                ExistingBlockScript.BlockID = ExistingScript.BlockID;
                                ScriptDBBuffer.TryAdd(ExistingBlockScript.BlockID + ExistingBlockScript.ExecutionNumber, ExistingBlockScript);

                                DatabaseSynchronization.InsertScript(ExistingBlockScript);
                                AddScriptToMonitoring(ExistingBlockScript);
                            }
                            else
                            {
                                if (!IsEqualScript(ExistingBlockScript, ThisBuffer.Script))  //Update existing BlockName
                                {
                                    ExistingBlockScript.CommandCounter = ThisBuffer.Script.CommandCounter;
                                    ExistingBlockScript.CommandName = ThisBuffer.Script.CommandName;
                                    ExistingBlockScript.DelayTime = ThisBuffer.Script.DelayTime;
                                    ExistingBlockScript.DeviceCategory = ThisBuffer.Script.DeviceCategory;
                                    ExistingBlockScript.DeviceName = ThisBuffer.Script.DeviceName;
                                    ExistingBlockScript.ExecutionTimeEnd = ThisBuffer.Script.ExecutionTimeEnd;
                                    ExistingBlockScript.ExecutionTimeStart = ThisBuffer.Script.ExecutionTimeStart;
                                    ExistingBlockScript.Owner = ThisBuffer.Script.Owner;
                                    ExistingBlockScript.Parameter = ThisBuffer.Script.Parameter;
                                    ExistingBlockScript.ScriptState = "CREATED";
                                    ExistingBlockScript.StationName = ThisBuffer.Script.StationName;

                                    UpdateScriptToMonitoring(ExistingBlockScript);
                                }
                            }
                        }

                        DatabaseSynchronization.ScriptSaveChange(true);
                    }

                    ResponseToClient(ThisBuffer);
                    ResponseToStation();
                    RemoveExpireScript();

                    Thread.Sleep(10);
                }
            });
        }

        private static Boolean IsEqualScript(ScriptTB OlbScript, ScriptTB NewScript)
        {
            if (OlbScript.CommandCounter != NewScript.CommandCounter)
                return false;

            if (OlbScript.CommandName != NewScript.CommandName)
                return false;

            if (OlbScript.DelayTime != NewScript.DelayTime)
                return false;

            if (OlbScript.DeviceCategory != NewScript.DeviceCategory)
                return false;

            if (OlbScript.DeviceName != NewScript.DeviceName)
                return false;

            if (OlbScript.ExecutionTimeEnd != NewScript.ExecutionTimeEnd)
                return false;

            if (OlbScript.ExecutionTimeStart != NewScript.ExecutionTimeStart)
                return false;

            if (OlbScript.Owner != NewScript.Owner)
                return false;

            if (OlbScript.Parameter != NewScript.Parameter)
                return false;

            if (OlbScript.StationName != NewScript.StationName)
                return false;

            return true;
        }

        private static void RemoveExpireScript()
        {
            Boolean IsRemove = false;
            foreach (ScriptTB ThisScript in ScriptDBBuffer.Values)
            {
                DateTime ThisTime = DateTime.UtcNow;
                if (ThisScript.ExecutionTimeStart.Value < ThisTime.AddDays(ScriptLifeTimeValue * -1))
                {
                    ScriptTB TempScript = null;
                    ScriptDBBuffer.TryRemove(ThisScript.BlockID + ThisScript.ExecutionNumber, out TempScript);

                    IsRemove = true;
                    RemoveScriptToMonitoring(TempScript);
                }
            }

            if (IsRemove)
                DatabaseSynchronization.ScriptSaveChange(true);
        }

        public static void UpdateScriptFromStation(String BlockID, String BlockName, String StationName, DateTime ExecutionTimeStart, DateTime ExecutionTimeEnd, int CommandCounter, int ExecutionNumber, String DeviceName, String DeviceCategory, String CommandName, String Owner, int DelayTime, String Parameter, String ScriptState)
        {
            ScriptTB ExistingScript = ScriptDBBuffer.FirstOrDefault(Item => Item.Value.BlockID == BlockID && Item.Value.ExecutionNumber == ExecutionNumber).Value;
            if (ExistingScript != null)
            {
                ExistingScript.ScriptState = ScriptState;
                UpdateScriptToMonitoring(ExistingScript);
                DatabaseSynchronization.ScriptSaveChange(true);
            }
        }

        private static void ResponseToClient(ScriptBuffer ThisBuffer)
        {
            if (ThisBuffer != null)
            {
                String Message = "";
                Boolean IsAllScriptRecived = IsBlockComplete(ThisBuffer.Script, out Message);
                if (IsAllScriptRecived)
                    WebSockets.ReturnScriptResult(ThisBuffer.WSConnection, ThisBuffer.Script.BlockName, ThisBuffer.Script.BlockID, ThisBuffer.Script.ExecutionNumber.ToString(), ThisBuffer.Script.CommandName.ToString(), "All script is sending to client.", "Script_Success");
                else
                    WebSockets.ReturnScriptResult(ThisBuffer.WSConnection, ThisBuffer.Script.BlockName, ThisBuffer.Script.BlockID, ThisBuffer.Script.ExecutionNumber.ToString(), ThisBuffer.Script.CommandName.ToString(), Message, "Script_OK");
            }
        }

        private static void ResponseToStation()
        {
            if (ScriptDBBuffer.Count > 0)
            {
                List<ScriptTB> WaitingScriptList = ScriptDBBuffer.Values.Where(Item => Item.ScriptState == "CREATED" || Item.ScriptState == "WAITINGSERVER" && Item.ExecutionNumber == 1).ToList();

                if (WaitingScriptList.Count > 0)
                {
                    Boolean IsSend = false;
                    while (WaitingScriptList.Count > 0)
                    {
                        String Message = "";

                        Boolean IsAllScriptRecived = IsBlockComplete(WaitingScriptList[0], out Message);
                        if (IsAllScriptRecived)
                        {
                            SendScriptToStation(WaitingScriptList[0]);
                            WaitingScriptList.RemoveAll(Item => Item.BlockID == WaitingScriptList[0].BlockID);
                            IsSend = true;
                        }
                        else
                            WaitingScriptList.RemoveAt(0);
                    }

                    if (IsSend)
                        DatabaseSynchronization.ScriptSaveChange(true);
                }
            }
        }

        public static void SendScriptToStation(ScriptTB CompletedScript)
        {
            STATIONNAME DestinationStation = STATIONNAME.NULL;
            List<ScriptTB> ScriptArr = ScriptDBBuffer.Values.Where(Item => Item.BlockID == CompletedScript.BlockID).ToList();
            ScriptStructure[] StructureArr = ConvertScriptTBToScriptStructure(ScriptArr);

            foreach (ScriptTB ThisScript in ScriptArr)
            {
                DestinationStation = TTCSHelper.StationStrConveter(ThisScript.StationName);
                ThisScript.ScriptState = SCRIPTSTATE.WAITINGSERVER.ToString();

                UpdateScriptToMonitoring(ThisScript);
            }

            StationHandler ThisStation = AstroData.GetStationObject(DestinationStation);
            if (ThisStation.IsStationConnected)
            {
                StructureArr.OrderBy(Item => Item.ExecutionNumber);
                AstroData.ScriptHandler(DestinationStation, StructureArr);

                foreach (ScriptTB ThisScript in ScriptArr)
                {
                    ThisScript.ScriptState = SCRIPTSTATE.SENDINGTOSTATION.ToString();
                    UpdateScriptToMonitoring(ThisScript);
                }
            }
        }

        private static Boolean BlockExecuteCompleted(String BlockID)
        {
            ScriptTB ThisScript = ScriptDBBuffer.FirstOrDefault(Item => Item.Value.BlockID == BlockID && Item.Value.ScriptState != "EXECUTED").Value;
            if (ThisScript != null)
                return false;

            return true;
        }

        private static Boolean IsBlockComplete(ScriptTB ScriptToCheck, out String Message)
        {
            List<KeyValuePair<String, ScriptTB>> BlockScript = ScriptDBBuffer.Where(Item => Item.Value.BlockID == ScriptToCheck.BlockID).ToList();
            String[] MissingScript = GetMissingScript(ScriptToCheck, BlockScript);

            if (MissingScript != null && MissingScript.Count() > 0)
            {
                Message = "Block Name : " + ScriptToCheck.BlockName + ", Script : " + ScriptToCheck.ExecutionNumber + " successful add to the system but waiting for script number (" + String.Join(", ", MissingScript) + ")";
                return false;
            }
            else
            {
                Message = "Block Name : " + ScriptToCheck.BlockName + ", Script : " + ScriptToCheck.ExecutionNumber + " successful add to the system. All script is ready to send.";
                return true;
            }
        }

        private static String[] GetMissingScript(ScriptTB ScriptToCheck, List<KeyValuePair<String, ScriptTB>> BlockScript)
        {
            try
            {
                Boolean[] ScriptReciveState = new Boolean[ScriptToCheck.CommandCounter.Value];
                ScriptReciveState[ScriptToCheck.ExecutionNumber - 1] = true;

                foreach (KeyValuePair<String, ScriptTB> ThisScript in BlockScript)
                    ScriptReciveState[ThisScript.Value.ExecutionNumber - 1] = true;

                List<String> Missing = new List<String>();
                for (int i = 0; i < ScriptReciveState.Count(); i++)
                {
                    if (!ScriptReciveState[i])
                        Missing.Add((i + 1).ToString());
                }

                return Missing.ToArray();
            }
            catch
            {
            }

            return null;
        }

        private static void UpdateScriptToMonitoring(ScriptTB ThisScript)
        {
            if (ScriptMonitoring != null)
            {
                MethodInfo MInfo = ScriptMonitoring.GetType().GetMethod("UpdateScriptGrid");
                MInfo.Invoke(ScriptMonitoring, new Object[] { ThisScript });
            }
        }

        private static void AddScriptToMonitoring(ScriptTB NewScript)
        {
            if (ScriptMonitoring != null)
            {
                MethodInfo MInfo = ScriptMonitoring.GetType().GetMethod("AddSingleScriptToGrid");
                MInfo.Invoke(ScriptMonitoring, new Object[] { NewScript });
            }
        }

        private static void RemoveScriptToMonitoring(ScriptTB NewScript)
        {
            if (ScriptMonitoring != null)
            {
                MethodInfo MInfo = ScriptMonitoring.GetType().GetMethod("RemoveScriptFromGrid");
                MInfo.Invoke(ScriptMonitoring, new Object[] { NewScript });
            }
        }

        private static ScriptStructure[] ConvertScriptTBToScriptStructure(List<ScriptTB> ScriptTBArr)
        {
            List<ScriptStructure> ScriptStructureList = new List<ScriptStructure>();

            foreach (ScriptTB ThisScript in ScriptTBArr)
            {
                ScriptStructure NewSturcture = new ScriptStructure();
                NewSturcture.BlockID = ThisScript.BlockID;
                NewSturcture.BlockName = ThisScript.BlockName;
                NewSturcture.CommandCounter = ThisScript.CommandCounter.Value;
                NewSturcture.CommandName = ThisScript.CommandName;
                NewSturcture.DelayTime = ThisScript.DelayTime;
                NewSturcture.DeviceCategory = TTCSHelper.DeviceCategoryStrConverter(ThisScript.DeviceCategory);
                NewSturcture.DeviceName = TTCSHelper.DeviceNameStrConverter(ThisScript.DeviceName);
                NewSturcture.ExecutionNumber = ThisScript.ExecutionNumber;
                NewSturcture.ExecutionTimeEnd = ThisScript.ExecutionTimeEnd;
                NewSturcture.ExecutionTimeStart = ThisScript.ExecutionTimeStart;
                NewSturcture.Owner = ThisScript.Owner;

                NewSturcture.Parameter = new List<Object>();
                String[] ValueStr = ThisScript.Parameter.Split(new char[] { ',' });
                foreach (String ThisValue in ValueStr)
                    NewSturcture.Parameter.Add(ThisValue); ;

                NewSturcture.ScriptState = TTCSHelper.ScriptStateStrConverter(ThisScript.ScriptState);
                NewSturcture.StationName = TTCSHelper.StationStrConveter(ThisScript.StationName);

                ScriptStructureList.Add(NewSturcture);
            }

            return ScriptStructureList.ToArray();
        }
    }
}
