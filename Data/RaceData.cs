using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UmatoMusume.Models;
using UmatoMusume.Utils;

namespace UmatoMusume.Data
{
    public static class RaceData
    {
        public static List<Race> GetRaces(this List<Race> _races, string _dateTime)
        {
            return _races
                .DistinctBy(x => x.RaceName)
                .Where(x => x.Schedule.Equals(_dateTime) || Helper.CheckRatio(x.Schedule, _dateTime))
                .ToList();
        }
    }
}
