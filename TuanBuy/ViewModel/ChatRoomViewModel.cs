using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TuanBuy.ViewModel
{
    public class ChatRoomViewModel
    {
        public string ChatRoomId { get; set; }

        public string CaatRoomTitle { get; set; }

        public UsersViewModel Users { get; set; }
    }
    public class ChatRoomListViewModel
    {
        public string ChatRoomId { get; set; }

        public string ChatRoomTitle { get; set; }

        public List<UsersViewModel> Users { get; set; }
    }
    public class UsersViewModel
    {
        public string ChatRoomId { get; set; }

        public string ChatRoomTitle { get; set; }

        public string PicPath { get; set; }
        public string UserAccount { get; set; }
        public string UserId { get; set; }
        public string NickName { get; set; }
    }
    public class ChatMessageViewModel
    {
        public string PicPath { get; set; }
        public string MessageId { get; set; }

        public string MemberId { get; set; }

        public string NickName { get; set; }

        public string MyMessage { get; set; }

        public string MyMessageImage { get; set; }
        public string FriendMessage { get; set; }
        public string FriendMessageImage { get; set; }
        public DateTime CreateDate { get; set; }
    }
}
