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
    
    public partial class orderLine
    {
        public int lineID { get; set; }
        public string orderID { get; set; }
        public Nullable<int> productID { get; set; }
        public Nullable<int> variationID { get; set; }
        public Nullable<int> quantity { get; set; }
        public Nullable<decimal> unitCost { get; set; }
        public string productName { get; set; }
        public string variationName { get; set; }
    }
}
