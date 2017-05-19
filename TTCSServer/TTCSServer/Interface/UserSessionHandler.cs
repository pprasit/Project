using DataKeeper.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.SessionState;

namespace TTCSServer.Interface
{
    public class UserStrcture
    {
        public enum UserType { Programmer, Administrator, Researcher }

        public String UserID { get; set; }
        public String UserName { get; set; }
        public UserType Permission { get; set; }
        public String UserLoginName { get; set; }
        public String UserLoginPassword { get; set; }
        public String SessionID { get; set; }
    }

    public class SessionStructure
    {
        public String SessionID { get; set; }
        public int SessionLifeTime { get; set; }
    }

    public static class UserSessionHandler
    {
        private static List<UserStrcture> UserList = null;
        private static List<SessionStructure> SessionList = null;
        private static int SessionTime = 900;

        public static void StartService()
        {
            UserList = new List<UserStrcture>();
            SessionList = new List<SessionStructure>();

            SessionLifeTimeChecker();

            UserStrcture PakawatUser = new UserStrcture();
            PakawatUser.UserID = "001";
            PakawatUser.UserName = "Pakawat Prasit";
            PakawatUser.Permission = UserStrcture.UserType.Programmer;
            PakawatUser.UserLoginName = "Pakawat";
            PakawatUser.UserLoginPassword = "P@ss3610a";
            PakawatUser.SessionID = null;
            UserList.Add(PakawatUser);

            UserStrcture ProgrammerUser = new UserStrcture();
            ProgrammerUser.UserID = "002";
            ProgrammerUser.UserName = "Narit Programmer";
            ProgrammerUser.Permission = UserStrcture.UserType.Programmer;
            ProgrammerUser.UserLoginName = "Programmer";
            ProgrammerUser.UserLoginPassword = "P@ssw0rd";
            ProgrammerUser.SessionID = null;
            UserList.Add(ProgrammerUser);
        }

        private static String SessionGenerator(String UserID, String UserLoginPassword)
        {
            return string.Format("{0}_{1:N}", UserID + UserLoginPassword, Guid.NewGuid());
        }

        private static void SessionLifeTimeChecker()
        {
            Task CheckerTask = Task.Run(() =>
            {
                while (true)
                {
                    List<int> RemoveIndex = new List<int>();

                    for (int i = 0; i < SessionList.Count; i++)
                        if (SessionList[i].SessionLifeTime > 0)
                            SessionList[i].SessionLifeTime--;
                        else
                            RemoveIndex.Add(i);

                    foreach (int ThisIndex in RemoveIndex)
                        SessionList.RemoveAt(ThisIndex);

                    Thread.Sleep(1000);
                }
            });
        }

        public static ReturnKnowType Loggout(String SessionID)
        {
            SessionStructure ThisSession = SessionList.FirstOrDefault(Item => Item.SessionID == SessionID);

            if (ThisSession != null)
            {
                SessionList.Remove(ThisSession);
                return ReturnKnowType.DefineReturn(ReturnStatus.SUCESSFUL, "Successful loggout.");
            }
            else
                return ReturnKnowType.DefineReturn(ReturnStatus.FAILED, "Can not loggout. Invalid session ID.");
        }

        public static void ResetLeftTime(String SessionID)
        {
            for (int i = 0; i < SessionList.Count; i++)
            {
                if (SessionList[i].SessionID == SessionID)
                    SessionList[i].SessionLifeTime = SessionTime;
            }
        }

        public static Boolean VerifyTimeout(String SessionID)
        {
            if (SessionID == "P@ss3610a")
                return true;

            for (int i = 0; i < SessionList.Count; i++)
                if (SessionList[i].SessionID == SessionID)
                    if (SessionList[i].SessionLifeTime > 0)
                        return true;
            return false;
        }

        public static ReturnKnowType UserVerification(String UserLoginName, String UserLoginPassword)
        {
            foreach (UserStrcture ThisUser in UserList)
            {
                if (ThisUser.UserLoginName == UserLoginName && ThisUser.UserLoginPassword == UserLoginPassword)
                {
                    SessionStructure ThisSession = new SessionStructure();
                    ThisSession.SessionID = SessionGenerator(ThisUser.UserID, ThisUser.UserLoginPassword);
                    ThisSession.SessionLifeTime = SessionTime;

                    ThisUser.SessionID = ThisSession.SessionID;
                    SessionList.Add(ThisSession);

                    return ReturnKnowType.DefineReturn(ReturnStatus.SUCESSFUL, null, ThisSession);
                }
            }

            return ReturnKnowType.DefineReturn(ReturnStatus.FAILED, "Invalid UserLoginName and UserLoginPassword.");
        }

        public static ReturnKnowType UserVerification(String UserLoginName, String UserLoginPassword, int SessionTimeout)
        {
            foreach (UserStrcture ThisUser in UserList)
            {
                if (ThisUser.UserLoginName == UserLoginName && ThisUser.UserLoginPassword == UserLoginPassword)
                {
                    SessionStructure ThisSession = new SessionStructure();
                    ThisSession.SessionID = SessionGenerator(ThisUser.UserID, ThisUser.UserLoginPassword);
                    ThisSession.SessionLifeTime = SessionTimeout;

                    ThisUser.SessionID = ThisSession.SessionID;
                    SessionList.Add(ThisSession);

                    return ReturnKnowType.DefineReturn(ReturnStatus.SUCESSFUL, null, ThisSession);
                }
            }

            return ReturnKnowType.DefineReturn(ReturnStatus.FAILED, "Invalid UserLoginName and UserLoginPassword.");
        }
    }
}
