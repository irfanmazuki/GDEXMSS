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
    
    public partial class transactionHistoryWallet
    {
        public int transactionID { get; set; }
        public Nullable<decimal> amount { get; set; }
        public string referenceID { get; set; }
        public string paymentMethod { get; set; }
        public Nullable<System.DateTime> paidDT { get; set; }
        public Nullable<int> walletID { get; set; }
    }
}
