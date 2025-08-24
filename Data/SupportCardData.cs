using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UmatoMusume.Models;
using UmatoMusume.Utils;

namespace UmatoMusume.Data
{
    public static class SupportCardData
    {
        public static List<Dictionary<string, string>> GetSupportCardEventOptions(this List<SupportCard> _cards, string _eventName)
        {
            var result = _cards
                .Where(x => x.EventName.Contains(_eventName))
                .Select(x => new Dictionary<string, string>(x.EventOptions))
                .Distinct(new DictionaryComparer());

            result = result.Any() ? result : _cards
                .Where(x => Helper.CheckRatio(x.EventName, _eventName))
                .Select(x => new Dictionary<string, string>(x.EventOptions))
                .Distinct(new DictionaryComparer());

            return result.ToList();
        }
    }
}
