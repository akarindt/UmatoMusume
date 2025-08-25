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
        public static List<Race> GetRaces(this List<Race> _races, string _dateTime, List<string> _grades, List<string> _distanceTypes, List<string> _terrainTypes)
        {
            var result = _races
                .CompareWithFallback("Schedule", _dateTime)
                .DistinctBy(x => x.RaceName)
                .ListPredicate(_grades, x => _grades.Contains(x.Grade))
                .ListPredicate(_distanceTypes, x => _distanceTypes.Contains(x.DistanceType))
                .ListPredicate(_terrainTypes, x => _terrainTypes.Contains(x.Terrain))
                .ToList();

            return result;
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
