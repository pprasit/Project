using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.EntityClient;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DataKeeper.Engine
{
    public static class DatabaseSynchronization
    {
        private static Entities db;
        private static Boolean IsSaving = false;
        private static String DatabaseName = "";
        private static String DatabaseUserName = "";
        private static String DatabasePassword = "";
        private static String DatabaseServerName = "";

        public static void SetDBConnection(String DatabaseName, String DatabaseUserName, String DatabasePassword, String DatabaseServerName)
        {
            DatabaseSynchronization.DatabaseName = DatabaseName;
            DatabaseSynchronization.DatabaseUserName = DatabaseUserName;
            DatabaseSynchronization.DatabasePassword = DatabasePassword;
            DatabaseSynchronization.DatabaseServerName = DatabaseServerName;
        }

        public static Entities RefreshDB()
        {
            if (DatabaseName != "" && DatabaseUserName != "" && DatabasePassword != "" && DatabaseServerName != "")
            {
                var db = new Entities();
                db.ChangeDatabase(initialCatalog: DatabaseName, userId: DatabaseUserName, password: DatabasePassword, dataSource: DatabaseServerName);

                return db;
            }
            else
                return null;
        }

        public static List<Object[]> SyncDataFromServer(String StationName, String TableName)
        {
            List<Object[]> AllValue = new List<Object[]>();
            AllValue.AddRange(GetAllInformation(StationName, TableName));

            return AllValue;
        }

        private static List<Object[]> GetAllInformation(String StationName, String TableName)
        {
            db = RefreshDB();
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

        public static List<UserTB> GetAllUser()
        {
            db = RefreshDB();

            if (db != null)
                return db.UserTBs.ToList();
            return null;
        }

        #region User

        public static Object SyncUser(DATAACTION Action, List<Object[]> TableField, STATIONNAME StationName)
        {
            if (Action == DATAACTION.SYNCALL)
                return SyncExistingUser(TableField);
            else if (Action == DATAACTION.UPDATE || Action == DATAACTION.INSERT)
                UpdateUser(TableField, StationName);
            else if (Action == DATAACTION.DELETE)
                DeleteUser(TableField);

            return null;
        }

        private static void DeleteUser(List<Object[]> TableField)
        {
            db = RefreshDB();
            db.UserTBs.Remove(db.UserTBs.FirstOrDefault(Item => Item.UserID == TableField.ElementAt(0)[0].ToString()));
            db.SaveChangesAsync();
        }

        private static void UpdateUser(List<Object[]> TableField, STATIONNAME StationName)
        {
            db = RefreshDB();
            UserTB CheckUser = new UserTB();
            CheckUser.UserID = TableField.ElementAt(0)[0].ToString();
            CheckUser.UserName = TableField.ElementAt(0)[1].ToString();
            CheckUser.UserLoginName = TableField.ElementAt(0)[2].ToString();
            CheckUser.UserLoginPassword = TableField.ElementAt(0)[3].ToString();
            CheckUser.UserPermissionType = TableField.ElementAt(0)[4].ToString();

            UserTB ThisUser = db.UserTBs.FirstOrDefault(Item => Item.UserID == CheckUser.UserID);

            if (ThisUser == null)
            {
                ThisUser = new UserTB();
                ThisUser.UserID = CheckUser.UserID;
                ThisUser.UserName = CheckUser.UserName;
                ThisUser.UserLoginName = CheckUser.UserLoginName;
                ThisUser.UserLoginPassword = CheckUser.UserLoginPassword;
                ThisUser.UserPermissionType = CheckUser.UserPermissionType;
                ThisUser.UserStationPermission = StationName.ToString();

                db.UserTBs.Add(ThisUser);
            }
            else
            {
                ThisUser.UserName = CheckUser.UserName;
                ThisUser.UserLoginName = CheckUser.UserLoginName;
                ThisUser.UserLoginPassword = CheckUser.UserLoginPassword;
                ThisUser.UserPermissionType = CheckUser.UserPermissionType;

                if (ThisUser.UserStationPermission == null)
                    ThisUser.UserStationPermission = StationName.ToString();
                else
                {
                    if (ThisUser.UserStationPermission.Trim() != "")
                    {
                        if (ThisUser.UserStationPermission != "All Station")
                            ThisUser.UserStationPermission = "," + StationName.ToString();
                    }
                    else
                        ThisUser.UserStationPermission = StationName.ToString();
                }
            }

            db.SaveChangesAsync();
        }

        private static Object SyncExistingUser(List<Object[]> TableField)
        {
            db = RefreshDB();
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
            db = RefreshDB();
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
            db = RefreshDB();
            db.LogTBs.Remove(db.LogTBs.FirstOrDefault(Item => Item.LogID == LogID));
            db.SaveChangesAsync();
        }

        private static Object InsertLog(String StationName, List<Object[]> TableField)
        {
            try
            {
                db = RefreshDB();
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

        #region ScriptHandler

        public static List<ScriptTB> GetScript()
        {
            db = RefreshDB();
            if (db != null)
                return db.ScriptTBs.ToList();
            else
                return null;
        }

        public static void InsertScript(ScriptTB ThisScript)
        {
            db = RefreshDB();

            int Temp = db.UserTBs.Count();
            db.ScriptTBs.Add(ThisScript);
        }

        public static void DeleteScript(ScriptTB ThisScript)
        {
            try
            {
                db = RefreshDB();
                db.ScriptTBs.Remove(ThisScript);
            }
            catch { }
        }

        public static void DeleteAllScript()
        {
            db = RefreshDB();
            db.ScriptTBs.RemoveRange(db.ScriptTBs);
        }

        public static Boolean ScriptSaveChange(Boolean IsSync)
        {
            try
            {
                if (!IsSaving)
                {
                    IsSaving = true;

                    if (IsSync)
                        db.SaveChanges();
                    else
                        db.SaveChangesAsync();

                    IsSaving = false;
                }

                return true;
            }
            catch
            {
                IsSaving = false;
                return false;
            }
        }

        #endregion
    }

    public static class ConnectionTools
    {
        // all params are optional
        public static void ChangeDatabase(
            this DbContext source,
            string initialCatalog = "",
            string dataSource = "",
            string userId = "",
            string password = "",
            bool integratedSecuity = true,
            string configConnectionStringName = "")
        /* this would be used if the
        *  connectionString name varied from 
        *  the base EF class name */
        {
            try
            {
                // use the const name if it's not null, otherwise
                // using the convention of connection string = EF contextname
                // grab the type name and we're done
                var configNameEf = string.IsNullOrEmpty(configConnectionStringName)
                    ? source.GetType().Name
                    : configConnectionStringName;

                // add a reference to System.Configuration
                var entityCnxStringBuilder = new EntityConnectionStringBuilder
                    (System.Configuration.ConfigurationManager.ConnectionStrings[configNameEf].ConnectionString);

                // init the sqlbuilder with the full EF connectionstring cargo
                var sqlCnxStringBuilder = new SqlConnectionStringBuilder
                    (entityCnxStringBuilder.ProviderConnectionString);

                // only populate parameters with values if added
                if (!string.IsNullOrEmpty(initialCatalog))
                    sqlCnxStringBuilder.InitialCatalog = initialCatalog;
                if (!string.IsNullOrEmpty(dataSource))
                    sqlCnxStringBuilder.DataSource = dataSource;
                if (!string.IsNullOrEmpty(userId))
                    sqlCnxStringBuilder.UserID = userId;
                if (!string.IsNullOrEmpty(password))
                    sqlCnxStringBuilder.Password = password;

                // set the integrated security status
                sqlCnxStringBuilder.IntegratedSecurity = integratedSecuity;

                // now flip the properties that were changed
                source.Database.Connection.ConnectionString
                    = sqlCnxStringBuilder.ConnectionString;
            }
            catch (Exception ex)
            {
                // set log item if required
            }
        }
    }
}
