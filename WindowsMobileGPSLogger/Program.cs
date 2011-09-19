using System.Net;
using System.Threading;
using Microsoft.WindowsMobile.Samples.Location;
using WindowsMobileGPSLogger.GPSLogger;

namespace WindowsMobileGPSLogger
{
    class Program
    {
        static Gps gps = new Gps();
        static GpsPosition position = null;

        static void Main(string[] args)
        {
            if (!gps.Opened)
            {
                gps.Open();
            }

            gps.LocationChanged += new LocationChangedEventHandler(gps_LocationChanged);

            Timer timer30sec = new Timer(TimerWork, null, 0, 30000);
        }

        private static void TimerWork(object state)
        {
            if (position != null)
            {
                string gpsTime = position.TimeValid ? position.Time.ToString("dd.MM.yyyy HH:mm:ss") : string.Empty;
                string x = position.Longitude.ToString();
                string y = position.Latitude.ToString();

                LogMyPosition(gpsTime, x, y);
            }
        }

        private static void LogMyPosition(string gpsTime, string x, string y)
        {
            const string deviceId = "MyDevice";
            Logger loggerService = new Logger { Proxy = new WebProxy() };
            loggerService.Log(deviceId, x, y, gpsTime);
        }

        static void gps_LocationChanged(object sender, LocationChangedEventArgs args)
        {
            position = args.Position;
        }
    }
}
