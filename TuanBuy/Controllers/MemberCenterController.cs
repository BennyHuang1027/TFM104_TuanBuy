using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Org.BouncyCastle.Math.EC.Rfc7748;
using TuanBuy.Models;
using TuanBuy.Models.Entities;
using TuanBuy.Models.Interface;
using TuanBuy.ViewModel;

namespace TuanBuy.Controllers
{
    //[Authorize(Roles = "User")]
    //[Authorize(Roles = "FullUser")]
    public class MemberCenterController : Controller
    {
        private readonly IRepository<User> _userRepository;

        public MemberCenterController(GenericRepository<User> userRepository)
        {
            _userRepository = userRepository;
        }
        
        //會員中心首頁
        public IActionResult Index()
        {
            return View();
        }
        //更改密碼頁面
        public IActionResult ResetPassword()
        {
            return View();
        }
        [AllowAnonymous]
        public IActionResult StartMemberState(string s)
        {
            Console.WriteLine(s);

            var decodeEmail = Models.AppUtlity.GoDecode.Decode(s);

            var targetUser = _userRepository.Get(x => x.Email == decodeEmail);
            if (_userRepository == null)
            {
                return BadRequest();
            }
            else
            {
                targetUser.State = UserState.普通會員.ToString();
                _userRepository.SaveChanges();
            }

            return RedirectToAction("Login", "Home");
        }
        public string Test(string s)
        {
            return s;
        }
    }
}
