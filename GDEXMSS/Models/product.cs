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
    using System.Web;

    public partial class product
    {
        public int productID { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public Nullable<int> quantity { get; set; }
        public Nullable<decimal> unitCost { get; set; }
        public Nullable<int> categoryID { get; set; }
        public Nullable<bool> isVariation { get; set; }
        public Nullable<bool> isExist { get; set; }
        public string imagePath { get; set; }
        public HttpPostedFileBase ImageFile { get; set; }
    }
    public class productModel
    {
        public List<product> Languages { get; set; }
    }
}
