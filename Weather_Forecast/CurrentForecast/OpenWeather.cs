using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;

namespace Weather_Forecast.CurrentForecast
{
    public class OpenWeather
    {
        public Coord coord { get; set; }
        public Weather[] weather { get; set; }
        //to lower case:  
        [JsonProperty("base")]
        public string Base;
        public Main main { get; set; }
        public Wind wind { get; set; }
        public Clouds clouds { get; set; }
        public int dt;
        public Sys sys { get; set; }
        public int id;
        public string name;
        public int cod;
    }
}