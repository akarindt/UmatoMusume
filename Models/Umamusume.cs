using Newtonsoft.Json;

namespace UmatoMusume.Models
{
	public class Umamusume
	{
		[JsonProperty("UmaName")]
		public string UmaName { get; set; } = string.Empty;

		[JsonProperty("UmaObjectives")]
		public List<UmaObjective> UmaObjectives { get; set; } = new List<UmaObjective>();

		[JsonProperty("UmaEvents")]
		public List<UmaEvent> UmaEvents { get; set; } = new List<UmaEvent>();

		public Umamusume() { }

		public Umamusume(string _name, List<UmaObjective> _umaObjectives, List<UmaEvent> _umaEvents)
		{

			UmaName = _name;
			UmaObjectives = _umaObjectives;
			UmaEvents = _umaEvents;
		}

	}

	public class UmaObjective
	{

		[JsonProperty("ObjectiveName")]
		public string ObjectiveName { get; set; } = string.Empty;

		[JsonProperty("Turn")]
		public string Turn { get; set; } = string.Empty;

		[JsonProperty("Time")]
		public string Time { get; set; } = string.Empty;

		[JsonProperty("ObjectiveCondition")]
		public string ObjectiveCondition { get; set; } = string.Empty;

		public UmaObjective() { }

		public UmaObjective(string _objectiveName, string _turn, string _time, string _objectiveCondition)
		{
			ObjectiveName = _objectiveName;
			Turn = _turn;
			Time = _time;
			ObjectiveCondition = _objectiveCondition;
		}
	}

	public class UmaEvent
	{
		[JsonProperty("EventName")]
		public string EventName { get; set; } = string.Empty;

		[JsonProperty("EventOptions")]
		public Dictionary<string, string> EventOptions { get; set; } = new Dictionary<string, string>();

		public UmaEvent() { }

		public UmaEvent(string _eventName, Dictionary<string, string> _eventOptions)
		{
			EventName = _eventName;
			EventOptions = _eventOptions;
		}
	}

}
