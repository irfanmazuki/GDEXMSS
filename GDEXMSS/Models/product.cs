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
    using System.Web;
    using System.Web.Mvc;

    public partial class product
    {
        [DisplayName("Product ID")]
        public int productID { get; set; }
        [Required]
        [DisplayName("Name")]
        public string name { get; set; }
        [Required]
        [DisplayName("Description")]
        public string description { get; set; }
        [Required]
        [DisplayName("Quantity")]
        public Nullable<int> quantity { get; set; }
        [Required]
        [DisplayName("Price")]
        public Nullable<decimal> unitCost { get; set; }
        [Required]
        [DisplayName("Category")]
        public Nullable<int> categoryID { get; set; }
        public Nullable<bool> isVariation { get; set; }
        public Nullable<bool> isExist { get; set; }
        public string imagePath { get; set; }
        public virtual SelectList CategoriesList { get; set; }
        [DisplayName("Sold")]
        public Nullable<int> quantitySold { get; set; }
    }
    public class CombinedProductCategories
    {
        public productCategory productCategory { get; set; }
        public product product { get; set; }
        public HttpPostedFileBase ImageFile { get; set; }
    }
    public class CombinedProductIndex
    {
        public List<productCategory> listCategories { get; set; }
        public List<product> listProduct { get; set; }
        public List<cartItem> listCartItem { get; set; }
    }
    public class CombinedProductReview
    {
        public product product { get; set; }
        public List<reviewOrder> listReviews { get; set; }
    }
}
