using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TuanBuy.Models.Entities
{
    public class ProductMessage
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public string MessageContent { get; set; }
        public int UserId { get; set; }
        public virtual Product Product { get; set; }
    }
}