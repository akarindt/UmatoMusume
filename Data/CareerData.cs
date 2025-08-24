using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UmatoMusume.Models;
using UmatoMusume.Utils;

namespace UmatoMusume.Data
{
    public static class CareerData
    {
        public static List<Dictionary<string, string>> GetCareerEvents(this List<Career> _careers, string _eventName)
        {
            var result = _careers
                .Where(x => x.EventName.Contains(_eventName))
                .Select(x => new Dictionary<string, string>(x.EventOptions))
                .Distinct(new DictionaryComparer());

            result = result.Any() ? result : _careers
                .Where(x => Helper.CheckRatio(x.EventName, _eventName))
                .Select(x => new Dictionary<string, string>(x.EventOptions))
                .Distinct(new DictionaryComparer());


            return result.ToList();
        }
    }
}
