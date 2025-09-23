using UmatoMusume.Models;
using UmatoMusume.Utils;

namespace UmatoMusume.Data
{
	public static class CareerData
	{
		public static List<Dictionary<string, string>> GetCareerEvents(this List<Career> _careers, string _eventName)
		{
			var result = _careers
				.CompareWithFallback("EventName", _eventName)
				.Select(x => new Dictionary<string, string>(x.EventOptions))
				.Distinct(new DictionaryComparer())
				.ToList();


			return result;
		}
	}
}
