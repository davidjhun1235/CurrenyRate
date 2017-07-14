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
using System.Configuration;

namespace CurrenyRate
{
    class Program
    {
        static void Main(string[] args)
        {
            string Year = DateTime.Now.AddMonths(-1).Year.ToString();
            string Month = DateTime.Now.AddMonths(-1).Month.ToString();
            string date = Year + "-" + ("0" + Month).Substring(0, 2);

            
            //*****新增幣別放這****//
            string[] cur = new string [] { "USD", "HKD", "JPY", "KRW" };

            decimal[] rate = new decimal[100];
            GetCurrencyRate CR;

            for (int i = 0; i < cur.Length; i++)
            {
                CR = new GetCurrencyRate(date, cur[i]);
                //***取匯率**
                rate[i] = CR.GetRate();
            }

            SqlConnection sqlc = new SqlConnection();
            sqlc.ConnectionString = ConfigurationManager.AppSettings["DB"];
            /*
            sqlc.ConnectionString = @"Persist Security Info=False;Integrated Security=true;
                            Initial Catalog = budget; Server = GHQDB2\MSSQL2; user id = budget; password = KMn32SRV";
            */
            //Initial Catalog = budget; Server = reportcenter; user id = sa; password = 1qaz@WSX3edc";

            sqlc.Open();

            for (int i = 0; i < cur.Length; i++)
            {
                //****塞資料庫****
                insertDB(sqlc, cur[i] ,rate[i],Year,Month);
            }
            sqlc.Close();
          
        }

        public static void insertDB(SqlConnection sqlc, string currency, decimal rate,string year,string month)
        {
            SqlCommand comm = new SqlCommand();
            comm.Connection = sqlc;
            SqlTransaction transaction;
            transaction = sqlc.BeginTransaction("CurrencyRateTransaction");
            comm.Transaction = transaction;

            try
            {
                //***先刪再塞****
                comm.CommandText = string.Format("delete tbl_CurrencyInfo where Year ={0} and Month ={1} and Currency='{2}' and [Type] = 'ACTUAL'", year, month, currency);
                comm.ExecuteNonQuery();
                comm.CommandText = string.Format("insert tbl_CurrencyInfo values ('{0}','{1}','{2}','{3}','ACTUAL') ", year, month, currency, rate);
                comm.ExecuteNonQuery();
                transaction.Commit();
                
            }
            catch (Exception e)
            {
                transaction.Rollback();
                throw e;
            }

        }
    }
}
