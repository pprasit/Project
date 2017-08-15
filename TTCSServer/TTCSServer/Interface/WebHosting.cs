using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel.Description;
using System.Web.Http;
using System.Web.Http.SelfHost;
using System.Net;
using System.Net.Http;
using DataKeeper;
using DataKeeper.Engine;
using System.IO;
using System.Xml.Serialization;
using System.Xml.XPath;
using System.Xml;
using DataKeeper.Engine.Command;
using System.Collections.Concurrent;
using System.Web.Script.Services;
using System.Web.Script.Serialization;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using System.Web.Http.Cors;
using System.Web;
using Microsoft.Web.WebSockets;
using System.Threading;
using System.Web.WebSockets;
using System.Net.WebSockets;
using System.Net.Sockets;
using System.Runtime.Remoting;
using DataKeeper.Interface;

namespace TTCSServer.Interface
{

    public enum MessageType { Success, Error, Information }

    public class ReturnMessage
    {
        public String Message { get; set; }
        public MessageType MessageType { get; set; }
        public DateTime MessageDateTime { get; set; }
    }

    public class HelpController : ApiController
    {
        //http://192.168.70.210:8093/TTCS/help
        public HttpResponseMessage Get()
        {
            return TTCSCommandHelp.GetPage();
        }
    }

    public class JsonContent : HttpContent
    {

        private readonly MemoryStream _Stream = new MemoryStream();
        public JsonContent(object value)
        {

            Headers.ContentType = new MediaTypeHeaderValue("application/json");
            var jw = new JsonTextWriter(new StreamWriter(_Stream));
            jw.Formatting = Newtonsoft.Json.Formatting.Indented;
            var serializer = new JsonSerializer();
            serializer.Serialize(jw, value);
            jw.Flush();
            _Stream.Position = 0;

        }
        protected override Task SerializeToStreamAsync(Stream stream, TransportContext context)
        {
            return _Stream.CopyToAsync(stream);
        }

        protected override bool TryComputeLength(out long length)
        {
            length = _Stream.Length;
            return true;
        }
    }

    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class LoginController : ApiController
    {
        [DisableCors]
        //http://192.168.70.210:8093/TTCS/Login?UserName=Pakawat&Password=P@ss3610a
        public Object Get(String UserName, String Password)
        {
            //return new HttpResponseMessage()
            //{
            //    Content = new JsonContent(new
            //    {
            //        Success = true, //error
            //        Message = "Success" //return exception
            //    })
            //};

            //return "Test";
            //Response.AppendHeader("Access-Control-Allow-Origin", "*");

            ReturnKnowType Object = Operation(UserName, Password);
            return new JavaScriptSerializer().Serialize(Object);
            //return Operation(UserName, Password)
        }

        public ReturnKnowType Post(String UserName, String Password)
        {
            return Operation(UserName, Password);
        }

        private ReturnKnowType Operation(String UserName, String Password)
        {
            XmlDocument myXml = new XmlDocument();
            XPathNavigator xNav = myXml.CreateNavigator();

            ReturnKnowType ThisUser = UserSessionHandler.UserVerification(UserName, Password);
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(SessionStructure));

            if (ThisUser.ReturnType == ReturnStatus.SUCESSFUL)
            {
                //using (var xs = xNav.AppendChild()) { xmlSerializer.Serialize(xs, ThisUser.ReturnValue); }
                //return new HttpResponseMessage() { Content = new StringContent(myXml.OuterXml, Encoding.UTF8, "text/xml") };
                return ThisUser;
            }
            else
                return ThisUser;
        }

        //http://192.168.70.210:8093/TTCS/Login?UserName=Pakawat&Password=P@ss3610a&SessionTimeout=500
        public HttpResponseMessage Get(String UserName, String Password, int SessionTimeout)
        {
            return Operation(UserName, Password, SessionTimeout);
        }

        public HttpResponseMessage Post(String UserName, String Password, int SessionTimeout)
        {
            return Operation(UserName, Password, SessionTimeout);
        }

        private HttpResponseMessage Operation(String UserName, String Password, int SessionTimeout)
        {
            XmlDocument myXml = new XmlDocument();
            XPathNavigator xNav = myXml.CreateNavigator();

            ReturnKnowType ThisUser = UserSessionHandler.UserVerification(UserName, Password, SessionTimeout);
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(SessionStructure));

            if (ThisUser.ReturnType == ReturnStatus.SUCESSFUL)
            {
                using (var xs = xNav.AppendChild()) { xmlSerializer.Serialize(xs, ThisUser.ReturnValue); }
                return new HttpResponseMessage() { Content = new StringContent(myXml.OuterXml, Encoding.UTF8, "text/xml") };
            }
            else
                return HostingHelper.ReturnError(ThisUser.ReturnMessage, myXml, xNav);
        }
    }

    public class LogoutController : ApiController
    {
        //http://192.168.70.210:8093/TTCS/Logout?SessionID=P@ss3610a
        public HttpResponseMessage Get(String SessionID)
        {
            return Operation(SessionID);
        }

        public HttpResponseMessage Post(String SessionID)
        {
            return Operation(SessionID);
        }

        public HttpResponseMessage Operation(String SessionID)
        {
            XmlDocument myXml = new XmlDocument();
            XPathNavigator xNav = myXml.CreateNavigator();

            ReturnKnowType LoggoutResult = UserSessionHandler.Loggout(SessionID);
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(ReturnKnowType));

            using (var xs = xNav.AppendChild()) { xmlSerializer.Serialize(xs, LoggoutResult); }
            return new HttpResponseMessage() { Content = new StringContent(myXml.OuterXml, Encoding.UTF8, "text/xml") };
        }
    }

    public class LogController : ApiController
    {
        //http://192.168.70.210:8093/TTCS/Log?SessionID=P@ss3610a
        public HttpResponseMessage Get(String SessionID)
        {
            return Operation(SessionID);
        }

        public HttpResponseMessage Post(String SessionID)
        {
            return Operation(SessionID);
        }

        private HttpResponseMessage Operation(String SessionID)
        {
            XmlDocument myXml = new XmlDocument();
            XPathNavigator xNav = myXml.CreateNavigator();

            ReturnLogInformation InformationResult = TTCSLog.GetLogList();
            List<InformationLogs> InformationList = (List<InformationLogs>)InformationResult.ReturnValue;
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(InformationLogs[]));

            if (InformationList != null && InformationList.Count > 0)
            {
                using (var xs = xNav.AppendChild()) { xmlSerializer.Serialize(xs, InformationList.ToArray()); }
                return new HttpResponseMessage() { Content = new StringContent(myXml.OuterXml, Encoding.UTF8, "text/xml") };
            }
            else if (InformationList.Count == 0)
                return HostingHelper.ReturnError("There is no log information update. Please try again later.", myXml, xNav);
            else
                return HostingHelper.ReturnError(InformationResult.ReturnMessage, myXml, xNav);
        }
    }

    public class SetCommandController : ApiController
    {
        //http://192.168.70.210:8093/TTCS/SetCommand?StationName=T07Airfoce&DeviceName=T07TS&SessionID=P@ss3610a
        public HttpResponseMessage Get(String StationName, String DeviceName, String SessionID)
        {
            return GetSetCommandStructure(StationName, DeviceName, SessionID);
        }

        public HttpResponseMessage Post(String StationName, String DeviceName, String SessionID)
        {
            return GetSetCommandStructure(StationName, DeviceName, SessionID);
        }

        private HttpResponseMessage GetSetCommandStructure(String StationNameStr, String DeviceCategoryStr, String SessionID)
        {
            XmlDocument myXml = new XmlDocument();
            XPathNavigator xNav = myXml.CreateNavigator();

            if (UserSessionHandler.VerifyTimeout(SessionID))
            {
                STATIONNAME ThisStation = HostingHelper.ConvertStationNameStrToSTATIONNAME(StationNameStr);
                DEVICECATEGORY ThisDeviceCategory = HostingHelper.ConvertDevicecCategoryStrToDEVICECATEGORY(DeviceCategoryStr);

                if (ThisStation == STATIONNAME.NULL)
                    return HostingHelper.ReturnError("Invalid station name. Please check.", myXml, xNav);

                if (ThisDeviceCategory == DEVICECATEGORY.NULL)
                    return HostingHelper.ReturnError("Invalid device name. Please check.", myXml, xNav);

                TTCSCommandDisplay[] ListOfDisplayCommand = CommandDefinition.GetListCommandName(ThisStation, ThisDeviceCategory).ToArray();
                if (ListOfDisplayCommand != null)
                {
                    var json = new JavaScriptSerializer().Serialize(ListOfDisplayCommand);
                    return new HttpResponseMessage() { Content = new StringContent(json, Encoding.UTF8, "application/json") };
                }
                else
                    return HostingHelper.ReturnError("There are no set command avaliable on this stationName and DeviceName.", myXml, xNav);
            }
            else
                return HostingHelper.ReturnError("Session is timeout. Please login to the system.", myXml, xNav);
        }
    }

    public class InformationController : ApiController
    {
        //http://192.168.70.210:8093/TTCS/Information?StationName=TTCS&SessionID=P@ss3610a
        public HttpResponseMessage Get(String StationName, String SessionID)
        {
            return GetByStationName(StationName, SessionID);
        }

        public HttpResponseMessage Post(String StationName, String SessionID)
        {
            return GetByStationName(StationName, SessionID);
        }

        private HttpResponseMessage GetByStationName(String StationName, String SessionID)
        {
            XmlDocument myXml = new XmlDocument();
            XPathNavigator xNav = myXml.CreateNavigator();

            if (UserSessionHandler.VerifyTimeout(SessionID))
            {
                STATIONNAME ThisStation = HostingHelper.ConvertStationNameStrToSTATIONNAME(StationName);

                if (ThisStation == STATIONNAME.NULL)
                    return HostingHelper.ReturnError("Invalid station name. Please check.", myXml, xNav);

                List<OUTPUTSTRUCT> InformationResult = AstroData.GetInformation(ThisStation);
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(OUTPUTSTRUCT[]));

                if (InformationResult != null)
                {
                    JavaScriptSerializer Serializer = new JavaScriptSerializer();
                    Serializer.MaxJsonLength = Int32.MaxValue;
                    var json = Serializer.Serialize(InformationResult);

                    return new HttpResponseMessage() { Content = new StringContent(json, Encoding.UTF8, "application/json") };
                }
                else
                    return HostingHelper.ReturnError("An error occur while getting information", myXml, xNav);
            }
            else
                return HostingHelper.ReturnError("Session is timeout. Please login to the system.", myXml, xNav);
        }

        //http://192.168.70.210:8093/TTCS/Information?StationName=T07Airfoce&DeviceName=T07TTCS&SessionID=P@ss3610a
        public HttpResponseMessage Get(String StationName, String DeviceName, String SessionID)
        {
            return GetByDeviceName(StationName, DeviceName, SessionID);
        }

        public HttpResponseMessage Post(String StationName, String DeviceName, String SessionID)
        {
            return GetByDeviceName(StationName, DeviceName, SessionID);
        }

        private HttpResponseMessage GetByDeviceName(String StationName, String DeviceCategory, String SessionID)
        {
            XmlDocument myXml = new XmlDocument();
            XPathNavigator xNav = myXml.CreateNavigator();

            if (UserSessionHandler.VerifyTimeout(SessionID))
            {
                STATIONNAME ThisStation = HostingHelper.ConvertStationNameStrToSTATIONNAME(StationName);
                DEVICECATEGORY ThisDeviceCategory = HostingHelper.ConvertDevicecCategoryStrToDEVICECATEGORY(DeviceCategory);

                if (ThisStation == STATIONNAME.NULL)
                    return HostingHelper.ReturnError("Invalid station name. Please check.", myXml, xNav);

                if (ThisDeviceCategory == DEVICECATEGORY.NULL)
                    return HostingHelper.ReturnError("Invalid device name. Please check.", myXml, xNav);

                List<OUTPUTSTRUCT> InformationResult = AstroData.GetInformation(ThisStation, ThisDeviceCategory);
                if (InformationResult != null)
                {
                    JavaScriptSerializer Serializer = new JavaScriptSerializer();
                    Serializer.MaxJsonLength = Int32.MaxValue;
                    var json = Serializer.Serialize(InformationResult);
                    return new HttpResponseMessage() { Content = new StringContent(json, Encoding.UTF8, "application/json") };
                }
                else
                    return HostingHelper.ReturnError("An error occur while getting information", myXml, xNav);
            }
            else
                return HostingHelper.ReturnError("Session is timeout. Please login to the system.", myXml, xNav);
        }

        //http://192.168.70.210:8093/TTCS/Information?StationName=TTCS&DeviceName=T07TTCS&CommandName=ttcsclient.uptime.data&SessionID=P@ss3610a
        public HttpResponseMessage Get(String StationName, String DeviceName, String FieldName, String SessionID)
        {
            return GetByCommandName(StationName, DeviceName, FieldName, SessionID);
        }

        public HttpResponseMessage Post(String StationName, String DeviceName, String FieldName, String SessionID)
        {
            return GetByCommandName(StationName, DeviceName, FieldName, SessionID);
        }

        private HttpResponseMessage GetByCommandName(String StationName, String DeviceName, String FieldName, String SessionID)
        {
            XmlDocument myXml = new XmlDocument();
            XPathNavigator xNav = myXml.CreateNavigator();

            if (UserSessionHandler.VerifyTimeout(SessionID))
            {
                STATIONNAME ThisStation = HostingHelper.ConvertStationNameStrToSTATIONNAME(StationName);
                DEVICENAME ThisDevice = HostingHelper.ConvertDevicecNameStrToDEVICENAME(DeviceName);
                dynamic ThisFieldName = HostingHelper.ConvertFieldNameStrToFIELDNAME(FieldName);

                if (ThisStation == STATIONNAME.NULL)
                    return HostingHelper.ReturnError("Invalid station name. Please check.", myXml, xNav);

                if (ThisDevice == DEVICENAME.NULL)
                    return HostingHelper.ReturnError("Invalid device name. Please check.", myXml, xNav);

                if (ThisFieldName == null)
                    return HostingHelper.ReturnError("Invalid field name. Please check.", myXml, xNav);

                OUTPUTSTRUCT InformationResult = AstroData.GetInformation(ThisStation, ThisDevice, FieldName);
                if (InformationResult != null)
                {
                    JavaScriptSerializer Serializer = new JavaScriptSerializer();
                    Serializer.MaxJsonLength = Int32.MaxValue;
                    var json = Serializer.Serialize(InformationResult);
                    return new HttpResponseMessage() { Content = new StringContent(json, Encoding.UTF8, "application/json") };
                }
                else
                    return HostingHelper.ReturnError("An error occur while getting information", myXml, xNav);
            }
            else
                return HostingHelper.ReturnError("Session is timeout. Please login to the system.", myXml, xNav);
        }
    }

    public class SubscribeInfo
    {
        public string StationNames { get; set; }
        public string DeviceNames { get; set; }
        public string CommandNames { get; set; }

    }

    public class GRBController : ApiController
    {
        //http://192.168.70.210:8093/TTCS/GRB?Ra=1 12 34.00&Dec=2 3 1.34&FOV=100&SessionID=P@ss3610a
        public HttpResponseMessage Get(String Ra, String Dec, String FOV, String SessionID)
        {
            return RelayCommand(Ra, Dec, FOV, SessionID);
        }

        public HttpResponseMessage Post(String Ra, String Dec, String FOV, String SessionID)
        {
            return RelayCommand(Ra, Dec, FOV, SessionID);
        }

        private HttpResponseMessage RelayCommand(String Ra, String Dec, String FOV, String SessionID)
        {
            XmlDocument myXml = new XmlDocument();
            XPathNavigator xNav = myXml.CreateNavigator();

            if (UserSessionHandler.VerifyTimeout(SessionID))
            {
                ReturnKnowType CommandResult = AstroData.GRBHandler(Ra, Dec, FOV, DateTime.Now);

                XmlSerializer xmlSerializer3 = new XmlSerializer(typeof(ReturnKnowType));
                using (var xs = xNav.AppendChild()) { xmlSerializer3.Serialize(xs, CommandResult); }
                return new HttpResponseMessage() { Content = new StringContent(myXml.OuterXml, Encoding.UTF8, "text/xml") };
            }
            else
                return HostingHelper.ReturnError("Session is timeout. Please login to the system.", myXml, xNav);
        }
    }

    public class RelayController : ApiController
    {
        //http://192.168.70.210:8093/TTCS/Relay?StationName=T07AUSTRALIA&DeviceName=T07TS&CommandName=Telescope07_Mount_SlewIncrementRaDec&Value=45.0,10.0&SessionID=P@ss3610a
        public HttpResponseMessage Get(String StationName, String DeviceName, String CommandName, String Value, String SessionID)
        {
            return Operation(StationName, DeviceName, CommandName, Value, SessionID);
        }

        public HttpResponseMessage Post(String StationName, String DeviceName, String CommandName, String Value, String SessionID)
        {
            return Operation(StationName, DeviceName, CommandName, Value, SessionID);
        }

        private HttpResponseMessage Operation(String StationName, String DeviceName, String CommandName, String Value, String SessionID)
        {
            XmlDocument myXml = new XmlDocument();
            XPathNavigator xNav = myXml.CreateNavigator();

            if (UserSessionHandler.VerifyTimeout(SessionID))
            {
                STATIONNAME ThisStation = HostingHelper.ConvertStationNameStrToSTATIONNAME(StationName);
                DEVICENAME ThisDeviceName = HostingHelper.ConvertDevicecNameStrToDEVICENAME(DeviceName);
                DEVICECATEGORY ThisDeviceCategory = HostingHelper.ConvertDeviceNameToDeviceCategory(ThisStation, ThisDeviceName);

                if (ThisStation == STATIONNAME.NULL)
                    return HostingHelper.ReturnError("Invalid station name. Please check.", myXml, xNav);

                if (ThisDeviceName == DEVICENAME.NULL)
                    return HostingHelper.ReturnError("Invalid device name. Please check.", myXml, xNav);

                ReturnKnowType VerificationResult = CommandDefinition.VerifyCommand(ThisStation, ThisDeviceCategory, CommandName, HostingHelper.SplitValue(Value));

                if (VerificationResult.ReturnType == ReturnStatus.SUCESSFUL)
                {
                    dynamic ThisCommandName = CommandDefinition.GetCommandNameENUM(CommandName);
                    XmlSerializer xmlSerializer2 = new XmlSerializer(typeof(ReturnMessage));

                    ReturnKnowType CommandResult = AstroData.SetCommandHandler(ThisStation, ThisDeviceCategory, ThisDeviceName, ThisCommandName.Value, CommandDefinition.ValueConvertion(HostingHelper.SplitValue(Value)), DateTime.Now);

                    XmlSerializer xmlSerializer3 = new XmlSerializer(typeof(ReturnKnowType));
                    using (var xs = xNav.AppendChild()) { xmlSerializer3.Serialize(xs, CommandResult); }
                    return new HttpResponseMessage() { Content = new StringContent(myXml.OuterXml, Encoding.UTF8, "text/xml") };
                }

                return HostingHelper.ReturnError(VerificationResult.ReturnMessage, myXml, xNav);
            }
            else
                return HostingHelper.ReturnError("Session is timeout. Please login to the system.", myXml, xNav);
        }
    }

    public static class HostingHelper
    {
        public static HttpResponseMessage ReturnError(String Message, XmlDocument myXml, XPathNavigator xNav)
        {
            OUTPUTSTRUCT ThisOutput = new OUTPUTSTRUCT();
            ThisOutput.StationName = STATIONNAME.NULL;
            ThisOutput.DeviceCategory = DEVICECATEGORY.NULL;
            ThisOutput.FieldName = "NULL";
            ThisOutput.Value = Message;
            ThisOutput.DataType = "Get_Error";
            ThisOutput.UpdateTime = DateTime.Now.ToString();

            var json = new JavaScriptSerializer().Serialize(ThisOutput);
            return new HttpResponseMessage() { Content = new StringContent(json, Encoding.UTF8, "application/json") };
        }

        public static dynamic ConvertFieldNameStrToFIELDNAME(String FieldName)
        {
            TS700MM ThisTS700MM = Enum.GetValues(typeof(TS700MM)).Cast<TS700MM>().ToList().FirstOrDefault(Item => Item.ToString() == FieldName);
            if (ThisTS700MM != TS700MM.NULL) return ThisTS700MM;

            ASTROHEVENDOME ThisASTROHEVENDOME = Enum.GetValues(typeof(ASTROHEVENDOME)).Cast<ASTROHEVENDOME>().ToList().FirstOrDefault(Item => Item.ToString() == FieldName);
            if (ThisASTROHEVENDOME != ASTROHEVENDOME.NULL) return ThisASTROHEVENDOME;

            IMAGING ThisIMAGING = Enum.GetValues(typeof(IMAGING)).Cast<IMAGING>().ToList().FirstOrDefault(Item => Item.ToString() == FieldName);
            if (ThisIMAGING != IMAGING.NULL) return ThisIMAGING;

            SQM ThisSQM = Enum.GetValues(typeof(SQM)).Cast<SQM>().ToList().FirstOrDefault(Item => Item.ToString() == FieldName);
            if (ThisSQM != SQM.NULL) return ThisSQM;

            SEEING ThisSEEING = Enum.GetValues(typeof(SEEING)).Cast<SEEING>().ToList().FirstOrDefault(Item => Item.ToString() == FieldName);
            if (ThisSEEING != SEEING.NULL) return ThisSEEING;

            ALLSKY ThisALLSKY = Enum.GetValues(typeof(ALLSKY)).Cast<ALLSKY>().ToList().FirstOrDefault(Item => Item.ToString() == FieldName);
            if (ThisALLSKY != ALLSKY.NULL) return ThisALLSKY;

            WEATHERSTATION ThisWEATHERSTATION = Enum.GetValues(typeof(WEATHERSTATION)).Cast<WEATHERSTATION>().ToList().FirstOrDefault(Item => Item.ToString() == FieldName);
            if (ThisWEATHERSTATION != WEATHERSTATION.NULL) return ThisWEATHERSTATION;

            LANOUTLET ThisLANOUTLET = Enum.GetValues(typeof(LANOUTLET)).Cast<LANOUTLET>().ToList().FirstOrDefault(Item => Item.ToString() == FieldName);
            if (ThisLANOUTLET != LANOUTLET.NULL) return ThisLANOUTLET;

            CCTV ThisCCTV = Enum.GetValues(typeof(CCTV)).Cast<CCTV>().ToList().FirstOrDefault(Item => Item.ToString() == FieldName);
            if (ThisCCTV != CCTV.NULL) return ThisCCTV;

            GPS ThisGPS = Enum.GetValues(typeof(GPS)).Cast<GPS>().ToList().FirstOrDefault(Item => Item.ToString() == FieldName);
            if (ThisGPS != GPS.NULL) return ThisGPS;

            ASTROCLIENT ThisASTROCLIENT = Enum.GetValues(typeof(ASTROCLIENT)).Cast<ASTROCLIENT>().ToList().FirstOrDefault(Item => Item.ToString() == FieldName);
            if (ThisASTROCLIENT != ASTROCLIENT.NULL) return ThisASTROCLIENT;

            ASTROSERVER ThisASTROSERVER = Enum.GetValues(typeof(ASTROSERVER)).Cast<ASTROSERVER>().ToList().FirstOrDefault(Item => Item.ToString() == FieldName);
            if (ThisASTROSERVER != ASTROSERVER.NULL) return ThisASTROSERVER;

            return null;
        }

        public static DEVICECATEGORY ConvertDevicecCategoryStrToDEVICECATEGORY(String DeviceCategoryStr)
        {
            if (DeviceCategoryStr == null)
                return DEVICECATEGORY.NULL;

            return Enum.GetValues(typeof(DEVICECATEGORY)).Cast<DEVICECATEGORY>().ToList().FirstOrDefault(Item => Item.ToString() == DeviceCategoryStr.Trim().ToUpper());
        }

        public static DEVICENAME ConvertDevicecNameStrToDEVICENAME(String DeviceNameStr)
        {
            if (DeviceNameStr == null)
                return DEVICENAME.NULL;
            return Enum.GetValues(typeof(DEVICENAME)).Cast<DEVICENAME>().ToList().FirstOrDefault(Item => Item.ToString() == DeviceNameStr.Trim().ToUpper());
        }

        public static STATIONNAME ConvertStationNameStrToSTATIONNAME(String StationNameStr)
        {
            return Enum.GetValues(typeof(STATIONNAME)).Cast<STATIONNAME>().ToList().FirstOrDefault(Item => Item.ToString() == StationNameStr.Trim().ToUpper());
        }

        public static DEVICECATEGORY ConvertDeviceNameToDeviceCategory(STATIONNAME StationName, DEVICENAME DeviceName)
        {
            return AstroData.GetDeviceCategoryByDeviceName(StationName, DeviceName);
        }

        public static String[] SplitValue(string Value)
        {
            if (Value != null)
                return Value.Split(',');
            else
                return null;
        }
    }

    public class WebHosting
    {
        public static void CreateWebService()
        {
            var config = new HttpSelfHostConfiguration("http://" + DataKeeper.TTCSHelper.GetLocalIPAddress() + ":8093");
            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "TTCS/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional });
            config.MessageHandlers.Add(new CustomHeaderHandler());

            HttpSelfHostServer server = new HttpSelfHostServer(config);
            server.OpenAsync();
        }
    }

    public class CustomHeaderHandler : DelegatingHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, System.Threading.CancellationToken cancellationToken)
        {
            return base.SendAsync(request, cancellationToken)
                .ContinueWith((task) =>
                {
                    HttpResponseMessage response = task.Result;
                    response.Headers.Add("Access-Control-Allow-Origin", "*");
                    return response;
                });
        }
    }
}
