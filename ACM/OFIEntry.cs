//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ACM
{
    using System;
    using System.Collections.Generic;
    
    public partial class OFIEntry
    {
        public int OFIEntryID { get; set; }
        public int OFIID { get; set; }
        public System.DateTime CreateDate { get; set; }
        public string CreateUserID { get; set; }
        public bool Deleted { get; set; }
        public string EntryText { get; set; }
        public Nullable<System.DateTime> UpdateDate { get; set; }
        public string UpdateUserID { get; set; }
    }
}
