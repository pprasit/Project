using DataKeeper.Engine;
using DataKeeper.Engine.Command;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TTCSServer
{
    public static class TTCSHelper
    {
        private static int ImageCounter = 0;

        public static String GenNewID()
        {
            long ticks = DateTime.Now.Ticks;
            byte[] bytes = BitConverter.GetBytes(ticks);
            string id = Convert.ToBase64String(bytes)
                                    .Replace('+', '_')
                                    .Replace('/', '-')
                                    .TrimEnd('=');

            return id;
        }

        public static byte[] ImageToByte(Image img)
        {
            byte[] byteArray = new byte[0];
            using (MemoryStream stream = new MemoryStream())
            {
                img.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                stream.Close();

                byteArray = stream.ToArray();
            }
            return byteArray;
        }

        public static ReturnKnowType DefineReturn(String Message, ReturnStatus Status, Object Value)
        {
            ReturnKnowType ThisReturn = new ReturnKnowType();
            ThisReturn.ReturnMessage = Message;
            ThisReturn.ReturnType = Status;
            ThisReturn.ReturnValue = Value;
            ThisReturn.ReturnDateTime = DateTime.Now;

            return ThisReturn;
        }

        public static List<DEVICECATEGORY> DeviceListConveter(String[] Devices)
        {
            if (Devices == null || Devices.Count() == 0)
                return null;

            List<DEVICECATEGORY> TempList = new List<DEVICECATEGORY>();
            List<DEVICECATEGORY> AllDeviceName = Enum.GetValues(typeof(DEVICECATEGORY)).Cast<DEVICECATEGORY>().ToList();

            foreach (DEVICECATEGORY ThisDevice in AllDeviceName)
                foreach (String DeviceStr in Devices)
                    if (ThisDevice.ToString() == DeviceStr)
                        TempList.Add(ThisDevice);

            return TempList;
        }

        public static STATIONNAME? StationConveter(String Name)
        {
            String ThisName = Name.Trim();
            List<STATIONNAME> AllStationName = Enum.GetValues(typeof(STATIONNAME)).Cast<STATIONNAME>().ToList();
            STATIONNAME? StationName = AllStationName.FirstOrDefault(Item => Item.ToString() == ThisName);
            return StationName;
        }

        public static DEVICECATEGORY? DeviceConverter(String Name)
        {
            String ThisName = Name.Trim();
            List<DEVICECATEGORY> AllStationName = Enum.GetValues(typeof(DEVICECATEGORY)).Cast<DEVICECATEGORY>().ToList();
            DEVICECATEGORY? DeviceName = AllStationName.FirstOrDefault(Item => Item.ToString() == ThisName);
            return DeviceName;
        }

        public static STATIONNAME? StationShortNameConveter(String Name)
        {
            if (Name == "TTCS")
                return STATIONNAME.ASTROSERVER;
            else if (Name == "AUS")
                return STATIONNAME.AUSTRALIA;
            else if (Name == "CHA")
                return STATIONNAME.CHINA;
            else if (Name == "AF")
                return STATIONNAME.AIRFORCE;
            else if (Name == "SGK")
                return STATIONNAME.SONGKLA;
            else if (Name == "USA")
                return STATIONNAME.USA;
            else if (Name == "TNO")
                return STATIONNAME.TNO;
            else if (Name == "CCO")
                return STATIONNAME.CHACHOENGSAO;
            return null;
        }

        public static Int64 GetDateTimeMiliSec()
        {
            int ThisYear = DateTime.Now.Year;

            if (ThisYear > 2500)
                ThisYear = ThisYear - 543;

            Int64 MiliSec = DateToMilisec(ThisYear.ToString(), DateTime.Now.Month.ToString(), DateTime.Now.Day.ToString(), DateTime.Now.Hour.ToString(), DateTime.Now.Minute.ToString(),
                DateTime.Now.Second.ToString(), DateTime.Now.Millisecond.ToString());

            return MiliSec;
        }

        public static void MilisecToStringDate(Int64 ThisMilisec, out String Year, out String Month, out String Day, out String Hour, out String Min, out String Sec, out String MiliSec)
        {
            TimeSpan time = TimeSpan.FromMilliseconds(ThisMilisec);
            DateTime ThisDate = new DateTime(time.Ticks);
            int TempYear = ThisDate.Year;

            if (TempYear > 2500)
                TempYear = TempYear - 543;

            Day = String.Format("{0:00}", ThisDate.Day);
            Month = String.Format("{0:00}", ThisDate.Month);
            Year = String.Format("{0:0000}", ThisDate.Year);
            Hour = String.Format("{0:00}", ThisDate.Hour);
            Min = String.Format("{0:00}", ThisDate.Minute);
            Sec = String.Format("{0:00}", ThisDate.Second);
            MiliSec = String.Format("{0:00}", ThisDate.Millisecond);
        }

        public static Int64 DateToMilisec(String Year, String Month, String Day, String Hour, String Min, String Sec, String Milisec)
        {
            DateTime TempDate = new DateTime(Convert.ToInt32(Year), Convert.ToInt32(Month), Convert.ToInt32(Day), Convert.ToInt32(Hour), Convert.ToInt32(Min), Convert.ToInt32(Sec), Convert.ToInt32(Milisec));
            Int64 milliseconds = TempDate.Ticks / TimeSpan.TicksPerMillisecond;

            return milliseconds;
        }

        public static String MMToFocusPosition(Object Value)
        {
            Double Focus = Convert.ToDouble(Value) * 1000;

            return String.Format("{0:+0.000;-0.00;0.000}", Focus);
        }

        public static String HHMMSSToArcsec(Object Value)
        {
            try
            {
                int i = Value.ToString().IndexOf('-');
                String[] AfterSplit = Value.ToString().Split(' ');
                Double FValue = Math.Abs(Convert.ToDouble(AfterSplit[0]) * 3600);
                Double SValue = Math.Abs(Convert.ToDouble(AfterSplit[1]) * 60);
                Double TValue = Math.Abs(Convert.ToDouble(AfterSplit[2]));

                Double Degree = FValue + SValue + TValue;

                if (i == -1)
                    Degree = Math.Abs(Degree);
                else
                    Degree = Math.Abs(Degree) * -1;

                return Degree.ToString();
            }
            catch { return "0"; };
        }

        //---------------------------------------------------------------------------------Degree Conveter Handler----------------------------------------------------------------------------------

        public static String DegreeToHHMMSS(Object Value)
        {
            try
            {
                Double Degree = Convert.ToDouble(Convert.ToDouble(Value) * 180 / Math.PI);
                TimeSpan span = TimeSpan.FromHours(Degree);

                if (span.TotalMilliseconds >= 0)
                    return "+" + String.Format("{0:000}", Math.Abs(span.Hours)) + " " + String.Format("{0:00}", Math.Abs(span.Minutes)) + " " + String.Format("{0:00}", Math.Abs(span.Seconds)) + "." + String.Format("{0:000}", Math.Abs(span.Milliseconds));
                else
                    return "-" + String.Format("{0:000}", Math.Abs(span.Hours)) + " " + String.Format("{0:00}", Math.Abs(span.Minutes)) + " " + String.Format("{0:00}", Math.Abs(span.Seconds)) + "." + String.Format("{0:000}", Math.Abs(span.Milliseconds));
            }
            catch (Exception e) { return e.Message; }
        }

        public static Double DegreesToRadian(Object Degrees)
        {
            Double DegreeDouble = 0;

            if (Double.TryParse(Degrees.ToString(), out DegreeDouble))
            {
                double radians = (Math.PI / 180) * DegreeDouble;
                return (radians);
            }
            return 0;
        }

        //---------------------------------------------------------------------------------Unit to Radian Handler-----------------------------------------------------------------------------------

        public static Double HHMMSSToRadian(Object Value)
        {
            Boolean State = false;
            String[] DataValue = Value.ToString().Split(' ');
            Double DataConverted = 0;

            if (DataValue.Count() > 0)
            {
                for (int i = 0; i < DataValue.Count(); i++)
                {
                    if (IsNumeric(DataValue[i]))
                        State = true;
                    else
                    {
                        State = false;
                        break;
                    }
                }
            }
            else
                State = false;

            //-------------------------------------------------Formula-----------------------------------------------------

            if (State)
            {
                Double D = 0, M = 0, S = 0;
                if (DataValue.Count() == 1)
                {
                    D = Math.Abs(Convert.ToDouble(DataValue[0]));
                    M = 0;
                    S = 0;
                }
                else if (DataValue.Count() == 2)
                {
                    D = Math.Abs(Convert.ToDouble(DataValue[0]));
                    M = Math.Abs(Convert.ToDouble(DataValue[1]));
                    S = 0;
                }
                else if (DataValue.Count() == 3)
                {
                    D = Math.Abs(Convert.ToDouble(DataValue[0]));
                    M = Math.Abs(Convert.ToDouble(DataValue[1]));
                    S = Math.Abs(Convert.ToDouble(DataValue[2]));
                }

                if (!DataValue[0].Contains("-"))
                    DataConverted = ((D + (M + (S / 60)) / 60) / 180 * Math.PI);
                else
                    DataConverted = ((D + (M + (S / 60)) / 60) / 180 * Math.PI) * -1;
            }

            return DataConverted;
        }

        public static Double RAHHMMSSToRadian(Object Value)
        {
            Boolean State = false;
            String[] TempData = Value.ToString().Split(' ');
            Double DataConverted = 0;

            if (TempData.Count() > 0)
            {
                for (int i = 0; i < TempData.Count(); i++)
                {
                    if (IsNumeric(TempData[i]))
                        State = true;
                    else
                    {
                        State = false;
                        break;
                    }
                }
            }
            else
                State = false;

            //-------------------------------------------------Formula-----------------------------------------------------

            if (State)
            {
                Double H = 0, M = 0, S = 0;
                if (TempData.Count() == 1)
                {
                    H = Convert.ToDouble(TempData[0]);
                    M = 0;
                    S = 0;
                }
                else if (TempData.Count() == 2)
                {
                    H = Convert.ToDouble(TempData[0]);
                    M = Convert.ToDouble(TempData[1]);
                    S = 0;
                }
                else if (TempData.Count() == 3)
                {
                    H = Convert.ToDouble(TempData[0]);
                    M = Convert.ToDouble(TempData[1]);
                    S = Convert.ToDouble(TempData[2]);
                }

                DataConverted = (H + (M + (S / 60)) / 60) / 12 * Math.PI;
            }

            return DataConverted;
        }

        public static Double DecDDMMSSToRadian(Object Value)
        {
            Boolean State = false;
            String[] DataValue = Value.ToString().Split(' ');
            Double DataConverted = 0;

            if (DataValue.Count() > 0)
            {
                for (int i = 0; i < DataValue.Count(); i++)
                {
                    if (IsNumeric(DataValue[i]))
                        State = true;
                    else
                    {
                        State = false;
                        break;
                    }
                }
            }
            else
                State = false;

            //-------------------------------------------------Formula-----------------------------------------------------

            if (State)
            {
                Double D = 0, M = 0, S = 0;
                if (DataValue.Count() == 1)
                {
                    D = Math.Abs(Convert.ToDouble(DataValue[0]));
                    M = 0;
                    S = 0;
                }
                else if (DataValue.Count() == 2)
                {
                    D = Math.Abs(Convert.ToDouble(DataValue[0]));
                    M = Math.Abs(Convert.ToDouble(DataValue[1]));
                    S = 0;
                }
                else if (DataValue.Count() == 3)
                {
                    D = Math.Abs(Convert.ToDouble(DataValue[0]));
                    M = Math.Abs(Convert.ToDouble(DataValue[1]));
                    S = Math.Abs(Convert.ToDouble(DataValue[2]));
                }

                if (!DataValue[0].Contains("-"))
                    DataConverted = ((D + (M + (S / 60)) / 60) / 180 * Math.PI);
                else
                    DataConverted = ((D + (M + (S / 60)) / 60) / 180 * Math.PI) * -1;
            }

            return DataConverted;
        }

        public static Boolean IsNumeric(System.Object Expression)
        {
            if (Expression == null || Expression is DateTime)
                return false;

            if (Expression is Int16 || Expression is Int32 || Expression is Int64 || Expression is Decimal || Expression is Single || Expression is Double || Expression is Boolean)
                return true;

            try
            {
                if (Expression is string)
                    Double.Parse(Expression as string);
                else
                    Double.Parse(Expression.ToString());
                return true;
            }
            catch { } // just dismiss errors but return false
            return false;
        }

        private static String RemoveDot(Double Data, Boolean Before)
        {
            String[] ArrayData = Convert.ToString(Data).Split('.');

            if (Before)
                return ArrayData[0];
            else
            {
                if (ArrayData.Count() == 1)
                    return "0.0";
                else
                    return "0." + ArrayData[1];
            }
        }

        //---------------------------------------------------------------------------------Radian Converter Handler---------------------------------------------------------------------------------

        public static String RadianToRaHHMMSS(Object Value)
        {
            Double Angle = Convert.ToDouble(Convert.ToDouble(Value) * 180 / Math.PI);
            Double AngleRa = Angle * 24 / 360;

            TimeSpan span = TimeSpan.FromHours(AngleRa);

            if (span.TotalMilliseconds >= 0)
                return "+" + String.Format("{0:000}", Math.Abs(span.Hours)) + " " + String.Format("{0:00}", Math.Abs(span.Minutes)) + " " + String.Format("{0:00}", Math.Abs(span.Seconds)) + "." + String.Format("{0:000}", Math.Abs(span.Milliseconds));
            else
                return "-" + String.Format("{0:000}", Math.Abs(span.Hours)) + " " + String.Format("{0:00}", Math.Abs(span.Minutes)) + " " + String.Format("{0:00}", Math.Abs(span.Seconds)) + "." + String.Format("{0:000}", Math.Abs(span.Milliseconds));
        }

        public static String RadianToDecDDMMSS(Object Value)
        {
            try
            {
                Double DoubleDegree = Convert.ToDouble(Convert.ToDouble(Value) * 180 / Math.PI);
                Double DoubleLipda = 0;
                Double Pilida = 0;
                int Degree = 0, Lipda = 0;

                Degree = Convert.ToInt32(RemoveDot(DoubleDegree, true));      // Answer Degree

                DoubleLipda = Convert.ToDouble(RemoveDot(DoubleDegree, false)) * (Double)60;
                Lipda = Convert.ToInt32(RemoveDot(DoubleLipda, true));        // Answer Lipda

                Pilida = Convert.ToDouble(RemoveDot(DoubleLipda, false)) * (Double)60;        // Answer Pilipda

                if (Convert.ToDouble(Value) >= 0)
                    return "+" + String.Format("{0:000}", (int)(Degree)) + " " + String.Format("{0:00}", (int)(Lipda)) + " " + String.Format("{0:00.000}", (Double)(Pilida));
                else
                    return "-" + String.Format("{0:000}", (int)(Math.Abs(Degree))) + " " + String.Format("{0:00}", (int)(Math.Abs(Lipda))) + " " + String.Format("{0:00.000}", (Double)(Math.Abs(Pilida)));
            }
            catch (Exception e) { return e.Message; }
        }

        public static String RadianToPaHHMMSS(Object Value)
        {
            try
            {
                Double Angle = Convert.ToDouble(Convert.ToDouble(Value) * 180 / Math.PI);
                TimeSpan span = TimeSpan.FromHours(Angle);

                if (span.TotalMilliseconds >= 0)
                    return "+" + String.Format("{0:000}", Math.Abs(span.Hours)) + " " + String.Format("{0:00}", Math.Abs(span.Minutes)) + " " + String.Format("{0:00}", Math.Abs(span.Seconds)) + "." + String.Format("{0:000}", Math.Abs(span.Milliseconds));
                else
                    return "-" + String.Format("{0:000}", Math.Abs(span.Hours)) + " " + String.Format("{0:00}", Math.Abs(span.Minutes)) + " " + String.Format("{0:00}", Math.Abs(span.Seconds)) + "." + String.Format("{0:000}", Math.Abs(span.Milliseconds));
            }
            catch (Exception e) { return e.Message; }
        }

        public static String RadianToHAHHMMSS(Object Value)
        {
            Double Angle = Convert.ToDouble(Convert.ToDouble(Value) * 12 / Math.PI);
            Double TempNoDecimal = Math.Floor(Angle);
            Double Result = Angle - TempNoDecimal;
            Double ArcSec = 0;
            Double ArcMin = Result * 60;
            TempNoDecimal = Math.Floor(ArcMin);
            Result = ArcMin - TempNoDecimal;
            ArcSec = Result * 60;

            if (Convert.ToDouble(Value) >= 0)
                return "+" + String.Format("{0:000}", (int)(Angle)) + " " + String.Format("{0:00}", (int)(ArcMin)) + " " + String.Format("{0:00.000}", (Double)(ArcSec));
            else
                return "-" + String.Format("{0:000}", (int)(Math.Abs(Angle))) + " " + String.Format("{0:00}", (int)(Math.Abs(ArcMin))) + " " + String.Format("{0:00.000}", (Double)(Math.Abs(ArcSec)));
        }

        public static String RadianToElHHMMSS(Object Value)
        {
            Double Angle = Convert.ToDouble(Convert.ToDouble(Value) * 180 / Math.PI);
            Double TempNoDecimal = Math.Floor(Angle);
            Double Result = Angle - TempNoDecimal;
            Double ArcSec = 0;
            Double ArcMin = Result * 60;
            TempNoDecimal = Math.Floor(ArcMin);
            Result = ArcMin - TempNoDecimal;
            ArcSec = Result * 60;

            if (Convert.ToDouble(Value) >= 0)
                return "+" + String.Format("{0:000}", (int)(Angle)) + " " + String.Format("{0:00}", (int)(ArcMin)) + " " + String.Format("{0:00.000}", (Double)(ArcSec));
            else
                return "-" + String.Format("{0:000}", (int)(Math.Abs(Angle))) + " " + String.Format("{0:00}", (int)(Math.Abs(ArcMin))) + " " + String.Format("{0:00.000}", (Double)(Math.Abs(ArcSec)));
        }

        public static String RadianToAzHHMMSS(Object Value)
        {
            Double Angle = Convert.ToDouble(Convert.ToDouble(Value) * 180 / Math.PI);
            Double TempNoDecimal = Math.Floor(Angle);
            Double Result = Angle - TempNoDecimal;
            Double ArcSec = 0;
            Double ArcMin = Result * 60;
            TempNoDecimal = Math.Floor(ArcMin);
            Result = ArcMin - TempNoDecimal;
            ArcSec = Result * 60;

            if (Convert.ToDouble(Value) >= 0)
                return "+" + String.Format("{0:000}", (int)(Angle)) + " " + String.Format("{0:00}", (int)(ArcMin)) + " " + String.Format("{0:00.000}", (Double)(ArcSec));
            else
                return "-" + String.Format("{0:000}", (int)(Math.Abs(Angle))) + " " + String.Format("{0:00}", (int)(Math.Abs(ArcMin))) + " " + String.Format("{0:00.000}", (Double)(Math.Abs(ArcSec)));
        }

        public static String RadianToDegree(Object Value)
        {
            try
            {
                Double Angle = Convert.ToDouble(Convert.ToDouble(Value) * 180 / Math.PI);
                return String.Format("{0:+0.000;-0.00;0.000}", Angle);
            }
            catch (Exception e) { return e.Message; }
        }

        //---------------------------------------------------------------------------------Decimal Handler---------------------------------------------------------------------------------

        public static String ToDecimal0Place(Object Value)
        {
            return String.Format("{0:0}", Convert.ToDouble(Value));
        }

        public static String ToDecimal2Place(Object Value)
        {
            return String.Format("{0:0.00}", Convert.ToDouble(Value));
        }

        public static String ToDecimal3Place(Object Value)
        {
            return String.Format("{0:0.000}", Convert.ToDouble(Value));
        }
    }
}
