using DataKeeper.Engine.Command;
using DataKeeper.Interface;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace DataKeeper.Engine
{
    public enum SCRIPTSTATE { WAITINGSERVER, WAITINGSTATION, EXECUTING, CANCELED, EXECUTED }

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

    //public class BlockStructure
    //{
    //    public STATIONNAME StationName { get; set; }
    //    public String BlockName { get; set; }
    //    public String BlockID { get; set; }
    //    public int LifeTime { get; set; }
    //    public Boolean IsSendChecking { get; set; }
    //    public Boolean[] RecivedScriptState { get; set; }
    //    public String Owner { get; set; }
    //    public ConcurrentDictionary<String, ScriptStructure> ScriptList { get; set; }
    //    public String BlockSendState { get; set; }

    //    //public void SendTimeoutChecking()
    //    //{
    //    //    Task TTCSTask = Task.Run(() =>
    //    //    {
    //    //        int SendTimeouot = 30;
    //    //        while (IsSendChecking)
    //    //        {
    //    //            if (BlockSendState == "STATIONRECIVED")
    //    //                return;

    //    //            if (SendTimeouot == 0)
    //    //                ScriptManager.SendScript(Owner, BlockName, BlockID);

    //    //            SendTimeouot--;
    //    //            Thread.Sleep(1000);
    //    //        }
    //    //    });
    //    //}
    //}

    public class ScriptStateStructure
    {
        public ScriptTB Script { get; set; }

    }

    public static class ScriptManager
    {
        private static Entities db;
        private static ConcurrentQueue<ScriptBuffer> ScriptBuffer = null;
        private static ConcurrentDictionary<String, ScriptTB> ScriptDBBuffer = null;

        private static Object ScriptMonitoring = null;

        public static void CreateScriptPool()
        {
            ScriptBuffer = new ConcurrentQueue<ScriptBuffer>();
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
                case ASTROCLIENT.ASTROCLIENT_LASTESTEXECUTIONSCRIPT_STATUS: UpdateExecutionState(StationName, Value); break;
                case ASTROCLIENT.ASTROCLIENT_LASTESTSCRIPT_RECIVED: SetStationStateToWaitingStation(Value); break;
            }
        }

        private static void UpdateExecutionState(STATIONNAME StationName, Object Value)
        {
            String[] ValueArr = Value.ToString().Split(new char[] { ',' });
            String BlockID = ValueArr[0];
            int ExecutionNumber = Convert.ToInt32(ValueArr[1]);
            String ScriptState = ValueArr[2];

            ScriptTB ScriptDB = ScriptDBBuffer.FirstOrDefault(Item => Item.Value.BlockID == BlockID && Item.Value.ExecutionNumber == ExecutionNumber).Value;
            ScriptDB.ScriptState = ScriptState;

            db.SaveChanges();
            UpdateScriptToMonitoring();
        }

        private static void SetStationStateToWaitingStation(Object ScriptBlockID)
        {
            String BlockID = ScriptBlockID.ToString();
            List<KeyValuePair<String, ScriptTB>> ScriptList = ScriptDBBuffer.Where(Item => Item.Value.BlockID == BlockID).ToList();

            foreach (KeyValuePair<String, ScriptTB> ScriptNode in ScriptList)
            {
                ScriptNode.Value.ScriptState = SCRIPTSTATE.WAITINGSTATION.ToString();
                db.SaveChanges();
            }
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

            ScriptBuffer.Enqueue(NewBuffer);
        }

        public static void LoadScriptFromDB()
        {
            db = new Entities();
            List<ScriptTB> ScriptList = db.ScriptTBs.ToList();

            foreach (ScriptTB ThisScript in ScriptList)
                ScriptDBBuffer.TryAdd(ThisScript.BlockID + ThisScript.ExecutionNumber, ThisScript);
        }

        public static void RemoveAllScriptDB()
        {
            ScriptDBBuffer.Clear();
            db.ScriptTBs.RemoveRange(db.ScriptTBs);
            db.SaveChanges();
        }

        public static Boolean RemoveScript(String BlockID, int ExecutionNumber)
        {
            try
            {
                ScriptTB ThisBuffer = ScriptDBBuffer.FirstOrDefault(Item => Item.Value.BlockID == BlockID && Item.Value.ExecutionNumber == ExecutionNumber).Value;
                ScriptDBBuffer.TryRemove(ThisBuffer.BlockID + ThisBuffer.ExecutionNumber, out ThisBuffer);
                db.ScriptTBs.Remove(db.ScriptTBs.FirstOrDefault(Item => Item.BlockID == BlockID && Item.ExecutionNumber == ExecutionNumber));
                db.SaveChanges();

                return true;
            }
            catch {
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
                    if (ScriptBuffer.TryDequeue(out ThisBuffer))
                    {
                        ScriptTB ExistingScript = ScriptDBBuffer.FirstOrDefault(Item => Item.Value.BlockName == ThisBuffer.Script.BlockName).Value;
                        if (ExistingScript == null)  //New block name
                        {
                            ThisBuffer.Script.BlockID = TTCSHelper.GenNewID();
                            ExistingScript = ThisBuffer.Script;
                            ExistingScript.ScriptState = "CREATED";
                            ScriptDBBuffer.TryAdd(ThisBuffer.Script.BlockID + ThisBuffer.Script.ExecutionNumber, ThisBuffer.Script);
                            db.ScriptTBs.Add(ThisBuffer.Script);
                        }
                        else  //Existing Block name
                        {
                            ScriptTB ExistingBlockScript = ScriptDBBuffer.FirstOrDefault(Item => Item.Value.BlockName == ThisBuffer.Script.BlockName && Item.Value.ExecutionNumber == ThisBuffer.Script.ExecutionNumber).Value;
                            
                            if (ExistingBlockScript == null)  //New execution number
                            {
                                ExistingBlockScript = ThisBuffer.Script;
                                ExistingBlockScript.BlockID = ExistingScript.BlockID;
                                ExistingBlockScript.ScriptState = "CREATED";
                                ScriptDBBuffer.TryAdd(ExistingBlockScript.BlockID + ExistingBlockScript.ExecutionNumber, ExistingBlockScript);
                                db.ScriptTBs.Add(ExistingBlockScript);
                            }
                            else
                            {
                                ExistingScript = ThisBuffer.Script;
                                ExistingScript.ScriptState = "CREATED";
                            }
                        }

                        db.SaveChanges();
                        UpdateScriptToMonitoring();                        
                    }

                    if(ThisBuffer != null)
                    {
                        //String Message = "";
                        //Boolean IsAllScriptRecived = IsBlockComplete(ExistingScript, out Message);
                        //if (IsAllScriptRecived)
                        //{
                        //    SendScript(ExistingScript);
                        //    WebSockets.ReturnScriptResult(ThisBuffer.WSConnection, ThisBuffer.Script.BlockName, ThisBuffer.Script.BlockID, ThisBuffer.Script.ExecutionNumber.ToString(), ThisBuffer.Script.CommandName.ToString(), "All script is sending to client.", "Script_Success");
                        //}
                        //else
                        //    WebSockets.ReturnScriptResult(ThisBuffer.WSConnection, ThisBuffer.Script.BlockName, ThisBuffer.Script.BlockID, ThisBuffer.Script.ExecutionNumber.ToString(), ThisBuffer.Script.CommandName.ToString(), Message, "Script_OK");
                    }

                    Thread.Sleep(1);
                }
            });
        }

        public static void SendScript(ScriptTB CompletedScript)
        {
            STATIONNAME DestinationStation = STATIONNAME.NULL;
            List<KeyValuePair<String, ScriptTB>> ScriptArr = ScriptDBBuffer.Where(Item => Item.Value.BlockID == CompletedScript.BlockID).ToList();
            ScriptStructure[] StructureArr = ConvertScriptTBToScriptStructure(ScriptArr);

            foreach (ScriptStructure ThisScript in StructureArr)
            {
                DestinationStation = ThisScript.StationName;
                ThisScript.ScriptState = SCRIPTSTATE.WAITINGSERVER;
            }

            StationHandler ThisStation = AstroData.GetStationObject(DestinationStation);
            if (ThisStation.IsStationConnected)
            {
                StructureArr.OrderBy(Item => Item.ExecutionNumber);
                AstroData.ScriptHandler(DestinationStation, StructureArr);
            }
        }

        private static Boolean IsBlockComplete(ScriptTB ScriptToCheck, out String Message)
        {
            List<KeyValuePair<String, ScriptTB>> AvaliableScripts = ScriptDBBuffer.Where(Item => Item.Value.BlockID == ScriptToCheck.BlockID).ToList();
            String[] MissingScript = GetMissingScript(ScriptToCheck, AvaliableScripts);

            if (MissingScript.Count() > 0)
            {
                Message = "Script " + ScriptToCheck.ExecutionNumber + " successful add to the system but waiting for script number (" + String.Join(", ", MissingScript) + ")";
                return false;
            }
            else
            {
                Message = "Script " + ScriptToCheck.ExecutionNumber + " successful add to the system. All script is ready to send.";
                return true;
            }
        }

        private static String[] GetMissingScript(ScriptTB ScriptToCheck, List<KeyValuePair<String, ScriptTB>> AvaliableScripts)
        {
            Boolean[] ScriptReciveState = new Boolean[ScriptToCheck.CommandCounter.Value];
            ScriptReciveState[ScriptToCheck.ExecutionNumber - 1] = true;

            foreach (KeyValuePair<String, ScriptTB> ThisScript in AvaliableScripts)
                ScriptReciveState[ThisScript.Value.ExecutionNumber - 1] = true;

            List<String> Missing = new List<String>();
            for (int i = 0; i < ScriptReciveState.Count(); i++)
            {
                if (!ScriptReciveState[i])
                    Missing.Add((i + 1).ToString());
            }

            return Missing.ToArray();
        }

        private static void UpdateScriptToMonitoring()
        {
            if (ScriptMonitoring != null)
            {
                MethodInfo MInfo = ScriptMonitoring.GetType().GetMethod("AddScriptToGrid");
                MInfo.Invoke(ScriptMonitoring, new Object[] { });
            }
        }

        private static ScriptStructure[] ConvertScriptTBToScriptStructure(List<KeyValuePair<String, ScriptTB>> ScriptTBArr)
        {
            List<ScriptStructure> ScriptStructureList = new List<ScriptStructure>();

            foreach (KeyValuePair<String, ScriptTB> ThisScript in ScriptTBArr)
            {
                ScriptStructure NewSturcture = new ScriptStructure();
                NewSturcture.BlockID = ThisScript.Value.BlockID;
                NewSturcture.BlockName = ThisScript.Value.BlockName;
                NewSturcture.CommandCounter = ThisScript.Value.CommandCounter.Value;
                NewSturcture.CommandName = ThisScript.Value.CommandName;
                NewSturcture.DelayTime = ThisScript.Value.DelayTime;
                NewSturcture.DeviceCategory = TTCSHelper.DeviceCategoryStrConverter(ThisScript.Value.DeviceCategory);
                NewSturcture.DeviceName = TTCSHelper.DeviceNameStrConverter(ThisScript.Value.DeviceName);
                NewSturcture.ExecutionNumber = ThisScript.Value.ExecutionNumber;
                NewSturcture.ExecutionTimeEnd = ThisScript.Value.ExecutionTimeEnd;
                NewSturcture.ExecutionTimeStart = ThisScript.Value.ExecutionTimeStart;
                NewSturcture.Owner = ThisScript.Value.Owner;

                NewSturcture.Parameter = new List<Object>();
                String[] ValueStr = ThisScript.Value.Parameter.Split(new char[] { ',' });
                foreach (String ThisValue in ValueStr)
                    NewSturcture.Parameter.Add(ThisValue); ;

                NewSturcture.ScriptState = TTCSHelper.ScriptStateStrConverter(ThisScript.Value.ScriptState);
                NewSturcture.StationName = TTCSHelper.StationStrConveter(ThisScript.Value.StationName);

                ScriptStructureList.Add(NewSturcture);
            }

            return ScriptStructureList.ToArray();
        }

        //private static void StartUpdateBufferFromDatabase()
        //{
        //    Task TTCSTask = Task.Run(() =>
        //    {
        //        while (true)
        //        {
        //            foreach (ScriptTB DatabaseBlock in MainScriptPoolDB.Values)
        //            {
        //                if (DatabaseBlock.ScriptState == "WAITINGSERVER")
        //                {
        //                    Boolean IsFound = false;

        //                    foreach (ScriptBuffer BufferBlock in BufferQueue)
        //                    {
        //                        if (DatabaseBlock.BlockID == BufferBlock.Script.BlockID && DatabaseBlock.ExecutionNumber == BufferBlock.Script.ExecutionNumber)
        //                        {
        //                            IsFound = true;
        //                            break;
        //                        }
        //                    }

        //                    if (!IsFound)
        //                    {
        //                        ScriptStructure NewScript = new ScriptStructure();
        //                        NewScript.BlockID = DatabaseBlock.BlockID;
        //                        NewScript.BlockName = DatabaseBlock.BlockName;
        //                        NewScript.CommandCounter = DatabaseBlock.CommandCounter.Value;
        //                        NewScript.CommandName = DatabaseBlock.CommandName;
        //                        NewScript.DelayTime = DatabaseBlock.DelayTime;
        //                        NewScript.DeviceCategory = TTCSHelper.DeviceCategoryStrConverter(DatabaseBlock.DeviceCategory);
        //                        NewScript.DeviceName = TTCSHelper.DeviceNameStrConverter(DatabaseBlock.DeviceName);
        //                        NewScript.ExecutionNumber = DatabaseBlock.ExecutionNumber;
        //                        NewScript.ExecutionTimeEnd = DatabaseBlock.ExecutionTimeEnd;
        //                        NewScript.ExecutionTimeStart = DatabaseBlock.ExecutionTimeStart;
        //                        NewScript.Owner = DatabaseBlock.Owner;

        //                        List<Object> ObjectValue = new List<Object>();
        //                        foreach (String ValueStr in DatabaseBlock.Parameter.Split(new char[] { ',' }))
        //                            ObjectValue.Add(ValueStr);

        //                        NewScript.Parameter = ObjectValue;

        //                        NewScript.ScriptState = TTCSHelper.ScriptStateStrConverter(DatabaseBlock.ScriptState);
        //                        NewScript.StationName = TTCSHelper.StationStrConveter(DatabaseBlock.StationName);

        //                        AddScriptToBuffer(NewScript, null);
        //                    }
        //                }
        //            }

        //            Thread.Sleep(1000);
        //        }
        //    });
        //}

        //public static void AddScriptToBuffer(ScriptStructure ThisScript, WSConnection ThisConnection)
        //{
        //    ScriptBuffer ThisBuffer = new ScriptBuffer();
        //    ThisBuffer.WSConnection = ThisConnection;
        //    ThisBuffer.Script = ThisScript;

        //    BufferQueue.Enqueue(ThisBuffer);
        //}

        //private static void StartBufferLoop()
        //{
        //    Task TTCSTask = Task.Run(() =>
        //    {
        //        DateTime D = DateTime.Now.ToUniversalTime();

        //        while (true)
        //        {
        //            ScriptBuffer ThisScriptBuffer = null;

        //            if (BufferQueue.TryDequeue(out ThisScriptBuffer))
        //            {
        //                String Message = "";
        //                Boolean ScriptState = false;
        //                ScriptTB ExistingBlockDatabase = GetExistingBlockFromDatabase(ThisScriptBuffer.Script.BlockName, ThisScriptBuffer.Script.Owner);
        //                ScriptTB ExistingBlockPool = GetExistingBlockFromPool(ThisScriptBuffer.Script.Owner, ThisScriptBuffer.Script.BlockName);
        //                Boolean ScriptAddedResult = false;

        //                String BlockID = TTCSHelper.GenNewID();
        //                if (ExistingBlockPool == null)          //Not in Pool
        //                    ScriptAddedResult = CreateBlock(ThisScriptBuffer, out Message, BlockID);
        //                else                                    //in Pool
        //                {
        //                    BlockID = ExistingBlockPool.BlockID;
        //                    ScriptAddedResult = (ThisScriptBuffer, out Message, BlockID, out ScriptState);  //Send Script Here
        //                }

        //                if (ExistingBlockDatabase != null)      //in Database
        //                {
        //                    BlockID = ExistingBlockDatabase.BlockID;
        //                    Message = "Update script " + ThisScriptBuffer.Script.ExecutionNumber + " successful";
        //                    ScriptAddedResult = UpdateScriptInDatabase(ExistingBlockDatabase.BlockID, ThisScriptBuffer.Script.ExecutionNumber.Value, ThisScriptBuffer.Script.ExecutionTimeStart.Value, ThisScriptBuffer.Script.ExecutionTimeEnd.Value,
        //                        ThisScriptBuffer.Script.CommandName.ToString(), ThisScriptBuffer.Script.DeviceCategory.ToString(), ThisScriptBuffer.Script.DeviceName.ToString(), String.Join(",", ThisScriptBuffer.Script.Parameter));
        //                }

        //                UpdateScriptToMonitoring();

        //                if (!ScriptAddedResult)
        //                {
        //                    if (ThisScriptBuffer.WSConnection != null)
        //                        WebSockets.ReturnScriptResult(ThisScriptBuffer.WSConnection, ThisScriptBuffer.Script.BlockName, BlockID, ThisScriptBuffer.Script.ExecutionNumber.ToString(), ThisScriptBuffer.Script.CommandName.ToString(), Message, "Script_Error");
        //                }
        //                else
        //                {
        //                    GetAllScript();

        //                    if (ThisScriptBuffer.WSConnection != null)
        //                    {
        //                        if (ScriptState)
        //                            WebSockets.ReturnScriptResult(ThisScriptBuffer.WSConnection, ThisScriptBuffer.Script.BlockName, BlockID, ThisScriptBuffer.Script.ExecutionNumber.ToString(), ThisScriptBuffer.Script.CommandName.ToString(), "All script is sending to client.", "Script_Success");
        //                        else
        //                            WebSockets.ReturnScriptResult(ThisScriptBuffer.WSConnection, ThisScriptBuffer.Script.BlockName, BlockID, ThisScriptBuffer.Script.ExecutionNumber.ToString(), ThisScriptBuffer.Script.CommandName.ToString(), Message, "Script_OK");
        //                    }
        //                }
        //            }

        //            Thread.Sleep(1);
        //        }
        //    });
        //}

        //public static void GetAllScript()
        //{
        //    Task TTCSTask = Task.Run(() =>
        //    {
        //        Thread.Sleep(2000);
        //        ScriptTB[] ThisScript = MainScriptPoolDB.Values.ToArray();
        //        var ReturningJson = new JavaScriptSerializer().Serialize(ThisScript);
        //        AstroData.NewASTROSERVERInformation(STATIONNAME.ASTROSERVER, DEVICENAME.ASTROPARK_SERVER, ASTROSERVER.ASTROSERVER_ALLSCRIPTBLOCK, ReturningJson, DateTime.Now);
        //    });
        //}

        //private static void UpdateScriptToMonitoring()
        //{
        //    if (ScriptMonitoring != null)
        //    {
        //        MethodInfo MInfo = ScriptMonitoring.GetType().GetMethod("AddScriptToGrid");
        //        MInfo.Invoke(ScriptMonitoring, new Object[] { });
        //    }
        //}

        //private static void UpdateScriptToMonitoring(String BlockID, int ExecutionNumber, String ScriptState)
        //{
        //    if (ScriptMonitoring != null)
        //    {
        //        MethodInfo MInfo = ScriptMonitoring.GetType().GetMethod("UpdateScriptGrid");
        //        MInfo.Invoke(ScriptMonitoring, new Object[] { BlockID, ExecutionNumber, ScriptState });
        //    }
        //}

        //public static void RefreshDBScript()
        //{
        //    db = new Entities();
        //    MainScriptPoolDB = new ConcurrentDictionary<string, ScriptTB>();
        //    foreach (ScriptTB ThisScript in db.ScriptTBs)
        //        MainScriptPoolDB.TryAdd(ThisScript.Owner + ThisScript.BlockName, ThisScript);
        //}

        //public static Boolean DeleteScript(String BlockID, int ExecutionNumber)
        //{
        //    ScriptTB ThisScript = db.ScriptTBs.FirstOrDefault(Item => Item.BlockID == BlockID && Item.ExecutionNumber == ExecutionNumber);

        //    try
        //    {
        //        if (ThisScript != null)
        //        {
        //            MainScriptPoolDB.TryRemove(ThisScript.Owner + ThisScript.BlockName, out ThisScript);
        //            db.ScriptTBs.Remove(ThisScript);
        //            db.SaveChanges();
        //            return true;
        //        }
        //    }
        //    catch
        //    {
        //        return false;
        //    }

        //    return false;
        //}

        //public static List<ScriptTB> GetAvaliableScript()
        //{
        //    return MainScriptPoolDB.Values.ToList();
        //}

        //public static ScriptTB GetExistingBlockFromPool(String Owner, String BlockName)
        //{
        //    return MainScriptPoolDB.FirstOrDefault(Item => Item.Value.Owner == Owner && Item.Value.BlockName == BlockName).Value;
        //}

        //public static ScriptTB GetExistingBlockFromDatabaseByBlockID(String BlockID)
        //{
        //    ScriptTB ThisScript = MainScriptPoolDB.FirstOrDefault(Item => Item.Value.BlockID == BlockID).Value;
        //    return ThisScript;
        //}

        //public static ScriptTB GetExistingBlockFromDatabaseByOwner(String Owner)
        //{
        //    ScriptTB ThisScript = MainScriptPoolDB.FirstOrDefault(Item => Item.Value.Owner == Owner).Value;
        //    return ThisScript;
        //}

        //public static ScriptTB GetExistingBlockFromDatabase(String BlockName, String Owner)
        //{
        //    ScriptTB ThisScript = MainScriptPoolDB.FirstOrDefault(Item => Item.Value.BlockName == BlockName && Item.Value.Owner == Owner).Value;
        //    return ThisScript;
        //}

        //public static void ClearAllScript()
        //{
        //    db = new Entities();
        //    db.ScriptTBs.RemoveRange(db.ScriptTBs);
        //    db.SaveChanges();

        //    MainScriptPoolDB = new ConcurrentDictionary<string, ScriptTB>();
        //}

        //public static Boolean CreateBlock(ScriptBuffer BlockBuffer, out String Message, String BlockID)
        //{
        //    BlockStructure ThisScriptNode = ThisScriptNode = new BlockStructure();
        //    ThisScriptNode.StationName = BlockBuffer.Script.StationName;
        //    ThisScriptNode.BlockName = BlockBuffer.Script.BlockName;
        //    ThisScriptNode.IsSendChecking = false;
        //    ThisScriptNode.LifeTime = 30;
        //    ThisScriptNode.BlockID = BlockID;
        //    ThisScriptNode.RecivedScriptState = new bool[BlockBuffer.Script.CommandCounter];
        //    ThisScriptNode.Owner = BlockBuffer.Script.Owner;
        //    ThisScriptNode.ScriptList = new ConcurrentDictionary<String, ScriptStructure>();
        //    BlockBuffer.Script.BlockID = BlockID;
        //    ThisScriptNode.ScriptList.TryAdd(BlockBuffer.Script.ExecutionNumber.ToString(), BlockBuffer.Script);
        //    ThisScriptNode.RecivedScriptState[BlockBuffer.Script.ExecutionNumber.Value - 1] = true;
        //    ThisScriptNode.BlockSendState = "CREATED";

        //    if (!MainPool.TryAdd(BlockBuffer.Script.Owner + BlockBuffer.Script.BlockName, ThisScriptNode))
        //    {
        //        Message = "Added script (Create) fail. IPAddress and Port are already exist.";
        //        Console.WriteLine(Message);
        //        return false;
        //    }

        //    List<int> WiatingForScript = new List<int>();

        //    if (BlockBuffer.Script.CommandCounter == 1)
        //    {
        //        SendScript(ThisScriptNode.Owner, ThisScriptNode.BlockName, BlockID);
        //        Message = "Added script (Create) successful and sending to client";
        //        Console.WriteLine(Message);
        //        return true;
        //    }

        //    for (int i = 0; i < ThisScriptNode.RecivedScriptState.Count(); i++)
        //    {
        //        if (!ThisScriptNode.RecivedScriptState[i])
        //            WiatingForScript.Add(i + 1);
        //    }

        //    Message = "Added script " + BlockBuffer.Script.ExecutionNumber + " (Create) successful but waiting for script number (" + String.Join(", ", WiatingForScript) + ")";
        //    Console.WriteLine(Message);
        //    return true;
        //}

        //public static Boolean AddOrUpdateInPool(ScriptBuffer BlockBuffer, out String Message, String BlockID, out Boolean IsScriptStateSuccess)
        //{
        //    BlockStructure ThisScriptNode = MainPool.FirstOrDefault(Item => Item.Value.Owner == BlockBuffer.Script.Owner && Item.Value.BlockName == BlockBuffer.Script.BlockName).Value;
        //    IsScriptStateSuccess = false;

        //    if (ThisScriptNode == null)
        //    {
        //        Message = "Script pool is empty.";
        //        Console.WriteLine(Message);
        //        return false;
        //    }

        //    String Key = BlockBuffer.Script.ExecutionNumber.ToString();
        //    BlockBuffer.Script.BlockID = BlockID;
        //    ThisScriptNode.RecivedScriptState[BlockBuffer.Script.ExecutionNumber.Value - 1] = true;

        //    Boolean IsScriptAdd = ThisScriptNode.ScriptList.TryAdd(Key, BlockBuffer.Script);
        //    if (!IsScriptAdd)
        //        ThisScriptNode.ScriptList.AddOrUpdate(Key, BlockBuffer.Script, (key, ExistingScript) => { return ExistingScript; });

        //    List<int> WiatingForScript = new List<int>();
        //    Boolean IsAllComplete = true;

        //    for (int i = 0; i < ThisScriptNode.RecivedScriptState.Count(); i++)
        //        if (!ThisScriptNode.RecivedScriptState[i])
        //        {
        //            IsAllComplete = false;
        //            WiatingForScript.Add(i + 1);
        //        }

        //    if (!IsAllComplete)
        //    {
        //        if (IsScriptAdd)
        //            Message = "Added script " + BlockBuffer.Script.ExecutionNumber + " successful but waiting for script number (" + String.Join(", ", WiatingForScript) + ")";
        //        else
        //            Message = "Update script " + BlockBuffer.Script.ExecutionNumber + " successful but waiting for script number (" + String.Join(", ", WiatingForScript) + ")";

        //        Console.WriteLine(Message);
        //        return true;
        //    }

        //    if (IsScriptAdd)
        //        Console.WriteLine("Added script " + BlockBuffer.Script.ExecutionNumber + " successful all script saved.");
        //    else
        //        Console.WriteLine("Update script " + BlockBuffer.Script.ExecutionNumber + " successful all script saved.");

        //    SendScript(BlockBuffer.Script.Owner, BlockBuffer.Script.BlockName, BlockID);
        //    IsScriptStateSuccess = true;
        //    Message = "Added script " + BlockBuffer.Script.ExecutionNumber + " successful. All script is ready to send.";

        //    if (BlockBuffer.WSConnection != null)
        //        WebSockets.ReturnScriptResult(BlockBuffer.WSConnection, BlockBuffer.Script.BlockName, BlockID, BlockBuffer.Script.ExecutionNumber.ToString(), BlockBuffer.Script.CommandName.ToString(), Message, "Script_OK");

        //    return true;
        //}

        //public static void SendScript(String Owner, String BlockName, String BlockID)
        //{
        //    BlockStructure ThisScriptNode = MainPool.FirstOrDefault(Item => Item.Value.Owner == Owner && Item.Value.BlockID == BlockID).Value;

        //    if (ThisScriptNode != null && (ThisScriptNode.BlockSendState == "CREATED" || ThisScriptNode.BlockSendState == "WAITINGSERVER"))
        //    {
        //        StationHandler ThisStation = AstroData.GetStationObject(ThisScriptNode.StationName);
        //        if (ThisStation.IsStationConnected)
        //        {
        //            ThisScriptNode.IsSendChecking = true;
        //            ThisScriptNode.SendTimeoutChecking();
        //            ThisScriptNode.BlockSendState = "WAITINGSERVER";
        //            ThisScriptNode.ScriptList.OrderBy(Item => Item.Value.ExecutionNumber);
        //            AstroData.ScriptHandler(ThisScriptNode);
        //            BackupScript(ThisScriptNode);
        //        }
        //    }
        //}                       

        //public static void RemoveScriptBlock(String Owner, String BlockName)
        //{
        //    BlockStructure ThisScriptNode = MainPool.FirstOrDefault(Item => Item.Value.Owner == Owner && Item.Value.BlockName == BlockName).Value;

        //    if (ThisScriptNode != null)
        //    {
        //        ThisScriptNode.IsSendChecking = false;
        //        MainPool.TryRemove(Owner + BlockName, out ThisScriptNode);
        //    }
        //}

        //private static void UpdateScriptState(String BlockID, int ExecutionNumber, String ScripState)
        //{
        //    ScriptTB ThisScript = MainScriptPoolDB.FirstOrDefault(Item => Item.BlockID == BlockID && Item.ExecutionNumber == ExecutionNumber);
        //    ThisScript.ScriptState = ScripState;
        //    db.SaveChanges();
        //}

        //private static Boolean UpdateScriptInDatabase(String BlockID, int ExecutionNumber, DateTime ExecutionTimeStart, DateTime ExecutionTimeEnd, String CommandName, String DeviceCategory, String DeviceName, String Parameter)
        //{
        //    try
        //    {
        //        ScriptTB ExistingScript = MainScriptPoolDB.FirstOrDefault(Item => Item.BlockID == BlockID && Item.ExecutionNumber == ExecutionNumber);
        //        ExistingScript.ExecutionTimeStart = ExecutionTimeStart;
        //        ExistingScript.ExecutionTimeEnd = ExecutionTimeEnd;
        //        ExistingScript.CommandName = CommandName;
        //        ExistingScript.DeviceCategory = DeviceCategory;
        //        ExistingScript.DeviceName = DeviceName;
        //        ExistingScript.Parameter = Parameter;

        //        db.SaveChanges();
        //        return true;
        //    }
        //    catch { return false; }
        //}

        //private static void BackupScript(BlockStructure ThisScriptNode)
        //{
        //    foreach (ScriptStructure ThisScript in ThisScriptNode.ScriptList.Values)
        //    {
        //        ScriptTB ExistingScript = MainScriptPoolDB.FirstOrDefault(Item => Item.BlockID == ThisScript.BlockID && Item.ExecutionNumber == ThisScript.ExecutionNumber);

        //        if (ExistingScript == null)
        //        {
        //            ScriptTB NewScript = new ScriptTB();
        //            NewScript.BlockID = ThisScript.BlockID;
        //            NewScript.BlockName = ThisScript.BlockName;
        //            NewScript.CommandCounter = ThisScript.CommandCounter;
        //            NewScript.CommandName = ThisScript.CommandName.ToString();
        //            NewScript.DeviceCategory = ThisScript.DeviceCategory.ToString();
        //            NewScript.DeviceName = ThisScript.DeviceName.ToString();
        //            NewScript.ExecutionNumber = ThisScript.ExecutionNumber.Value;
        //            NewScript.ExecutionTimeEnd = ThisScript.ExecutionTimeEnd;
        //            NewScript.ExecutionTimeStart = ThisScript.ExecutionTimeStart;
        //            NewScript.Owner = ThisScript.Owner;
        //            NewScript.Parameter = String.Join(",", ThisScript.Parameter);
        //            NewScript.ScriptState = ThisScript.ScriptState.ToString();
        //            NewScript.StationName = ThisScript.StationName.ToString();

        //            MainScriptPoolDB.Add(NewScript);
        //            db.ScriptTBs.Add(NewScript);
        //        }
        //        else
        //        {
        //            ExistingScript.BlockName = ThisScript.BlockName;
        //            ExistingScript.CommandCounter = ThisScript.CommandCounter;
        //            ExistingScript.CommandName = ThisScript.CommandName.ToString();
        //            ExistingScript.DeviceCategory = ThisScript.DeviceCategory.ToString();
        //            ExistingScript.DeviceName = ThisScript.DeviceName.ToString();
        //            ExistingScript.ExecutionNumber = ThisScript.ExecutionNumber.Value;
        //            ExistingScript.ExecutionTimeEnd = ThisScript.ExecutionTimeEnd;
        //            ExistingScript.ExecutionTimeStart = ThisScript.ExecutionTimeStart;
        //            ExistingScript.Owner = ThisScript.Owner;
        //            ExistingScript.Parameter = String.Join(",", ThisScript.Parameter);
        //            ExistingScript.ScriptState = ThisScript.ScriptState.ToString();
        //            ExistingScript.StationName = ThisScript.StationName.ToString();
        //        }
        //    }

        //    db.SaveChanges();
        //}

        //private static void StartBackUpScriptExpireLoop()
        //{
        //    Task TTCSTask = Task.Run(() =>
        //    {
        //        while (true)
        //        {
        //            try
        //            {
        //                DateTime VeridateDate = DateTime.Now.AddDays(-2);
        //                List<ScriptTB> ThisScript = MainScriptPoolDB.Where(Item => Item.ExecutionTimeStart < VeridateDate).ToList();
        //                if (ThisScript != null && ThisScript.Count > 0)
        //                {
        //                    foreach (ScriptTB ScriptNode in ThisScript)
        //                    {
        //                        MainScriptPoolDB.Remove(ScriptNode);
        //                        db.ScriptTBs.Remove(ScriptNode);
        //                    }

        //                    db.SaveChanges();
        //                }
        //            }
        //            catch { }

        //            Thread.Sleep(10000);
        //        }
        //    });
        //}
    }
}
