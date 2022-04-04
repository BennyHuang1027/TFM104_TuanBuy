using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

#nullable disable

namespace TuanBuy.Models.Entities
{
    [Table("Product")]

    public  class Product
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int Id { get; set; }
        public DateTime CreateTime { get; set; }
        public DateTime EndTime { get; set; }

        [Required]
        public string Name { get; set; }
        [Required]
        public string Description { get; set; }

        public string Content { get; set; }
        public string PicPath { get; set; }
        [Required]
        public decimal Price { get; set; }
        [Required]
        public string Category { get; set; }

        public bool Disable { get; set; } = false;

        public int UserId { get; set; }

        public virtual User User { get; set; }
        public virtual ICollection<Order> Order { get; set; }
        public virtual  ICollection<ProductPic>  ProductPics { get; set; }
        public virtual ICollection<ProductMessage> ProductMessage { get; set; }

    }
}
