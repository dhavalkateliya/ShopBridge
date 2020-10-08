using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ShopBridge.Models
{
    public class ProductViewModel
    {
        public Int32 ID { get; set; }
        [Display(Name = "ProdcutName")]
        [Required(ErrorMessage = "Product name is Required")]
        public string ProdcutName { get; set; }

        [Display(Name = "Description")]
        [Required(ErrorMessage = "Description is Required")]
        public string Description { get; set; }


        [Display(Name = "Price")]
        [Required(ErrorMessage = "Price is Required")]
        [Range(typeof(Decimal), "1", "9999", ErrorMessage = "{0} must be a decimal/number between {1} and {2}.")]

        public decimal Price { get; set; }
        [Display(Name = "Quantity")]
        [Required(ErrorMessage = "Quantity is Required")]
        [Range(typeof(Int32), "1", "9999", ErrorMessage = "{0} must be a number between {1} and {2}.")]
        public Int32 Quantity { get; set; }
       
    }

}