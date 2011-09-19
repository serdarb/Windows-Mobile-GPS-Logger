using System;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.OracleClient;
using System.IO;
using System.Web.Services;

namespace WindowsMobileGPSLogger.Service
{
    /// <summary>
    /// Summary description for Logger
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class Logger : System.Web.Services.WebService
    {
        [WebMethod]
        public bool Log(string deviceId, string x, string y, string time)
        {
            //you should think about a ddos attack for this method

            bool result = false;
            try
            {
                OracleParameter[] prms = new OracleParameter[4];
                prms[0] = new OracleParameter("DEVICEID", deviceId);
                prms[1] = new OracleParameter("X", x);
                prms[2] = new OracleParameter("Y", y);
                prms[3] = new OracleParameter("POSITIONTIME", time);

                using (OracleConnection cnn = new OracleConnection(ConfigurationManager.ConnectionStrings["cnnStr"].ConnectionString))
                {
                    //You must have a table named GPSLOGS
                    //With these columns DEVICEID,X,Y,POSITIONTIME
                    using (OracleCommand cmd = new OracleCommand("INSERT INTO GPSLOGS (DEVICEID,X,Y,POSITIONTIME) VALUES (:DEVICEID,:X,:Y,:POSITIONTIME)", cnn))
                    {
                        try
                        {
                            cmd.Parameters.AddRange(prms);
                            cmd.CommandType = CommandType.Text;

                            cnn.Open();
                            result = cmd.ExecuteNonQuery() != 0;
                        }
                        catch (Exception ex)
                        {
                            LogException(ex, Server.MapPath("Exception.txt"));
                        }
                        finally { cnn.Close(); }
                    }
                }
            }
            catch (Exception ex)
            {
                LogException(ex, Server.MapPath("Exception.txt"));
            }
            return result;
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
    }
}
