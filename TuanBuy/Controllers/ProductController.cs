using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TuanBuy.Models;
using TuanBuy.Models.Entities;
using TuanBuy.Models.Interface;
using TuanBuy.ViewModel;

namespace TuanBuy.Controllers
{
    public class ProductController : Controller
    {
        private readonly IRepository<Product> _productsRepository;
        private readonly IWebHostEnvironment _environment;
        private readonly IRepository<User> _userRepository;
        private TuanBuyContext _sqldb;

        public ProductController(GenericRepository<Product> productsRepository, IWebHostEnvironment environment, GenericRepository<User> userRepository, TuanBuyContext sqldb)
        {
            _productsRepository = productsRepository;
            _environment = environment;
            _userRepository = userRepository;
            _sqldb = sqldb;
        }
        //新增商品首頁
        [Authorize(Roles = "FullUser")]
        public IActionResult Index()
        {
            return View();
        }

        //商品介紹頁
        public IActionResult DemoProduct()
        {
            return View();           
        }

        #region 取得商品頁資料
        [HttpGet]
        public DemoProductViewModel GetProductData(int id)
        {
            ProductManage product = new ProductManage(_sqldb);
            var result = product.GetDemoProductData(id);
            return result;
        }
        #endregion

        #region 取得商品頁留言
        //public ProductMessage GetProductMessage(int id)
        //{
        //    ProductManage product = new ProductManage(_sqldb);
        //    var result = product.GetDemoProductData(id);
        //    return result;
        //}
        #endregion
        //等待開團商品頁
        [Authorize(Roles = "FullUser")]
        public IActionResult WaitingProduct()
        {
            return View();
        }
        //已開團商品頁
        [Authorize(Roles = "FullUser")]
        public IActionResult ReadyProduct()
        {
            return View();
        }
        //新增商品
        [HttpPost]
        [Authorize(Roles = "FullUser")]
        public IActionResult PostProduct(AddProductViewModel product)
        {
            var path = _environment.WebRootPath + "/ProductPicture";
            if (product.PicPath == null)
            {
                return BadRequest();
            }
            var pic = product.PicPath.FirstOrDefault();

            if (pic != null)
            {
                var fileName = DateTime.Now.Ticks.ToString() + pic.FileName;

                using (var fs = System.IO.File.Create($"{path}/{fileName}"))
                {
                    pic.CopyTo(fs);
                }
                
                var claim = HttpContext.User.Claims;
                var userEmail = claim.FirstOrDefault(a => a.Type == ClaimTypes.Email)?.Value;
                var targetUser = _userRepository.Get(a => a.Email == userEmail);
                
                _productsRepository.Create(new Product()
                {
                    Name = product.Name,
                    PicPath = fileName,
                    Content = product.Content,
                    Category = product.Category,
                    Description = product.Description,
                    CreateTime = DateTime.Now,
                    EndTime = product.EndTime,
                    Price = product.Price,
                    User = targetUser
                });
                return Ok();
            }
            else
            {
                return BadRequest();
            }
        }

        [Authorize(Roles = "FullUser")]
        public IActionResult MyProduct()
        {
            return View();
        }
    }
}
