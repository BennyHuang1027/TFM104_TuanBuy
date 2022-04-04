using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TuanBuy.ViewModel;

namespace TuanBuy.Models.Entities
{
    public class ProductManage
    {
        private TuanBuyContext _dbContext;
        public ProductManage(TuanBuyContext dbContext)
        {
            _dbContext = dbContext;
        }

        public DemoProductViewModel GetDemoProductData(int ProductId)
        {
            //群組join
            var result = _dbContext.Product.ToList().GroupJoin(
                    _dbContext.ProductPics.ToList(),
                    prd => prd.Id,
                    pit => pit.ProductId,
                    (product,productimg)=> new {product}                
                ).ToList().GroupJoin(_dbContext.User,
                    prd => prd.product.UserId,
                    user => user.Id,
                    (prd,user) => new { prd}
                ).ToList().GroupJoin(_dbContext.Order,
                    user => user.prd.product.Id,
                    order=> order.ProductId,
                    (user,order) => new {user}                
                );
            DemoProductViewModel demoProductViewModel = new DemoProductViewModel();
            List<string> productPicPath = new List<string>();
            foreach (var item in result)
            {
                demoProductViewModel.Id = item.user.prd.product.Id;
                demoProductViewModel.ProductTitle = item.user.prd.product.Name;
                demoProductViewModel.ProductSummary = item.user.prd.product.Content;
                demoProductViewModel.ProductCategory = item.user.prd.product.Category;
                demoProductViewModel.ProductDescription = item.user.prd.product.Description;
                demoProductViewModel.ProductTargetPrice = item.user.prd.product.Price;
                demoProductViewModel.ProductStartTime = item.user.prd.product.CreateTime;
                demoProductViewModel.ProductEndTime = item.user.prd.product.EndTime;
                //開團日期及結束日期差計算
                TimeSpan timeSpan = item.user.prd.product.EndTime.Subtract(item.user.prd.product.CreateTime).Duration();
                demoProductViewModel.ProductLastTime = timeSpan.Days.ToString() + "天" + timeSpan.Hours.ToString() + "小時" + timeSpan.Minutes.ToString() + "分鐘";
                //賣家開團主相關資訊
                demoProductViewModel.SellerId = item.user.prd.product.User.Id;
                demoProductViewModel.Seller = item.user.prd.product.User.NickName;
                //目前團購已訂購人數
                if(item.user.prd.product.Order !=null)
                {
                    demoProductViewModel.Buyers = item.user.prd.product.Order.Count.ToString();
                }
                foreach (var picpath in item.user.prd.product.ProductPics)
                {
                    productPicPath.Add($"/ProductPicture/{picpath.PicPath}");
                }
                demoProductViewModel.ProductPicPath = productPicPath;
            }
            return demoProductViewModel;
        }

    }
}
