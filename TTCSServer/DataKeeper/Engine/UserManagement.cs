using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DataKeeper.Engine
{
    public static class UserManagement
    {
        private struct TransectionBuffer
        {
            public DATAACTION UserAction { get; set; }
            public UserTB UserInformation { get; set; }
        }

        private static ConcurrentQueue<TransectionBuffer> BufferList = new ConcurrentQueue<TransectionBuffer>();
        private static Entities db = null;

        public static List<UserTB> GetAllUser()
        {
            db = new Entities();
            return db.UserTBs.ToList();
        }

        public static Boolean IsUserNameExsiting(String UserID, String UserName)
        {
            UserTB ThisUser;
            if (UserID == null)
                ThisUser = db.UserTBs.FirstOrDefault(Item => Item.UserName == UserName);
            else
                ThisUser = db.UserTBs.FirstOrDefault(Item => Item.UserName == UserName && Item.UserID != UserID);

            if (ThisUser == null)
                return false;

            return true;
        }

        public static Boolean IsLoginInformationExisting(String UserID, String LoginName, String LoginPassword)
        {
            UserTB ThisUser;
            if (UserID == null)
                ThisUser = db.UserTBs.FirstOrDefault(Item => Item.UserLoginName == LoginName && Item.UserLoginPassword == LoginPassword);
            else
                ThisUser = db.UserTBs.FirstOrDefault(Item => Item.UserLoginName == LoginName && Item.UserLoginPassword == LoginPassword && Item.UserID != UserID);

            if (ThisUser == null)
                return false;

            return true;
        }

        public static UserTB GetUserByID(String UserID)
        {
            if (UserID != null)
            {
                UserTB ThisUser = db.UserTBs.FirstOrDefault(Item => Item.UserID == UserID);
                return ThisUser;
            }

            return null;
        }

        public static Boolean DeleteUser(String UserID)
        {
            try
            {
                UserTB ThisUser = db.UserTBs.FirstOrDefault(Item => Item.UserID == UserID);
                db.UserTBs.Remove(ThisUser);
                db.SaveChanges();

                AddToBuffer(ThisUser, DATAACTION.DELETE);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static Boolean AddUser(String UserID, String UserName, String UserLoginName, String UserLoginPassword, String UserPermissionType, String UserStationPermission)
        {
            try
            {
                UserTB NewUser = new UserTB();
                NewUser.UserID = UserID;
                NewUser.UserName = UserName;
                NewUser.UserLoginName = UserLoginName;
                NewUser.UserLoginPassword = UserLoginPassword;
                NewUser.UserPermissionType = UserPermissionType;
                NewUser.UserStationPermission = UserStationPermission;

                db.UserTBs.Add(NewUser);
                db.SaveChanges();

                AddToBuffer(NewUser, DATAACTION.INSERT);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static void UserCheckerLoop()
        {
            Task UserTask = Task.Run(() =>
            {
                while (true)
                {
                    TransectionBuffer ThisUser;

                    if (BufferList.TryPeek(out ThisUser))
                    {
                        List<String> StationArr = ThisUser.UserInformation.UserStationPermission.Split(new char[] { ',' }).ToList();

                        foreach (String StationNameStr in StationArr)
                            if (StationNameStr == "All Station")
                            {
                                StationArr.Add("AIRFORCE");
                                StationArr.Add("CHACHOENGSAO");
                                StationArr.Add("NAKHONRATCHASIMA");
                                StationArr.Add("CHINA");
                                StationArr.Add("USA");
                                //StationArr.Add("ASTROPARK");
                                break;
                            }

                        StationArr.Remove("All Station");
                        StationArr.Remove("NULL");

                        Boolean IsSend = true;
                        foreach (String StationName in StationArr)
                        {
                            ReturnKnowType ReturnResult = AstroData.UpdateStationUser(ThisUser.UserInformation.UserID, ThisUser.UserInformation.UserName, ThisUser.UserInformation.UserLoginName, ThisUser.UserInformation.UserLoginPassword,
                                ThisUser.UserInformation.UserPermissionType, ThisUser.UserInformation.UserStationPermission, StationName, ThisUser.UserAction);

                            if (ReturnResult.ReturnType != ReturnStatus.SUCESSFUL)
                                IsSend = false;
                        }

                        if (IsSend)
                            BufferList.TryDequeue(out ThisUser);
                    }
                    Thread.Sleep(1000);
                }
            });
        }

        public static void AddToBuffer(UserTB NewUser, DATAACTION UserAction)
        {
            TransectionBuffer NewTransection = new TransectionBuffer();
            NewTransection.UserAction = UserAction;
            NewTransection.UserInformation = NewUser;

            BufferList.Enqueue(NewTransection);
        }

        public static Boolean UpdateUser(String UserID, String UserName, String UserLoginName, String UserLoginPassword, String UserPermissionType, String UserStationPermission)
        {
            UserTB ExistingUser = db.UserTBs.FirstOrDefault(Item => Item.UserID == UserID);

            if (ExistingUser != null)
            {
                ExistingUser.UserName = UserName;
                ExistingUser.UserLoginName = UserLoginName;
                ExistingUser.UserLoginPassword = UserLoginPassword;
                ExistingUser.UserPermissionType = UserPermissionType;
                ExistingUser.UserStationPermission = UserStationPermission;
                db.SaveChanges();

                AddToBuffer(ExistingUser, DATAACTION.UPDATE);
                return true;
            }

            return false;
        }
    }
}
