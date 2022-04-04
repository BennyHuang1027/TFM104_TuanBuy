using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace TuanBuy.Models
{
    public class ChatMessages
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int MessageId { get; set; }

        [ForeignKey("ChatRoom")]
        public Guid ChatRoomId { get; set; }

        public int MemberId { get; set; }

        public string Message { get; set; }

        public string MessageImage { get; set; }

        public DateTime CreateDate { get; set; } = DateTime.Now;
        //public int ChatId { get; set; }
        public virtual ChatRoom ChatRoom { get; set; }

    }
}
