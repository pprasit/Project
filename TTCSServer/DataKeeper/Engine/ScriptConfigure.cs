using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataKeeper.Engine
{
    public class ScriptConfigure
    {
        public string config_name { set; get; }
        public string config_value { set; get; }
        public bool config_status { set; get; }
        public bool config_isaddtodb { set; get; }

        public ScriptConfigure(string config_name, string config_value, bool config_status, bool config_isaddtodb)
        {
            this.config_name = config_name;
            this.config_value = config_value;
            this.config_status = config_status;
            this.config_isaddtodb = config_isaddtodb;
        }
    }
}
