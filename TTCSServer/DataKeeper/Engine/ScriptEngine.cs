﻿using DataKeeper.Engine.Command;
using FluentFTP;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using Newtonsoft.Json;

namespace DataKeeper.Engine
{
    public class StationScript
    {
        public List<ScriptStructureNew> ScriptCollection = null;
        public STATIONNAME StationName = STATIONNAME.NULL;
        public String LastestScriptFileName = null;

        public StationScript(STATIONNAME StationName)
        {
            ScriptCollection = new List<ScriptStructureNew>();
            this.StationName = StationName;
        }

        public void RemoveAllScript()
        {
            if (ScriptCollection != null)
                ScriptCollection.Clear();
        }

        public void AddScript(List<ScriptStructureNew> NewScript)
        {
            if (ScriptCollection == null)
                return;

            ScriptCollection.AddRange(NewScript);
        }

        public void RemoveScript(String ScriptID)
        {
            ScriptCollection.RemoveAll(Item => Item.ScriptID == ScriptID);
        }

        public List<ScriptStructureNew> GetScript()
        {
            return ScriptCollection;
        }
    }

    public static class ScriptEngine
    {
        private static FtpClient Client = null;
        private static List<StationScript> ScriptStation = new List<StationScript>();
        private static Object _ScriptMonitoring = null;
        public static List<ScriptConfigure> scriptConfigure = new List<ScriptConfigure>();
        private static String ScriptConfig = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + @"\script_config.json";

        public static void SetMonitoringObject(Object _ScriptMonitoring)
        {
            ScriptEngine._ScriptMonitoring = _ScriptMonitoring;
        }

        public static void RefreshScript(STATIONNAME StationName)
        {
            StationScript stationScript = GetStationScript(StationName);
            if (stationScript != null)
            {
                stationScript.RemoveAllScript();
            }
            else
            {                
                stationScript = new StationScript(StationName);
                ScriptStation.Add(stationScript);
            }

            String LastestScript = GetLastestFile("\\\\192.168.2.110\\ftp\\Script\\" + StationName);

            if (ExtractScriptData(LastestScript, StationName, scriptConfigure, false))
                SendScriptToStation(StationName);
            else
            {
                DisplayScript("", StationName);
            }
        }

        public static void NewScriptChecker(String ScriptServerAddress, String LoginUser, String LoginPassword)
        {
            String ScriptPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + @"\script_config.json";

            if (!File.Exists(ScriptPath))
            {
                File.Create(ScriptPath).Dispose();                
            }

            using (StreamReader r = new StreamReader(ScriptPath))
            {
                String jsonString = r.ReadToEnd();
                scriptConfigure = JsonConvert.DeserializeObject<List<ScriptConfigure>>(jsonString);

                initialConfig();
            }

            System.Net.NetworkCredential readCredentials = new NetworkCredential(@"AstroNET", "P@ssw0rd");

            Task ScriptTask = Task.Run(async () =>
            {               
                while (true)
                {                   
                    foreach (STATIONNAME StationName in Enum.GetValues(typeof(STATIONNAME)))
                    {                     
                        if (StationName != STATIONNAME.NULL && StationName != STATIONNAME.ASTROSERVER)
                        {
                            StationHandler ThisStation = AstroData.GetStationObject(StationName);

                            if (ThisStation != null)
                            {
                                if (!ThisStation.IsStationConnected) continue;
                                if (ThisStation.ServerCallBackObject == null) continue;
                                if (ThisStation.IsSendingScriptToStation) continue;                                

                                StationScript stationScript = GetStationScript(StationName);

                                if (stationScript == null)
                                {
                                    stationScript = new StationScript(StationName);
                                    ScriptStation.Add(stationScript);
                                }

                                using (new NetworkConnection("\\\\192.168.2.110\\FTP", readCredentials))
                                {
                                    String LastestScript = GetLastestFile("\\\\192.168.2.110\\FTP\\Script\\" + StationName);

                                    if (LastestScript != null)
                                    {
                                        //Console.WriteLine("Script Task: " + LastestScript);

                                        STATIONNAME ScriptStationName = StationName;

                                        if (ExtractScriptData(LastestScript, StationName, scriptConfigure, true))
                                        {
                                            Console.WriteLine("Extracted Script... : " + LastestScript);

                                            ThisStation.IsSendingScriptToStation = true;                                 
                                            SendScriptToStation(StationName);
                                        }
                                    }
                                }
                            }
                                
                            //Console.WriteLine(StationName);
                        }
                
                    }

                    using (StreamWriter sw = new StreamWriter(ScriptConfig))
                    {
                        //Console.WriteLine(scriptConfigure.Count);

                        String DataJson = JsonConvert.SerializeObject(scriptConfigure);
                        sw.WriteLine(DataJson);
                    }

                    await Task.Delay(1000);
                }
            });

        }

        public static void SendScriptToStation(STATIONNAME ScriptStationName)
        {
            if (ScriptStationName != STATIONNAME.NULL)
            {
                String Message = null;
                StationHandler StationCommunication = AstroData.GetStationObject(ScriptStationName);
                StationScript StationScript = GetStationScript(ScriptStationName);

                DBScheduleEngine.UpdateFailSchedule(ScriptStationName);

                //DBScheduleEngine.DropSchedule(ScriptStationName);
                Console.WriteLine("Preparing Station Script before sending.");

                foreach (ScriptStructureNew Script in StationScript.ScriptCollection)
                {
                    //if (Script.ScriptState != SCRIPTSTATE.CANCELED.ToString())
                    //{
                    //    Script.ScriptState = SCRIPTSTATE.SENDINGTOSTATION.ToString();
                    //    DBScheduleEngine.UpdateSchedule(Script);
                    //}
                    //else
                    //{
                    //    Console.WriteLine("CANCEL");
                    //}
                    Script.ScriptState = SCRIPTSTATE.SENDINGTOSTATION.ToString();
                    DBScheduleEngine.UpdateSchedule(Script);
                }


                Console.WriteLine("Sending Script to "+ ScriptStationName + " ...");
                if (StationCommunication.NewScriptInformation(StationScript.GetScript(), out Message))
                {       
                    
                    //DBScheduleEngine.DropSchedule(ScriptStationName);

                    /*
                    foreach (ScriptStructureNew Script in StationScript.ScriptCollection)
                    {
                        Script.ScriptState = SCRIPTSTATE.WAITINGSTATION.ToString();
                        DBScheduleEngine.UpdateSchedule(Script);
                    }
                    */
                }

                DisplayScriptMessage(Message);
            }
        }

        private static StationScript GetStationScript(STATIONNAME StationName)
        {
            StationScript ThisStation = ScriptStation.FirstOrDefault(Item => Item.StationName == StationName);
            return ThisStation;
        }

        public static void GenNewScript()
        {
            FtpClient Client = new FtpClient("192.168.2.110");
            Client.Credentials = new NetworkCredential("astronet", "P@ssw0rd");
            Client.ConnectTimeout = 1000;
            Client.Connect();

            //-----------------------------------------------------------------------------------Test Json Write-----------------------------------------------------------------------------------------
            List<ScriptStructureNew> ScriptList = new List<ScriptStructureNew>();

            int station_random = new Random().Next(1, 4);
            String stationName = null;

            /*
            if(station_random == 1)
            {
                stationName = "AIRFORCE";
                ScriptList.Add(new ScriptStructureNew(DateTime.UtcNow.Ticks.ToString(), DateTime.UtcNow.Ticks.ToString(), "30", STATIONNAME.AIRFORCE.ToString(), DEVICENAME.AIRFORCE_TS700MM.ToString(), TS700MMSET.TS700MM_MOUNT_SETENABLE.ToString(), new List<String> { }, SCRIPTSTATE.WAITINGSERVER.ToString(), DateTime.UtcNow.AddMinutes(-2).Ticks.ToString(), DateTime.UtcNow.AddMinutes(+2).Ticks.ToString()));
                ScriptList.Add(new ScriptStructureNew(DateTime.UtcNow.Ticks.ToString(), DateTime.UtcNow.Ticks.ToString(), "30", STATIONNAME.AIRFORCE.ToString(), DEVICENAME.AIRFORCE_IMAGING.ToString(), IMAGINGSET.IMAGING_CCD_EXPOSE.ToString(), new List<String> { "FileName", "12.0", "true" }, SCRIPTSTATE.WAITINGSERVER.ToString(), DateTime.UtcNow.AddMinutes(-2).Ticks.ToString(), DateTime.UtcNow.AddMinutes(+2).Ticks.ToString()));
            }
            else if(station_random == 2)
            {
                stationName = "ASTROPARK";
                ScriptList.Add(new ScriptStructureNew(DateTime.UtcNow.Ticks.ToString(), DateTime.UtcNow.Ticks.ToString(), "30", STATIONNAME.ASTROPARK.ToString(), DEVICENAME.ASTROPARK_TS700MM.ToString(), TS700MMSET.TS700MM_MOUNT_SETENABLE.ToString(), new List<String> { }, SCRIPTSTATE.WAITINGSERVER.ToString(), DateTime.UtcNow.AddMinutes(-2).Ticks.ToString(), DateTime.UtcNow.AddMinutes(+2).Ticks.ToString()));
                ScriptList.Add(new ScriptStructureNew(DateTime.UtcNow.Ticks.ToString(), DateTime.UtcNow.Ticks.ToString(), "30", STATIONNAME.ASTROPARK.ToString(), DEVICENAME.ASTROPARK_IMAGING.ToString(), IMAGINGSET.IMAGING_CCD_EXPOSE.ToString(), new List<String> { "FileName", "12.0", "true" }, SCRIPTSTATE.WAITINGSERVER.ToString(), DateTime.UtcNow.AddMinutes(-2).Ticks.ToString(), DateTime.UtcNow.AddMinutes(+2).Ticks.ToString()));
            }
            else if (station_random == 3)
            {
                stationName = "USA";
                ScriptList.Add(new ScriptStructureNew(DateTime.UtcNow.Ticks.ToString(), DateTime.UtcNow.Ticks.ToString(), "30", STATIONNAME.USA.ToString(), DEVICENAME.USA_TS700MM.ToString(), TS700MMSET.TS700MM_MOUNT_SETENABLE.ToString(), new List<String> { }, SCRIPTSTATE.WAITINGSERVER.ToString(), DateTime.UtcNow.AddMinutes(-2).Ticks.ToString(), DateTime.UtcNow.AddMinutes(+2).Ticks.ToString()));
                ScriptList.Add(new ScriptStructureNew(DateTime.UtcNow.Ticks.ToString(), DateTime.UtcNow.Ticks.ToString(), "30", STATIONNAME.USA.ToString(), DEVICENAME.USA_IMAGING.ToString(), IMAGINGSET.IMAGING_CCD_EXPOSE.ToString(), new List<String> { "FileName", "12.0", "true" }, SCRIPTSTATE.WAITINGSERVER.ToString(), DateTime.UtcNow.AddMinutes(-2).Ticks.ToString(), DateTime.UtcNow.AddMinutes(+2).Ticks.ToString()));
            }   
            */
            stationName = "ASTROPARK";
            ScriptList.Add(new ScriptStructureNew(DateTime.UtcNow.Ticks.ToString(), DateTime.UtcNow.Ticks.ToString(), DateTime.UtcNow.AddMilliseconds(+1).Ticks.ToString(), "30", stationName, stationName + "_" + "TS700MM", TS700MMSET.TS700MM_MOUNT_SETENABLE.ToString(), new List<String> { }, SCRIPTSTATE.WAITINGSERVER.ToString(), DateTime.UtcNow.AddMinutes(+1).Ticks.ToString(), DateTime.UtcNow.AddMinutes(+2).Ticks.ToString(), "", "", "CHAMP"));
            ScriptList.Add(new ScriptStructureNew(DateTime.UtcNow.Ticks.ToString(), DateTime.UtcNow.Ticks.ToString(), DateTime.UtcNow.AddMilliseconds(+2).Ticks.ToString(), "30", stationName, stationName + "_" + "TS700MM", TS700MMSET.TS700MM_MOUNT_SLEWRADEC.ToString(), new List<String> { "4 55 23.32", "+14 32 54.12" }, SCRIPTSTATE.WAITINGSERVER.ToString(), DateTime.UtcNow.AddMinutes(+1).Ticks.ToString(), DateTime.UtcNow.AddMinutes(+2).Ticks.ToString(), "", "", "CHAMP"));
            ScriptList.Add(new ScriptStructureNew(DateTime.UtcNow.Ticks.ToString(), DateTime.UtcNow.Ticks.ToString(), DateTime.UtcNow.AddMilliseconds(+3).Ticks.ToString(), "30", stationName, stationName + "_" + "IMAGING", IMAGINGSET.IMAGING_CCD_EXPOSE.ToString(), new List<String> { "FileName", "1.0", "true", "TigerStar" }, SCRIPTSTATE.WAITINGSERVER.ToString(), DateTime.UtcNow.AddMinutes(+1).Ticks.ToString(), DateTime.UtcNow.AddMinutes(+2).Ticks.ToString(), "", "", "CHAMP"));

            String DataJsonTest = JsonConvert.SerializeObject(ScriptList);

            var remoteFileStream = Client.OpenWrite(@"\Script\"+ stationName +"\\" + DateTime.UtcNow.Ticks.ToString() + ".txt");

            Byte[] DataByte = System.Text.Encoding.UTF8.GetBytes(DataJsonTest);
            remoteFileStream.Write(DataByte, 0, DataByte.Count());
            remoteFileStream.Close();
            FtpReply status = Client.GetReply();
            Client.Disconnect();
            //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        }

        private static Boolean ExtractScriptData(String FileName, STATIONNAME StationName, List<ScriptConfigure> scriptConfigures, bool checkSameScript)
        {
            Boolean IsScriptOK = false;
            //StationName = STATIONNAME.NULL;
  
            String FilePathStr = FileName;

            if (File.Exists(FilePathStr))
            {
                try
                {                        
                    using (FileStream fs = new FileStream(FilePathStr, FileMode.Open, FileAccess.Read, FileShare.None))
                    {
                        using (StreamReader r = new StreamReader(fs))
                        {
                            //Console.WriteLine("Reading... " + FilePathStr);                            
                            String jsonString = r.ReadToEnd();

                            if (jsonString.Length > 0)
                            {
                                try
                                {
                                    List<ScriptStructureNew> NewScriptCollection = JsonConvert.DeserializeObject<List<ScriptStructureNew>>(jsonString);

                                    if (NewScriptCollection.Count > 0)
                                    {
                                        String TempFileNameStr = FilePathStr.Split('\\').Last();
                                        TempFileNameStr = TempFileNameStr.Replace(".txt", "");

                                        FILESTATE FileState = IsTheSameScript(TempFileNameStr, NewScriptCollection, StationName, scriptConfigures);

                                        if (FileState != FILESTATE.SAME || !checkSameScript)
                                        {
                                            bool IsMustInsertToDB = true;

                                            Console.WriteLine("Extracting Script... : " + FilePathStr);

                                            StationScript scriptTemp = ScriptStation.FirstOrDefault(Item => Item.StationName == StationName);
                                            scriptTemp.LastestScriptFileName = TempFileNameStr;

                                            if (scriptConfigure == null)
                                            {
                                                scriptConfigure = new List<ScriptConfigure>();
                                                scriptConfigure.Add(new ScriptConfigure(StationName.ToString(), TempFileNameStr, false, true));
                                            }
                                            else if (scriptTemp == null)
                                            {
                                                scriptConfigure.Add(new ScriptConfigure(StationName.ToString(), TempFileNameStr, false, true));
                                            }
                                            else
                                            {
                                                ScriptConfigure tempScript = scriptConfigure.FirstOrDefault(Item => Item.config_name == StationName.ToString());

                                                if (tempScript != null)
                                                {
                                                    if (tempScript.config_isaddtodb == true)
                                                    {
                                                        IsMustInsertToDB = false;
                                                    }
                                                    else
                                                    {
                                                        tempScript.config_isaddtodb = true;
                                                    }

                                                    tempScript.config_value = TempFileNameStr;
                                                    tempScript.config_status = false;
                                                }
                                                else
                                                {
                                                    scriptConfigure.Add(new ScriptConfigure(StationName.ToString(), TempFileNameStr, false, true));
                                                }
                                            }

                                            if (FileState == FILESTATE.NOTSAME)
                                            {
                                                IsMustInsertToDB = true;
                                            }

                                            //DBScheduleEngine.DropSchedule(StationName);
                                            //Console.WriteLine(IsMustInsertToDB);                                         

                                            String Message = "";

                                            Console.WriteLine("Verifying Script... : " + FilePathStr);

                                            if (VerifyScript(StationName, NewScriptCollection, out Message))
                                            {
                                                Console.WriteLine("Clear Fail Script to Database... : " + FilePathStr);
                                                DBScheduleEngine.CancleFailSchedule(StationName);

                                                Console.WriteLine("Insert Script to Database... (DB: " + IsMustInsertToDB + ") : " + FilePathStr);
                                                foreach (ScriptStructureNew Script in NewScriptCollection)
                                                {
                                                    //DUMMY ONLY
                                                    //Script.TargetID = "XXXXXX";
                                                    if (IsMustInsertToDB)
                                                    {                                                        
                                                        String _id = DBScheduleEngine.InsertSchedule(Script);
                                                        if (_id != null)
                                                        {
                                                            Script._id = _id;
                                                        }
                                                    }
                                                    else
                                                    {
                                                        String _id = DBScheduleEngine.GetId(Script);
                                                        Script._id = _id;
                                                        //Console.WriteLine("OLD ID: " + _id);
                                                    }

                                                    //if (Script.ScriptState != SCRIPTSTATE.CANCELED.ToString())
                                                    //{
                                                    //    Script.ScriptState = SCRIPTSTATE.WAITINGSERVER.ToString();
                                                    //}
                                                }

                                                Console.WriteLine("Add Script to Object... : " + FilePathStr);

                                                StationName = TTCSHelper.StationStrConveter(NewScriptCollection.FirstOrDefault().StationName);
                                                StationScript ThisStationTemp = GetStationScript(StationName);

                                                if (ThisStationTemp == null)
                                                {
                                                    ThisStationTemp = new StationScript(StationName);
                                                    ScriptStation.Add(ThisStationTemp);
                                                }
                                                else
                                                {
                                                    ThisStationTemp.RemoveAllScript();
                                                }

                                                ThisStationTemp.AddScript(NewScriptCollection);

                                                DisplayScript(Message, StationName);
                                                IsScriptOK = true;

                                                fs.Close();
                                            }
                                            else
                                            {
                                                fs.Close();

                                                DisplayScriptMessage(Message);

                                                Console.WriteLine("Verifying command failed, Deleted.");
                                                Console.WriteLine(Message);
                                                File.Delete(@FilePathStr);                                            
                                            }
                                        }
                                    }
                                    else
                                    {
                                        //Console.WriteLine("No Data Received, Deleted.");
                                    }
                                }
                                catch (JsonReaderException ex)
                                {
                                    fs.Close();

                                    try
                                    {
                                        Console.WriteLine("File isn't jSon, Deleted.");
                                        File.Delete(@FilePathStr);
                                    }
                                    catch (Exception x)
                                    {
                                        Console.WriteLine(x.Message);
                                    }
                                }
                                catch (Exception ex2)
                                {
                                    fs.Close();

                                    Console.WriteLine(ex2.Message);
                                    File.Delete(@FilePathStr);
                                }
                            }
                            else
                            {
                                fs.Close();

                                try
                                {
                                    Console.WriteLine("File size <= 0 byte, Deleted.");
                                    File.Delete(@FilePathStr);
                                }
                                catch (Exception x)
                                {
                                    Console.WriteLine(x);
                                }
                            }
                        }
                    }
                }
                catch(Exception z)
                {
                    Console.WriteLine(z);
                }
            }

            return IsScriptOK;
        }

        public static void DisplayScript(String Message, STATIONNAME StationName)
        {
            if (_ScriptMonitoring != null)
            {
                StationScript ThisStation = GetStationScript(StationName);

                MethodInfo MInfo = _ScriptMonitoring.GetType().GetMethod("AddScript");
                MInfo.Invoke(_ScriptMonitoring, new Object[] { ThisStation.ScriptCollection });

                if(Message != "")
                    DisplayScriptMessage(Message);
            }
        }

        public static void DisplayScriptMessage(String Message)
        {
            if (_ScriptMonitoring != null)
            {
                MethodInfo MInfo = _ScriptMonitoring.GetType().GetMethod("AddScriptMessage");
                MInfo.Invoke(_ScriptMonitoring, new Object[] { Message });
            }
        }

        private static FILESTATE IsTheSameScript(String FilePathStr, List<ScriptStructureNew> NewScriptCollection, STATIONNAME StationName, List<ScriptConfigure> scriptConfigures)
        {                        
            StationScript ThisStation = GetStationScript(StationName);
            if (ThisStation != null)
            {
                if (ThisStation.LastestScriptFileName != FilePathStr)
                {
                    return FILESTATE.NOTSAME;
                }

                if (scriptConfigure != null)
                {
                    ScriptConfigure tempScript = scriptConfigure.FirstOrDefault(Item => Item.config_name == StationName.ToString());

                    if (tempScript != null)
                    {
                        if (tempScript.config_status == false)
                        {
                            Console.WriteLine(StationName + " | RE-SENDING NOT SEND TO STATION -- FILE STATE (" + tempScript.config_status + ")");
                            return FILESTATE.RESEND;
                        }
                    }
                }                                

                if (NewScriptCollection.Count() != ThisStation.ScriptCollection.Count() && ThisStation.ScriptCollection.Count() > 0)
                {
                    Console.WriteLine(StationName + " | COUNT: " + NewScriptCollection.Count() + " == " + ThisStation.ScriptCollection.Count());
                    return FILESTATE.COMMANDCHANGE;
                }

                if (ThisStation.LastestScriptFileName == FilePathStr)
                {
                    return FILESTATE.SAME;
                }

                for (int i = 0; i < NewScriptCollection.Count(); i++)
                {
                    if (NewScriptCollection[i].ScriptID != ThisStation.ScriptCollection[i].ScriptID)
                    {
                        Console.WriteLine(StationName + " | NOT SAME -- SCRIPT ID (" + NewScriptCollection[i].ScriptID + " != " + ThisStation.ScriptCollection[i].ScriptID);
                        return FILESTATE.DIFFID;
                    }

                    if(NewScriptCollection[i].ScriptState == SCRIPTSTATE.WAITINGSERVER.ToString() || NewScriptCollection[i].ScriptState == SCRIPTSTATE.SENDINGTOSTATION.ToString())
                    {
                        Console.WriteLine(StationName + " | RE-SENDING NOT SEND TO STATION -- STATE (" + NewScriptCollection[i].ScriptState + ")");
                        return FILESTATE.RESEND;
                    }
                }                
                //Console.WriteLine("SAME FILE ALERT");
            }
            else
            {
                return FILESTATE.NOTSAME;
            }

            return FILESTATE.SAME;
        }

        private static Boolean VerifyScript(STATIONNAME StationName, List<ScriptStructureNew> ScriptCollection, out String Message)
        {
            foreach (ScriptStructureNew ThisScript in ScriptCollection)
            {
                //DUMMY ONLY
                //ThisScript.TargetID = "XXXXXX";

                if (String.IsNullOrEmpty(ThisScript.ScriptID))
                {
                    Message = "Invalid ScriptID can't be null or empty value. Please check.";
                    return false;
                }

                if (String.IsNullOrEmpty(ThisScript.BlockID))
                {
                    Message = "Invalid BlockID can't be null or empty value at Script ID : " + ThisScript.ScriptID + ". Please check.";
                    return false;
                }

                STATIONNAME ScriptStationName = TTCSHelper.StationStrConveter(ThisScript.StationName);
                
                if (StationName == STATIONNAME.NULL)
                {
                    Message = "Invalid station same at script ID : " + ThisScript.BlockID + ". Please check spelling.";
                    return false;
                }

                if (StationName != ScriptStationName)
                {
                    Message = "Invalid station name not match in folder at script ID : " + ThisScript.BlockID + ". Please check spelling.";
                    return false;
                }

                DEVICENAME DeviceName = TTCSHelper.DeviceNameStrConverter(ThisScript.DeviceName);
                if (DeviceName == DEVICENAME.NULL)
                {
                    Message = "Invalid device name at script ID : " + ThisScript.BlockID + ". Please check spelling.";
                    return false;
                }

                DEVICECATEGORY DeviceCategory = TTCSHelper.ConvertDeviceNameToDeviceCategory(StationName, DeviceName);
                if (DeviceCategory == DEVICECATEGORY.NULL)
                {
                    Message = "Invalid devicecategory at script ID : " + ThisScript.BlockID + ". Please check spelling.";
                    return false;
                }

                Object CommandName = TTCSHelper.CommandNameConverter(DeviceCategory, ThisScript.CommandName);
                if (CommandName == null)
                {
                    Message = "Invalid command name at script ID : " + ThisScript.BlockID + ". Please check spelling.";
                    return false;
                }

                Int64 StartDateLong = 0;
                if (!Int64.TryParse(ThisScript.ExecutionTimeStart, out StartDateLong))
                {
                    Message = "Invalid start datetime at script ID : " + ThisScript.BlockID + ". Please check spelling.";
                    return false;
                }

                Int64 EndDateLong = 0;
                if (!Int64.TryParse(ThisScript.ExecutionTimeEnd, out EndDateLong))
                {
                    Message = "Invalid end datetime at script ID : " + ThisScript.BlockID + ". Please check spelling.";
                    return false;
                }

                int Life = 0;
                if (!int.TryParse(ThisScript.Life, out Life))
                {
                    Message = "Invalid life time at " + ThisScript.BlockID + ". Please check spelling.";
                    return false;
                }

                if (String.IsNullOrEmpty(ThisScript.TargetID))
                {
                    Message = "Invalid TargetID can't be null or empty value at Script ID : " + ThisScript.BlockID + ". Please check.";
                    return false;
                }

                if (String.IsNullOrEmpty(ThisScript.Owner))
                {
                    Message = "Invalid Owner can't be null or empty value at Script ID : " + ThisScript.BlockID + ". Please check.";
                    return false;
                }

                ReturnKnowType CommandResult = CommandDefinition.VerifyCommand(StationName, DeviceCategory, CommandName, ThisScript.Parameters.ToArray());
                if (CommandResult.ReturnType == ReturnStatus.FAILED)
                {
                    Message = "Invalid parameter at script ID : " + ThisScript.ScriptID + " at Command " + CommandName +". With: " + CommandResult.ReturnMessage +". Please check spelling.";
                    return false;
                }
            }

            Message = "All script verified.";
            return true;
        }

        private static String GetLastestFile(String RootPath)
        {
            String LastestScriptStr = null;
            Int64 LastestFileDate = Properties.Settings.Default.LastestScriptDate;

            if (Directory.Exists(RootPath))
            {
                //Console.WriteLine("FOUND FILE: " + RootPath);

                String[] ScriptList = Directory.GetFiles(RootPath);

                Int64 TempName = 0;

                foreach (String ThisFile in ScriptList)
                {
                    String TempFileNameStr = ThisFile.Replace(RootPath + "\\", "");
                    TempFileNameStr = TempFileNameStr.Replace(".txt", "");

                    Int64.TryParse(TempFileNameStr, out TempName);
                    if (LastestFileDate < TempName)
                    {
                        LastestScriptStr = ThisFile;
                    }
                    
                }
            }
            else
            {
                Directory.CreateDirectory(RootPath);
            }

            return LastestScriptStr;
        }

        private static void initialConfig()
        {           
            if (scriptConfigure != null)
            {
                foreach (ScriptConfigure config in scriptConfigure)
                {
                    StationScript stationScript = new StationScript(TTCSHelper.StationStrConveter(config.config_name));
                    stationScript.LastestScriptFileName = config.config_value;
                    ScriptStation.Add(stationScript);
                }
            }          
        }
    }
}
