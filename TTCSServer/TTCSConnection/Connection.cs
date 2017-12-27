using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using DataKeeper.Engine;
using DataKeeper;
using System.ServiceModel.Channels;
using System.ServiceModel.Web;
using DataKeeper.Engine.Command;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using DataKeeper.Interface;
using System.Drawing;
using Emgu.CV;
using Emgu.CV.Structure;
using Newtonsoft.Json.Linq;


namespace TTCSConnection
{
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, InstanceContextMode = InstanceContextMode.PerSession)]
    public partial class Connection : IConnection
    {
        private Double RequirePackageVersion = 1.8;


        public Boolean TTCSCheckConnection()
        {
            return true;
        }

        #region Client Handler

        public ReturnKnowType AstroCreateStation(STATIONNAME StationName, Double Version = 0)
        {
            try
            {
                if (Version != RequirePackageVersion)
                {
                    return ReturnKnowType.DefineReturn(ReturnStatus.FAILED, "(#Co003) Failed to create "+ StationName + ". Your client version is "+ Version + " (Require version "+ RequirePackageVersion +").");
                }
                else
                {                
                    OperationContext context = OperationContext.Current;
                    ServerCallBack SiteCallBack = OperationContext.Current.GetCallbackChannel<ServerCallBack>();
                    CallBackHandler.AddSiteConnection(StationName, context.SessionId, SiteCallBack);

                    OperationContext.Current.Channel.Closed += StationChannel_Closed;
                    OperationContext.Current.Channel.Faulted += StationChannel_Closed;

                    ReturnKnowType CreateSiteResult = AstroData.CreateStation(StationName, context.SessionId, SiteCallBack);
                    return CreateSiteResult;
                }
            }
            catch (Exception e)
            {
                return ReturnKnowType.DefineReturn(ReturnStatus.FAILED, "(#Co001) Failed to create site at TTCSCreateSite see. (" + e.Message + ")");
            }
        }

        public ReturnKnowType AstroCreateClientInterface(String InterfaceName)
        {
            try
            {
                OperationContext context = OperationContext.Current;
                ServerCallBack ServerCallBack = OperationContext.Current.GetCallbackChannel<ServerCallBack>();
                CallBackHandler.AddInterfaceConnection(InterfaceName, context.SessionId, ServerCallBack);

                OperationContext.Current.Channel.Closed += InterfaceChannel_Closed;
                OperationContext.Current.Channel.Faulted += InterfaceChannel_Closed;

                return ReturnKnowType.DefineReturn(ReturnStatus.SUCESSFUL, null); ;
            }
            catch (Exception e)
            {
                return ReturnKnowType.DefineReturn(ReturnStatus.FAILED, "Failed to create client interface at TTCSCreateClientInterface see. (" + e.Message + ")");
            }
        }

        #endregion

        #region New Site Information 

        public Object DatabaseSync(STATIONNAME StationName, String TableName, DATAACTION Action, List<Object[]> Value)
        {
            if (TableName == "UserTB")
                return DatabaseSynchronization.SyncUser(Action, Value, StationName);
            else
                return DatabaseSynchronization.SyncLog(StationName.ToString(), Action, Value);
        }

        public void ReciveAcknowledge(STATIONNAME StationName, DEVICENAME DeviceName, String FieldName, Object[] Value, DateTime TimeRecive)
        {
            ACKNOWLEDGE ThisAcknowledge = new ACKNOWLEDGE();
            ThisAcknowledge.StationName = StationName.ToString();
            ThisAcknowledge.DeviceName = DeviceName.ToString();
            ThisAcknowledge.FieldName = FieldName;
            ThisAcknowledge.Value = String.Join(",", Value);
            ThisAcknowledge.ReviceDateTime = TimeRecive.ToString();

            var ReturningJson = new JavaScriptSerializer().Serialize(ThisAcknowledge);
            AstroData.NewASTROCLIENTInformation(StationName, DeviceName, ASTROCLIENT.ASTROCLIENT_LASTESTEXECTIONCOMMAND, ReturningJson.ToString(), TimeRecive);
        }

        public ReturnKnowType AddImage(STATIONNAME StationName, DEVICENAME DeviceName, UInt16[][] Value, DateTime DataTimeStamp)
        {
            AstroData.NewIMAGINGInformation(StationName, DeviceName, IMAGING.IMAGING_CCD_IMAGEARRAY16, Value, DataTimeStamp);
            return ReturnKnowType.DefineReturn(ReturnStatus.SUCESSFUL, null);
        }

        public void AddTS700MM(STATIONNAME StationName, DEVICENAME DeviceName, TS700MM[] FieldName, Object[] Value, DateTime[] DateTime)
        {
            for (int i = 0; i < FieldName.Count(); i++)
                AstroData.NewTS700MMInformation(StationName, DeviceName, FieldName[i], Value[i], DateTime[i]);
        }

        public void AddASTROHEVENDOME(STATIONNAME StationName, DEVICENAME DeviceName, ASTROHEVENDOME[] FieldName, Object[] Value, DateTime[] DateTime)
        {
            for (int i = 0; i < FieldName.Count(); i++)
                AstroData.NewASTROHEVENDOMEInformation(StationName, DeviceName, FieldName[i], Value[i], DateTime[i]);
        }

        public void AddIMAGING(STATIONNAME StationName, DEVICENAME DeviceName, IMAGING[] FieldName, Object[] Value, DateTime[] DateTime)
        {
            for (int i = 0; i < FieldName.Count(); i++)
                AstroData.NewIMAGINGInformation(StationName, DeviceName, FieldName[i], Value[i], DateTime[i]);
        }

        public void AddIMAGINGARRAY(STATIONNAME StationName, DEVICENAME DeviceName, int[][] Value, DateTime DateTime)
        {
            Task DomeSending = Task.Run(() => { AstroData.NewIMAGINGInformation(StationName, DeviceName, IMAGING.IMAGING_CCD_IMAGEARRAY16, Value, DateTime); });
        }

        public void AddSQM(STATIONNAME StationName, DEVICENAME DeviceName, SQM[] FieldName, Object[] Value, DateTime[] DateTime)
        {
            for (int i = 0; i < FieldName.Count(); i++)
                AstroData.NewSQMInformation(StationName, DeviceName, FieldName[i], Value[i], DateTime[i]);
        }

        public void AddSEEING(STATIONNAME StationName, DEVICENAME DeviceName, SEEING[] FieldName, Object[] Value, DateTime[] DateTime)
        {
            for (int i = 0; i < FieldName.Count(); i++)
                AstroData.NewSEEINGInformation(StationName, DeviceName, FieldName[i], Value[i], DateTime[i]);
        }

        public void AddALLSKY(STATIONNAME StationName, DEVICENAME DeviceName, ALLSKY[] FieldName, Object[] Value, DateTime[] DateTime)
        {
            for (int i = 0; i < FieldName.Count(); i++)
                AstroData.NewALLSKYInformation(StationName, DeviceName, FieldName[i], Value[i], DateTime[i]);
        }

        public void AddWEATHERSTATION(STATIONNAME StationName, DEVICENAME DeviceName, WEATHERSTATION[] FieldName, Object[] Value, DateTime[] DateTime)
        {
            for (int i = 0; i < FieldName.Count(); i++)
                AstroData.NewWEATHERSTATIONInformation(StationName, DeviceName, FieldName[i], Value[i], DateTime[i]);
        }

        public void AddLANOUTLET(STATIONNAME StationName, DEVICENAME DeviceName, LANOUTLET[] FieldName, Object[] Value, DateTime[] DateTime)
        {
            for (int i = 0; i < FieldName.Count(); i++)
                AstroData.NewLANOUTLETInformation(StationName, DeviceName, FieldName[i], Value[i], DateTime[i]);
        }

        public void AddCCTV(STATIONNAME StationName, DEVICENAME DeviceName, CCTV[] FieldName, Object[] Value, DateTime[] DateTime)
        {
            for (int i = 0; i < FieldName.Count(); i++)
                AstroData.NewCCTVInformation(StationName, DeviceName, FieldName[i], Value[i], DateTime[i]);
        }

        public void AddGPS(String DataGroupID, STATIONNAME StationName, DEVICENAME DeviceName, GPS[] FieldName, Object[] Value, DateTime[] DateTime, Boolean IsHistory)
        {
            for (int i = 0; i < FieldName.Count(); i++)
                AstroData.NewGPSInformation(DataGroupID, StationName, DeviceName, FieldName[i], Value[i], DateTime[i], IsHistory);

            AstroData.ReturnAckState(DataGroupID, StationName, DeviceName);
        }

        public void AddDeviceData(STATIONNAME StationName, String ScriptsJSON, Boolean IsInsertDB = true, Boolean IsSentWebSocket = true)
        {
            //Console.WriteLine(StationName);
            Task DeviceTask = Task.Run(() =>
            {
                if (ScriptsJSON == null) return;

                if (ScriptsJSON[0] != '[' && ScriptsJSON['1'] != '{')
                {
                    ScriptsJSON = StringCompression.DecompressString(ScriptsJSON);
                }

                if (ScriptsJSON == null) return;


                //Console.WriteLine("AddDeviceData - " + StationName.ToString() + " (" + Datas[0].DataId + ") - Packet: " + Datas.Count() + " Rows");
                DataPacket[] Datas = (DataPacket[])Newtonsoft.Json.JsonConvert.DeserializeObject(ScriptsJSON, typeof(DataPacket[]));

                StationHandler StationCommunication = AstroData.GetStationObject(StationName);

                foreach (DataPacket Data in Datas)
                {

                    //if (Data.FieldName == "IMAGING_FILTER_FILTERPOSITION")
                    //{
                    //    Console.WriteLine(Data.Value);
                    //}

                    if (Data.DeviceCategory == DEVICECATEGORY.CCTV)
                    {
                        //Console.WriteLine(Data.Value);

                        if (Data.FieldName.ToString() == CCTV.CCTV_DEVICE1_IMAGE.ToString() || Data.FieldName.ToString() == CCTV.CCTV_DEVICE2_IMAGE.ToString())
                        {                            
                            Data.Value = Convert.ToBase64String(((JArray)Data.Value).ToObject<byte[]>());
                        }
                    }
                    else if (Data.DeviceCategory == DEVICECATEGORY.ALLSKY)
                    {                       
                        if (Data.FieldName.ToString() == ALLSKY.ALLSKY_IMAGE.ToString())
                        {
                            //Console.WriteLine("RECEIVED ALLSKY: " + IsSentWebSocket);

                            if (typeof(JArray).Equals(Data.Value.GetType()))
                            {
                                Data.Value = Convert.ToBase64String(((JArray)Data.Value).ToObject<byte[]>());
                            }                            
                        }
                    }
                    else if (Data.DeviceCategory == DEVICECATEGORY.IMAGING)
                    {
                        //Console.WriteLine(Data.FieldName);

                        if (Data.FieldName.ToString() == IMAGING.IMAGING_CCD_DOWNLOAD_STATUS.ToString())
                        {
                            String[] TempValue = Data.Value.ToString().Split(';');

                            if (TempValue.Count() > 1)
                            {
                                if (TempValue[0] == "Completed")
                                {
                                    //   BlockID#/files/AIRFORCE/FITS/maIeayp9iEO57G9LXZVPA_TakenFromClient.FITS
                                    String[] PreFileName = TempValue[1].Split('#');

                                    if (PreFileName.Count() > 1)
                                    {
                                        String BlockID = PreFileName[0];

                                        String[] TmpFileName = PreFileName[1].Split('/');
                                        String FileName = TmpFileName[(TmpFileName.Count() - 1)] + ".FITS";
                                        String[] TempTargetID = FileName.Split('_');
                                        String TargetID = TempTargetID[0];

                                        DBScheduleEngine.InsertFITSData(TargetID, BlockID, StationName, FileName, Data.DateTimeUTC, DateTime.UtcNow.Ticks);
                                    }

                                    Data.Value = "Completed";
                                }
                            }                            
                        }
                        else if (Data.FieldName.ToString() == IMAGING.IMAGING_PREVIEW_DOWNLOAD_STATUS.ToString())
                        {       
                            AstroData.LoadPerviewImage(StationName, Data.DeviceName, StationCommunication);   
                        }

                        //AstroData.NewIMAGINGInformationHandle(StationName, Data.DeviceName, Data.FieldName, Data.Value, new DateTime(Data.DateTimeUTC));
                    }

                    if (IsInsertDB)
                    {
                        DBScheduleEngine.InsertData(Data.DataId, StationName, Data.DeviceCategory, Data.DeviceName, Data.FieldName, Data.Value, Data.DateTimeUTC);
                    }

                    if (IsSentWebSocket)
                    {
                        WebSockets.ReturnWebSubscribe(StationName, Data.DeviceName, Data.FieldName.ToString(), Data.Value, new DateTime(Data.DateTimeUTC));
                    }

                    AstroData.UpdateInformation(StationName, Data.DeviceName, Data.FieldName, Data.Value, new DateTime(Data.DateTimeUTC));
                }
            });
        }

        public Boolean AddDelayDeviceData(STATIONNAME StationName, String ScriptsJSON)
        {
            Task DelayTask = Task.Run(() =>
            {
                if (ScriptsJSON == null) return;

                if (ScriptsJSON[0] != '[' && ScriptsJSON['1'] != '{')
                {
                    ScriptsJSON = StringCompression.DecompressString(ScriptsJSON);
                }                

                DataPacket[] Datas = (DataPacket[])Newtonsoft.Json.JsonConvert.DeserializeObject(ScriptsJSON, typeof(DataPacket[]));

                Console.WriteLine("AddDelayDeviceData - " + StationName.ToString() + " (" + Datas[0].DataId + ") - Packet: " + Datas.Count() + " Rows");

                StationHandler StationCommunication = AstroData.GetStationObject(StationName);

                //StationCommunication.ReceivedInformation(Datas, out Msg);

                foreach (DataPacket Data in Datas)
                {
                    if (Data.DeviceCategory == DEVICECATEGORY.CCTV)
                    {
                        if (Data.FieldName.ToString() != CCTV.CCTV_CONNECTED.ToString())
                        {
                            Data.Value = Convert.ToBase64String(((JArray)Data.Value).ToObject<byte[]>());
                        }
                    }
                    else if (Data.DeviceCategory == DEVICECATEGORY.ALLSKY)
                    {
                        //Console.WriteLine(Data.Value);

                        if (Data.FieldName.ToString() == ALLSKY.ALLSKY_IMAGE.ToString())
                        {
                            if (typeof(JArray).Equals(Data.Value.GetType()))
                            {
                                Data.Value = Convert.ToBase64String(((JArray)Data.Value).ToObject<byte[]>());
                            }
                            else if(Data.Value.ToString() == "System.Byte[]")
                            {
                                Data.Value = null;
                            }
                            else
                            {
                                Data.Value = Convert.ToBase64String((byte[])Data.Value);
                            }
                        }
                    }
                    else if (Data.DeviceCategory == DEVICECATEGORY.IMAGING)
                    {
                        //Console.WriteLine(Data.FieldName);

                        if (Data.FieldName.ToString() == IMAGING.IMAGING_CCD_DOWNLOAD_STATUS.ToString())
                        {
                            String[] TempValue = Data.Value.ToString().Split(';');

                            if (TempValue.Count() > 1)
                            {
                                if (TempValue[0] == "Completed")
                                {
                                    //   /files/AIRFORCE/FITS/maIeayp9iEO57G9LXZVPA_TakenFromClient.FITS

                                    String[] TmpFileName = TempValue[1].Split('/');
                                    String FileName = TmpFileName[(TmpFileName.Count() - 1)];
                                    String[] TempBlockID = FileName.Split('_');
                                    String TargetID = TempBlockID[0];
                                    String BlockID = TempBlockID[1];

                                    DBScheduleEngine.InsertFITSData(TargetID, BlockID, StationName, FileName, Data.DateTimeUTC, DateTime.UtcNow.Ticks);
                                }
                            }
                        }
                    }

                    DBScheduleEngine.InsertData(Data.DataId, StationName, Data.DeviceCategory, Data.DeviceName, Data.FieldName, Data.Value, Data.DateTimeUTC);
                }
            });           

            return true;
        }
   
        public Boolean ScheduleEvented(String ScriptsJSON)
        {
            String jSonC = StringCompression.DecompressString(ScriptsJSON);
            ScriptStructureNew Script = (ScriptStructureNew)Newtonsoft.Json.JsonConvert.DeserializeObject(jSonC, typeof(ScriptStructureNew));

            DBScheduleEngine.UpdateSchedule(Script);
            return true;
        }

        public Boolean DelayScheduleEvented(String ScriptsJSON)
        {
            String jSonC = StringCompression.DecompressString(ScriptsJSON);
            ScriptStructureNew[] Scripts = (ScriptStructureNew[])Newtonsoft.Json.JsonConvert.DeserializeObject(jSonC, typeof(ScriptStructureNew[]));

            foreach (ScriptStructureNew Script in Scripts)            
                DBScheduleEngine.UpdateSchedule(Script);

            Console.WriteLine("RECEIVED DelayScheduleEvented() = " + Scripts.Count());

            return true;
        }

        //public Boolean DelayScheduleEvented(ScriptStructureNew[] Scripts)
        //{
        //    foreach (ScriptStructureNew Script in Scripts)
        //    {
        //        DBScheduleEngine.UpdateSchedule(Script);
        //    }

        //    return true;
        //}


        public void GetNextScriptPart(STATIONNAME StationName)
        {
            String Output = "";
            StationHandler ThisStation = AstroData.GetStationObject(StationName);
            ThisStation.NextScriptInformation(out Output);
        }        

        public Boolean AddASTROCLIENT(STATIONNAME StationName, DEVICENAME DeviceName, ASTROCLIENT[] FieldName, Object[] Value, DateTime[] DateTime)
        {
            for (int i = 0; i < FieldName.Count(); i++)
                AstroData.NewASTROCLIENTInformation(StationName, DeviceName, FieldName[i], Value[i], DateTime[i]);

            return true;
        }

        public Boolean UpdateScriptFromStation(String BlockID, String BlockName, String StationName, DateTime ExecutionTimeStart, DateTime ExecutionTimeEnd, int CommandCounter, int ExecutionNumber, String DeviceName, String DeviceCategory, String CommandName, String Owner, int DelayTime, String Parameter, String ScriptState)
        {
            ScriptManager.UpdateScriptFromStation(BlockID, BlockName, StationName, ExecutionTimeStart, ExecutionTimeEnd, CommandCounter, ExecutionNumber, DeviceName, DeviceCategory, CommandName, Owner, DelayTime, Parameter, ScriptState);
            return true;
        }

        public void AddASTROSERVER(STATIONNAME StationName, DEVICENAME DeviceName, ASTROSERVER[] FieldName, Object[] Value, DateTime[] DateTime)
        {
            for (int i = 0; i < FieldName.Count(); i++)
                AstroData.NewASTROSERVERInformation(StationName, DeviceName, FieldName[i], Value[i], DateTime[i]);
        }

        public void GetStructure(ASTROCLIENT Astroclient, TS700MM TS700mm, ASTROHEVENDOME Dome, IMAGING Imaging, SQM Sqm, SEEING Seeing, ALLSKY Allsky, WEATHERSTATION Weatherstation, LANOUTLET Lanoutlet, CCTV CCTV, GPS GPS, ASTROSERVER Astroserver)
        {

        }

        #endregion

        #region Set Information

        public ReturnKnowType AstroSet(STATIONNAME StationName, DEVICENAME DeviceName, dynamic CommandName, Object[] Values, DateTime CommandDateTime)
        {
            DEVICECATEGORY DeviceCategory = TTCSHelper.ConvertDeviceNameToDeviceCategory(StationName, DeviceName);
            return AstroData.SetCommandHandler(StationName, DeviceCategory, DeviceName, CommandName, Values, CommandDateTime);
        }

        #endregion

        #region Subscription Information

        public void SubscribeInformation(STATIONNAME StationName, DEVICENAME DeviceName, dynamic FieldName)
        {
            ServerCallBack ClientCallBack = OperationContext.Current.GetCallbackChannel<ServerCallBack>();
            OperationContext Context = OperationContext.Current;
            AstroData.SubscribeInformation(StationName, DeviceName, FieldName, Context.SessionId, ClientCallBack);
        }

        #endregion

        #region Unsubscribe Information

        public void UnsubscribeBySessionID()
        {
            OperationContext context = OperationContext.Current;
            AstroData.UnsubscribeBySessionID(context.SessionId);
        }

        public void UnsubscribeByFieldName(STATIONNAME StationName, DEVICENAME DeviceName, dynamic FieldName)
        {
            OperationContext context = OperationContext.Current;
            AstroData.UnsubscribeByFieldName(context.SessionId, StationName, DeviceName, FieldName);
        }


        #endregion

        #region Get Information

        public List<INFORMATIONSTRUCT> GetInformation(STATIONNAME StationName, DEVICENAME DeviceName, dynamic FieldName)
        {
            return AstroData.GetInformation(StationName, DeviceName, FieldName);
        }

        #endregion

        //------------------------------------------------------------------------------------Event Handler--------------------------------------------------------------------------------

        private void InterfaceChannel_Closed(object sender, EventArgs e)
        {
            IContextChannel ThisContext = (IContextChannel)sender;
            CallBackHandler.RemoveInterfaceConnection(ThisContext.SessionId);

            AstroData.UnsubscribeBySessionID(ThisContext.SessionId);
        }

        private void StationChannel_Closed(object sender, EventArgs e)
        {
            IContextChannel ThisContext = (IContextChannel)sender;
            CallBackHandler.RemoveSiteConnection(ThisContext.SessionId);

            if (AstroData.IsStationConnected(ThisContext.SessionId))
            {
                STATIONNAME ThisStation = AstroData.GetStationName(ThisContext.SessionId);
                TTCSLog.NewLogInformation(ThisStation, DateTime.UtcNow, "Station name : " + ThisStation.ToString() + " is now disconnceted.", LogType.COMMUNICATION, null);
                AstroData.SetStationDisconnected(ThisContext.SessionId);
                AstroData.StationDisconnected(ThisStation);
            }
        }

        //--------------------------------------------------------------------------------------Server Handler----------------------------------------------------------------------------------

        private void GetConnectionInfo(OperationContext context, ref String IPaddress, ref int? Port)
        {
            MessageProperties prop = context.IncomingMessageProperties;
            RemoteEndpointMessageProperty endpoint = prop[RemoteEndpointMessageProperty.Name] as RemoteEndpointMessageProperty;
            IPaddress = endpoint.Address;
            Port = endpoint.Port;
        }

        public void GetSetStructure(TS700MMSET TSSet, IMAGINGSET IMGSet, LANOUTLETSET LANSet, DOMESET DOMESet)
        {

        }
    }
}
