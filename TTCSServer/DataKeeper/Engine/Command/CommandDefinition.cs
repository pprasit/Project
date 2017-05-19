using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataKeeper.Engine.Command
{
    public class TCSCommandStructure
    {
        public List<STATIONNAME> StationOwner { get; set; }
        public List<DEVICENAME> DeviceOwner { get; set; }
        public DEVICECATEGORY DeviceCategory { get; set; }
        public dynamic Command { get; set; }
        public List<Type> Parameter { get; set; }
        public List<String> ParameterDescription { get; set; }
    }

    public class TTCSCommandDisplay
    {
        public String CommandName { get; set; }
        public String Parameter { get; set; }
        public String ParameterDescription { get; set; }
    }

    public class CommandDefinition
    {
        private static List<TCSCommandStructure> CommandList = new List<TCSCommandStructure>();

        public static List<TTCSCommandDisplay> GetListCommandName(STATIONNAME StationName, DEVICECATEGORY DeviceCategory)
        {
            List<TCSCommandStructure> ListOfCommands = CommandList.Where(Item => Item.StationOwner.Exists(Item2 => Item2 == StationName) && Item.DeviceCategory == DeviceCategory).ToList();
            List<TTCSCommandDisplay> ListOfDisplayCommand = new List<TTCSCommandDisplay>();

            foreach (TCSCommandStructure ThisCommand in ListOfCommands)
            {
                TTCSCommandDisplay NewReturnCommand = new TTCSCommandDisplay();
                NewReturnCommand.CommandName = ThisCommand.Command.ToString();

                String ParameterStr = "";
                if (ThisCommand.Parameter != null)
                    ParameterStr = String.Join(", ", ThisCommand.Parameter);

                ParameterStr = ParameterStr.Replace("System.", "");
                NewReturnCommand.Parameter = ParameterStr;

                String ParaDesc = "";
                if (ThisCommand.ParameterDescription != null)
                    ParaDesc = String.Join("<br>", ThisCommand.ParameterDescription);

                NewReturnCommand.ParameterDescription = ParaDesc;

                ListOfDisplayCommand.Add(NewReturnCommand);
            }

            return ListOfDisplayCommand;
        }

        public static ReturnKnowType VerifyCommand(STATIONNAME StationName, DEVICECATEGORY DeviceCategory, dynamic CommandName, Object[] Value)
        {
            if (CommandName == null)
                return ReturnKnowType.DefineReturn(ReturnStatus.FAILED, "(#CD001) Failed to get command name from list see. (Command name was not avaliable.)");

            TCSCommandStructure ThisCommand = CommandList.FirstOrDefault(Item => Item.StationOwner.Contains(StationName) && Item.DeviceCategory == DeviceCategory && Item.Command.ToString() == CommandName.ToString());

            if (ThisCommand != null)
            {
                List<String> ValueStr = new List<String>();
                foreach (Object ThisValue in Value)
                    ValueStr.Add(ThisValue.ToString());

                Object[] Values = ValueConvertion(ValueStr.ToArray());
                ReturnKnowType ThisReturn = VerifyParameter(ThisCommand, Values);
                return ThisReturn;
            }

            return ReturnKnowType.DefineReturn(ReturnStatus.FAILED, "(#CD004) Could not be found commmand (" + CommandName + ") that contain station name : (" + StationName + ") Device Name : (" + DeviceCategory + "). Please check station name or device name.");
        }

        private static ReturnKnowType VerifyParameter(TCSCommandStructure ThisCommand, Object[] Value)
        {
            String ParameterFormat = "";

            if (ThisCommand.Parameter != null)
                ParameterFormat = String.Join(", ", ThisCommand.Parameter);

            if ((Value == null || Value.Count() == 0) && ThisCommand.Parameter != null)
                return ReturnKnowType.DefineReturn(ReturnStatus.FAILED, "(#CD005) Missing parameter for Command name " + ThisCommand.Command + "(" + ParameterFormat + ")");
            else if ((Value == null || Value.Count() == 0) && ThisCommand.Parameter == null)
                return ReturnKnowType.DefineReturn(ReturnStatus.SUCESSFUL, null);
            else if ((Value == null || Value.Count() == 0) && ThisCommand.Parameter == null)
                return ReturnKnowType.DefineReturn(ReturnStatus.FAILED, "(#CD006) Command name (" + ThisCommand.Command + ") has no parameter. Please check.");

            if (ThisCommand.Parameter.Count() != Value.Count())
                return ReturnKnowType.DefineReturn(ReturnStatus.FAILED, "(#CD002) Command name (" + ThisCommand.Command + ") does not contain " + Value.Count() + " Parameters. Please cheange it to " + ThisCommand.Command + "(" + ParameterFormat + ")");
            else if (ThisCommand.Parameter.Count() == 0 && Value.Count() == 0)
                return ReturnKnowType.DefineReturn(ReturnStatus.SUCESSFUL, null);

            for (int i = 0; i < Value.Count(); i++)
            {
                Type ParaType = VerifyDataType(Value[i], ThisCommand.Parameter[i]);
                if (ThisCommand.Parameter[i] != ParaType)
                    return ReturnKnowType.DefineReturn(ReturnStatus.FAILED, "(#CD003) Invalid parameter see. Command name " + ThisCommand.Command + "(" + ParameterFormat + ")");
            }

            return ReturnKnowType.DefineReturn(ReturnStatus.SUCESSFUL, null);
        }

        public static Object[] ValueConvertion(String[] ValueStr)
        {
            List<Object> ListValue = new List<Object>();

            foreach (String ThisValue in ValueStr)
            {
                if (ThisValue == null || ThisValue == "")
                    return null;

                int ValueInt = 0;
                Double ValueDouble = 0.0;
                Boolean ValueBoolean = false;

                if (ThisValue.Contains("."))
                {
                    if (Double.TryParse(ThisValue, out ValueDouble))
                        ListValue.Add(ValueDouble);
                    else
                        ListValue.Add(ThisValue);
                }
                else if (int.TryParse(ThisValue, out ValueInt))
                    ListValue.Add(ValueInt);
                else if (Boolean.TryParse(ThisValue, out ValueBoolean))
                    ListValue.Add(ValueBoolean);
                else
                    ListValue.Add(ThisValue);
            }

            return ListValue.ToArray();
        }

        private static Type VerifyDataType(Object Value, Type DataType)
        {
            if (Value == null)
                return null;

            if (Value.GetType() == DataType)
                return DataType;
            else
                return null;
        }

        private static Type VerifyDataType(String Value, Type DataType)
        {
            if (Value == null || Value == "")
                return null;

            if (DataType == typeof(int))
            {
                int TInt = 0;
                if (int.TryParse(Value, out TInt))
                    return typeof(int);
                return null;
            }
            else if (DataType == typeof(Double))
            {
                Double TDouble = 0.0;
                if (Double.TryParse(Value, out TDouble))
                    return typeof(Double);
                return null;
            }
            else if (DataType == typeof(Boolean))
            {
                Boolean TBoolean = false;
                if (Boolean.TryParse(Value, out TBoolean))
                    return typeof(Boolean);
                return null;
            }

            return typeof(String);
        }

        public static TCSCommandStructure GetCommandDefinition(dynamic CommandName)
        {
            try
            {
                TCSCommandStructure ThisCommand = CommandList.FirstOrDefault(Item => Item.Command == CommandName);
                if (ThisCommand.Parameter.Count > 0)
                {
                    String Temp = "";
                }

                return ThisCommand;
            }
            catch { return null; }
        }

        public static void DefineCommand()
        {
            #region AstroServer

            CrateCommand(new List<STATIONNAME>() { STATIONNAME.ASTROSERVER }, DEVICECATEGORY.ASTROSERVER, ASTROSERVERSET.ASTROSERVER_DATABASE_SYNC, Decare2Parameter(typeof(String), typeof(String)), ParaDesc2("Parameter 1 : Destination station name to sync -> Data type String. Example 'AIRFORCE'", "Parameter 2 : Table name that you going to sync -> Data type String. Example 'UserTB'"));

            #endregion

            #region Both Focuser

            CrateCommand(T07StationList(), DEVICECATEGORY.TS700MM, TS700MMSET.TS700MM_FOCUSER_SETCONNECT, null, null);
            CrateCommand(T07StationList(), DEVICECATEGORY.TS700MM, TS700MMSET.TS700MM_FOCUSER_SETDISCONNECT, null, null);
            CrateCommand(T07StationList(), DEVICECATEGORY.TS700MM, TS700MMSET.TS700MM_FOCUSER_SETPOSITION, Decare1Parameter(typeof(int)), ParaDesc1("Parameter 1 : Focuser position -> Data type Interger 0 - 30000 microns. Example 23000."));
            CrateCommand(T07StationList(), DEVICECATEGORY.TS700MM, TS700MMSET.TS700MM_FOCUSER_SETINCREMENT, Decare1Parameter(typeof(int)), ParaDesc1("Parameter 1 : Focuser position -> Data type Interger in microns. Example -100"));
            CrateCommand(T07StationList(), DEVICECATEGORY.TS700MM, TS700MMSET.TS700MM_FOCUSER_SETSTOP, null, null);
            CrateCommand(T07StationList(), DEVICECATEGORY.TS700MM, TS700MMSET.TS700MM_FOCUSER_STARTAUTOFOCUS, null, null);

            CrateCommand(T07StationList(), DEVICECATEGORY.TS700MM, TS700MMSET.TS700MM_FOCUSER1_SETCONNECT, null, null);
            CrateCommand(T07StationList(), DEVICECATEGORY.TS700MM, TS700MMSET.TS700MM_FOCUSER1_SETDISCONNECT, null, null);
            CrateCommand(T07StationList(), DEVICECATEGORY.TS700MM, TS700MMSET.TS700MM_FOCUSER1_SETPOSITION, Decare1Parameter(typeof(int)), ParaDesc1("Parameter 1 : Focuser 1 position -> Data type Interger 0 - 30000 microns. Example 23000"));
            CrateCommand(T07StationList(), DEVICECATEGORY.TS700MM, TS700MMSET.TS700MM_FOCUSER1_SETINCREMENT, Decare1Parameter(typeof(int)), ParaDesc1("Parameter 1 : Focuser 1 position -> Data type Interger in microns. Example -100"));
            CrateCommand(T07StationList(), DEVICECATEGORY.TS700MM, TS700MMSET.TS700MM_FOCUSER1_SETSTOP, null, null);
            CrateCommand(T07StationList(), DEVICECATEGORY.TS700MM, TS700MMSET.TS700MM_FOCUSER1_STARTAUTOFOCUS, null, null);

            CrateCommand(T07StationList(), DEVICECATEGORY.TS700MM, TS700MMSET.TS700MM_FOCUSER2_SETCONNECT, null, null);
            CrateCommand(T07StationList(), DEVICECATEGORY.TS700MM, TS700MMSET.TS700MM_FOCUSER2_SETDISCONNECT, null, null);
            CrateCommand(T07StationList(), DEVICECATEGORY.TS700MM, TS700MMSET.TS700MM_FOCUSER2_SETPOSITION, Decare1Parameter(typeof(int)), ParaDesc1("Parameter 1 : Focuser 2 position -> Data type Interger 0 - 30000 microns. Example 23000"));
            CrateCommand(T07StationList(), DEVICECATEGORY.TS700MM, TS700MMSET.TS700MM_FOCUSER2_SETINCREMENT, Decare1Parameter(typeof(int)), ParaDesc1("Parameter 1 : Focuser 2 position -> Data type Interger in microns. Example -100"));
            CrateCommand(T07StationList(), DEVICECATEGORY.TS700MM, TS700MMSET.TS700MM_FOCUSER2_SETSTOP, null, null);
            CrateCommand(T07StationList(), DEVICECATEGORY.TS700MM, TS700MMSET.TS700MM_FOCUSER2_STARTAUTOFOCUS, null, null);

            #endregion

            #region Both Rotator

            CrateCommand(T07StationList(), DEVICECATEGORY.TS700MM, TS700MMSET.TS700MM_ROTATOR_SETPOSITION, Decare1Parameter(typeof(Double)), ParaDesc1("Parameter 1 : Rotator position -> Data type Double 0 - 359 Degree. Example 125.00"));
            CrateCommand(T07StationList(), DEVICECATEGORY.TS700MM, TS700MMSET.TS700MM_ROTATOR_SETINCREMENT, Decare1Parameter(typeof(Double)), ParaDesc1("Parameter 1 : Rotator position -> Data type Double 0 - 359 Degree. Example 13.00"));
            CrateCommand(T07StationList(), DEVICECATEGORY.TS700MM, TS700MMSET.TS700MM_ROTATOR_SETSTOP, null, null);
            CrateCommand(T07StationList(), DEVICECATEGORY.TS700MM, TS700MMSET.TS700MM_ROTATOR_SETDEROTATORSTART, null, null);
            CrateCommand(T07StationList(), DEVICECATEGORY.TS700MM, TS700MMSET.TS700MM_ROTATOR_SETDEROTATORSTOP, null, null);

            CrateCommand(T07StationList(), DEVICECATEGORY.TS700MM, TS700MMSET.TS700MM_ROTATOR1_SETPOSITION, Decare1Parameter(typeof(Double)), ParaDesc1("Parameter 1 : Rotator 1 position -> Data type Double 0 - 359 Degree. Example 125.00"));
            CrateCommand(T07StationList(), DEVICECATEGORY.TS700MM, TS700MMSET.TS700MM_ROTATOR1_SETINCREMENT, Decare1Parameter(typeof(Double)), ParaDesc1("Parameter 1 : Rotator 1 position -> Data type Double 0 - 359 Degree. Example 13.00"));
            CrateCommand(T07StationList(), DEVICECATEGORY.TS700MM, TS700MMSET.TS700MM_ROTATOR1_SETSTOP, null, null);
            CrateCommand(T07StationList(), DEVICECATEGORY.TS700MM, TS700MMSET.TS700MM_ROTATOR1_SETDEROTATORSTART, null, null);
            CrateCommand(T07StationList(), DEVICECATEGORY.TS700MM, TS700MMSET.TS700MM_ROTATOR1_SETDEROTATORSTOP, null, null);

            CrateCommand(T07StationList(), DEVICECATEGORY.TS700MM, TS700MMSET.TS700MM_ROTATOR2_SETPOSITION, Decare1Parameter(typeof(Double)), ParaDesc1("Parameter 1 : Rotator 2 position -> Data type Double 0 - 359 Degree. Example 125.00"));
            CrateCommand(T07StationList(), DEVICECATEGORY.TS700MM, TS700MMSET.TS700MM_ROTATOR2_SETINCREMENT, Decare1Parameter(typeof(Double)), ParaDesc1("Parameter 1 : Rotator 2 position -> Data type Double 0 - 359 Degree. Example 13.00"));
            CrateCommand(T07StationList(), DEVICECATEGORY.TS700MM, TS700MMSET.TS700MM_ROTATOR2_SETSTOP, null, null);
            CrateCommand(T07StationList(), DEVICECATEGORY.TS700MM, TS700MMSET.TS700MM_ROTATOR2_SETDEROTATORSTART, null, null);
            CrateCommand(T07StationList(), DEVICECATEGORY.TS700MM, TS700MMSET.TS700MM_ROTATOR2_SETDEROTATORSTOP, null, null);

            #endregion

            #region Mount

            CrateCommand(T07StationList(), DEVICECATEGORY.TS700MM, TS700MMSET.TS700MM_MOUNT_SETCONNECT, null, null);
            CrateCommand(T07StationList(), DEVICECATEGORY.TS700MM, TS700MMSET.TS700MM_MOUNT_SETDISCONNECT, null, null);
            CrateCommand(T07StationList(), DEVICECATEGORY.TS700MM, TS700MMSET.TS700MM_MOUNT_SETENABLE, null, null);
            CrateCommand(T07StationList(), DEVICECATEGORY.TS700MM, TS700MMSET.TS700MM_MOUNT_SETDISABLE, null, null);
            CrateCommand(T07StationList(), DEVICECATEGORY.TS700MM, TS700MMSET.TS700MM_MOUNT_SLEWINCREMENTRADEC, Decare2Parameter(typeof(Double), typeof(Double)), ParaDesc2("Parameter 1 : Increment Ra -> Data type Double in arcsecs. Example 3.14", "Parameter 2 : Increment Dec -> Data type Double in arcsecs. Example 3.14"));
            CrateCommand(T07StationList(), DEVICECATEGORY.TS700MM, TS700MMSET.TS700MM_MOUNT_SLEWINCREMENTAZMALT, Decare2Parameter(typeof(Double), typeof(Double)), ParaDesc2("Parameter 1 : Increment Azm -> Data type Double in arcsecs. Example 3.14", "Parameter 2 : Increment Alt -> Data type Double in arcsecs. Example 3.14"));
            CrateCommand(T07StationList(), DEVICECATEGORY.TS700MM, TS700MMSET.TS700MM_MOUNT_SLEWRADEC, Decare2Parameter(typeof(String), typeof(String)), ParaDesc2("Parameter 1 : Ra -> Data type String. Example 4 55 23.32", "Parameter 2 :Dec -> Data type String. Example +14 32 54.12"));
            CrateCommand(T07StationList(), DEVICECATEGORY.TS700MM, TS700MMSET.TS700MM_MOUNT_SLEWRADEC2000, Decare2Parameter(typeof(String), typeof(String)), ParaDesc2("Parameter 1 : Ra -> Data type String. Example 4 55 23.32", "Parameter 2 : Dec -> Data type String. Example +14 32 54.12"));
            CrateCommand(T07StationList(), DEVICECATEGORY.TS700MM, TS700MMSET.TS700MM_MOUNT_SLEWAZMALT, Decare2Parameter(typeof(Double), typeof(Double)), ParaDesc2("Parameter 1 : Azm -> Data type Double. Example 36.14 Degree", "Parameter 2 : Alt -> Data type Double. Example 12.05 Degree"));
            CrateCommand(T07StationList(), DEVICECATEGORY.TS700MM, TS700MMSET.TS700MM_MOUNT_SETSTOP, null, null);
            CrateCommand(T07StationList(), DEVICECATEGORY.TS700MM, TS700MMSET.TS700MM_MOUNT_SETTRACKINGON, null, null);
            CrateCommand(T07StationList(), DEVICECATEGORY.TS700MM, TS700MMSET.TS700MM_MOUNT_SETTRACKINGOFF, null, null);
            CrateCommand(T07StationList(), DEVICECATEGORY.TS700MM, TS700MMSET.TS700MM_MOUNT_SETTRACKINGRATES, Decare2Parameter(typeof(Double), typeof(Double)), ParaDesc2("Parameter 1 : Rarate -> Data type Double. Example 12.01 Arcsec/Sec", "Parameter 2 : Decrate -> Data type Double. Example 5.012 Arcsec/sec"));
            CrateCommand(T07StationList(), DEVICECATEGORY.TS700MM, TS700MMSET.TS700MM_MOUNT_SETMOUNTMODEL, Decare1Parameter(typeof(String)), ParaDesc1("Parameter 1 : Mount model file name -> Data type String. Example 'Port2-21-12-2016 300points.PXP'"));
            CrateCommand(T07StationList(), DEVICECATEGORY.TS700MM, TS700MMSET.TS700MM_MOUNT_SETJOG, Decare2Parameter(typeof(Double), typeof(Double)), ParaDesc2("Parameter 1 : altitude axis -> Data type Double. Example 5.2 Degree/Sec", "Parameter 2 : azimuth axis -> Data type Double. Example 1.25 Degree/Sec"));
            CrateCommand(T07StationList(), DEVICECATEGORY.TS700MM, TS700MMSET.TS700MM_MOUNT_SETHOME, null, null);

            #endregion

            #region M3

            CrateCommand(T07StationList(), DEVICECATEGORY.TS700MM, TS700MMSET.TS700MM_M3_SETCONNECT, null, null);
            CrateCommand(T07StationList(), DEVICECATEGORY.TS700MM, TS700MMSET.TS700MM_M3_SETDISCONNECT, null, null);
            CrateCommand(T07StationList(), DEVICECATEGORY.TS700MM, TS700MMSET.TS700MM_M3_SETSELECTPORT, Decare1Parameter(typeof(int)), ParaDesc1("Parameter 1 : Port number -> Data type Integer port 1 or port 2. Example 1"));
            CrateCommand(T07StationList(), DEVICECATEGORY.TS700MM, TS700MMSET.TS700MM_M3_SETSTOP, null, null);

            #endregion

            #region Imaging

            CrateCommand(T07StationList(), DEVICECATEGORY.IMAGING, IMAGINGSET.IMAGING_CCD_CONNECT, null, null);
            CrateCommand(T07StationList(), DEVICECATEGORY.IMAGING, IMAGINGSET.IMAGING_CCD_DISCONNECT, null, null);
            CrateCommand(T07StationList(), DEVICECATEGORY.IMAGING, IMAGINGSET.IMAGING_CCD_EXPOSE, Decare3Parameter(typeof(String), typeof(Double), typeof(Boolean)), ParaDesc3("Parameter 1 : Image Name -> Data type String. Example Jupiter_03022016", "Parameter 2 : Exposure time -> Data type Double seconds. Example 30.5", "Parameter 2 : Taking light frame -> Data Type Boolean Double. Example true, false"));
            CrateCommand(T07StationList(), DEVICECATEGORY.IMAGING, IMAGINGSET.IMAGING_CCD_ABORTEXPOSE, null, null);
            CrateCommand(T07StationList(), DEVICECATEGORY.IMAGING, IMAGINGSET.IMAGING_CCD_QUIT, null, null);
            CrateCommand(T07StationList(), DEVICECATEGORY.IMAGING, IMAGINGSET.IMAGING_CCD_FITSKEY, Decare2Parameter(typeof(String), typeof(Object)), ParaDesc2("Parameter 1 : FITS Key name -> Data type String. Example RA", "Parameter 2 : FITS value object -> Data type object. Example 10 25 30.2"));
            CrateCommand(T07StationList(), DEVICECATEGORY.IMAGING, IMAGINGSET.IMAGING_CCD_FULLFRAME, null, null);
            CrateCommand(T07StationList(), DEVICECATEGORY.IMAGING, IMAGINGSET.IMAGING_CCD_SHOWWINDOW, Decare1Parameter(typeof(Boolean)), ParaDesc1("Parameter 1 : Turn window on or off -> Data type Boolean true or false. Example true"));
            CrateCommand(T07StationList(), DEVICECATEGORY.IMAGING, IMAGINGSET.IMAGING_CCD_STARTDOWNLOAD, null, null);
            CrateCommand(T07StationList(), DEVICECATEGORY.IMAGING, IMAGINGSET.IMAGING_CCD_AUTODOWNLOAD, Decare1Parameter(typeof(Boolean)), ParaDesc1("Parameter 1 : Set auto download on or off after expose -> Data type Boolean true or false. Example true"));
            CrateCommand(T07StationList(), DEVICECATEGORY.IMAGING, IMAGINGSET.IMAGING_CCD_BINXY, Decare2Parameter(typeof(int), typeof(int)), ParaDesc2("Parameter 1 : Set binX -> Data type int. Example 1", "Parameter 2 : Set binY -> Data type int. Example 2"));
            CrateCommand(T07StationList(), DEVICECATEGORY.IMAGING, IMAGINGSET.IMAGING_CCD_COOLERON, Decare1Parameter(typeof(Boolean)), ParaDesc1("Parameter 1 : Set cooling system of the CCD on or off -> Data type Boolean true or false. Example true"));
            CrateCommand(T07StationList(), DEVICECATEGORY.IMAGING, IMAGINGSET.IMAGING_CCD_FANENABLE, Decare1Parameter(typeof(Boolean)), ParaDesc1("Parameter 1 : Set fan of the CCD on or off -> Data type Boolean true or false. Example true"));
            CrateCommand(T07StationList(), DEVICECATEGORY.IMAGING, IMAGINGSET.IMAGING_CCD_FASTREADOUT, Decare1Parameter(typeof(Boolean)), ParaDesc1("Parameter 1 : Set readout speed of the CCD to fast mode -> Data type Boolean true or false. Example true"));
            CrateCommand(T07StationList(), DEVICECATEGORY.IMAGING, IMAGINGSET.IMAGING_CCD_FILTER, Decare1Parameter(typeof(int)), ParaDesc1("Parameter 1 : Set active position of the filter wheel-> Data type int. Example 3"));
            CrateCommand(T07StationList(), DEVICECATEGORY.IMAGING, IMAGINGSET.IMAGING_CCD_SUBFRAMESIZE, Decare2Parameter(typeof(int), typeof(int)), ParaDesc2("Parameter 1 : Set subframe size width -> Data type int. Example 1024", "Parameter 2 : Set subframe size height -> Data type int. Example 480"));
            CrateCommand(T07StationList(), DEVICECATEGORY.IMAGING, IMAGINGSET.IMAGING_CCD_READOUTMODE, Decare1Parameter(typeof(int)), ParaDesc1("Parameter 1 : Set readout mode -> Data type int. Example 2"));
            CrateCommand(T07StationList(), DEVICECATEGORY.IMAGING, IMAGINGSET.IMAGING_CCD_SETSPEED, Decare1Parameter(typeof(int)), ParaDesc1("Parameter 1 : Set ISO speed -> Data type int. Example 2"));
            CrateCommand(T07StationList(), DEVICECATEGORY.IMAGING, IMAGINGSET.IMAGING_CCD_SUBFRAMESTARTXY, Decare2Parameter(typeof(int), typeof(int)), ParaDesc2("Parameter 1 : Set subframe X -> Data type int. Example 1024", "Parameter 2 : Set subframe Y -> Data type int. Example 480"));
            CrateCommand(T07StationList(), DEVICECATEGORY.IMAGING, IMAGINGSET.IMAGING_CCD_TEMPERATURESETPOINT, Decare1Parameter(typeof(int)), ParaDesc1("Parameter 1 : Set CCD temperature set point -> Data type int. Example -20"));
            CrateCommand(T07StationList(), DEVICECATEGORY.IMAGING, IMAGINGSET.IMAGING_IMAGE_DISPLAY_SIZE, Decare1Parameter(typeof(int)), ParaDesc1("Parameter 1 : Set display size by setting ratio of resolution 1.00 = same, > 1.00 = increase resolution, < 1.00 = decrease resolution -> Data type double. Example 1.05"));
            CrateCommand(T07StationList(), DEVICECATEGORY.IMAGING, IMAGINGSET.IMAGING_IMAGE_STRETCH_PROFILE, Decare1Parameter(typeof(int)), ParaDesc1("Parameter 1 : Set stretch profile to adjust a perview image 1 - 5 -> Data type int. Example 3"));
            CrateCommand(T07StationList(), DEVICECATEGORY.IMAGING, IMAGINGSET.IMAGING_IMAGE_FILENAME, Decare1Parameter(typeof(String)), ParaDesc1("Parameter 1 : Set image file name -> Data type String. Example Jupiter_03022016"));

            #endregion

            #region LAN Switch

            CrateCommand(T07StationList(), DEVICECATEGORY.LANOUTLET, LANOUTLETSET.LANOUTLET_SWITCHONOFF, Decare2Parameter(typeof(int), typeof(Boolean)), ParaDesc2("Parameter 1 : Power LAN swtich slot number (1-8) -> Data type Integer. Example 2", "Parameter 2 : Set selected slot On or off -> Data type Boolean. Example true"));
            CrateCommand(T07StationList(), DEVICECATEGORY.LANOUTLET, LANOUTLETSET.LANOUTLET_SWITCHSWITCHING, Decare1Parameter(typeof(int)), ParaDesc1("Parameter 1 : Swap selected slot from On to Off or otherwise -> Data type Integer. Example 3"));

            #endregion

            #region Dome

            CrateCommand(T07StationList(), DEVICECATEGORY.ASTROHEVENDOME, DOMESET.Dome_FULLYOPENSHUTTERA, null, null);
            CrateCommand(T07StationList(), DEVICECATEGORY.ASTROHEVENDOME, DOMESET.Dome_FULLYOPENSHUTTERB, null, null);
            CrateCommand(T07StationList(), DEVICECATEGORY.ASTROHEVENDOME, DOMESET.Dome_FULLYCLOSESHUTTERA, null, null);
            CrateCommand(T07StationList(), DEVICECATEGORY.ASTROHEVENDOME, DOMESET.Dome_FULLYCLOSESHUTTERB, null, null);
            CrateCommand(T07StationList(), DEVICECATEGORY.ASTROHEVENDOME, DOMESET.Dome_OPENSHUTTERA, null, null);
            CrateCommand(T07StationList(), DEVICECATEGORY.ASTROHEVENDOME, DOMESET.Dome_OPENSHUTTERB, null, null);
            CrateCommand(T07StationList(), DEVICECATEGORY.ASTROHEVENDOME, DOMESET.Dome_CLOSESHUTTERA, null, null);
            CrateCommand(T07StationList(), DEVICECATEGORY.ASTROHEVENDOME, DOMESET.Dome_CLOSESHUTTERB, null, null);

            #endregion
        }

        public static dynamic GetCommandNameENUM(String CommandNameStr)
        {
            TS700MMSET ThisTS700MMSET = Enum.GetValues(typeof(TS700MMSET)).Cast<TS700MMSET>().ToList().FirstOrDefault(Item => Item.ToString() == CommandNameStr);
            if (ThisTS700MMSET != TS700MMSET.NULL) return ThisTS700MMSET;

            IMAGINGSET ThisIMAGINGSET = Enum.GetValues(typeof(IMAGINGSET)).Cast<IMAGINGSET>().ToList().FirstOrDefault(Item => Item.ToString() == CommandNameStr);
            if (ThisIMAGINGSET != IMAGINGSET.NULL) return ThisIMAGINGSET;

            LANOUTLETSET ThisLANOUTLETSET = Enum.GetValues(typeof(LANOUTLETSET)).Cast<LANOUTLETSET>().ToList().FirstOrDefault(Item => Item.ToString() == CommandNameStr);
            if (ThisLANOUTLETSET != LANOUTLETSET.NULL) return ThisLANOUTLETSET;

            DOMESET ThisDOMESET = Enum.GetValues(typeof(DOMESET)).Cast<DOMESET>().ToList().FirstOrDefault(Item => Item.ToString() == CommandNameStr);
            if (ThisDOMESET != DOMESET.NULL) return ThisDOMESET;

            return null;
        }

        public static List<Type> Decare1Parameter(Type Para1)
        {
            List<Type> Parameter = new List<Type>();
            Parameter.Add(Para1);
            return Parameter;
        }

        public static List<Type> Decare2Parameter(Type Para1, Type Para2)
        {
            List<Type> Parameter = new List<Type>();
            Parameter.Add(Para1);
            Parameter.Add(Para2);
            return Parameter;
        }

        public static List<Type> Decare3Parameter(Type Para1, Type Para2, Type Para3)
        {
            List<Type> Parameter = new List<Type>();
            Parameter.Add(Para1);
            Parameter.Add(Para2);
            Parameter.Add(Para3);
            return Parameter;
        }

        public static List<String> ParaDesc1(String Desc1)
        {
            List<String> Parameter = new List<String>();
            Parameter.Add(Desc1);
            return Parameter;
        }

        public static List<String> ParaDesc2(String Desc1, String Desc2)
        {
            List<String> Parameter = new List<String>();
            Parameter.Add(Desc1);
            Parameter.Add(Desc2);
            return Parameter;
        }

        public static List<String> ParaDesc3(String Desc1, String Desc2, String Desc3)
        {
            List<String> Parameter = new List<String>();
            Parameter.Add(Desc1);
            Parameter.Add(Desc2);
            Parameter.Add(Desc3);
            return Parameter;
        }

        private static void CrateCommand(List<STATIONNAME> StationNames, DEVICECATEGORY DeviceName, dynamic SetCommand, List<Type> Parameter, List<String> ParameterDescription)
        {
            TCSCommandStructure NewCommand = new TCSCommandStructure();
            NewCommand.StationOwner = StationNames;
            NewCommand.DeviceCategory = DeviceName;
            NewCommand.Command = SetCommand;
            NewCommand.Parameter = Parameter;
            NewCommand.ParameterDescription = ParameterDescription;
            CommandList.Add(NewCommand);
        }

        private static List<STATIONNAME> T07StationList()
        {
            return new List<STATIONNAME>() { STATIONNAME.AIRFORCE, STATIONNAME.AUSTRIA, STATIONNAME.CHACHOENGSAO, STATIONNAME.CHINA, STATIONNAME.NAKHONRATCHASIMA, STATIONNAME.SONGKLA, STATIONNAME.USA, STATIONNAME.ASTROPARK };
        }
    }
}
