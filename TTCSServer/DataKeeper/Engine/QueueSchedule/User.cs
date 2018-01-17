using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AstroNET.QueueSchedule
{
    public class User
    {
        public int id;
        public String name;
        public int groupId;
        public String groupName;
        public double priority;
        
        public static List<AstroQueue> FindPriority(List<AstroQueue> astroQueues)
        {
            astroQueues = astroQueues.OrderBy(x => x.User.priority).ToList();
            return astroQueues;
        }
    }    
}
