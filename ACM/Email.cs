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
    
    public partial class Email
    {
        public int EmailID { get; set; }
        public string Subject { get; set; }
        public string EmailBody { get; set; }
        public Nullable<int> OFINum { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public string CreatedUserID { get; set; }
        public Nullable<System.DateTime> SentDate { get; set; }
        public Nullable<int> Result { get; set; }
    }
}
