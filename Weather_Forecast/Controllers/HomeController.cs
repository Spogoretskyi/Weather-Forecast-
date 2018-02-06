using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using Newtonsoft.Json;
using System.Web.Mvc;
using Weather_Forecast.CurrentForecast;
using System.Globalization;
using Weather_Forecast.Forecast;

namespace Weather_Forecast.Controllers
{
    public class HomeController : Controller
    {

        public ActionResult Index(string city = "Kyiv")
        {
            //call API
            OpenWeather _weatherCurrent = CurrentWeather(city);
            RootObject _weatherForecast = WeatherForecast(city, _weatherCurrent.sys.country);

            ViewData["city"] = _weatherCurrent.name;
            ViewData["country"] = _weatherCurrent.sys.country;
            //make path full path
            ViewData["icon"] = "/Content/Img/" + _weatherCurrent.weather[0].icon + ".png";
            //convert to time
            ViewData["sunrise"] = SecToTime(_weatherCurrent.sys.sunrise);
            ViewData["sunset"] = SecToTime(_weatherCurrent.sys.sunset);
            ViewData["temp"] = Math.Round(_weatherCurrent.main.temp, 1);
            ViewData["pressure"] = _weatherCurrent.main.pressure;
            ViewData["humidity"] = _weatherCurrent.main.humidity;
            ViewData["temp_min"] = _weatherCurrent.main.temp_min;
            ViewData["temp_max"] = _weatherCurrent.main.temp_max;
            ViewData["weather"] = _weatherCurrent.weather[0].description;
            ViewData["weather_main"] = _weatherCurrent.weather[0].main;
            ViewData["wind"] = _weatherCurrent.wind.speed;
            //convert degrees
            ViewData["wind_deg_dir"] = DegreesToDiraction(_weatherCurrent.wind.deg);
            ViewData["wind_deg"] = _weatherCurrent.wind.deg;
            ViewData["cloudiness"] = _weatherCurrent.clouds.all;
            //current time 
            ViewData["time"] = DateTime.Now.ToString("HH:mm MMM d",
                        CultureInfo.CreateSpecificCulture("en-US"));
            //make full path, temp round and convert time for view in th list
            foreach (var forecast in _weatherForecast.list)
            {
                forecast.main.temp_min = Math.Round(forecast.main.temp_min, 1);
                forecast.main.temp_max = Math.Round(forecast.main.temp_max, 1);
                forecast.weather[0].icon = "/Content/Img/" + forecast.weather[0].icon + ".png";
                forecast.dt_txt = DateTime.Parse(forecast.dt_txt).ToString("HH:mm MMM d", 
                    CultureInfo.CreateSpecificCulture("en-US"));
            }

            return View(_weatherForecast.list);
        }

        private OpenWeather CurrentWeather(string city)
        {
            string _strCurrent = $"http://api.openweathermap.org/data/2.5/weather?q={city}&appid=a4e0e1aff9d7a30ddb0dba92c9fb2c6a&units=metric";
            string _answerCurrent = String.Empty;
            // Get current weather(object) from API
            HttpWebRequest _requestCurrent = WebRequest.Create(_strCurrent) as HttpWebRequest;
            try
            {
                using (HttpWebResponse response = _requestCurrent.GetResponse() as HttpWebResponse)
                {
                    StreamReader reader = new StreamReader(response.GetResponseStream());
                    _answerCurrent = reader.ReadToEnd();
                }
                // Deserialize object
                OpenWeather _jsonObjectCurrent = JsonConvert.DeserializeObject<OpenWeather>(_answerCurrent);
                return _jsonObjectCurrent;
            }
            catch (JsonException ex)
            {
                // if user input incorect city -> 404
                return new OpenWeather();
            }
        }
        private RootObject WeatherForecast(string city, string country)
        {
            string _strForecast = $"http://api.openweathermap.org/data/2.5/forecast?q={city},{country}&units=metric&appid=a4e0e1aff9d7a30ddb0dba92c9fb2c6a";
            string _answerForecast = String.Empty;
            // Get weather forecast(objects) from API
            HttpWebRequest _requestForecast = WebRequest.Create(_strForecast) as HttpWebRequest;
            try
            {
                using (HttpWebResponse response = _requestForecast.GetResponse() as HttpWebResponse)
                {
                    StreamReader reader = new StreamReader(response.GetResponseStream());
                    _answerForecast = reader.ReadToEnd();
                }
                // Deserialize objects
                var _jsonObjectForecast = JsonConvert.DeserializeObject<RootObject>(_answerForecast);
                return _jsonObjectForecast;
            }
            catch (JsonException ex)
            {
                // if user input incorect city -> 404
                return new RootObject();
            }
        }
        // Convert from degrees to diracrions
        public static string DegreesToDiraction(int degrees)
        {
            string[] diraction = { "N", "NE", "E", "SE", "S", "SW", "W", "NW", "N" };
            return diraction[(int)Math.Round(((double)degrees % 360) / 45)];
        }
        // Convert from seconds to time
        public static string SecToTime(int sec)
        {
            TimeSpan time = TimeSpan.FromSeconds(sec);
            return time.ToString(@"hh\:mm");
        }
    }
}