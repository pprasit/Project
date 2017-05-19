using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace DataKeeper.Engine
{
    public static class TTCSCommandHelp
    {
        public static HttpResponseMessage GetPage()
        {
            var response = new HttpResponseMessage();
            response.Content = new StringContent(StringPage());
            response.Content.Headers.ContentType = new MediaTypeHeaderValue("text/html");
            return response;
        }

        private static String StringPage()
        {
            string html = File.ReadAllText(AppDomain.CurrentDomain.BaseDirectory + @"\Engine\CommandHelp.html");
            return html;
            //return "<html>" +
            //    "<body>" +
            //    "Hello World" +
            //    "</body>" +
            //    "</html>";
        }
    }
}
