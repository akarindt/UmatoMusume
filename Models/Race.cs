using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UmatoMusume.Models
{
    public class Race
    {
        [JsonProperty("RaceName")]
        public string RaceName { get; set; } = string.Empty;

        [JsonProperty("Schedule")]
        public string Schedule { get; set; } = string.Empty;

        [JsonProperty("Grade")]
        public string Grade { get; set; } = string.Empty;

        [JsonProperty("Terrain")]
        public string Terrain { get; set; } = string.Empty;

        [JsonProperty("DistanceType")]
        public string DistanceType { get; set; } = string.Empty;

        [JsonProperty("DistanceMeter")]
        public string DistanceMeter { get; set; } = string.Empty;

        [JsonProperty("Season")]
        public string Season { get; set; } = string.Empty;

        [JsonProperty("FansRequired")]
        public string FansRequired { get; set; } = string.Empty;

        [JsonProperty("FansGained")]
        public string FansGained { get; set; } = string.Empty;

        public Race() {}

        public Race(string _raceName, string _schedule ,string _grade, string _terrain, string _distanceType, string _distanceMeter, string _season, string _fansRequired, string _fansGained)
        {
            RaceName = _raceName.Trim();
            Schedule = _schedule.Trim();
            Grade = _grade.Trim();
            Terrain = _terrain.Trim();
            DistanceType = _distanceType.Trim();
            DistanceMeter = _distanceMeter.Trim();
            Season = _season.Trim();
            FansRequired = _fansRequired.Trim();
            FansGained = _fansGained.Trim();
        }
    }
}
