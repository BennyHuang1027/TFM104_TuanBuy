using System;
using System.Collections.Generic;

namespace TuanBuy.ViewModel
{
    public class ProductViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime EndTime { get; set; }
        public string Description { get; set; }
        public string Content { get; set; }
        public string PicPath { get; set; }
        public decimal Price { get; set; }
        public string Category { get; set; }
        public string Href { get; set; } = "";

    }

    public class DemoProductViewModel
    {
        //該產品Id
        public int Id { get; set; }
        //團主名稱
        public string Seller { get; set; }
        //團主會員ID
        public int SellerId { get; set; }
        //商品品名
        public string ProductTitle { get; set; }
        //商品種類
        public string ProductCategory { get; set; }
        //商品詳細敘述
        public string ProductDescription { get; set; }
        //商品簡述
        public string ProductSummary { get; set; }
        //產品目標金額
        public decimal ProductTargetPrice { get; set; }
        //產品開團時間
        public DateTime ProductStartTime { get; set; }
        //開團結束時間
        public DateTime ProductEndTime { get; set; }
        //產品剩餘天數
        public string ProductLastTime { get; set; }
        //加入團購人數
        public string Buyers { get; set;}
        //產品圖片
        public List<string> ProductPicPath { get; set; }
    }

    public class ProductMessage
    {
        //留言者頭像
        public string MessagePicPaht { get; set; }
        //留言者姓名
        public string MessageName { get; set; }
        //留言者內容
        public string MessageContent { get; set; }



    }

}