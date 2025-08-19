using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UmatoMusume.Models
{
    public class Career
    {
        [JsonProperty("EventName")]
        public string EventName { get; set; } = string.Empty;

        [JsonProperty("EventOptions")]
        public Dictionary<string, string> EventOptions { get; set; } = new Dictionary<string, string>();

        public Career() { }

        public Career(string _eventName, Dictionary<string, string> _eventOptions)
        {
            EventName = _eventName;
            EventOptions = _eventOptions;
        }
    }
}
