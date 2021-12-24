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
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Web.Mvc;

    public partial class order
    {
        [Key]
        [DisplayName("Order ID")]
        public string orderID { get; set; }
        [DisplayName("Created Date Time")]
        public Nullable<System.DateTime> createdDT { get; set; }
        [DisplayName("Shipped Date Time")]
        public Nullable<System.DateTime> shippedDT { get; set; }
        [DisplayName("Purchaser ID")]
        public Nullable<int> userID { get; set; }
        [DisplayName("Status")]
        public string status { get; set; }
        public Nullable<int> pointRedeemed { get; set; }
        public Nullable<decimal> amountPaid { get; set; }
        public string consignment { get; set; }
        public Nullable<decimal> totalCost { get; set; }
    }
    public class combinedOrderModel
    {
        public order order { get; set; }
        public orderShippingInfo orderShippingInfo { get; set; }
        public cartItem cartItem { get; set; }
        public List<cartItem> listItems { get; set; }
        public mssSystem mssSystem { get; set; }
    }
    public class combinedOrderList
    {
        public order order { get; set; }
        public List<order> listOrder { get; set; }
        public orderShippingInfo orderShippingInfo { get; set; }
        public cartItem cartItem { get; set; }
        public List<cartItem> listItems { get; set; }
    }
}
