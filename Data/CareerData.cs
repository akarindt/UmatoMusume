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
            return _careers
                .Where(x => x.EventName.Contains(_eventName) || Helper.CheckRatio(x.EventName, _eventName))
                .Select(x => new Dictionary<string, string>(x.EventOptions))
                .Distinct(new DictionaryComparer())
                .ToList();
        }
    }
}
