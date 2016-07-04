using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Business_Logic.Helpers
{
    public class MapHelper
    {
        const double EarthRadiusInKilometers = 6372.797560856;
        /* DegreToRadian = PI/180 */
        const double DegreToRadian = 0.017453292519943295769236907684886;
        /* RadianToDegre = 1/DegreToRadian */
        const double RadianToDegre = 57.295779513082320876798154814105;

        const double DefaultLat = 32.086368;
        const double DefaultLng = 34.889135;
        const int DefaultZoom = 12;

        public static Double CenterLat
        {
            get { return ReadCenterCoords()[0]; }
            set
            {
                SettingsHelper.SetSettingValue("Map", "StartPoint",
                  string.Format("lat{0};lng{1}", value, CenterLng));
            }
        }

        public static Double CenterLng
        {
            get { return ReadCenterCoords()[1]; }
            set
            {
                SettingsHelper.SetSettingValue("Map", "StartPoint",
                  string.Format("lat{0};lng{1}", CenterLat, value));
            }
        }

        public static int Zoom
        {
            get
            {
                var strValue = SettingsHelper.GetSettingValue("Map", "Zoom");
                if (string.IsNullOrEmpty(strValue))
                {
                    SettingsHelper.SetSettingValue("Map", "Zoom", DefaultZoom.ToString());
                    return DefaultZoom;
                }
                else
                {
                    return Int32.Parse(strValue);
                }
            }
            set
            {
                SettingsHelper.SetSettingValue("Map", "Zoom", value.ToString());
            }
        }

        public static List<int> HiddenLines
        {
            get
            {
                var hiddenLines = SettingsHelper.GetSettingValue("Map", "HiddenLines");
                if(hiddenLines!=null)
                    return JsonConvert.DeserializeObject<List<int>>(hiddenLines);
                else 
                    return new List<int>();
            }
            set
            {
                SettingsHelper.SetSettingValue("Map", "HiddenLines",
                    value == null ? "[]" : JsonConvert.SerializeObject(value));
            }
        }

        public static List<int> HiddenStations
        {
            get
            {
                var hiddenStations = SettingsHelper.GetSettingValue("Map", "HiddenStations");
                if (hiddenStations != null)
                    return JsonConvert.DeserializeObject<List<int>>(hiddenStations);
                else
                    return new List<int>();
            }
            set
            {
                SettingsHelper.SetSettingValue("Map", "HiddenStations", value == null ? "[]" : JsonConvert.SerializeObject(value));
            }
        }

        public static List<int> HiddenStudents
        {
            get
            {
                var hiddenStudents = SettingsHelper.GetSettingValue("Map", "HiddenStudents");
                if (hiddenStudents != null)
                    return JsonConvert.DeserializeObject<List<int>>(hiddenStudents);
                else
                    return new List<int>();
            }
            set
            {
                SettingsHelper.SetSettingValue("Map", "HiddenStudents", value == null ? "[]" : JsonConvert.SerializeObject(value));
            }
        }

        public static bool ShowStationsWithoutLine
        {
            get
            {
                bool res;
                if (bool.TryParse(SettingsHelper.GetSettingValue("Map", "ShowStationsWithoutLine"), out res))
                    return res;
                return true;
            }
            set
            {
                SettingsHelper.SetSettingValue("Map", "ShowStationsWithoutLine", value.ToString());
            }
        }
        public static string ApiKey
        {
            get { return System.Web.Configuration.WebConfigurationManager.AppSettings["GoogleMapsKey"]; }
        }

        private static double[] ReadCenterCoords()
        {

            var strValue = SettingsHelper.GetSettingValue("Map", "StartPoint");
            if (String.IsNullOrEmpty(strValue))
            {
                SettingsHelper.SetSettingValue("Map", "StartPoint",
                    string.Format("lat{0};lng{1}", DefaultLat, DefaultLng));
                return new double[] { DefaultLat, DefaultLng };
            }
            else
            {
                var strParts = strValue.Split(';');
                return new double[]
                {
                    double.Parse(StringHelper.FixDecimalSeparator( strParts[0].Substring(3))),
                    double.Parse(StringHelper.FixDecimalSeparator(strParts[1].Substring(3)))
                };
            }
        }



        public static bool IsPointInCircle(double latPoint, double lonPoint, double latCenterCircle, double lonCenterCircle, double radius)
        {
            // set radius in kilometers

            var lon1 = lonCenterCircle - radius / Math.Abs((double)(Math.Cos((double)(DegreToRadian * latCenterCircle)) * 111.0));
            var lon2 = lonCenterCircle + radius / Math.Abs((double)(Math.Cos((double)(DegreToRadian * latCenterCircle)) * 111.0));
            var lat1 = latCenterCircle - (radius / 111.0);
            var lat2 = latCenterCircle + (radius / 111.0);

            if (lonPoint > lon1 && lonPoint < lon2 && latPoint > lat1 && latPoint < lat2)
                return true;
            else return false;
        }

        public static double DistanceBetweenCoordinates(double latPoint1, double lonPoint1, double latPoint2, double lonPoint2)
        {
            var dTheta = (latPoint1 - latPoint2) * DegreToRadian;
            var dLambda = (lonPoint1 - lonPoint2) * DegreToRadian;
            var mean_t = (latPoint1 + latPoint2) * DegreToRadian / 2.0;
            var cos_mean_t = Math.Cos(mean_t);

            // return distance in kilometers
            return EarthRadiusInKilometers * Math.Sqrt(dTheta * dTheta + cos_mean_t * cos_mean_t * dLambda * dLambda);
        }
    }
}