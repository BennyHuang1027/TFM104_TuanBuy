using System;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TuanBuy.Models;
using TuanBuy.Models.Entities;
using TuanBuy.Models.Interface;
using TuanBuy.ViewModel;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TuanBuy.Service
{
    [Route("api/[controller]")]
    [ApiController]
    public class MemberCenterController : ControllerBase
    {
        private readonly IWebHostEnvironment _environment;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IRepository<User> _userRepository;

        public MemberCenterController(GenericRepository<User> userRepository, IHttpContextAccessor httpContextAccessor,
            IWebHostEnvironment environment)
        {
            _httpContextAccessor = httpContextAccessor;
            _userRepository = userRepository;
            _environment = environment;
        }
        // GET: api/<MemberCenterController>
        //[HttpGet]
        //public IEnumerable<string> Get()
        //{

        //    return new string[] { "value1", "value2" };
        //}

        // GET api/<MemberCenterController>/5
        [HttpGet]
        public UserViewModel Get()
        {
            var targetUser = GetTargetUser();
            if (targetUser == null) return new UserViewModel();
            var userData = new UserViewModel
            {
                Email = targetUser.Email,
                Name = targetUser.Name,
                Phone = targetUser.Phone,
                Address = targetUser.Address,
                Birth = targetUser.Birth,
                Sex = targetUser.Sex,
                BankAccount = targetUser.BankAccount,
                PicPath = "/MemberPicture/" + targetUser.PicPath
            };
            return userData;
        }

        // POST api/<MemberCenterController>
        //更新使用者資料
        [HttpPost]
        public void Post([FromForm] UserUpdate user)
        {
            var fileName = "";
            if (user.PicPath != null)
            {
                var path = _environment.WebRootPath + "/MemberPicture";
                var pic = user.PicPath;
                fileName = DateTime.Now.Ticks + pic.FileName;
                using var fs = System.IO.File.Create($"{path}/{fileName}");
                pic.CopyTo(fs);
            }

            var fullMember = user.Name != "" || user.Phone != "" || user.Address != "" || user.Birth != null;
            if (_httpContextAccessor.HttpContext != null)
            {
                var targetUser = GetTargetUser();
                targetUser.Name = user.Name;
                targetUser.Phone = user.Phone;
                targetUser.Birth = user.Birth;
                targetUser.Address = user.Address;
                targetUser.Sex = user.Sex;
                targetUser.PicPath = fileName;
                if (fullMember)
                {
                    targetUser.State = UserState.正式會員.ToString();
                    //這段有問題 不能直接更改會員資料
                    //var claims = new Claim(ClaimTypes.Role, "FullUser");
                    //var claimsIdentity = new ClaimsIdentity();
                    //claimsIdentity.AddClaim(claims);
                    //var claimsPrincipal = new ClaimsPrincipal();
                    //claimsPrincipal.AddIdentity(claimsIdentity);
                    //HttpContext.SignInAsync(claimsPrincipal);
                    //TODO 用回應加Cookies做?
                    //HttpContext.Response.Cookies.Append("","");
                }
            }

            _userRepository.SaveChanges();
        }

        // 更改使用者密碼
        [HttpPut]
        public void Put([FromBody] UserUpdate user)
        {
            if (_httpContextAccessor.HttpContext != null)
            {
                var claim = _httpContextAccessor.HttpContext.User.Claims.ToList();
                var userMail = claim.First(a => a.Type == "UserEmail").Value;
                var targetUser = _userRepository.Get(a => a.Email == userMail);
                targetUser.Password = user.Password;
            }

            _userRepository.SaveChanges();
        }


        // DELETE api/<MemberCenterController>/5
        //軟刪除使用者
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var target = _userRepository.Get(x => x.Id == id);
            target.Disable = true;
            return Ok("使用者刪除成功");
        }

        private User GetTargetUser()
        {
            var claim = HttpContext.User.Claims;
            var userEmail = claim.FirstOrDefault(a => a.Type == ClaimTypes.Email)?.Value;
            var targetUser = _userRepository.Get(a => a.Email == userEmail);
            return targetUser;
        }
    }
}