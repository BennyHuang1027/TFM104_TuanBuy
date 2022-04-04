using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using TuanBuy.Models.Entities;

namespace TuanBuy.Models
{
    public class ChatRoom
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid ChatRoomId { get; set; }

        public string ChatRoomTitle { get; set; }

        //與會員多對多關係建立連接
        //public List<Member> Member { get; set; } = new List<Member>();

        public virtual ICollection<User> Member { get; set; }

    }
}
