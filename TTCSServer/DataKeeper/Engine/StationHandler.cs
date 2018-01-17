using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using DataKeeper;
using System.Threading;
using DataKeeper.Engine.Command;
using DataKeeper.Interface;
using System.Web.Script.Serialization;
using AstroNET.QueueSchedule;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DataKeeper.Engine
{
    public class StationHandler
    {
        public STATIONNAME StationName;
        public String StationSessionID = null;
        public Boolean IsStationConnected = false;
        public Object ServerCallBackObject = null;

        public List<DEVICEMAPPER> AvaliableDevices = null;
        public ConcurrentDictionary<DEVICEMAPPER, Object> DeviceStroage;

        List<List<ScriptStructureNew>> Buffer;

        public int CountScript = 0;
        public int NowScript = 1;

        public Boolean IsSendingScriptToStation = false;

        int MAX_SCRIPT_PER_SEND = 20;        

        public ReturnKnowType CreateEngine(String SiteSessionID, Object ServerCallBackObject)
        {
            try
            {
                this.StationSessionID = SiteSessionID;
                this.IsStationConnected = true;
                this.IsSendingScriptToStation = false;
                this.ServerCallBackObject = ServerCallBackObject;

                
                return ReturnKnowType.DefineReturn(ReturnStatus.SUCESSFUL, null);
            }
            catch (Exception e)
            {
                return ReturnKnowType.DefineReturn(ReturnStatus.FAILED, "(#Si001) Failed to create site engine see. (" + e.Message + ")");
            }
        }

        public List<DEVICEMAPPER> GetAvaliableDevices()
        {
            return AvaliableDevices;
        }

        public DEVICECATEGORY GetDeviceCategoryByDeviceName(DEVICENAME DeviceName)
        {
            if (DeviceName == DEVICENAME.NULL) return DEVICECATEGORY.NULL;

            return AvaliableDevices.FirstOrDefault(Item => Item.DeviceName == DeviceName).DeviceCategory;
        }

        public void CreateEngine(STATIONNAME StationName, List<DEVICEMAPPER> AvaliableDevices)
        {
            this.StationName = StationName;
            this.AvaliableDevices = AvaliableDevices;
            DeviceStroage = new ConcurrentDictionary<DEVICEMAPPER, Object>();

            foreach (DEVICEMAPPER ThisDevice in AvaliableDevices)
                switch (ThisDevice.DeviceCategory)
                {
                    case DEVICECATEGORY.TS700MM:
                        {
                            ConcurrentDictionary<TS700MM, INFORMATIONSTRUCT> TS700MMObject = new ConcurrentDictionary<TS700MM, INFORMATIONSTRUCT>();
                            List<TS700MM> InformationFields = Enum.GetValues(typeof(TS700MM)).Cast<TS700MM>().ToList();

                            foreach (TS700MM ThisFieldName in InformationFields)
                                TS700MMObject.TryAdd(ThisFieldName, CreateNewInformationField(DEVICECATEGORY.TS700MM, ThisDevice.DeviceName, ThisFieldName));

                            DeviceStroage.TryAdd(ThisDevice, TS700MMObject);
                            break;
                        }
                    case DEVICECATEGORY.ASTROHEVENDOME:
                        {
                            ConcurrentDictionary<ASTROHEVENDOME, INFORMATIONSTRUCT> ASTROHEVENDOMEObject = new ConcurrentDictionary<ASTROHEVENDOME, INFORMATIONSTRUCT>();
                            List<ASTROHEVENDOME> InformationFields = Enum.GetValues(typeof(ASTROHEVENDOME)).Cast<ASTROHEVENDOME>().ToList();

                            foreach (ASTROHEVENDOME ThisFieldName in InformationFields)
                                ASTROHEVENDOMEObject.TryAdd(ThisFieldName, CreateNewInformationField(DEVICECATEGORY.ASTROHEVENDOME, ThisDevice.DeviceName, ThisFieldName));

                            DeviceStroage.TryAdd(ThisDevice, ASTROHEVENDOMEObject);
                            break;
                        }
                    case DEVICECATEGORY.WEATHERSTATION:
                        {
                            ConcurrentDictionary<WEATHERSTATION, INFORMATIONSTRUCT> WEATHERSTATIONObject = new ConcurrentDictionary<WEATHERSTATION, INFORMATIONSTRUCT>();
                            List<WEATHERSTATION> InformationFields = Enum.GetValues(typeof(WEATHERSTATION)).Cast<WEATHERSTATION>().ToList();

                            foreach (WEATHERSTATION ThisFieldName in InformationFields)
                                WEATHERSTATIONObject.TryAdd(ThisFieldName, CreateNewInformationField(DEVICECATEGORY.WEATHERSTATION, ThisDevice.DeviceName, ThisFieldName));

                            DeviceStroage.TryAdd(ThisDevice, WEATHERSTATIONObject);
                            break;
                        }
                    case DEVICECATEGORY.CCTV:
                        {
                            ConcurrentDictionary<CCTV, INFORMATIONSTRUCT> CCTVObject = new ConcurrentDictionary<CCTV, INFORMATIONSTRUCT>();
                            List<CCTV> InformationFields = Enum.GetValues(typeof(CCTV)).Cast<CCTV>().ToList();

                            foreach (CCTV ThisFieldName in InformationFields)
                                CCTVObject.TryAdd(ThisFieldName, CreateNewInformationField(DEVICECATEGORY.CCTV, ThisDevice.DeviceName, ThisFieldName));

                            DeviceStroage.TryAdd(ThisDevice, CCTVObject);
                            break;
                        }
                    case DEVICECATEGORY.IMAGING:
                        {
                            ConcurrentDictionary<IMAGING, INFORMATIONSTRUCT> IMAGINGObject = new ConcurrentDictionary<IMAGING, INFORMATIONSTRUCT>();
                            List<IMAGING> InformationFields = Enum.GetValues(typeof(IMAGING)).Cast<IMAGING>().ToList();

                            foreach (IMAGING ThisFieldName in InformationFields)
                                IMAGINGObject.TryAdd(ThisFieldName, CreateNewInformationField(DEVICECATEGORY.IMAGING, ThisDevice.DeviceName, ThisFieldName));

                            DeviceStroage.TryAdd(ThisDevice, IMAGINGObject);
                            break;
                        }
                    case DEVICECATEGORY.LANOUTLET:
                        {
                            ConcurrentDictionary<LANOUTLET, INFORMATIONSTRUCT> LANOUTLETObject = new ConcurrentDictionary<LANOUTLET, INFORMATIONSTRUCT>();
                            List<LANOUTLET> InformationFields = Enum.GetValues(typeof(LANOUTLET)).Cast<LANOUTLET>().ToList();

                            foreach (LANOUTLET ThisFieldName in InformationFields)
                                LANOUTLETObject.TryAdd(ThisFieldName, CreateNewInformationField(DEVICECATEGORY.LANOUTLET, ThisDevice.DeviceName, ThisFieldName));

                            DeviceStroage.TryAdd(ThisDevice, LANOUTLETObject);
                            break;
                        }
                    case DEVICECATEGORY.GPS:
                        {
                            ConcurrentDictionary<GPS, INFORMATIONSTRUCT> GPSObject = new ConcurrentDictionary<GPS, INFORMATIONSTRUCT>();
                            List<GPS> InformationFields = Enum.GetValues(typeof(GPS)).Cast<GPS>().ToList();

                            foreach (GPS ThisFieldName in InformationFields)
                                GPSObject.TryAdd(ThisFieldName, CreateNewInformationField(DEVICECATEGORY.GPS, ThisDevice.DeviceName, ThisFieldName));

                            DeviceStroage.TryAdd(ThisDevice, GPSObject);
                            break;
                        }
                    case DEVICECATEGORY.SQM:
                        {
                            ConcurrentDictionary<SQM, INFORMATIONSTRUCT> SQMObject = new ConcurrentDictionary<SQM, INFORMATIONSTRUCT>();
                            List<SQM> InformationFields = Enum.GetValues(typeof(SQM)).Cast<SQM>().ToList();

                            foreach (SQM ThisFieldName in InformationFields)
                                SQMObject.TryAdd(ThisFieldName, CreateNewInformationField(DEVICECATEGORY.SQM, ThisDevice.DeviceName, ThisFieldName));

                            DeviceStroage.TryAdd(ThisDevice, SQMObject);
                            break;
                        }
                    case DEVICECATEGORY.SEEING:
                        {
                            ConcurrentDictionary<SEEING, INFORMATIONSTRUCT> SEEINGObject = new ConcurrentDictionary<SEEING, INFORMATIONSTRUCT>();
                            List<SEEING> InformationFields = Enum.GetValues(typeof(SEEING)).Cast<SEEING>().ToList();

                            foreach (SEEING ThisFieldName in InformationFields)
                                SEEINGObject.TryAdd(ThisFieldName, CreateNewInformationField(DEVICECATEGORY.SEEING, ThisDevice.DeviceName, ThisFieldName));

                            DeviceStroage.TryAdd(ThisDevice, SEEINGObject);
                            break;
                        }
                    case DEVICECATEGORY.ALLSKY:
                        {
                            ConcurrentDictionary<ALLSKY, INFORMATIONSTRUCT> ALLSKYObject = new ConcurrentDictionary<ALLSKY, INFORMATIONSTRUCT>();
                            List<ALLSKY> InformationFields = Enum.GetValues(typeof(ALLSKY)).Cast<ALLSKY>().ToList();

                            foreach (ALLSKY ThisFieldName in InformationFields)
                                ALLSKYObject.TryAdd(ThisFieldName, CreateNewInformationField(DEVICECATEGORY.ALLSKY, ThisDevice.DeviceName, ThisFieldName));

                            DeviceStroage.TryAdd(ThisDevice, ALLSKYObject);
                            break;
                        }
                    case DEVICECATEGORY.ASTROCLIENT:
                        {
                            ConcurrentDictionary<ASTROCLIENT, INFORMATIONSTRUCT> ASTROCLIENTObject = new ConcurrentDictionary<ASTROCLIENT, INFORMATIONSTRUCT>();
                            List<ASTROCLIENT> InformationFields = Enum.GetValues(typeof(ASTROCLIENT)).Cast<ASTROCLIENT>().ToList();

                            foreach (ASTROCLIENT ThisFieldName in InformationFields)
                                ASTROCLIENTObject.TryAdd(ThisFieldName, CreateNewInformationField(DEVICECATEGORY.ASTROCLIENT, ThisDevice.DeviceName, ThisFieldName));

                            DeviceStroage.TryAdd(ThisDevice, ASTROCLIENTObject);
                            break;
                        }
                    case DEVICECATEGORY.ASTROSERVER:
                        {
                            ConcurrentDictionary<ASTROSERVER, INFORMATIONSTRUCT> ASTROSERVERObject = new ConcurrentDictionary<ASTROSERVER, INFORMATIONSTRUCT>();
                            List<ASTROSERVER> InformationFields = Enum.GetValues(typeof(ASTROSERVER)).Cast<ASTROSERVER>().ToList();

                            foreach (ASTROSERVER ThisFieldName in InformationFields)
                                ASTROSERVERObject.TryAdd(ThisFieldName, CreateNewInformationField(DEVICECATEGORY.ASTROSERVER, ThisDevice.DeviceName, ThisFieldName));

                            DeviceStroage.TryAdd(ThisDevice, ASTROSERVERObject);
                            break;
                        }
                }
        }

        private INFORMATIONSTRUCT CreateNewInformationField(DEVICECATEGORY DeviceCategory, DEVICENAME DeviceName, dynamic FieldName)
        {
            INFORMATIONSTRUCT NewField = new INFORMATIONSTRUCT();
            NewField.StationName = StationName;
            NewField.DeviceCategory = DeviceCategory;
            NewField.DeviceName = DeviceName;
            NewField.FieldName = FieldName;
            NewField.Value = null;
            NewField.UpdateTime = null;
            NewField.ClientSubscribe = new ConcurrentDictionary<string, object>();

            return NewField;
        }

        public Boolean CancelScript(String TargetID)
        {
            if (ServerCallBackObject == null)
            {
                Console.WriteLine("Station not connected.");
                return false;
            }

            Console.WriteLine("TargetID: " + TargetID + " Cancled");

            MethodInfo MInfo = ServerCallBackObject.GetType().GetMethod("OnCancleScript");
            MInfo.Invoke(ServerCallBackObject, new Object[] { TargetID });

            return true;
        }

        public Boolean NewScriptInformation(List<ScriptStructureNew> ScriptCollections, out String Message)
        {
            if (ServerCallBackObject == null)
            {
                Message = "Station not connected.";
                return false;
            }

            String OutputMessage = "";         

            Boolean ResultState = false;
            List<ScriptStructureNew> ScriptStructureTemp = null;
            Buffer = new List<List<ScriptStructureNew>>();

            ScriptStructureTemp = new List<ScriptStructureNew>();

            int i = 0;
                
            foreach (ScriptStructureNew ScriptCollection in ScriptCollections)
            {
                ScriptStructureTemp.Add(ScriptCollection);

                if (i >= (MAX_SCRIPT_PER_SEND-1))
                {
                    Buffer.Add(ScriptStructureTemp);
                    ScriptStructureTemp = new List<ScriptStructureNew>();
                    i = 0;
                }

                ++i;
            }

            if(i != 0)
            {
                Buffer.Add(ScriptStructureTemp);
            }

            Boolean IsHaveNextScript = ((Buffer.Count-1) > 0 ? true : false);

            foreach (List<ScriptStructureNew> ScriptCollection in Buffer)
            {
                Task TaskPost = Task.Run(() =>
                {
                    try
                    {
                        this.IsSendingScriptToStation = true;
                        NowScript = 1;
                        CountScript = Buffer.Count;
                        
                        String json = new JavaScriptSerializer().Serialize(ScriptCollection);
                        String jSonC = StringCompression.CompressString(json);

                        Console.WriteLine("Sending Script to ---> " + StationName + " (Parts: " + CountScript + ") | (O: " + json.Length + " | C: " + jSonC.Length + ")");

                        MethodInfo MInfo = ServerCallBackObject.GetType().GetMethod("OnNewScript");
                        MInfo.Invoke(ServerCallBackObject, new Object[] { jSonC, CountScript, IsHaveNextScript });
                        ResultState = true;
                        Buffer.RemoveAt(0);

                        if(!IsHaveNextScript)
                        {
                            ScriptEngine.scriptConfigure.FirstOrDefault(Item => Item.config_name == StationName.ToString()).config_status = true;                            

                            this.IsSendingScriptToStation = false;
                            OutputMessage = "Script Sent to Station Successfully.";
                            Console.WriteLine("Sending Script to ---> " + StationName + " is successfully");
                        }
                        else
                        {
                            NowScript++;
                            OutputMessage = "Script Sent to Station (Continue).";
                        }                        
                    }
                    catch (Exception e)
                    {
                        this.IsSendingScriptToStation = false;
                        OutputMessage = e.Message;
                    }
                });

                if (!TaskPost.Wait(1000))
                {
                    this.IsSendingScriptToStation = false;
                    OutputMessage = "The TTCS Client is timeout to response due to network problem or TTCS Client is lost connection.";
                }
                else
                {
                    if (ResultState)
                        OutputMessage = "Send script to station successful";
                    else
                    {
                        this.IsSendingScriptToStation = false;
                        OutputMessage = "An erroe occur because (" + OutputMessage + ")";
                    }
                }

                Message = OutputMessage;

                break;
            }          

            Message = OutputMessage;

            return ResultState;
        }

        public Boolean NextScriptInformation(out String Message)
        {
            if (ServerCallBackObject == null)
            {
                Message = "Station not connected.";
                return false;
            }

            String OutputMessage = "";

            Boolean ResultState = false;

            Boolean IsHaveNextScript = ((Buffer.Count - 1) > 0 ? true : false);

            foreach (List<ScriptStructureNew> ScriptCollection in Buffer)
            {            
                Task TaskPost = Task.Run(() =>
                {
                    try
                    {
                        this.IsSendingScriptToStation = true;
                        
                        String json = new JavaScriptSerializer().Serialize(ScriptCollection);
                        String jSonC = StringCompression.CompressString(json);

                        Console.WriteLine("Sending Script to ---> " + StationName + " (Continue / " + Buffer.Count + " Lefts) | (O: " + json.Length + " | C: " + jSonC.Length + ")");

                        MethodInfo MInfo = ServerCallBackObject.GetType().GetMethod("OnNextScript");
                        MInfo.Invoke(ServerCallBackObject, new Object[] { jSonC, NowScript, IsHaveNextScript });
                        ResultState = true;
                        Buffer.RemoveAt(0);

                        if (!IsHaveNextScript)
                        {
                            ScriptEngine.scriptConfigure.FirstOrDefault(Item => Item.config_name == StationName.ToString()).config_status = true;
                            this.IsSendingScriptToStation = false;
                            OutputMessage = "Script Sent to Station Successfully.";
                            Console.WriteLine("Sending Script to ---> " + StationName + " is successfully");
                        }
                        else
                        {
                            NowScript++;
                            OutputMessage = "Script Sent to Station (Continue).";
                        }
                    }
                    catch (Exception e)
                    {
                        this.IsSendingScriptToStation = false;
                        OutputMessage = e.Message;
                    }
                });

                if (!TaskPost.Wait(1000))
                {
                    this.IsSendingScriptToStation = false;
                    OutputMessage = "The TTCS Client is timeout to response due to network problem or TTCS Client is lost connection.";
                }
                else
                {
                    if (ResultState)
                        OutputMessage = "Send script to station successful";
                    else
                    {
                        this.IsSendingScriptToStation = false;
                        OutputMessage = "An erroe occur because (" + OutputMessage + ")";
                    }
                }

                Message = OutputMessage;

                break;
            }
            
            Message = OutputMessage;

            return ResultState;
        }

        public void CheckLastesInformation(long DateTimeUTC)
        {
            String OutputMessage = "";
            if (ServerCallBackObject == null)
            {
                return;
            }

            Task TaskPost = Task.Run(() =>
            {
                try
                {
                    MethodInfo MInfo = ServerCallBackObject.GetType().GetMethod("OnCheckLastestInformation");
                    try
                    {
                        MInfo.Invoke(ServerCallBackObject, new Object[] { DateTimeUTC });
                    }
                    catch(Exception e)
                    {
                        Console.WriteLine(e.Message);
                    }                   
                }
                catch (Exception e)
                {
                    OutputMessage = e.Message;
                }
            });

            if (!TaskPost.Wait(1000))
                OutputMessage = "The TTCS Client is timeout to response due to network problem or TTCS Client is lost connection.";
        }

        public void NewTS700MMInformation(DEVICENAME DeviceName, TS700MM FieldName, Object Value, DateTime DataTimestamp)
        {
            if (DBEngine._client == null)
                return;

            ConcurrentDictionary<TS700MM, INFORMATIONSTRUCT> ExistingInformation = (ConcurrentDictionary<TS700MM, INFORMATIONSTRUCT>)DeviceStroage.FirstOrDefault(Item => Item.Key.DeviceName == DeviceName && Item.Key.DeviceCategory == DEVICECATEGORY.TS700MM).Value;
            if (ExistingInformation != null)
            {
                INFORMATIONSTRUCT ThisField = ExistingInformation.FirstOrDefault(Item => Item.Key == FieldName).Value;
                if (ThisField != null)
                {
                    UpdateInformation(ThisField, DeviceName, Value, DataTimestamp);
                    WebSockets.ReturnWebSubscribe(StationName, DeviceName, FieldName.ToString(), Value, DataTimestamp);
                }
            }
        }

        public void NewASTROHEVENDOMEInformation(DEVICENAME DeviceName, ASTROHEVENDOME FieldName, Object Value, DateTime DataTimestamp)
        {
            if (DBEngine._client == null)
                return;

            ConcurrentDictionary<ASTROHEVENDOME, INFORMATIONSTRUCT> ExistingInformation = (ConcurrentDictionary<ASTROHEVENDOME, INFORMATIONSTRUCT>)DeviceStroage.FirstOrDefault(Item => Item.Key.DeviceName == DeviceName && Item.Key.DeviceCategory == DEVICECATEGORY.ASTROHEVENDOME).Value;
            if (ExistingInformation != null)
            {
                INFORMATIONSTRUCT ThisField = ExistingInformation.FirstOrDefault(Item => Item.Key == FieldName).Value;
                if (ThisField != null)
                {
                    DBEngine.insert(StationName.ToString(), DeviceName.ToString(), FieldName.ToString(), Value.ToString(), DataTimestamp);

                    String State_Temp = Value.ToString();

                    if (State_Temp.Equals("Open") || State_Temp.Equals("Opening"))
                    {
                        DBEngine.insert_dome_open(StationName.ToString(), DeviceName.ToString(), FieldName.ToString(), Value.ToString(), DataTimestamp, DataTimestamp);
                    }
                    else if (State_Temp.Equals("Closed"))
                    {
                        DBEngine.insert_dome_close(StationName.ToString(), DeviceName.ToString(), FieldName.ToString(), Value.ToString(), DataTimestamp, DataTimestamp);
                    }

                    UpdateInformation(ThisField, DeviceName, Value, DataTimestamp);
                    WebSockets.ReturnWebSubscribe(StationName, DeviceName, FieldName.ToString(), Value, DataTimestamp);
                }
                UpdateInformation(ThisField, DeviceName, Value, DataTimestamp);
            }
        }

        public void NewWEATHERSTATIONInformation(DEVICENAME DeviceName, WEATHERSTATION FieldName, Object Value, DateTime DataTimestamp)
        {
            if (DBEngine._client == null)
                return;

            ConcurrentDictionary<WEATHERSTATION, INFORMATIONSTRUCT> ExistingInformation = (ConcurrentDictionary<WEATHERSTATION, INFORMATIONSTRUCT>)DeviceStroage.FirstOrDefault(Item => Item.Key.DeviceName == DeviceName && Item.Key.DeviceCategory == DEVICECATEGORY.WEATHERSTATION).Value;
            if (ExistingInformation != null)
            {
                INFORMATIONSTRUCT ThisField = ExistingInformation.FirstOrDefault(Item => Item.Key == FieldName).Value;
                if (ThisField != null)
                {
                    DBEngine.insert(StationName.ToString(), DeviceName.ToString(), FieldName.ToString(), Value.ToString(), DataTimestamp);

                    UpdateInformation(ThisField, DeviceName, Value, DataTimestamp);
                    WebSockets.ReturnWebSubscribe(StationName, DeviceName, FieldName.ToString(), Value, DataTimestamp);
                }
            }
        }

        public void NewCCTVInformation(DEVICENAME DeviceName, CCTV FieldName, Object Value, DateTime DataTimestamp)
        {
            if (DBEngine._client == null)
                return;

            ConcurrentDictionary<CCTV, INFORMATIONSTRUCT> ExistingInformation = (ConcurrentDictionary<CCTV, INFORMATIONSTRUCT>)DeviceStroage.FirstOrDefault(Item => Item.Key.DeviceName == DeviceName && Item.Key.DeviceCategory == DEVICECATEGORY.CCTV).Value;
            if (ExistingInformation != null)
            {
                INFORMATIONSTRUCT ThisField = ExistingInformation.FirstOrDefault(Item => Item.Key == FieldName).Value;
                if (ThisField != null)
                {
                    UpdateInformation(ThisField, DeviceName, Value, DataTimestamp);
                    WebSockets.ReturnWebSubscribe(StationName, DeviceName, FieldName.ToString(), Value, DataTimestamp);
                }
            }
        }

        public void NewIMAGINGInformation(DEVICENAME DeviceName, IMAGING FieldName, Object Value, DateTime DataTimestamp)
        {
            if (DBEngine._client == null)
                return;

            ConcurrentDictionary<IMAGING, INFORMATIONSTRUCT> ExistingInformation = (ConcurrentDictionary<IMAGING, INFORMATIONSTRUCT>)DeviceStroage.FirstOrDefault(Item => Item.Key.DeviceName == DeviceName && Item.Key.DeviceCategory == DEVICECATEGORY.IMAGING).Value;
            if (ExistingInformation != null)
            {
                INFORMATIONSTRUCT ThisField = ExistingInformation.FirstOrDefault(Item => Item.Key == FieldName).Value;

                if (ThisField != null)
                {
                    UpdateInformation(ThisField, DeviceName, Value, DataTimestamp);
                    WebSockets.ReturnWebSubscribe(StationName, DeviceName, FieldName.ToString(), Value, DataTimestamp);
                }
            }
        }

        public void NewLANOUTLETInformation(DEVICENAME DeviceName, LANOUTLET FieldName, Object Value, DateTime DataTimestamp)
        {
            if (DBEngine._client == null)
                return;

            ConcurrentDictionary<LANOUTLET, INFORMATIONSTRUCT> ExistingInformation = (ConcurrentDictionary<LANOUTLET, INFORMATIONSTRUCT>)DeviceStroage.FirstOrDefault(Item => Item.Key.DeviceName == DeviceName && Item.Key.DeviceCategory == DEVICECATEGORY.LANOUTLET).Value;
            if (ExistingInformation != null)
            {
                INFORMATIONSTRUCT ThisField = ExistingInformation.FirstOrDefault(Item => Item.Key == FieldName).Value;
                if (ThisField != null)
                {
                    DBEngine.insert(StationName.ToString(), DeviceName.ToString(), FieldName.ToString(), Value.ToString(), DataTimestamp);

                    UpdateInformation(ThisField, DeviceName, Value, DataTimestamp);
                    WebSockets.ReturnWebSubscribe(StationName, DeviceName, FieldName.ToString(), Value, DataTimestamp);
                }
            }
        }

        public void NewGPSInformation(String DataGroupID, DEVICENAME DeviceName, GPS FieldName, Object Value, DateTime DataTimestamp, Boolean IsHistory)
        {
            if (DBEngine._client == null)
                return;

            ConcurrentDictionary<GPS, INFORMATIONSTRUCT> ExistingInformation = (ConcurrentDictionary<GPS, INFORMATIONSTRUCT>)DeviceStroage.FirstOrDefault(Item => Item.Key.DeviceName == DeviceName && Item.Key.DeviceCategory == DEVICECATEGORY.GPS).Value;
            if (ExistingInformation != null)
            {
                INFORMATIONSTRUCT ThisField = ExistingInformation.FirstOrDefault(Item => Item.Key == FieldName).Value;
                if (ThisField != null)
                {
                    if (!IsHistory)
                    {
                        UpdateInformation(ThisField, DeviceName, Value, DataTimestamp);
                        WebSockets.ReturnWebSubscribe(StationName, DeviceName, FieldName.ToString(), Value, DataTimestamp);
                    }
                    else
                    {
                        //Update Database
                    }
                }
            }
        }

        public void ReturnAckState(String DataGroupID, DEVICENAME DeviceName)
        {
            ServerInformationAck.ReturnNTPAckToStation(DeviceName, DataGroupID, ServerCallBackObject);
        }

        public void NewSQMInformation(DEVICENAME DeviceName, SQM FieldName, Object Value, DateTime DataTimestamp)
        {
            if (DBEngine._client == null)
                return;

            ConcurrentDictionary<SQM, INFORMATIONSTRUCT> ExistingInformation = (ConcurrentDictionary<SQM, INFORMATIONSTRUCT>)DeviceStroage.FirstOrDefault(Item => Item.Key.DeviceName == DeviceName && Item.Key.DeviceCategory == DEVICECATEGORY.SQM).Value;
            if (ExistingInformation.Values != null)
            {
                INFORMATIONSTRUCT ThisField = ExistingInformation.FirstOrDefault(Item => Item.Key == FieldName).Value;
                if (ThisField != null)
                {
                    Value = Value.ToString().Trim();

                    var Unit = "";
                    if (FieldName.ToString() == "SQM_SKYBRIGNESS_DATA")
                    {
                        Unit = "m";
                        Value = Value.ToString().Substring(0, Value.ToString().Length - 1);
                    }
                    else if (FieldName.ToString() == "SQM_FREQUENCYOFSENSOR_DATA")
                    {
                        Unit = "Hz";
                        Value = Value.ToString().Substring(0, Value.ToString().Length - 2);
                    }
                    else if (FieldName.ToString() == "SQM_PREIODKHZ_DATA")
                    {
                        Unit = "c";
                        Value = Value.ToString().Substring(0, Value.ToString().Length - 1);
                    }
                    else if (FieldName.ToString() == "SQM_PERIODMS_DATA")
                    {
                        Unit = "s";
                        Value = Value.ToString().Substring(0, Value.ToString().Length - 1);
                    }
                    else if (FieldName.ToString() == "SQM_TEMPERATURE_DATA")
                    {
                        Unit = "C";
                        Value = Value.ToString().Substring(0, Value.ToString().Length - 1);
                    }


                    DBEngine.insert_unit(StationName.ToString(), DeviceName.ToString(), FieldName.ToString(), Value.ToString(), Unit.ToString(), DataTimestamp);
                    UpdateInformation(ThisField, DeviceName, Value, DataTimestamp);
                    WebSockets.ReturnWebSubscribe(StationName, DeviceName, FieldName.ToString(), Value, DataTimestamp);
                }
            }
        }

        public void NewSEEINGInformation(DEVICENAME DeviceName, SEEING FieldName, Object Value, DateTime DataTimestamp)
        {
            if (DBEngine._client == null)
                return;

            ConcurrentDictionary<SEEING, INFORMATIONSTRUCT> ExistingInformation = (ConcurrentDictionary<SEEING, INFORMATIONSTRUCT>)DeviceStroage.FirstOrDefault(Item => Item.Key.DeviceName == DeviceName && Item.Key.DeviceCategory == DEVICECATEGORY.SEEING).Value;
            if (ExistingInformation != null)
            {
                INFORMATIONSTRUCT ThisField = ExistingInformation.FirstOrDefault(Item => Item.Key == FieldName).Value;
                if (ThisField != null)
                {
                    DBEngine.insert(StationName.ToString(), DeviceName.ToString(), FieldName.ToString(), Convert.ToBase64String((byte[])Value), DataTimestamp);
                    UpdateInformation(ThisField, DeviceName, Value, DataTimestamp);
                    WebSockets.ReturnWebSubscribe(StationName, DeviceName, FieldName.ToString(), Value, DataTimestamp);
                }
            }
        }

        public void NewALLSKYInformation(DEVICENAME DeviceName, ALLSKY FieldName, Object Value, DateTime DataTimestamp)
        {
            if (DBEngine._client == null)
                return;

            ConcurrentDictionary<ALLSKY, INFORMATIONSTRUCT> ExistingInformation = (ConcurrentDictionary<ALLSKY, INFORMATIONSTRUCT>)DeviceStroage.FirstOrDefault(Item => Item.Key.DeviceName == DeviceName && Item.Key.DeviceCategory == DEVICECATEGORY.ALLSKY).Value;
            if (ExistingInformation != null)
            {
                INFORMATIONSTRUCT ThisField = ExistingInformation.FirstOrDefault(Item => Item.Key == FieldName).Value;
                if (ThisField != null)
                {
                    DBEngine.insert(StationName.ToString(), DeviceName.ToString(), FieldName.ToString(), Convert.ToBase64String((byte[])Value), DataTimestamp);
                    UpdateInformation(ThisField, DeviceName, Value, DataTimestamp);
                    WebSockets.ReturnWebSubscribe(StationName, DeviceName, FieldName.ToString(), Value, DataTimestamp);
                }
            }
        }        

        public void NewASTROCLIENTInformation(DEVICENAME DeviceName, ASTROCLIENT FieldName, Object Value, DateTime DataTimestamp)
        {
            if (DBEngine._client == null)
                return;

            ConcurrentDictionary<ASTROCLIENT, INFORMATIONSTRUCT> ExistingInformation = (ConcurrentDictionary<ASTROCLIENT, INFORMATIONSTRUCT>)DeviceStroage.FirstOrDefault(Item => Item.Key.DeviceName == DeviceName && Item.Key.DeviceCategory == DEVICECATEGORY.ASTROCLIENT).Value;

            if (ASTROCLIENT.ASTROCLIENT_LASTESTSCRIPT_RECIVED == FieldName ||
                ASTROCLIENT.ASTROCLIENT_LASTESTEXECTIONCOMMAND == FieldName ||
                ASTROCLIENT.ASTROCLIENT_LASTESTEXECUTIONSCRIPT_STATUS == FieldName)
            {
                ScriptManager.ScriptInformationIdentification(StationName, DeviceName, FieldName, Value, DataTimestamp);
            }

            else if (ASTROCLIENT.ASTROCLIENT_LASTESTUSER_NAME == FieldName)
            {
                String[] temp_value = Value.ToString().Split(',');
                DateTime dt = Convert.ToDateTime(temp_value[2]);

                //ASTROCLIENT.ASTROCLIENT_LASTESTUSER_STATE

                if (temp_value[0].ToString() != "NULL")
                {
                    if (temp_value[1].ToString() == "LOGIN")
                    {
                        DBEngine.insert_user_login(StationName.ToString(), DeviceName.ToString(), FieldName.ToString(), temp_value[0].ToString(), temp_value[1].ToString(), dt, DataTimestamp);
                    }
                    else
                    {
                        DBEngine.insert_user_logout(StationName.ToString(), DeviceName.ToString(), FieldName.ToString(), temp_value[0].ToString(), temp_value[1].ToString(), dt, DataTimestamp);
                    }
                }
            }

            if (ExistingInformation != null)
            {
                INFORMATIONSTRUCT ThisField = ExistingInformation.FirstOrDefault(Item => Item.Key == FieldName).Value;
                if (ThisField != null)
                {
                    UpdateInformation(ThisField, DeviceName, Value, DataTimestamp);
                    WebSockets.ReturnWebSubscribe(StationName, DeviceName, FieldName.ToString(), Value, DataTimestamp);
                }
            }
        }

        public void NewASTROSERVERInformation(DEVICENAME DeviceName, ASTROSERVER FieldName, Object Value, DateTime DataTimestamp)
        {
            if (DBEngine._client == null)
                return;

            ConcurrentDictionary<ASTROSERVER, INFORMATIONSTRUCT> ExistingInformation = (ConcurrentDictionary<ASTROSERVER, INFORMATIONSTRUCT>)DeviceStroage.FirstOrDefault(Item => Item.Key.DeviceName == DeviceName && Item.Key.DeviceCategory == DEVICECATEGORY.ASTROSERVER).Value;
            if (ExistingInformation != null)
            {
                INFORMATIONSTRUCT ThisField = ExistingInformation.FirstOrDefault(Item => Item.Key == FieldName).Value;
                if (ThisField != null)
                {
                    UpdateInformation(ThisField, DeviceName, Value, DataTimestamp);
                    WebSockets.ReturnWebSubscribe(StationName, DeviceName, FieldName.ToString(), Value, DataTimestamp);
                }
            }
        }

        public void StationConnected()
        {           
            ConcurrentDictionary<ASTROCLIENT, INFORMATIONSTRUCT> ExistingInformation = (ConcurrentDictionary<ASTROCLIENT, INFORMATIONSTRUCT>)DeviceStroage.FirstOrDefault(Item => Item.Key.DeviceCategory == DEVICECATEGORY.ASTROCLIENT).Value;
            if (ExistingInformation != null)
            {
                INFORMATIONSTRUCT ThisField = ExistingInformation.FirstOrDefault(Item => Item.Key == ASTROCLIENT.ASTROCLIENT_CONNECTED).Value;
                if (ThisField != null)
                {
                    ThisField.Value = true;
                    ThisField.UpdateTime = DateTime.UtcNow;

                    WebSockets.ReturnWebSubscribe(StationName, ThisField.DeviceName, ThisField.FieldName.ToString(), ThisField.Value, ThisField.UpdateTime.Value);
                    UIHandler.DisplayToUI(StationName, ThisField.DeviceName, ThisField);
                }
            }
        }

        public void StationDisconnected()
        {
            this.IsStationConnected = false;
            this.IsSendingScriptToStation = false;

            ConcurrentDictionary<ASTROCLIENT, INFORMATIONSTRUCT> ExistingInformation = (ConcurrentDictionary<ASTROCLIENT, INFORMATIONSTRUCT>)DeviceStroage.FirstOrDefault(Item => Item.Key.DeviceCategory == DEVICECATEGORY.ASTROCLIENT).Value;
            if (ExistingInformation != null)
            {
                INFORMATIONSTRUCT ThisField = ExistingInformation.FirstOrDefault(Item => Item.Key == ASTROCLIENT.ASTROCLIENT_CONNECTED).Value;
                if (ThisField != null)
                {
                    ThisField.Value = false;
                    ThisField.UpdateTime = DateTime.UtcNow;

                    WebSockets.ReturnWebSubscribe(StationName, ThisField.DeviceName, ThisField.FieldName.ToString(), ThisField.Value, ThisField.UpdateTime.Value);
                    UIHandler.DisplayToUI(StationName, ThisField.DeviceName, ThisField);
                }
            }
        }

        private void UpdateInformation(INFORMATIONSTRUCT ThisField, DEVICENAME DeviceName, Object Value, DateTime DataTimestamp)
        {
            ThisField.Value = Value;
            ThisField.UpdateTime = DataTimestamp;
            UIHandler.DisplayToUI(StationName, DeviceName, ThisField);
        }

        #region Set Information

        public ReturnKnowType RelayGRB(String Ra, String Dec, String FOV, DateTime UpdateTime)
        {
            Boolean ResultState = false;
            String Message = "";

            Task TTask = Task.Run(() =>
            {
                try
                {
                    MethodInfo MInfo = ServerCallBackObject.GetType().GetMethod("OnGRBTTCS");
                    MInfo.Invoke(ServerCallBackObject, new Object[] { Ra, Dec, FOV, UpdateTime });
                    ResultState = true;
                }
                catch (Exception e)
                {
                    Message = e.Message;
                }
            });

            if (!TTask.Wait(10))
                return CreateKnowTypeMessage("The TTCS Client is timeout to response due to network problem or TTCS Client is lost connection.", ReturnStatus.FAILED);
            else
            {
                if (ResultState)
                    return CreateKnowTypeMessage(null, ReturnStatus.SUCESSFUL);
                else
                    return CreateKnowTypeMessage("An erroe occur because (" + Message + ")", ReturnStatus.FAILED);
            }
        }

        public ReturnKnowType SendScriptToStation(ScriptStructure[] ThisScriptList)
        {
            Boolean ResultState = false;
            String Message = "";

            Task TaskPost = Task.Run(() =>
            {
                try
                {
                    MethodInfo MInfo = ServerCallBackObject.GetType().GetMethod("OnScriptSET");
                    MInfo.Invoke(ServerCallBackObject, new Object[] { ThisScriptList.ToList() });
                    ResultState = true;
                }
                catch (Exception e)
                {
                    Message = e.Message;
                }
            });

            if (!TaskPost.Wait(10))
                return CreateKnowTypeMessage("The TTCS Client is timeout to response due to network problem or TTCS Client is lost connection.", ReturnStatus.FAILED);
            else
            {
                if (ResultState)
                    return CreateKnowTypeMessage(null, ReturnStatus.SUCESSFUL);
                else
                    return CreateKnowTypeMessage("An erroe occur because (" + Message + ")", ReturnStatus.FAILED);
            }
        }

        public ReturnKnowType UpdateStationUser(String UserID, String UserName, String UserLoginName, String UserLoginPassword, String UserPermissionType, String UserStationPermission, DATAACTION UserAction)
        {
            Boolean ResultState = false;
            String Message = "";

            Task TaskPost = Task.Run(() =>
            {
                try
                {
                    MethodInfo MInfo = ServerCallBackObject.GetType().GetMethod("OnUpdateUser");
                    MInfo.Invoke(ServerCallBackObject, new Object[] { UserID, UserName, UserLoginName, UserLoginPassword, UserPermissionType, UserStationPermission, UserAction });
                    ResultState = true;
                }
                catch (Exception e)
                {
                    Message = e.Message;
                }
            });

            if (!TaskPost.Wait(10000))
                return CreateKnowTypeMessage("The Astro Client is timeout to response due to network problem or Astro Client is lost connection.", ReturnStatus.FAILED);
            else
            {
                if (ResultState)
                    return CreateKnowTypeMessage(null, ReturnStatus.SUCESSFUL);
                else
                    return CreateKnowTypeMessage("An erroe occur because (" + Message + ")", ReturnStatus.FAILED);
            }
        }

        public ReturnKnowType RelayCommandToStation(DEVICECATEGORY DeviceCategory, DEVICENAME DeviceName, dynamic CommandName, Object[] Values)
        {
            Boolean ResultState = false;
            String Message = "";

            Task TaskPost = Task.Run(() =>
            {
                try
                {
                    if (DeviceName == DEVICENAME.ASTROPARK_SERVER)
                    {
                        if ((ASTROSERVERSET)CommandName == ASTROSERVERSET.ASTROSERVER_DATABASE_SYNC)
                        {
                            List<Object[]> AllValue = DatabaseSynchronization.SyncDataFromServer(Values[0].ToString(), Values[1].ToString());

                            MethodInfo MInfo = ServerCallBackObject.GetType().GetMethod("OnDatabaseSync");
                            MInfo.Invoke(ServerCallBackObject, new Object[] { AllValue });
                            ResultState = true;
                        }
                    }
                    else
                    {
                        String MethodName = null;
                        switch (DeviceCategory)
                        {
                            case DEVICECATEGORY.TS700MM: MethodName = "OnTS700MMSET"; break;
                            case DEVICECATEGORY.IMAGING: MethodName = "OnIMAGINGSET"; break;
                            case DEVICECATEGORY.ASTROHEVENDOME: MethodName = "OnDOMESET"; break;
                            case DEVICECATEGORY.LANOUTLET: MethodName = "OnLANOUTLETSET"; break;
                        }

                        MethodInfo MInfo = ServerCallBackObject.GetType().GetMethod(MethodName);
                        MInfo.Invoke(ServerCallBackObject, new Object[] { StationName, DeviceName, CommandName, Values, DateTime.UtcNow });
                        ResultState = true;
                    }
                }
                catch (Exception e)
                {
                    Message = e.Message;
                }
            });

            if (!TaskPost.Wait(10))
                return CreateKnowTypeMessage("The TTCS Client is timeout to response due to network problem or TTCS Client is lost connection.", ReturnStatus.FAILED);
            else
            {
                if (ResultState)
                    return CreateKnowTypeMessage(null, ReturnStatus.SUCESSFUL);
                else
                    return CreateKnowTypeMessage("An erroe occur because (" + Message + ")", ReturnStatus.FAILED);
            }
        }

        private static ReturnKnowType CreateKnowTypeMessage(String Message, ReturnStatus Status)
        {
            ReturnKnowType ThisKnowType = new ReturnKnowType();
            ThisKnowType.ReturnDateTime = DateTime.UtcNow;
            ThisKnowType.ReturnMessage = Message;
            ThisKnowType.ReturnType = Status;
            ThisKnowType.ReturnValue = null;

            return ThisKnowType;
        }

        #endregion

        #region Get Information

        public List<OUTPUTSTRUCT> GetInformation()
        {
            List<OUTPUTSTRUCT> InformationList = new List<OUTPUTSTRUCT>();
            foreach (DEVICEMAPPER ThisDevice in DeviceStroage.Keys)
                InformationList.AddRange(DeviceInformationHandler(ThisDevice));

            return InformationList;
        }

        public List<OUTPUTSTRUCT> GetInformation(DEVICECATEGORY DeviceCategory)
        {
            List<OUTPUTSTRUCT> InformationList = new List<OUTPUTSTRUCT>();
            DEVICEMAPPER ThisDevice = DeviceStroage.FirstOrDefault(Item => Item.Key.DeviceCategory == DeviceCategory).Key;

            if (ThisDevice == null)
                return null;

            InformationList.AddRange(DeviceInformationHandler(ThisDevice));
            return InformationList;
        }

        public OUTPUTSTRUCT GetInformation(DEVICENAME DeviceName, dynamic FieldName)
        {
            DEVICEMAPPER ThisDevice = DeviceStroage.FirstOrDefault(Item => Item.Key.DeviceName == DeviceName).Key;
            List<OUTPUTSTRUCT> TempList = DeviceInformationHandler(ThisDevice);

            foreach (OUTPUTSTRUCT ThisInformation in TempList)
                if (ThisInformation.FieldName.ToString() == IMAGING.IMAGING_PREVIEW_BASE64.ToString())
                {
                    String T = ThisInformation.Value.ToString();
                }

            OUTPUTSTRUCT ThisOutput = TempList.FirstOrDefault(Item => Item.FieldName == FieldName.ToString());
            return ThisOutput;
        }
        
        public OUTPUTSTRUCT GetInformation(DEVICENAME DeviceName, dynamic FieldName, Object[] Parameter)
        {
            DEVICEMAPPER ThisDevice = DeviceStroage.FirstOrDefault(Item => Item.Key.DeviceName == DeviceName).Key;
            List<OUTPUTSTRUCT> TempList = DeviceInformationHandler(ThisDevice);

            foreach (OUTPUTSTRUCT ThisInformation in TempList)
                if (ThisInformation.FieldName.ToString() == IMAGING.IMAGING_PREVIEW_BASE64.ToString())
                {
                    String T = ThisInformation.Value.ToString();
                }

            OUTPUTSTRUCT ThisOutput = TempList.FirstOrDefault(Item => Item.FieldName == FieldName.ToString());
            return ThisOutput;
        }

        public INFORMATIONSTRUCT GetInformationObject(DEVICENAME DeviceName, dynamic FieldName)
        {
            //Console.WriteLine(DeviceName.ToString());

            DEVICEMAPPER ThisDevice = DeviceStroage.FirstOrDefault(Item => Item.Key.DeviceName == DeviceName).Key;

            if (ThisDevice == null) return null;

            List<INFORMATIONSTRUCT> TempList = DeviceInformationObjectHandler(ThisDevice);
            INFORMATIONSTRUCT ThisOutput = TempList.FirstOrDefault(Item => Item.FieldName.ToString() == FieldName.ToString());
            return ThisOutput;
        }

        private List<INFORMATIONSTRUCT> DeviceInformationObjectHandler(DEVICEMAPPER ThisDevice)
        {
            switch (ThisDevice.DeviceCategory)
            {
                case DEVICECATEGORY.TS700MM:
                    ConcurrentDictionary<TS700MM, INFORMATIONSTRUCT> TS700MMField = (ConcurrentDictionary<TS700MM, INFORMATIONSTRUCT>)DeviceStroage.FirstOrDefault(Item => Item.Key.DeviceCategory == ThisDevice.DeviceCategory).Value;
                    return TS700MMField.Values.ToList();
                case DEVICECATEGORY.ASTROHEVENDOME:
                    ConcurrentDictionary<ASTROHEVENDOME, INFORMATIONSTRUCT> ASTROHEVENDOMEField = (ConcurrentDictionary<ASTROHEVENDOME, INFORMATIONSTRUCT>)DeviceStroage.FirstOrDefault(Item => Item.Key.DeviceCategory == ThisDevice.DeviceCategory).Value;
                    return ASTROHEVENDOMEField.Values.ToList();
                case DEVICECATEGORY.WEATHERSTATION:
                    ConcurrentDictionary<WEATHERSTATION, INFORMATIONSTRUCT> WEATHERSTATIONField = (ConcurrentDictionary<WEATHERSTATION, INFORMATIONSTRUCT>)DeviceStroage.FirstOrDefault(Item => Item.Key.DeviceCategory == ThisDevice.DeviceCategory).Value;
                    return WEATHERSTATIONField.Values.ToList();
                case DEVICECATEGORY.CCTV:
                    ConcurrentDictionary<CCTV, INFORMATIONSTRUCT> CCTVField = (ConcurrentDictionary<CCTV, INFORMATIONSTRUCT>)DeviceStroage.FirstOrDefault(Item => Item.Key.DeviceCategory == ThisDevice.DeviceCategory).Value;
                    return CCTVField.Values.ToList();
                case DEVICECATEGORY.IMAGING:
                    ConcurrentDictionary<IMAGING, INFORMATIONSTRUCT> IMAGINGField = (ConcurrentDictionary<IMAGING, INFORMATIONSTRUCT>)DeviceStroage.FirstOrDefault(Item => Item.Key.DeviceCategory == ThisDevice.DeviceCategory).Value;
                    return IMAGINGField.Values.ToList();
                case DEVICECATEGORY.LANOUTLET:
                    ConcurrentDictionary<LANOUTLET, INFORMATIONSTRUCT> LANOUTLETField = (ConcurrentDictionary<LANOUTLET, INFORMATIONSTRUCT>)DeviceStroage.FirstOrDefault(Item => Item.Key.DeviceCategory == ThisDevice.DeviceCategory).Value;
                    return LANOUTLETField.Values.ToList();
                case DEVICECATEGORY.GPS:
                    ConcurrentDictionary<GPS, INFORMATIONSTRUCT> GPSField = (ConcurrentDictionary<GPS, INFORMATIONSTRUCT>)DeviceStroage.FirstOrDefault(Item => Item.Key.DeviceCategory == ThisDevice.DeviceCategory).Value;
                    return GPSField.Values.ToList();
                case DEVICECATEGORY.SQM:
                    ConcurrentDictionary<SQM, INFORMATIONSTRUCT> SQMField = (ConcurrentDictionary<SQM, INFORMATIONSTRUCT>)DeviceStroage.FirstOrDefault(Item => Item.Key.DeviceCategory == ThisDevice.DeviceCategory).Value;
                    return SQMField.Values.ToList();
                case DEVICECATEGORY.SEEING:
                    ConcurrentDictionary<SEEING, INFORMATIONSTRUCT> SEEINGField = (ConcurrentDictionary<SEEING, INFORMATIONSTRUCT>)DeviceStroage.FirstOrDefault(Item => Item.Key.DeviceCategory == ThisDevice.DeviceCategory).Value;
                    return SEEINGField.Values.ToList();
                case DEVICECATEGORY.ALLSKY:
                    ConcurrentDictionary<ALLSKY, INFORMATIONSTRUCT> ALLSKYField = (ConcurrentDictionary<ALLSKY, INFORMATIONSTRUCT>)DeviceStroage.FirstOrDefault(Item => Item.Key.DeviceCategory == ThisDevice.DeviceCategory).Value;
                    return ALLSKYField.Values.ToList();
                case DEVICECATEGORY.ASTROCLIENT:
                    ConcurrentDictionary<ASTROCLIENT, INFORMATIONSTRUCT> ASTROCLIENTField = (ConcurrentDictionary<ASTROCLIENT, INFORMATIONSTRUCT>)DeviceStroage.FirstOrDefault(Item => Item.Key.DeviceCategory == ThisDevice.DeviceCategory).Value;
                    return ASTROCLIENTField.Values.ToList();
                case DEVICECATEGORY.ASTROSERVER:
                    ConcurrentDictionary<ASTROSERVER, INFORMATIONSTRUCT> ASTROSERVERField = (ConcurrentDictionary<ASTROSERVER, INFORMATIONSTRUCT>)DeviceStroage.FirstOrDefault(Item => Item.Key.DeviceCategory == ThisDevice.DeviceCategory).Value;
                    return ASTROSERVERField.Values.ToList();
                default: return null;
            }
        }

        private List<OUTPUTSTRUCT> DeviceInformationHandler(DEVICEMAPPER ThisDevice)
        {
            List<OUTPUTSTRUCT> InformationList = new List<OUTPUTSTRUCT>();

            if (ThisDevice == null)
                return null;

            switch (ThisDevice.DeviceCategory)
            {
                case DEVICECATEGORY.TS700MM:
                    ConcurrentDictionary<TS700MM, INFORMATIONSTRUCT> TS700MMField = (ConcurrentDictionary<TS700MM, INFORMATIONSTRUCT>)DeviceStroage.FirstOrDefault(Item => Item.Key.DeviceCategory == ThisDevice.DeviceCategory).Value;
                    foreach (INFORMATIONSTRUCT ThisInformation in TS700MMField.Values)
                        if (ThisInformation.FieldName != TS700MM.NULL)
                            InformationList.Add(CreateOutputNode(ThisInformation));
                    break;
                case DEVICECATEGORY.ASTROHEVENDOME:
                    ConcurrentDictionary<ASTROHEVENDOME, INFORMATIONSTRUCT> ASTROHEVENDOMEField = (ConcurrentDictionary<ASTROHEVENDOME, INFORMATIONSTRUCT>)DeviceStroage.FirstOrDefault(Item => Item.Key.DeviceCategory == ThisDevice.DeviceCategory).Value;
                    foreach (INFORMATIONSTRUCT ThisInformation in ASTROHEVENDOMEField.Values)
                        if (ThisInformation.FieldName != ASTROHEVENDOME.NULL)
                            InformationList.Add(CreateOutputNode(ThisInformation));
                    break;
                case DEVICECATEGORY.WEATHERSTATION:
                    ConcurrentDictionary<WEATHERSTATION, INFORMATIONSTRUCT> WEATHERSTATIONField = (ConcurrentDictionary<WEATHERSTATION, INFORMATIONSTRUCT>)DeviceStroage.FirstOrDefault(Item => Item.Key.DeviceCategory == ThisDevice.DeviceCategory).Value;
                    foreach (INFORMATIONSTRUCT ThisInformation in WEATHERSTATIONField.Values)
                        if (ThisInformation.FieldName != WEATHERSTATION.NULL)
                            InformationList.Add(CreateOutputNode(ThisInformation));
                    break;
                case DEVICECATEGORY.CCTV:
                    ConcurrentDictionary<CCTV, INFORMATIONSTRUCT> CCTVField = (ConcurrentDictionary<CCTV, INFORMATIONSTRUCT>)DeviceStroage.FirstOrDefault(Item => Item.Key.DeviceCategory == ThisDevice.DeviceCategory).Value;
                    foreach (INFORMATIONSTRUCT ThisInformation in CCTVField.Values)
                        if (ThisInformation.FieldName != CCTV.NULL)
                            InformationList.Add(CreateOutputNode(ThisInformation));
                    break;
                case DEVICECATEGORY.IMAGING:
                    ConcurrentDictionary<IMAGING, INFORMATIONSTRUCT> IMAGINGField = (ConcurrentDictionary<IMAGING, INFORMATIONSTRUCT>)DeviceStroage.FirstOrDefault(Item => Item.Key.DeviceCategory == ThisDevice.DeviceCategory).Value;
                    foreach (INFORMATIONSTRUCT ThisInformation in IMAGINGField.Values)
                        if (ThisInformation.FieldName != IMAGING.NULL)
                            InformationList.Add(CreateOutputNode(ThisInformation));
                    break;
                case DEVICECATEGORY.LANOUTLET:
                    ConcurrentDictionary<LANOUTLET, INFORMATIONSTRUCT> LANOUTLETField = (ConcurrentDictionary<LANOUTLET, INFORMATIONSTRUCT>)DeviceStroage.FirstOrDefault(Item => Item.Key.DeviceCategory == ThisDevice.DeviceCategory).Value;
                    foreach (INFORMATIONSTRUCT ThisInformation in LANOUTLETField.Values)
                        if (ThisInformation.FieldName != LANOUTLET.NULL)
                            InformationList.Add(CreateOutputNode(ThisInformation));
                    break;
                case DEVICECATEGORY.GPS:
                    ConcurrentDictionary<GPS, INFORMATIONSTRUCT> GPSField = (ConcurrentDictionary<GPS, INFORMATIONSTRUCT>)DeviceStroage.FirstOrDefault(Item => Item.Key.DeviceCategory == ThisDevice.DeviceCategory).Value;
                    foreach (INFORMATIONSTRUCT ThisInformation in GPSField.Values)
                        if (ThisInformation.FieldName != GPS.NULL)
                            InformationList.Add(CreateOutputNode(ThisInformation));
                    break;
                case DEVICECATEGORY.SQM:
                    ConcurrentDictionary<SQM, INFORMATIONSTRUCT> SQMField = (ConcurrentDictionary<SQM, INFORMATIONSTRUCT>)DeviceStroage.FirstOrDefault(Item => Item.Key.DeviceCategory == ThisDevice.DeviceCategory).Value;
                    foreach (INFORMATIONSTRUCT ThisInformation in SQMField.Values)
                        if (ThisInformation.FieldName != SQM.NULL)
                            InformationList.Add(CreateOutputNode(ThisInformation));
                    break;
                case DEVICECATEGORY.SEEING:
                    ConcurrentDictionary<SEEING, INFORMATIONSTRUCT> SEEINGField = (ConcurrentDictionary<SEEING, INFORMATIONSTRUCT>)DeviceStroage.FirstOrDefault(Item => Item.Key.DeviceCategory == ThisDevice.DeviceCategory).Value;
                    foreach (INFORMATIONSTRUCT ThisInformation in SEEINGField.Values)
                        if (ThisInformation.FieldName != SEEING.NULL)
                            InformationList.Add(CreateOutputNode(ThisInformation));
                    break;
                case DEVICECATEGORY.ALLSKY:
                    ConcurrentDictionary<ALLSKY, INFORMATIONSTRUCT> ALLSKYField = (ConcurrentDictionary<ALLSKY, INFORMATIONSTRUCT>)DeviceStroage.FirstOrDefault(Item => Item.Key.DeviceCategory == ThisDevice.DeviceCategory).Value;
                    foreach (INFORMATIONSTRUCT ThisInformation in ALLSKYField.Values)
                        if (ThisInformation.FieldName != ALLSKY.NULL)
                            InformationList.Add(CreateOutputNode(ThisInformation));
                    break;
                case DEVICECATEGORY.ASTROCLIENT:
                    ConcurrentDictionary<ASTROCLIENT, INFORMATIONSTRUCT> ASTROCLIENTField = (ConcurrentDictionary<ASTROCLIENT, INFORMATIONSTRUCT>)DeviceStroage.FirstOrDefault(Item => Item.Key.DeviceCategory == ThisDevice.DeviceCategory).Value;
                    foreach (INFORMATIONSTRUCT ThisInformation in ASTROCLIENTField.Values)
                        if (ThisInformation.FieldName != ASTROCLIENT.NULL)
                            InformationList.Add(CreateOutputNode(ThisInformation));
                    break;
                case DEVICECATEGORY.ASTROSERVER:
                    ConcurrentDictionary<ASTROSERVER, INFORMATIONSTRUCT> ASTROSERVERField = (ConcurrentDictionary<ASTROSERVER, INFORMATIONSTRUCT>)DeviceStroage.FirstOrDefault(Item => Item.Key.DeviceCategory == ThisDevice.DeviceCategory).Value;
                    foreach (INFORMATIONSTRUCT ThisInformation in ASTROSERVERField.Values)
                        if (ThisInformation.FieldName != ASTROSERVER.NULL)
                            InformationList.Add(CreateOutputNode(ThisInformation));
                    break;
                default: return null;
            }

            return InformationList;
        }

        private OUTPUTSTRUCT CreateOutputNode(INFORMATIONSTRUCT Inforstructure)
        {
            String Value = "null";

            if (Inforstructure.Value != null)
            {
                if (Inforstructure.Value.GetType() == typeof(Byte[]))
                    Value = Convert.ToBase64String((byte[])Inforstructure.Value);
                else
                    Value = Inforstructure.Value.ToString();
            }

            OUTPUTSTRUCT ThisOutput = new OUTPUTSTRUCT();
            ThisOutput.StationName = Inforstructure.StationName;
            ThisOutput.DeviceCategory = Inforstructure.DeviceCategory;
            ThisOutput.FieldName = Inforstructure.FieldName.ToString();
            ThisOutput.Value = Value;
            ThisOutput.DataType = Inforstructure.Value == null ? "null" : Inforstructure.Value.GetType().ToString().Replace("System.", "");
            ThisOutput.UpdateTime = Inforstructure.UpdateTime == null ? "null" : Inforstructure.UpdateTime.Value.ToString();

            return ThisOutput;
        }

        #endregion

        #region Subscription Information

        public void SubscribeInformation(DEVICENAME DeviceName, dynamic FieldName, String SessionID, Object CallBackObject)
        {
            ConcurrentDictionary<dynamic, INFORMATIONSTRUCT> ExistingInformation = (ConcurrentDictionary<dynamic, INFORMATIONSTRUCT>)DeviceStroage.FirstOrDefault(Item => Item.Key.DeviceName == DeviceName).Value;

            if (ExistingInformation != null)
            {
                INFORMATIONSTRUCT ThisField = ExistingInformation.FirstOrDefault(Item => Item.Key == FieldName).Value;
                if (ThisField != null)
                {
                    ThisField.ClientSubscribe.TryAdd(SessionID, CallBackObject);
                    ReturnSubscribe(DeviceName, CallBackObject, FieldName, ThisField.Value, ThisField.UpdateTime);
                }
            }
        }

        private void ReturnSubscribe(DEVICENAME DeviceName, Object CallBackObject, dynamic FieldName, Object Value, DateTime? UpdateTime)
        {
            Task t = Task.Run(() =>
            {
                try
                {
                    MethodInfo MInfo = CallBackObject.GetType().GetMethod("OnPublish");
                    MInfo.Invoke(CallBackObject, new Object[] { StationName, DeviceName, FieldName, Value, UpdateTime });
                }
                catch (Exception e)
                {
                    ReturnKnowType.DefineReturn(ReturnStatus.FAILED, "(#Si013) Failed to return subscribe information at ReturnSubscribe see. (" + e.Message + ")");
                }
            });
        }

        public void UnsubscribeBySessionID(String SessionID)
        {
            foreach (ConcurrentDictionary<dynamic, INFORMATIONSTRUCT> ThisDevice in DeviceStroage.Values)
                foreach (INFORMATIONSTRUCT ThisInformation in ThisDevice.Values)
                {
                    Object Value = null;
                    ThisInformation.ClientSubscribe.TryRemove(SessionID, out Value);
                }
        }

        public void UnsubscribeByFieldName(String SessionID, DEVICENAME DeviceName, dynamic FieldName)
        {
            ConcurrentDictionary<dynamic, INFORMATIONSTRUCT> ExistingInformation = (ConcurrentDictionary<dynamic, INFORMATIONSTRUCT>)DeviceStroage.FirstOrDefault(Item => Item.Key.DeviceName == DeviceName).Value;

            if (ExistingInformation != null)
            {
                INFORMATIONSTRUCT ThisField = ExistingInformation.FirstOrDefault(Item => Item.Key == FieldName).Value;
                if (ThisField != null)
                {
                    Object Value = null;
                    ThisField.ClientSubscribe.TryRemove(SessionID, out Value);
                }
            }
        }

        #endregion

        public bool SendingNewTarget(List<AstroQueueImpl> astroQueues)
        {
            bool returnData = false;
            
            if (ServerCallBackObject == null)
            {
                return returnData;
            }

            if(!IsStationConnected)
            {
                return returnData;
            }            

            Task TaskPost = Task.Run(() =>
            {
                try
                {
                    MethodInfo MInfo = ServerCallBackObject.GetType().GetMethod("OnNewTarget");
                    try
                    {
                        AstroQueueHandle(astroQueues, QUEUE_STATUS.WAITINGSERVER, SENDING_STATUS.IDLE, SENDING_STATUS.SENDING);

                        List<AstroQueue> astroQueueList = AstroQueue.Clone(astroQueues);

                        String jSon = JsonConvert.SerializeObject(astroQueueList);
                        MInfo.Invoke(ServerCallBackObject, new Object[] { StringCompression.CompressString(jSon) });

                        AstroQueueHandle(astroQueues, QUEUE_STATUS.WAITINGSERVER, SENDING_STATUS.SENDING, SENDING_STATUS.COMPLETED);

                        returnData = true;
                    }
                    catch (Exception e)
                    {
                        AstroQueueHandle(astroQueues, QUEUE_STATUS.WAITINGSERVER, SENDING_STATUS.SENDING, SENDING_STATUS.FAILED, e.Message);
                        Console.WriteLine(e.Message);
                    }
                }
                catch (Exception e)
                {
                    AstroQueueHandle(astroQueues, QUEUE_STATUS.WAITINGSERVER, SENDING_STATUS.SENDING, SENDING_STATUS.FAILED, e.Message);
                    //OutputMessage = e.Message;
                }
            });

            if (!TaskPost.Wait(1000))
            {
                AstroQueueHandle(astroQueues, QUEUE_STATUS.WAITINGSERVER, SENDING_STATUS.SENDING, SENDING_STATUS.FAILED, "New target sending failed. (Timeout)");
                //OutputMessage = "New target sending failed.";                
            }

            return returnData;
        }

        public bool AckTarget(AstroQueueImpl astroQueue, QUEUE_STATUS queueStatus, SENDING_STATUS sendingStatus)
        {

            Task TaskPost = Task.Run(() =>
            {
                try
                {
                    MethodInfo MInfo = ServerCallBackObject.GetType().GetMethod("OnReceivedTarget");

                    try
                    {
                        JObject jSonObject = new JObject(
                            new JProperty("Id", astroQueue.Id),
                            new JProperty("Event", "ACK"),
                            new JProperty("TimeStamp", DateTime.UtcNow)
                        );
                        
                        MInfo.Invoke(ServerCallBackObject, new Object[] { StringCompression.CompressString(jSonObject.ToString()) });
                        
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                    }
                }
                catch (Exception e)
                {
                    //OutputMessage = e.Message;
                }
            });

            if (!TaskPost.Wait(1000))
            {
                //OutputMessage = "New target sending failed.";                
            }

            return true;
        }

        public void AstroQueueHandle(List<AstroQueueImpl> astroQueues, QUEUE_STATUS queueStatus, SENDING_STATUS sendingStatus, SENDING_STATUS updatedStatus, String FailMessage = null)
        {
            Console.WriteLine(queueStatus.ToString() + " : " + sendingStatus.ToString() + " --> " + updatedStatus.ToString());

            astroQueues.ToList().ForEach(q =>
            {
                QueueStatus query = q.QueueStatus.Where(x => x.queueStatus == queueStatus && x.sendingStatus == sendingStatus).First();
                query.sendingStatus = updatedStatus;
                query.timeStamp = DateTime.UtcNow;

                if (updatedStatus == SENDING_STATUS.FAILED)
                {
                    query.message = FailMessage;

                    q.QueueStatus.Add(new QueueStatus(queueStatus, SENDING_STATUS.IDLE, null));
                }
                else if(updatedStatus == SENDING_STATUS.COMPLETED)
                {
                    if (q.Target.exposedHistory.Count() <= 0)
                    {
                        List<ExposureInfo> exposureInfos = Exposure.generate(q);

                        if (q.Target.exposedHistory == null)
                        {
                            q.Target.exposedHistory = new List<ExposedHistory>();
                        }

                        int z = 0;
                        foreach (ExposureInfo exposureInfo in exposureInfos)
                        {
                            ExposedHistory exposedHistory = new ExposedHistory();
                            exposedHistory.filterName = exposureInfo.filterName;
                            exposedHistory.executedStatus = EXECUTESTATUS.WAIT;
                            //exposedHistory.executedDate = DateTime.UtcNow;
                            q.Target.exposedHistory.Add(exposedHistory);
                            //Console.WriteLine(z + " - " + exposureInfo.filterName + " : " + exposureInfo.exposureTime);
                            ++z;
                        }
                    }
                }

                DBQueueEngine.UpdateObject(this.StationName, q);
            });
        }      
    }
}
