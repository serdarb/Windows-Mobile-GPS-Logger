using System;
using System.Configuration;
using System.Data;
using System.Data.OracleClient;
using System.IO;
using System.Text;
using System.Web;
using System.Web.Services;

namespace WindowsMobileGPSLogger.WebUI
{
    /// <summary>
    /// Summary description for $codebehindclassname$
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    public class GetPositions : IHttpHandler
    {
        public void ProcessRequest(HttpContext context)
        {
            context.Response.Clear();
            context.Response.Cache.SetCacheability(HttpCacheability.Public);
            context.Response.Cache.SetExpires(DateTime.MinValue);

            context.Response.ContentType = "text/plain";
            context.Response.ContentEncoding = Encoding.UTF8;



            StringBuilder sb = new StringBuilder();

            try
            {
                string device = string.Format("{0}", context.Request.QueryString["device"]).Trim();

                OracleParameter[] prms = new OracleParameter[1];
                prms[0] = new OracleParameter("DEVICEID", device);

                string cmdText =
                    @"select distinct x,y,positiontime
                                from gps_log 
                                where DEVICEID = :DEVICEID 
                                        and to_number(x) <> 0 and to_number(y) <> 0
                                order by positiontime";

                DataTable dt = new DataTable();
                using (
                    OracleConnection con =
                        new OracleConnection(ConfigurationManager.ConnectionStrings["cnnStr"].ConnectionString))
                {
                    using (OracleDataAdapter da = new OracleDataAdapter(cmdText, con))
                    {
                        da.SelectCommand.Parameters.AddRange(prms);
                        da.Fill(dt);
                    }
                }

                foreach (DataRow myRow in dt.Rows)
                {
                    //x-y-positiontime#
                    sb.AppendFormat("{0}-{1}-{2}#",
                                    myRow[1].ToString().Replace(",", "."),
                                    myRow[0].ToString().Replace(",", "."),
                                    myRow[2].ToString().Replace(",", "."));
                }

                context.Response.Write(sb.ToString().Substring(0, sb.ToString().Length - 1));
            }
            catch (Exception ex)
            {
                LogException(ex, HttpContext.Current.Server.MapPath("Exception.txt"));
            }
        }

        private static void LogException(Exception ex, string filePath)
        {
            using (StreamWriter sw = new StreamWriter(filePath, true))
            {
                sw.WriteLine(String.Format("{1}{0}{2}{0}{3}{0}",
                                       Environment.NewLine,
                                       ex.Message,
                                       "-".PadRight(30, '-'),
                                       ex.StackTrace));
            }
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}
