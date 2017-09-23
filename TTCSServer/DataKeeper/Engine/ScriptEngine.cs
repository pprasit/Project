using DataKeeper.Engine.Command;
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

        public static void SetMonitoringObject(Object _ScriptMonitoring)
        {
            ScriptEngine._ScriptMonitoring = _ScriptMonitoring;
        }

        public static void RefreshScript(STATIONNAME StationName)
        {
            StationScript ThisStation = GetStationScript(StationName);
            if(ThisStation != null)
                ThisStation.RemoveAllScript();
        }

        public static void NewScriptChecker(String ScriptServerAddress, String LoginUser, String LoginPassword)
        {
            Task ScriptTask = Task.Run(async () =>
            {
                while (true)
                {
                    String LastestScript = GetLastestFile("\\\\192.168.2.110/ftp/Script");
                    STATIONNAME ScriptStationName = STATIONNAME.NULL;
                    if (ExtractScriptData(LastestScript, ScriptServerAddress, LoginUser, LoginPassword, out ScriptStationName))
                        SendScriptToStation(ScriptStationName);

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
                StationScript StationScript = ScriptStation.FirstOrDefault(Item => Item.StationName == ScriptStationName);

                if(StationCommunication.NewScriptInformation(StationScript.GetScript(), out Message))
                {

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
            ScriptList.Add(new ScriptStructureNew(DateTime.Now.Ticks.ToString(), DateTime.Now.Ticks.ToString(), "30", STATIONNAME.ASTROPARK.ToString(), DEVICENAME.ASTROPARK_TS700MM.ToString(), TS700MMSET.TS700MM_MOUNT_SETENABLE.ToString(), new List<String> { }, SCRIPTSTATE.WAITINGSERVER.ToString(), DateTime.Now.AddMinutes(-2).Ticks.ToString(), DateTime.Now.AddMinutes(+2).Ticks.ToString()));
            ScriptList.Add(new ScriptStructureNew(DateTime.Now.Ticks.ToString(), DateTime.Now.Ticks.ToString(), "30", STATIONNAME.ASTROPARK.ToString(), DEVICENAME.ASTROPARK_IMAGING.ToString(), IMAGINGSET.IMAGING_CCD_EXPOSE.ToString(), new List<String> { "FileName", "12.0", "true" }, SCRIPTSTATE.WAITINGSERVER.ToString(), DateTime.Now.AddMinutes(-2).Ticks.ToString(), DateTime.Now.AddMinutes(+2).Ticks.ToString()));

            String DataJsonTest = ConvertObjectToJSon(ScriptList);
            var remoteFileStream = Client.OpenWrite(@"\Script\" + DateTime.Now.Ticks.ToString() + ".txt");

            Byte[] DataByte = System.Text.Encoding.UTF8.GetBytes(DataJsonTest);
            remoteFileStream.Write(DataByte, 0, DataByte.Count());
            //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        }

        private static Boolean ExtractScriptData(String FileName, String FTPServerAddress, String LoginUser, String LoginPassword, out STATIONNAME StationName)
        {
            Boolean IsScriptOK = false;
            StationName = STATIONNAME.NULL;

            using (var Impersonation = new ImpersonateUser(LoginUser, FTPServerAddress, LoginPassword, ImpersonateUser.LOGON32_LOGON_NEW_CREDENTIALS))
            {
                String FilePathStr = FileName;
                using (FileStream fs = new FileStream(FilePathStr, FileMode.Open, FileAccess.Read, FileShare.None))
                {
                    using (StreamReader r = new StreamReader(fs))
                    {
                        String jsonString = r.ReadToEnd();
                        List<ScriptStructureNew> NewScriptCollection = ConvertJSonToObject<List<ScriptStructureNew>>(jsonString);

                        //if (!IsTheSameScript(NewScriptCollection))
                        //{
                        //    String Message = "";
                        //    if (VerifyScript(NewScriptCollection, out Message))
                        //    {
                        //        StationName = TTCSHelper.StationStrConveter(NewScriptCollection.FirstOrDefault().StationName);
                        //        StationScript ThisStation = GetStationScript(StationName);
                        //        ThisStation.AddScript(NewScriptCollection);
                        //        DisplayScript(Message, StationName);
                        //        IsScriptOK = true;
                        //    }
                        //    else
                        //        DisplayScriptMessage(Message);
                        //}

                        fs.Close();
                    }
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

        private static Boolean IsTheSameScript(List<ScriptStructureNew> NewScriptCollection, STATIONNAME StationName)
        {
            StationScript ThisStation = GetStationScript(StationName);

            if (NewScriptCollection.Count() != ThisStation.ScriptCollection.Count())
                return false;

            for (int i = 0; i < NewScriptCollection.Count(); i++)
                if (NewScriptCollection[i].ScriptID != ThisStation.ScriptCollection[i].ScriptID)
                    return false;

            return true;
        }

        private static Boolean VerifyScript(List<ScriptStructureNew> ScriptCollection, out String Message)
        {
            foreach (ScriptStructureNew ThisScript in ScriptCollection)
            {
                STATIONNAME StationName = TTCSHelper.StationStrConveter(ThisScript.StationName);
                if (StationName == STATIONNAME.NULL)
                {
                    Message = "Invlid station same at script ID : " + ThisScript.ScriptID + ". Please check spiling.";
                    return false;
                }

                DEVICENAME DeviceName = TTCSHelper.DeviceNameStrConverter(ThisScript.DeviceName);
                if (DeviceName == DEVICENAME.NULL)
                {
                    Message = "Invlid device name at script ID : " + ThisScript.ScriptID + ". Please check spiling.";
                    return false;
                }

                DEVICECATEGORY DeviceCategory = TTCSHelper.ConvertDeviceNameToDeviceCategory(StationName, DeviceName);
                if (DeviceCategory == DEVICECATEGORY.NULL)
                {
                    Message = "Invlid devicecategory at script ID : " + ThisScript.ScriptID + ". Please check spiling.";
                    return false;
                }

                Object CommandName = TTCSHelper.CommandNameConverter(DeviceCategory, ThisScript.CommandName);
                if (CommandName == null)
                {
                    Message = "Invlid command name at script ID : " + ThisScript.ScriptID + ". Please check spiling.";
                    return false;
                }

                Int64 StartDateLong = 0;
                if (!Int64.TryParse(ThisScript.ExecuteionTimeStart, out StartDateLong))
                {
                    Message = "Invlid start datetime at script ID : " + ThisScript.ScriptID + ". Please check spiling.";
                    return false;
                }

                Int64 EndDateLong = 0;
                if (!Int64.TryParse(ThisScript.ExecuteionTimeEnd, out EndDateLong))
                {
                    Message = "Invlid end datetime at script ID : " + ThisScript.ScriptID + ". Please check spiling.";
                    return false;
                }

                int Life = 0;
                if (!int.TryParse(ThisScript.Life, out Life))
                {
                    Message = "Invlid life time at " + ThisScript.ScriptID + ". Please check spiling.";
                    return false;
                }

                ReturnKnowType CommandResult = CommandDefinition.VerifyCommand(StationName, DeviceCategory, CommandName, ThisScript.Parameters.ToArray());
                if (CommandResult.ReturnType == ReturnStatus.FAILED)
                {
                    Message = "Invlid parameter at script ID : " + ThisScript.ScriptID + ". Please check spiling.";
                    return false;
                }
            }

            Message = "All script verified.";
            return true;
        }

        public static string ConvertObjectToJSon<T>(T obj)
        {
            DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(T));
            MemoryStream ms = new MemoryStream();
            ser.WriteObject(ms, obj);
            string jsonString = Encoding.UTF8.GetString(ms.ToArray());
            ms.Close();
            return jsonString;
        }

        public static T ConvertJSonToObject<T>(string jsonString)
        {
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(T));
            MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(jsonString));
            T obj = (T)serializer.ReadObject(ms);
            return obj;
        }

        private static String GetLastestFile(String RootPath)
        {
            String[] ScriptList = Directory.GetFiles(RootPath);

            String LastestScriptStr = null;
            Int64 LastestFileDate = Properties.Settings.Default.LastestScriptDate;

            foreach (String ThisFile in ScriptList)
            {
                String TempFileNameStr = ThisFile.Replace(RootPath + "\\", "");
                TempFileNameStr = TempFileNameStr.Replace(".txt", "");
                Int64 TempName = 0;
                Int64.TryParse(TempFileNameStr, out TempName);
                if (LastestFileDate < TempName)
                    LastestScriptStr = ThisFile;
            }

            return LastestScriptStr;
        }
    }
}
