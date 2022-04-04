using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TuanBuy.Models;
using TuanBuy.Models.AppUtlity;
using TuanBuy.Models.Entities;

namespace Topic.Hubs
{
    //聊天室使用者清單
    public class UserData : IEquatable<UserData>
    {
        public string Sid { get; set; }
        public string UserAccount { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        public string Group { get; set; }

        public List<Guid> ChatId { get; set; }
        public string Role { get; set; } = "一般使用者";
        public DateTime ConnectedTime { get; set; }
        public string Id { get; set; }

        #region 實作IEnumerable介面類別資料比對查詢
        public bool Equals(UserData other)
        {
            if (other is null)
                return false;

            return this.Id == other.Id;
        }

        public override bool Equals(object obj) => Equals(obj as UserData);
        public override int GetHashCode() => (Name, Id).GetHashCode();
        #endregion
    }

    public class UserService : DbContext
    {
        public static List<UserData> _userDatas = new List<UserData>();
        //手動建立連線
        private IConfiguration _config;
        public UserService()
        {
            _config = AppUtility.getConfiguration("appsettings.json");
            //_userDatas = new List<UserData>();
        }
        //建立使用者資料泛型集合 新增使用者名單
        public List<UserData> addList(UserData userData)
        {
            _userDatas.Add(userData);
            return _userDatas;
        }

        public List<UserData> GetUserList()
        {
            return _userDatas;
        }

        public List<UserData> RemoveList(string connectID)
        {
            //所有在線清單搜索判斷並從中移除該使用者
            for (int i = 0; i < _userDatas.Count; i++)
            {
                if (_userDatas[i].Sid == connectID)
                {
                    _userDatas.Remove(_userDatas[i]);
                }
            }
            return _userDatas;
        }

        //取得目前登入使用者好友在線清單
        public List<UserData> GetOnlineUser(int MemeberID)
        {
            //手動建立DbContextOptions
            var contextOptions = new DbContextOptionsBuilder<TuanBuyContext>()
            .UseSqlServer(_config.GetConnectionString("TuanBuy"))
            .Options;
            TuanBuyContext sqlservices = new TuanBuyContext(contextOptions);
            //在線清單抓取及使用者好友抓取進行比對
            UserDb db = new UserDb(sqlservices);
            var UserFriend = db.GetChatUser(MemeberID);
            var onlineuser = GetUserList();
            List<UserData> userDatas = new List<UserData>();
            foreach (var item in UserFriend)
            {
                UserData userData = new UserData();
                userData.Name = item.Users.NickName;
                userData.UserAccount = item.Users.UserAccount;
                userData.Id = item.Users.UserId;
                userDatas.Add(userData);
            };
            //實作IEnumerable介面類別資料比對查詢
            IEnumerable<UserData> queryOnlineuser = onlineuser.Intersect<UserData>(userDatas);
            List<UserData> onlineusers = queryOnlineuser.ToList<UserData>();
            //取得目前使用者的好友
            return onlineusers;
        }

        //找尋目前登入者擁有那些聊天室
        public UserData GetOnlineUserChat(string username, int MemeberId, string userAccount)
        {
            //手動建立DbContextOptions
            var contextOptions = new DbContextOptionsBuilder<TuanBuyContext>()
            .UseSqlServer(_config.GetConnectionString("TuanBuy"))
            .Options;
            TuanBuyContext sqlservices = new TuanBuyContext(contextOptions);
            //在線清單抓取及使用者好友抓取進行比對
            var result = sqlservices.Member_Chats.Where(x => x.MemberId == MemeberId).Select(x => new { x.ChatRoomId});
            UserData _user = new UserData();
            _user.Name = username;
            _user.ConnectedTime = DateTime.Now;
            _user.Id = MemeberId.ToString();
            _user.UserAccount = userAccount;
            List<Guid> guids = new List<Guid>();
            foreach(var item in result)
            {
                Guid guid = item.ChatRoomId;
                guids.Add(guid);
            }
            _user.ChatId = guids;
            return _user;
        }

        //新增聊天訊息
        public void CreateMessage(Guid ChatRoomId,int MemberId,string Message)
        {
            //手動建立DbContextOptions
            var contextOptions = new DbContextOptionsBuilder<TuanBuyContext>()
            .UseSqlServer(_config.GetConnectionString("TuanBuy"))
            .Options;
            TuanBuyContext sqlservices = new TuanBuyContext(contextOptions);
            using(sqlservices)
            {
                ChatMessages messages = new ChatMessages()
                {
                    ChatRoomId = ChatRoomId,
                    MemberId = MemberId,
                    Message = Message,
                    CreateDate = DateTime.Now
                };
                sqlservices.ChatMessages.Add(messages);
                sqlservices.SaveChanges();
            }
        }
    }
}
