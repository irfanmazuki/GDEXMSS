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
    
    public partial class user
    {
        public int userID { get; set; }
        public string user_type { get; set; }
        public string fullname { get; set; }
        public string email { get; set; }
        public string password { get; set; }
        public string race { get; set; }
        public Nullable<long> icnumber { get; set; }
        public Nullable<long> contact_number { get; set; }
        public string bank_name { get; set; }
        public Nullable<long> bank_number { get; set; }
        public string home_address { get; set; }
        public string home_poscode { get; set; }
        public string home_city { get; set; }
        public string home_state { get; set; }
    }
}
