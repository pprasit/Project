//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace DataKeeper
{
    using System;
    using System.Collections.Generic;
    
    public partial class LogTB
    {
        public string LogID { get; set; }
        public string StationName { get; set; }
        public Nullable<System.DateTime> LogDateTime { get; set; }
        public string UserID { get; set; }
        public string Note { get; set; }
        public string Problem { get; set; }
    
        public virtual UserTB UserTB { get; set; }
    }
}
