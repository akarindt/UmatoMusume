using UmatoMusume.Models;
using UmatoMusume.Utils;

namespace UmatoMusume.Data
{
    public static class UmaData
    {
        public static List<UmaObjective> GetUmaObjectives(this List<Umamusume> _umas, string _umaName)
        {
            return _umas.Where(x => x.UmaName.Equals(_umaName))
                .SelectMany(x => x.UmaObjectives)
                .DistinctBy(x => (x.ObjectiveName, x.Turn, x.ObjectiveCondition, x.Time))
                .ToList();
        }

        public static List<Dictionary<string, string>> GetUmaEventOptions(this List<Umamusume> _umas, string _umaName, string _eventName)
        {
            var result = _umas.Where(x => x.UmaName.Equals(_umaName))
                .SelectMany(x => x.UmaEvents)
                .CompareWithFallback("EventName", _eventName)
                .Select(e => new Dictionary<string, string>(e.EventOptions))
                .Distinct(new DictionaryComparer())
                .ToList();

            return result;
        }
    }
}
