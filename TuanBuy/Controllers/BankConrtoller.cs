﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TuanBuy.Models.Bank;
using TuanBuy.Models.Bank.Utlity;

namespace TuanBuy.Controllers
{
    public class BankController : Controller
    {
        /// <summary>
        /// 金流基本資料(可再移到Web.config或資料庫設定)
        /// </summary>
        private BankInfoModel _bankInfoModel = new BankInfoModel
        {
            MerchantID = "MS134170605",
            HashKey = "BwxEe1eql1HJPqI2SQoiQDzaJHhFwtJb",
            HashIV = "CVaUnLTCPBSAcSrP",
            ReturnURL = "http://yourWebsitUrl/Bank/SpgatewayReturn",
            NotifyURL = "http://yourWebsitUrl/Bank/SpgatewayNotify",
            CustomerURL = "http://yourWebsitUrl/Bank/SpgatewayCustomer",
            AuthUrl = "https://ccore.spgateway.com/MPG/mpg_gateway",
            CloseUrl = "https://core.newebpay.com/API/CreditCard/Close"
        };

        /// <summary>
        /// 付款頁面
        /// </summary>
        /// <returns></returns>
        public ActionResult PayBill()
        {
            return View();
        }

        /// <summary>
        /// [智付通支付]金流介接
        /// </summary>
        /// <param name="ordernumber">訂單單號</param>
        /// <param name="amount">訂單金額</param>
        /// <param name="payType">請款類型</param>
        /// <returns></returns>
        /// 

        //金流只在意付款方式、價格、訂單編號
        [HttpPost]
        public async Task SpgatewayPayBillAsync(string ordernumber, int amount, string PayMethod)
        {
            string version = "2.0";
            ordernumber = "111";
            amount = 500;
            PayMethod = "creditcard";

            TradeInfo tradeInfo = new TradeInfo()
            {
                // * 商店代號
                MerchantID = _bankInfoModel.MerchantID,
                // * 回傳格式
                RespondType = "String",
                // * TimeStamp
                // 目前時間轉換 +08:00, 防止傳入時間或Server時間時區不同造成錯誤
                TimeStamp = DateTimeOffset.Now.ToOffset(new TimeSpan(8, 0, 0)).ToUnixTimeSeconds().ToString(),
                // * 串接程式版本
                Version = version,
                // * 商店訂單編號
                //MerchantOrderNo = $"T{DateTime.Now.ToString("yyyyMMddHHmm")}",
                MerchantOrderNo = ordernumber,
                // * 訂單金額
                Amt = amount,
                // * 商品資訊
                ItemDesc = "商品資訊(自行修改)",
                // 繳費有效期限(適用於非即時交易)
                ExpireDate = null,
                // 支付完成 返回商店網址
                //ReturnURL = _bankInfoModel.ReturnURL,
                ReturnURL = null,
                // 支付通知網址
                NotifyURL = _bankInfoModel.NotifyURL,
                // 商店取號網址
                CustomerURL = _bankInfoModel.CustomerURL,
                // 支付取消 返回商店網址
                ClientBackURL = null,
                // * 付款人電子信箱
                Email = string.Empty,
                // 付款人電子信箱 是否開放修改(1=可修改 0=不可修改)
                EmailModify = 0,
                // 商店備註
                OrderComment = null,
                // 信用卡 一次付清啟用(1=啟用、0或者未有此參數=不啟用)
                CREDIT = null,
                // WEBATM啟用(1=啟用、0或者未有此參數，即代表不開啟)
                WEBATM = null,
                // ATM 轉帳啟用(1=啟用、0或者未有此參數，即代表不開啟)
                VACC = null,
                // 超商代碼繳費啟用(1=啟用、0或者未有此參數，即代表不開啟)(當該筆訂單金額小於 30 元或超過 2 萬元時，即使此參數設定為啟用，MPG 付款頁面仍不會顯示此支付方式選項。)
                CVS = null,
                // 超商條碼繳費啟用(1=啟用、0或者未有此參數，即代表不開啟)(當該筆訂單金額小於 20 元或超過 4 萬元時，即使此參數設定為啟用，MPG 付款頁面仍不會顯示此支付方式選項。)
                BARCODE = null,
                LINEPAY = null
            };

            if (PayMethod == "creditcard")
            {
                tradeInfo.CREDIT = 1;
            }
            else if (PayMethod == "linePay")
            {
                tradeInfo.LINEPAY = 1;
            }

            Atom<string> result = new Atom<string>()
            {
                IsSuccess = true
            };

            var inputModel = new SpgatewayInputModel
            {
                MerchantID = _bankInfoModel.MerchantID,
                Version = version
            };

            // 將model 轉換為List<KeyValuePair<string, string>>, null值不轉
            List<KeyValuePair<string, string>> tradeData = LambdaUtil.ModelToKeyValuePairList<TradeInfo>(tradeInfo);
            // 將List<KeyValuePair<string, string>> 轉換為 key1=Value1&key2=Value2&key3=Value3...
            var tradeQueryPara = string.Join("&", tradeData.Select(x => $"{x.Key}={x.Value}"));
            tradeQueryPara = tradeQueryPara + "&SAMSUNGPAY=1&ANDROIDPAY=1";
            // AES 加密
            inputModel.TradeInfo = CryptoUtil.EncryptAESHex(tradeQueryPara, _bankInfoModel.HashKey, _bankInfoModel.HashIV);
            // SHA256 加密
            inputModel.TradeSha = CryptoUtil.EncryptSHA256($"HashKey={_bankInfoModel.HashKey}&{inputModel.TradeInfo}&HashIV={_bankInfoModel.HashIV}");

            // 將model 轉換為List<KeyValuePair<string, string>>, null值不轉
            List<KeyValuePair<string, string>> postData = LambdaUtil.ModelToKeyValuePairList<SpgatewayInputModel>(inputModel);

            StringBuilder s = new StringBuilder();
            s.Append("<html>");
            s.AppendFormat("<body onload='document.forms[\"form\"].submit()'>");
            s.AppendFormat("<form name='form' action='{0}' method='post'>", _bankInfoModel.AuthUrl);
            foreach (KeyValuePair<string, string> item in postData)
            {
                s.AppendFormat("<input type='hidden' name='{0}' value='{1}' />", item.Key, item.Value);
            }
            s.Append("</form></body></html>");


            Response.ContentType = "text/html";
            //using (var sw = new StreamWriter(Response.Body))
            //{
            //   await sw.WriteAsync(s.ToString());
            //}
            var bytes = Encoding.UTF8.GetBytes(s.ToString());
            await Response.Body.WriteAsync(bytes, 0, bytes.Length);
        }



        /// <summary>
        /// 銀行API測試
        /// </summary>
        /// <returns></returns>
        public ActionResult RefundTest()
        {
            return View();
        }


    }

}