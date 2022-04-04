using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace Topic.Hubs
{
    public class ChatHub : Hub
    {
        private UserData _userData;
        private UserService _userservice;
        //public ChatHub(){}

        public ChatHub(UserService userService)
        {
            _userData = new UserData();
            _userservice = userService;
        }

        //public async Task GetOnlineUserList(string UserAccount)
        //{
        //    await Clients.Client(Context.ConnectionId).SendAsync("GetOnlineUserList", _userservice.GetOnlineUser(UserAccount));
        //}

        //取得當前使用者目前連線id
        public string GetConnectionId()
        {
            return Context.ConnectionId;
        }

        //一對一寄發訊息
        public async Task SendPrivateMessage(string nowChatRoomId, int userid, string message)
        {
            //TODO 可以做成若對方不再線可顯示個訊息對方不在線中請耐心等待對方上線哦~
            //找出在線中並且擁有該聊天室ID的使用者
            var userlist = _userservice.GetUserList();
            var user = _userservice.GetUserList().Find(x => (x.ChatId.Select(X => X).Contains(Guid.Parse(nowChatRoomId))) && x.Sid != Context.ConnectionId);
            if (user != null)
            {
                string userId = user.Sid;
                string currentUser = Context.User.Identity.Name;
                var currentUserImg = Context.User.Claims.FirstOrDefault(x => x.Type == "PicPath").Value;
                _userservice.CreateMessage(Guid.Parse(nowChatRoomId), userid, message);
                //await this.Clients.Client(userId).SendAsync("PrivateMsgRecevied", message, currentUser, currentUserImg);
                await this.Clients.GroupExcept(nowChatRoomId, Context.ConnectionId).SendAsync("PrivateMsgRecevied", message, currentUser, currentUserImg);
            }
            else
            {
                _userservice.CreateMessage(Guid.Parse(nowChatRoomId), userid, message);
            }
        }
        public async Task SendPrivateImageMessage(string nowChatRoomId, int userid,string userName, string userPicPath, string messagecontext, string userConnectionId, string imgMsgPicPath)
        {
            var userlist = _userservice.GetUserList();
            var user = _userservice.GetUserList().Find(x => (x.ChatId.Select(X => X).Contains(Guid.Parse(nowChatRoomId))) && x.Sid != userConnectionId);
            if (user != null)
            {
                string userId = user.Sid;
                //string currentUser = sendUserName;
                //var currentUserImg = sendUserPicPath;
                await this.Clients.GroupExcept(nowChatRoomId, userConnectionId).SendAsync("PrivateImgMsgRecevied", messagecontext, userName, userPicPath, imgMsgPicPath);
            }
        }

        //取得目前連線使用者
        public async Task GetUserList()
        {
            await Clients.All.SendAsync("GetUserList", _userservice.GetUserList());
        }

        //將目前使用者先加入線上群組 
        //,_userservice.addList(_user) 新增使用者後回傳使用者list
        //使用者一登入連上HUB後呼叫該方法
        public async void AddUserList(string username, int userId, string userAccount)
        {
            UserData _user = new UserData();
            if (userAccount != null)
            {
                _user = _userservice.GetOnlineUserChat(username, userId, userAccount);
                _user.Sid = Context.ConnectionId;
                //將使用者加入個別聊天市群組
                for (var i = 0; i < _user.ChatId.Count; i++)
                {
                    await Groups.AddToGroupAsync(_user.Sid, _user.ChatId[i].ToString());
                }
                await Groups.AddToGroupAsync(_user.Sid, "onlineGroup");
                //即時發送所有有在這個群組裡面的使用者
                await Clients.Groups("onlineGroup").SendAsync("GetUserList", _userservice.addList(_user));
                //判斷目前登入使用者的所有好友有沒有在線若有則使用迴圈發送給在線好友消息告知上線
                var onlineFriend = _userservice.GetOnlineUser(userId);
                if (onlineFriend.Count > 0)
                {
                    foreach (var user in onlineFriend)
                    {
                        //寄發給目前在線使用者好友上線通知
                        await this.Clients.Client(user.Sid).SendAsync("SendOnlineUserList", _user);
                    }
                }
            }
        }

        //取得目前在線好友
        public async Task GetOnlineUserList(int MemeberID)
        {
          await Clients.Client(Context.ConnectionId).SendAsync("GetOnlineUserList", _userservice.GetOnlineUser(MemeberID));
        }

        // 連線
        public override async Task OnConnectedAsync()
        {
            await base.OnConnectedAsync();
        }
        public override async Task OnDisconnectedAsync(Exception exception)
        {
            await base.OnDisconnectedAsync(exception);
            await Clients.Group("onlineGroup").SendAsync("GetUserList", _userservice.RemoveList(Context.ConnectionId));
        }
    }
}
