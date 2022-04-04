using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TuanBuy.Models.Entities;
using TuanBuy.ViewModel;

namespace TuanBuy.Models
{
    public class UserDb
    {
        private TuanBuyContext _dbContext;
        public UserDb(TuanBuyContext dbContext)
        {
            _dbContext = dbContext;
        }

        #region 取得使用者聊天室清單 (可能為多人聊天室等情況抓取)
        public List<ChatRoomListViewModel> GetChatUserList(int MemberId)
        {
            //抓取到該名使用者擁有的聊天室
            var getChatID = _dbContext.Member_Chats.Where(x => x.MemberId == MemberId).Select(x => x.ChatRoomId);
            //找尋除了自己之外相同聊天室的其他使用者
            var getChatuser = (from item in _dbContext.Member_Chats
                               where (getChatID).Contains(item.ChatRoomId)
                               select new { item.MemberId, item.ChatRoomId })
                         .Except(_dbContext.Member_Chats.Where(x => x.MemberId == MemberId)
                         .Select(x => new { x.MemberId, x.ChatRoomId }));
            var result = (from chat in _dbContext.Member_Chats
                          join user in _dbContext.User on chat.MemberId equals user.Id
                          join chatroom in _dbContext.ChatRooms on chat.ChatRoomId equals chatroom.ChatRoomId
                          where ((getChatuser.Select(x => x.ChatRoomId).Contains(chat.ChatRoomId)
                          && (getChatuser.Select(u => u.MemberId).Contains(user.Id))))
                          select new { user, chat.ChatRoomId, chatroom });
            //將使用者分為群 一個聊天室可能有多個使用者
            Dictionary<Guid, List<UsersViewModel>> keyValuePairs = new Dictionary<Guid, List<UsersViewModel>>();
            foreach (var item in result)
            {
                if (keyValuePairs.ContainsKey(item.ChatRoomId))
                {
                    UsersViewModel usersViewModel = new UsersViewModel();
                    //List<ChatRoom> chatRooms = new List<ChatRoom>();
                    //ChatRoom chatRoom = new ChatRoom();
                    usersViewModel.ChatRoomId = item.ChatRoomId.ToString();
                    usersViewModel.ChatRoomTitle = item.chatroom.ChatRoomTitle;
                    usersViewModel.NickName = item.user.NickName;
                    usersViewModel.UserAccount = item.user.Email;
                    usersViewModel.PicPath = item.user.PicPath;
                    usersViewModel.UserId = item.user.Id.ToString();
                    keyValuePairs[item.ChatRoomId].Add(usersViewModel);
                }
                else
                {
                    List<UsersViewModel> usersViewModels = new List<UsersViewModel>();
                    UsersViewModel usersViewModel = new UsersViewModel();
                    //List<ChatRoom> chatRooms = new List<ChatRoom>();
                    //ChatRoom chatRoom = new ChatRoom();
                    usersViewModel.ChatRoomId = item.ChatRoomId.ToString();
                    usersViewModel.ChatRoomTitle = item.chatroom.ChatRoomTitle;
                    usersViewModel.NickName = item.user.NickName;
                    usersViewModel.UserAccount = item.user.Email;
                    usersViewModel.PicPath = item.user.PicPath;
                    usersViewModel.UserId = item.user.Id.ToString();
                    usersViewModels.Add(usersViewModel);
                    keyValuePairs.Add(item.ChatRoomId, usersViewModels);
                }
            }
            List<ChatRoomListViewModel> chatRoomViewModels = new List<ChatRoomListViewModel>();
            foreach(var item in keyValuePairs)
            {
                ChatRoomListViewModel chatRoomListViewModel = new ChatRoomListViewModel();
                List<UsersViewModel> usersViewModels = new List<UsersViewModel>();
                chatRoomListViewModel.ChatRoomId = item.Key.ToString();
                foreach(var users in item.Value)
                {
                    UsersViewModel usersViewModel = new UsersViewModel();
                    usersViewModel.ChatRoomId = users.ChatRoomId;
                    if (users.ChatRoomTitle != "")
                    {
                        chatRoomListViewModel.ChatRoomTitle = users.ChatRoomTitle; 
                        usersViewModel.ChatRoomTitle = users.ChatRoomTitle;
                    }
                    usersViewModel.PicPath = users.PicPath;
                    usersViewModel.NickName = users.NickName;
                    usersViewModel.UserAccount = users.UserAccount;
                    usersViewModel.UserId = users.UserId;
                    usersViewModels.Add(usersViewModel);
                }
                chatRoomListViewModel.Users = usersViewModels;
                chatRoomViewModels.Add(chatRoomListViewModel);
            }
            return chatRoomViewModels;
        }
        #endregion 取使用者聊天室清單

        #region 取得使用者聊天室清單
        public List<ChatRoomViewModel> GetChatUser(int MemberId)
        {
            //抓取到該名使用者擁有的聊天室
            var getChatID = _dbContext.Member_Chats.Where(x => x.MemberId == MemberId).Select(x => x.ChatRoomId);
            //找尋除了自己之外相同聊天室的其他使用者
            var getChatuser = (from item in _dbContext.Member_Chats
                               where (getChatID).Contains(item.ChatRoomId)
                               select new { item.MemberId, item.ChatRoomId })
                         .Except(_dbContext.Member_Chats.Where(x => x.MemberId == MemberId)
                         .Select(x => new { x.MemberId, x.ChatRoomId }));
            //var result = (from chat in _dbContext.Member_Chats
            //              join user in _dbContext.Members on chat.MemberId equals user.Id
            //              where (getChatuser).Contains(user.Id)
            //              select new
            //              {
            //                  chat.ChatRoomId,
            //                  user
            //              });
            var result = from chat in _dbContext.Member_Chats
                         join user in _dbContext.User on chat.MemberId equals user.Id
                         where ((getChatuser.Select(x => x.ChatRoomId).Contains(chat.ChatRoomId)
                         && (getChatuser.Select(u => u.MemberId).Contains(user.Id))))
                         select new { user, chat.ChatRoomId };
            List<ChatRoomViewModel> chatRoomViewModels = new List<ChatRoomViewModel>();
            foreach (var item in result)
            {
                ChatRoomViewModel chatRoomViewModel = new ChatRoomViewModel();
                UsersViewModel user = new UsersViewModel();
                chatRoomViewModel.ChatRoomId = item.ChatRoomId.ToString();
                user.UserId = item.user.Id.ToString();
                user.NickName = item.user.NickName;
                user.UserAccount = item.user.Email;
                //chatRoomViewModel.UserId = item.user.Id.ToString();
                //chatRoomViewModel.UserAccount = item.user.UserAccount;
                //chatRoomViewModel.NickName = item.user.NickName;
                chatRoomViewModel.Users = user;
                chatRoomViewModels.Add(chatRoomViewModel);
            }
            return chatRoomViewModels;
        }
        #endregion 取使用者聊天室清單

        #region 取得聊天室聊天訊息
        public object GetChatMessage(Guid Chatid, int Id)
        {
            //抓出該聊天室訊息
            var getMessage = _dbContext.ChatMessages.Where(x => x.ChatRoomId == Chatid).Select(x => x.MemberId);
            //var result = from user in _dbContext.Members
            //             join usermessage in _dbContext.ChatMessages on usermessage.MemberId equals user.Id
            //             where (getMessage).Contains(user.Id)
            //             select new { user,usermessage};
            var result = (from usermessage in _dbContext.ChatMessages
                          join user in _dbContext.User on usermessage.MemberId equals user.Id
                          where (getMessage).Contains(user.Id)
                          select new { user, usermessage });


            List<ChatMessageViewModel> chatMessages = new List<ChatMessageViewModel>();
            foreach (var item in result)
            {
                //不是該聊天室的訊息則重新迴圈
                if (item.usermessage.ChatRoomId != Chatid)
                {
                    continue;
                }
                ChatMessageViewModel chatMessage = new ChatMessageViewModel();
                //先進行處理判斷是否為自己的訊息
                if (item.usermessage.MemberId == Id)
                {
                    chatMessage.MyMessage = item.usermessage.Message;
                    chatMessage.MyMessageImage = item.usermessage.MessageImage;
                }
                else
                {
                    chatMessage.FriendMessage = item.usermessage.Message;
                    chatMessage.FriendMessageImage = item.usermessage.MessageImage;
                }
                chatMessage.PicPath = item.user.PicPath;
                chatMessage.MemberId = item.usermessage.MemberId.ToString();
                chatMessage.NickName = item.user.NickName;
                chatMessage.MessageId = item.usermessage.MessageId.ToString();
                chatMessage.CreateDate = item.usermessage.CreateDate;
                chatMessages.Add(chatMessage);
            }

            return chatMessages;
        }
        #endregion

        #region 使用者新增聊天訊息
        public string CreateMessage(int MemberId, Guid ChatRoomId, string Message)
        {
            _dbContext.ChatMessages.Add(new ChatMessages()
            {
                ChatRoomId = ChatRoomId,
                MemberId = MemberId,
                Message = Message,
            });
            return "OK";
        }
        #endregion
    }
}
