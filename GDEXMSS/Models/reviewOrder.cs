//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace GDEXMSS.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class reviewOrder
    {
        public int lineID { get; set; }
        public string orderID { get; set; }
        public Nullable<System.DateTime> reviewDT { get; set; }
        public string reviewComment { get; set; }
        public Nullable<int> reviewStar { get; set; }
        public Nullable<int> ProductID { get; set; }
        public Nullable<int> userID { get; set; }
    }
}
