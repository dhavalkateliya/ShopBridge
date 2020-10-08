using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShopBridge.Models
{
    public class Product
    {
        public Int32 ID { get; set; }
        [Display(Name = "ProdcutName")]
        public string ProdcutName { get; set; }

        [Display(Name = "Description")]
        public string Description { get; set; }

       
        [Display(Name = "Price")]
        public decimal Price { get; set; }
        [Display(Name = "Quantity")]
        public Int32 Quantity { get; set; }
       
       

    }
}
