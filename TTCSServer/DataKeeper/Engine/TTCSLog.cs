using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DataKeeper.Engine
{
    public enum LogType { MESSAGE, FAILED, COMMUNICATION, ERROR, RELAY }

    public class InformationLogs
    {
        public long LogID { get; set; }
        public long? UserID { get; set; }
        public String LogName { get; set; }
        public String LogMessage { get; set; }
        public DateTime LogDate { get; set; }
        public String LogValue { get; set; }
        public LogType LogCategory { get; set; }
        public STATIONNAME StationName { get; set; }

    }

    public static class TTCSLog
    {
        public static List<InformationLogs> TTCSLogInformation { get; set; }
        public static List<InformationLogs> TTCSTempLogInformation { get; set; }
        public static DataGridView TTCSLogGrid { get; set; }
        public static DateTime StartDate { get; set; }
        public static DateTime EndDate { get; set; }
        public static Boolean IsSearchByAllStation { get; set; }

        public static void CreateTTCSLog(DataGridView TTCSLogGrid)
        {
            TTCSLog.TTCSLogGrid = TTCSLogGrid;
            IsSearchByAllStation = false;
            TTCSLogInformation = new List<InformationLogs>();
            TTCSTempLogInformation = new List<InformationLogs>();
        }

        public static void NewLogInformation(STATIONNAME StationName, DateTime LogDate, String Message, LogType LogCategory, long? UserID)
        {
            InformationLogs NewLog = new InformationLogs();
            NewLog.LogID = (long)(DateTime.Now - DateTime.MinValue).TotalMilliseconds;
            NewLog.UserID = UserID;
            NewLog.LogName = "Log from " + StationName.ToString();
            NewLog.LogDate = LogDate;
            NewLog.LogValue = null;
            NewLog.LogMessage = Message;
            NewLog.LogCategory = LogCategory;
            NewLog.StationName = StationName;

            TTCSLogInformation.Add(NewLog);
            TTCSTempLogInformation.Add(NewLog);

            if (!TTCSLog.IsSearchByAllStation)
                AddLogToGrid(StationName, LogDate, Message, LogCategory, null, UserID);
            else if (TTCSLog.IsSearchByAllStation && LogDate >= StartDate && LogDate <= EndDate)
                AddLogToGrid(StationName, LogDate, Message, LogCategory, null, UserID);
        }

        public static void NewLogInformation(STATIONNAME StationName, DateTime LogDate, String LogName, Object[] Value, String Message, LogType LogCategory, long? UserID)
        {
            InformationLogs NewLog = new InformationLogs();
            NewLog.LogID = (long)(DateTime.Now - DateTime.MinValue).TotalMilliseconds;
            NewLog.UserID = UserID;
            NewLog.LogName = LogName;
            NewLog.LogDate = LogDate;
            NewLog.LogValue = Value != null ? String.Join(", ", Value) : null;
            NewLog.LogMessage = Message;
            NewLog.LogCategory = LogCategory;
            NewLog.StationName = StationName;

            if (DateTime.Now.Hour == 1)
                TTCSTempLogInformation.Clear();

            TTCSLogInformation.Add(NewLog);
            TTCSTempLogInformation.Add(NewLog);

            if (!TTCSLog.IsSearchByAllStation)
                AddLogToGrid(StationName, LogDate, Message, LogCategory, NewLog.LogValue, UserID);
            else if (TTCSLog.IsSearchByAllStation && LogDate >= StartDate && LogDate <= EndDate)
                AddLogToGrid(StationName, LogDate, Message, LogCategory, NewLog.LogValue, UserID);
        }

        public static ReturnLogInformation GetLogList()
        {
            List<InformationLogs> TempLogs = new List<InformationLogs>();

            for (int i = 0; i < TTCSTempLogInformation.Count; i++)
                TempLogs.Add(TTCSTempLogInformation[i]);

            TTCSTempLogInformation.Clear();
            return ReturnLogInformation.DefineReturn(ReturnStatus.SUCESSFUL, null, TempLogs);
        }

        public static void GetLogBySearchInformarion(STATIONNAME StationName, DateTime StartDate, DateTime EndDate, Boolean IsSearchByAllStation)
        {
            List<InformationLogs> TTCSLogBySearch = TTCSLogInformation;
            TTCSLog.StartDate = StartDate;
            TTCSLog.EndDate = EndDate;
            TTCSLog.IsSearchByAllStation = IsSearchByAllStation;

            if (IsSearchByAllStation)
                if (StationName == STATIONNAME.NULL)
                    TTCSLogBySearch = TTCSLogInformation.Where(Item => Item.LogDate.Date >= StartDate.Date && Item.LogDate.Date <= EndDate.Date).ToList();
                else
                    TTCSLogBySearch = TTCSLogInformation.Where(Item => Item.StationName == StationName && Item.LogDate.Date >= StartDate.Date && Item.LogDate.Date <= EndDate.Date).ToList();

            TTCSLogGrid.Rows.Clear();
            if (TTCSLogBySearch != null)
                for (int i = 0; i < TTCSLogBySearch.Count; i++)
                    AddLogToGrid(TTCSLogBySearch[i].StationName, TTCSLogBySearch[i].LogDate, TTCSLogBySearch[i].LogMessage, TTCSLogBySearch[i].LogCategory, TTCSLogBySearch[i].LogValue, TTCSLogBySearch[i].UserID);
        }

        private static void AddLogToGrid(STATIONNAME StationName, DateTime LogDate, String Message, LogType LogDataType, String Value, long? UserID)
        {
            if (TTCSLogGrid.InvokeRequired)
            {
                TTCSLogGrid.Invoke((Action)(() => ActionDataGridView(StationName, LogDate, Message, LogDataType, Value, UserID)));
                return;
            }

            ActionDataGridView(StationName, LogDate, Message, LogDataType, Value, UserID);
        }

        private static DateTime ConvertToLocalTime(long TimeLong)
        {
            DateTime runtimeKnowsThisIsUtc = DateTime.SpecifyKind(new DateTime(TimeLong), DateTimeKind.Utc);
            DateTime localVersion = runtimeKnowsThisIsUtc.ToLocalTime();
            return localVersion;
        }

        //---------------------------------------------------------------------------DataGridView Thread Handler-----------------------------------------------

        private static void ActionDataGridView(STATIONNAME StationName, DateTime LogDate, String Message, LogType LogDataType, String Value, long? UserID)
        {
            if (TTCSLogGrid.RowCount == 500)
                TTCSLogGrid.Rows.RemoveAt(0);

            TTCSLogGrid.Rows.Add(LogDate.ToString("MM/dd/yyyy HH:mm:ss.fff"), Message, LogDataType.ToString(), StationName.ToString(), Value, UserID.ToString());

            if (LogDataType == LogType.COMMUNICATION)
                TTCSLogGrid.Rows[TTCSLogGrid.RowCount - 1].Cells[2].Style = new DataGridViewCellStyle { ForeColor = Color.Blue };
            else if (LogDataType == LogType.ERROR)
                TTCSLogGrid.Rows[TTCSLogGrid.RowCount - 1].Cells[2].Style = new DataGridViewCellStyle { ForeColor = Color.Red };
            else if (LogDataType == LogType.MESSAGE)
                TTCSLogGrid.Rows[TTCSLogGrid.RowCount - 1].Cells[2].Style = new DataGridViewCellStyle { ForeColor = Color.White };
            else if (LogDataType == LogType.RELAY)
                TTCSLogGrid.Rows[TTCSLogGrid.RowCount - 1].Cells[2].Style = new DataGridViewCellStyle { ForeColor = Color.Orange };
            else
                TTCSLogGrid.Rows[TTCSLogGrid.RowCount - 1].Cells[2].Style = new DataGridViewCellStyle { ForeColor = Color.White };

            TTCSLogGrid.FirstDisplayedScrollingRowIndex = TTCSLogGrid.RowCount - 1;
        }
    }
}
