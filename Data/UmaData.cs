using UmatoMusume.Models;
using UmatoMusume.Utils;

namespace UmatoMusume.Data
{
    public static class UmaData
    {
        public static List<UmaObjective> GetUmaObjectives(this List<Umamusume> _umas, string _umaName)
        {
            return _umas.Where(x => x.UmaName.Contains(_umaName))
                .SelectMany(x => x.UmaObjectives)
                .DistinctBy(x => (x.ObjectiveName, x.Turn, x.ObjectiveCondition, x.Time))
                .ToList();
        }

        public static List<Dictionary<string, string>> GetUmaEventOptions(this List<Umamusume> _umas, string _umaName, string _eventName)
        {
            var result = _umas.Where(x => x.UmaName.Contains(_umaName))
                .SelectMany(x => x.UmaEvents)
                .Where(e => e.EventName.Contains(_eventName))
                .Select(e => new Dictionary<string, string>(e.EventOptions))
                .Distinct(new DictionaryComparer());

            result = result.Any() ? result : _umas.Where(x => x.UmaName.Contains(_umaName))
                .SelectMany(x => x.UmaEvents)
                .Where(e => Helper.CheckRatio(e.EventName, _eventName))
                .Select(e => new Dictionary<string, string>(e.EventOptions))
                .Distinct(new DictionaryComparer());

            return result.ToList();
        }
    }
}
