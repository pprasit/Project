using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AstroNET.QueueSchedule
{    
    public enum EXECUTESTATUS
    {
        WAIT = 0,
        SUCCESS = 1,
        FAIL = 2
    }

    public class ExposedHistory
    {
        public String filterName = null;
        public EXECUTESTATUS executedStatus;
        public DateTime? executedDate = null;
    }
}
