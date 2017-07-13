using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using HtmlAgilityPack;
using System.Net;
using System.IO;
using System.Data;
using System.Data.SqlClient;

namespace CurrenyRate
{
    public class GetCurrencyRate
    {
        string webUrl;
        string Currency;
        WebClient url = new WebClient();
        decimal Rate;
        MemoryStream ms;
        HtmlDocument doc;
        HtmlDocument hdc;
        HtmlNodeCollection htnode;

        string d1 = string.Empty;
        string d2 = string.Empty;

        double[] RateIn ;
        double[] RateOut ;

        public GetCurrencyRate(string date,string currency)
        {
            webUrl = @"http://rate.bot.com.tw/xrt/quote/" + date + "/" + currency;
            Currency = currency;
        }

        public decimal GetRate()
        {
            try
            {
                ms = new MemoryStream(url.DownloadData(webUrl));
            }
            catch (Exception e)
            {
                throw e;
            }

            doc = new HtmlDocument();
            doc.Load(ms, Encoding.UTF8);

            hdc = new HtmlDocument();
            hdc.LoadHtml(doc.DocumentNode.SelectSingleNode("/html[1]/body[1]/div[1]/main[1]/div[4]/table[1]/tbody[1]").InnerHtml);
            htnode = hdc.DocumentNode.SelectNodes("./tr");

            RateIn = new double[htnode.Count];
            RateOut = new double[htnode.Count];

            for (int a = 0; a < htnode.Count; a++)
            {
                if (Currency != "KRW")
                {
                    d1 = string.Format("./tr[{0}]/td[5]", a + 1);
                    d2 = string.Format("./tr[{0}]/td[6]", a + 1);
                }
                else //韓元沒即期匯率，所以抓現金匯率
                {
                    d1 = string.Format("./tr[{0}]/td[3]", a + 1);
                    d2 = string.Format("./tr[{0}]/td[4]", a + 1);
                }
                RateIn[a] = Convert.ToDouble(hdc.DocumentNode.SelectSingleNode(d1).InnerText);
                RateOut[a] = Convert.ToDouble(hdc.DocumentNode.SelectSingleNode(d2).InnerText);
            }

            Rate = Convert.ToDecimal((RateIn.Sum() + RateOut.Sum()) / (htnode.Count * 2));
            doc = null;
            hdc = null;
            url = null;
            ms.Close();
            return Rate;
        }
        
    }
}
