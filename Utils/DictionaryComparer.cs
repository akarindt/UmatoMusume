namespace UmatoMusume.Utils
{
	public class DictionaryComparer : IEqualityComparer<Dictionary<string, string>>
	{
		private const int INITIAL_HASH = 17;
		private const int HASH_MULTIPLIER = 23;

		public bool Equals(Dictionary<string, string>? x, Dictionary<string, string>? y)
		{
			if (x == null && y == null) return true;
			if (x == null || y == null) return false;

			if (x.Count != y.Count) return false;
			return x.OrderBy(kv => kv.Key).SequenceEqual(y.OrderBy(kv => kv.Key));
		}

		public int GetHashCode(Dictionary<string, string> obj)
		{
			int hash = INITIAL_HASH;
			foreach (var kv in obj.OrderBy(kv => kv.Key))
			{
				hash = hash * HASH_MULTIPLIER + kv.Key.GetHashCode();
				hash = hash * HASH_MULTIPLIER + kv.Value.GetHashCode();
			}
			return hash;
		}
	}
}
