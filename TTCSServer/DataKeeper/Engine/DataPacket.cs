using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataKeeper.Engine
{
    public class DataPacket
    {
        public String DataId;
        public DEVICECATEGORY DeviceCategory;
        public DEVICENAME DeviceName;
        public String FieldName;
        public Object Value;
        public String DataType;
        public long DateTimeUTC;
    }
}
