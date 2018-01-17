using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AstroNET.QueueSchedule
{
    public class ExposureInfo
    {
        public int bin;
        public String filterName;
        public double exposureTime;
        public int exposureAmount;
    }

    public static class Exposure
    {
        public static List<ExposureInfo> generate(AstroQueue task)
        {
            /*
             * 
             *          SORT = 1, //R R R G G G B B B
                        SEQUENCE = 2, // R G B   R G B
             * 
             */
            List<ExposureInfo> exposeJobs = new List<ExposureInfo>();

            if (task.Target.filterMode == FILTER_MODE.SORT)
            {
                foreach (ExposureInfo exposeInfo in task.Target.exposureInfo)
                {
                    for (int i = 0; i < exposeInfo.exposureAmount; ++i)
                    {
                        exposeJobs.Add(exposeInfo);
                    }
                }
            }
            else if (task.Target.filterMode == FILTER_MODE.SEQUENCE)
            {
                while (true)
                {
                    Boolean isComplete = false;

                    foreach (ExposureInfo exposeInfo in task.Target.exposureInfo)
                    {
                        if (exposeInfo.exposureAmount > exposeJobs.Where(x => x.filterName == exposeInfo.filterName).Count())
                        {
                            exposeJobs.Add(exposeInfo);
                            //--exposeInfo.exposureAmount;
                            isComplete = true;
                        }
                    }

                    if (!isComplete)
                        break;
                }
            }

            return exposeJobs;
        }
    }
}
