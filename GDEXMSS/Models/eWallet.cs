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
    
    public partial class eWallet
    {
        public int walletID { get; set; }
        public Nullable<int> userID { get; set; }
        public Nullable<int> availablePoints { get; set; }
        public Nullable<decimal> amountRM { get; set; }
    }
    public class CombinedWalletUser
    {
        public user userObj { get; set; }
        public eWallet eWallet { get; set; }
    }
    public class CombinedListWalletUser
    {
        public List<user> listUser { get; set; }
        public List<eWallet> listWallet { get; set; }
    }
}
