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
        public static List<Race> GetRaces(this List<Race> _races, string _dateTime, List<string> _grades)
        {
            if (_grades.Any())
            {
                return _races.DistinctBy(x => x.RaceName)
                    .Where(x => x.Schedule.Equals(_dateTime) || Helper.CheckRatio(x.Schedule, _dateTime))
                    .Where(x => _grades.Contains(x.Grade))
                    .ToList();
            }

            return _races
                .DistinctBy(x => x.RaceName)
                .Where(x => x.Schedule.Equals(_dateTime) || Helper.CheckRatio(x.Schedule, _dateTime))
                .ToList();
        }

        public static List<string> GetRaceGrades(this List<Race> _races)
        {
            return _races
                .Select(x => x.Grade)
                .Distinct()
                .Where(x => !string.IsNullOrEmpty(x))
                .OrderBy(x => x)
                .ToList();
        }

        public static List<string> GetRaceTerrains(this List<Race> _races)
        {
            return _races
                .Select(x => x.Terrain)
                .Distinct()
                .Where(x => !string.IsNullOrEmpty(x))
                .OrderBy(x => x)
                .ToList();

        }

        public static List<string> GetRaceDistanceTypes(this List<Race> _races)
        {
            return _races
                .Select(x => x.DistanceType)
                .Distinct()
                .Where(x => !string.IsNullOrEmpty(x))
                .OrderBy(x => x)
                .ToList();
        }
    }
}
