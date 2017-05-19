using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataKeeper.Engine
{
    public static class DatabaseSynchronization
    {
        private static Entities db;

        public static List<Object[]> SyncDataFromServer(String StationName, String TableName)
        {
            List<Object[]> AllValue = new List<Object[]>();
            AllValue.AddRange(GetAllInformation(StationName, TableName));

            return AllValue;
        }

        private static List<Object[]> GetAllInformation(String StationName, String TableName)
        {
            db = new Entities();
            if (TableName == "UserTB")
            {
                List<UserTB> UserList = db.UserTBs.ToList();
                List<Object[]> ValueList = new List<Object[]>();

                foreach (UserTB ThisUser in UserList)
                {
                    List<Object> ValueField = new List<Object>();
                    ValueField.Add("UserTB");
                    ValueField.Add(ThisUser.UserID);
                    ValueField.Add(ThisUser.UserName);
                    ValueField.Add(ThisUser.UserLoginName);
                    ValueField.Add(ThisUser.UserLoginPassword);
                    ValueField.Add(ThisUser.UserPermissionType);

                    ValueList.Add(ValueField.ToArray());
                }

                return ValueList;
            }
            else
            {
                List<LogTB> LogList = db.LogTBs.Where(Item => Item.StationName == StationName).ToList();
                List<Object[]> ValueList = new List<Object[]>();

                foreach (LogTB ThisLog in LogList)
                {
                    List<Object> ValueField = new List<Object>();
                    ValueField.Add("LogTB");
                    ValueField.Add(ThisLog.LogID);
                    ValueField.Add(ThisLog.StationName);
                    ValueField.Add(ThisLog.LogDateTime);
                    ValueField.Add(ThisLog.UserID);
                    ValueField.Add(ThisLog.Note);
                    ValueField.Add(ThisLog.Problem);

                    ValueList.Add(ValueField.ToArray());
                }

                return ValueList;
            }



        }

        #region User

        public static Object SyncUser(DATAACTION Action, List<Object[]> TableField)
        {
            if (Action == DATAACTION.SYNCALL)
            {
                return SyncExistingUser(TableField);
            }
            else if (Action == DATAACTION.UPDATE)
            {
                UpdateUser(TableField);
            }

            return null;
        }

        private static void UpdateUser(List<Object[]> TableField)
        {
            UserTB CheckUser = new UserTB();
            CheckUser.UserID = TableField.ElementAt(0)[0].ToString();
            CheckUser.UserName = TableField.ElementAt(0)[1].ToString();
            CheckUser.UserLoginName = TableField.ElementAt(0)[2].ToString();
            CheckUser.UserLoginPassword = TableField.ElementAt(0)[3].ToString();
            CheckUser.UserPermissionType = TableField.ElementAt(0)[4].ToString();

            UserTB ExistingUser = db.UserTBs.FirstOrDefault(Item => Item.UserID == CheckUser.UserID);
            ExistingUser.UserName = CheckUser.UserName;
            ExistingUser.UserLoginName = CheckUser.UserLoginName;
            ExistingUser.UserLoginPassword = CheckUser.UserLoginPassword;
            ExistingUser.UserPermissionType = CheckUser.UserPermissionType;
            db.SaveChangesAsync();
        }

        private static Object SyncExistingUser(List<Object[]> TableField)
        {
            db = new Entities();
            List<UserTB> UserList = db.UserTBs.ToList();
            List<UserTB> NewUser = new List<UserTB>();
            List<UserTB> StationUsers = new List<UserTB>();

            foreach (Object[] ThisObject in TableField)
            {
                UserTB NewStationUser = new UserTB();
                NewStationUser.UserID = ThisObject[0].ToString();
                NewStationUser.UserLoginName = ThisObject[1].ToString();
                NewStationUser.UserLoginPassword = ThisObject[2].ToString();
                NewStationUser.UserPermissionType = ThisObject[3].ToString();
                StationUsers.Add(NewStationUser);
            }

            foreach (UserTB ServerUser in UserList)
            {
                Boolean IsFound = false;
                foreach (UserTB StationUser in StationUsers)
                {
                    if (StationUser.UserID == ServerUser.UserID)
                    {
                        IsFound = true;
                        break;
                    }
                }

                if (!IsFound)
                    NewUser.Add(ServerUser);
            }

            return NewUser;
        }

        #endregion User

        #region Log

        public static Object SyncLog(String StationName, DATAACTION Action, List<Object[]> TableField)
        {
            if (Action == DATAACTION.INSERT || Action == DATAACTION.SYNCALL)
            {
                InsertLog(StationName, TableField);
            }
            else if (Action == DATAACTION.DELETE)
            {
                DeleteLog(TableField.ElementAt(0)[0].ToString());
            }
            else if (Action == DATAACTION.UPDATE)
            {
                UpdateLog(StationName, TableField);
            }

            return null;
        }

        private static void UpdateLog(String StationName, List<Object[]> TableField)
        {
            LogTB CheckLog = new LogTB();
            CheckLog.LogID = TableField.ElementAt(0)[0].ToString();
            CheckLog.StationName = TableField.ElementAt(0)[0].ToString();
            CheckLog.LogDateTime = Convert.ToDateTime(TableField.ElementAt(0)[2]);
            CheckLog.UserID = TableField.ElementAt(0)[3].ToString();
            CheckLog.Note = TableField.ElementAt(0)[4].ToString();
            CheckLog.Problem = TableField.ElementAt(0)[5].ToString();

            LogTB ExistingLog = db.LogTBs.FirstOrDefault(Item => Item.LogID == CheckLog.LogID);
            ExistingLog.LogDateTime = CheckLog.LogDateTime;
            ExistingLog.UserID = CheckLog.UserID;
            ExistingLog.Note = CheckLog.Note;
            ExistingLog.Problem = CheckLog.Problem;

            db.SaveChangesAsync();
        }

        private static void DeleteLog(String LogID)
        {
            db.LogTBs.Remove(db.LogTBs.FirstOrDefault(Item => Item.LogID == LogID));
            db.SaveChangesAsync();
        }

        private static Object InsertLog(String StationName, List<Object[]> TableField)
        {
            try
            {
                db = new Entities();
                List<LogTB> LogList = db.LogTBs.Where(Item => Item.StationName == StationName).ToList();
                List<LogTB> NewServerLog = new List<LogTB>();
                List<LogTB> NewStationLog = new List<LogTB>();
                List<LogTB> StationLogList = new List<LogTB>();

                foreach (Object[] ThisObject in TableField)
                {
                    LogTB NewLog = new LogTB();
                    NewLog.LogID = ThisObject[0].ToString();
                    NewLog.StationName = ThisObject[1].ToString();
                    NewLog.LogDateTime = Convert.ToDateTime(ThisObject[2]);
                    NewLog.UserID = ThisObject[3].ToString();
                    NewLog.Note = ThisObject[4].ToString();
                    NewLog.Problem = ThisObject[5].ToString();
                    StationLogList.Add(NewLog);
                }

                foreach (LogTB StationLog in StationLogList)
                {
                    Boolean IsFound = false;
                    foreach (LogTB ServerLog in LogList)
                    {
                        if (StationLog.LogID == ServerLog.LogID)
                        {
                            IsFound = true;
                            break;
                        }
                    }

                    if (!IsFound)
                        NewServerLog.Add(StationLog);
                }

                foreach (LogTB ServerLog in StationLogList)
                {
                    Boolean IsFound = false;
                    foreach (LogTB StationLog in LogList)
                    {
                        if (StationLog.LogID == ServerLog.LogID)
                        {
                            IsFound = true;
                            break;
                        }
                    }

                    if (!IsFound)
                        NewStationLog.Add(ServerLog);
                }

                db.LogTBs.AddRange(NewServerLog);
                db.SaveChangesAsync();
                return NewStationLog;
            }
            catch (Exception e)
            {
                String Message = e.Message;
                return null;
            }
        }

        #endregion
    }
}
