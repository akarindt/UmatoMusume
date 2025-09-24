using F23.StringSimilarity;
using FuzzySharp;
using Newtonsoft.Json;
using OpenQA.Selenium;
using System.Drawing.Imaging;
using System.Text;
using UmatoMusume.Models;

namespace UmatoMusume.Utils
{
	public static class Helper
	{
		private const int DEFAULT_BUFFER_SIZE = 8192;
		private const int DEFAULT_OFFSET = 0;
		private const int PROGRESS_INITIAL = 0;
		private const int PROGRESS_TOTAL = 100;
		private const int RATIO = 85;
		private const int MAX_RATIO = 100;

		private readonly static string[] YEAR = new string[] { "Junior", "Classic", "Senior" };
		private readonly static string[] TIME = new string[] { "Early", "Late" };
		private readonly static string[] MONTH = new string[] { "PreDebut", "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec" };

		public static string GetConfigValue(string _configName, string _defaultValue, string _configFilePath = "config.txt")
		{
			var configDict = ReadConfig(_configFilePath);
			return configDict.GetValueOrDefault(_configName, _defaultValue);
		}


		public static Dictionary<string, string> ReadConfig(string _configFilePath = "config.txt")
		{
			var configDict = new Dictionary<string, string>();

			try
			{
				if (!File.Exists(_configFilePath))
				{
					Console.WriteLine($"Config file not found: {_configFilePath}");
					return configDict;
				}

				string[] lines = File.ReadAllLines(_configFilePath, Encoding.UTF8);

				foreach (string line in lines)
				{
					if (string.IsNullOrWhiteSpace(line) || line.TrimStart().StartsWith("#") || line.TrimStart().StartsWith("//"))
						continue;

					int equalsIndex = line.IndexOf('=');
					if (equalsIndex > 0 && equalsIndex < line.Length - 1)
					{
						string key = line.Substring(0, equalsIndex).Trim();
						string value = line.Substring(equalsIndex + 1).Trim();

						if ((value.StartsWith("\"") && value.EndsWith("\"")) ||
							(value.StartsWith("'") && value.EndsWith("'")))
						{
							value = value.Substring(1, value.Length - 2);
						}

						configDict[key] = value;
					}
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Error reading config file: {ex.Message}");
			}

			return configDict;
		}


		public static bool UpdateConfigValue(string _key, string _value, string _configFilePath = "config.txt")
		{
			try
			{
				var configDict = ReadConfig(_configFilePath);
				configDict[_key] = _value;
				return WriteConfig(configDict, _configFilePath);
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Error updating config value: {ex.Message}");
				return false;
			}
		}

		public static bool WriteConfig(Dictionary<string, string> _configDict, string _configFilePath = "config.txt")
		{
			try
			{
				string? directory = Path.GetDirectoryName(_configFilePath);
				if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
				{
					Directory.CreateDirectory(directory);
				}

				var lines = new List<string>();
				foreach (var kvp in _configDict)
				{
					string escapedValue = kvp.Value;
					if (kvp.Value.Contains("=") || kvp.Value.Contains("#") || kvp.Value.Contains("//") ||
						kvp.Value.StartsWith(" ") || kvp.Value.EndsWith(" "))
					{
						escapedValue = $"\"{kvp.Value}\"";
					}

					lines.Add($"{kvp.Key}={escapedValue}");
				}

				File.WriteAllLines(_configFilePath, lines, Encoding.UTF8);
				return true;
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Error writing config file: {ex.Message}");
				return false;
			}
		}

		public static IWebElement? FindElementSafe(ISearchContext _driver, By _by)
		{
			try
			{
				return _driver.FindElement(_by);
			}
			catch (NoSuchElementException)
			{
				return null;
			}
			catch (StaleElementReferenceException)
			{
				return null;
			}
		}


		public static IReadOnlyCollection<IWebElement> FindElementsSafe(ISearchContext _driver, By _by)
		{
			try
			{
				return _driver.FindElements(_by);
			}
			catch (NoSuchElementException)
			{
				return new List<IWebElement>();
			}
			catch (StaleElementReferenceException)
			{
				return new List<IWebElement>();
			}
		}
		public static bool SaveAsJson<T>(List<T> _items, string _filePath, Newtonsoft.Json.Formatting _formatting = Newtonsoft.Json.Formatting.Indented)
		{
			try
			{
				string? directory = Path.GetDirectoryName(_filePath);
				if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
				{
					Directory.CreateDirectory(directory);
				}

				string json = JsonConvert.SerializeObject(_items, _formatting);
				File.WriteAllText(_filePath, json, Encoding.UTF8);
				return true;
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Error saving JSON file: {ex.Message}");
				return false;
			}
		}

		public static List<T> LoadFromJson<T>(string _filePath)
		{
			try
			{
				if (!File.Exists(_filePath))
				{
					return new List<T>();
				}

				string json = File.ReadAllText(_filePath, Encoding.UTF8);
				return JsonConvert.DeserializeObject<List<T>>(json) ?? new List<T>();
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Error loading JSON file: {ex.Message}");
				return new List<T>();
			}
		}

		public static T? SingleLoadFromJson<T>(string _filePath) where T : class
		{
			try
			{
				if (!File.Exists(_filePath))
				{
					return null;
				}

				string json = File.ReadAllText(_filePath, Encoding.UTF8);
				return JsonConvert.DeserializeObject<T>(json);
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Error loading JSON file: {ex.Message}");
				return null;
			}
		}

		public static async Task<bool> DownloadJsonAsync(string _url, string _filePath, IProgress<ProgressGroup>? _progress = null)
		{
			try
			{
				using var httpClient = new HttpClient();
				_progress?.Report(new ProgressGroup(PROGRESS_INITIAL, PROGRESS_TOTAL, "Starting download..."));
				using var response = await httpClient.GetAsync(_url, HttpCompletionOption.ResponseHeadersRead);
				response.EnsureSuccessStatusCode();
				var contentLength = response.Content.Headers.ContentLength;
				using var stream = await response.Content.ReadAsStreamAsync();
				string? directory = Path.GetDirectoryName(_filePath);

				if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
				{
					Directory.CreateDirectory(directory);
				}

				using (var fileStream = new FileStream(_filePath, FileMode.Create, FileAccess.Write, FileShare.None))
				{
					var buffer = new byte[DEFAULT_BUFFER_SIZE];
					long totalRead = 0;
					int read;
					while ((read = await stream.ReadAsync(buffer, DEFAULT_OFFSET, buffer.Length)) > 0)
					{
						await fileStream.WriteAsync(buffer, DEFAULT_OFFSET, read);
						totalRead += read;
						if (contentLength.HasValue && contentLength.Value > 0)
						{
							int percent = (int)(totalRead * PROGRESS_TOTAL / contentLength.Value);
							_progress?.Report(new ProgressGroup(percent, PROGRESS_TOTAL, $"Downloading... {percent}%"));
						}
					}
					_progress?.Report(new ProgressGroup(PROGRESS_TOTAL, PROGRESS_TOTAL, "Download complete!"));
					return true;
				}
			}
			catch (Exception ex)
			{
				_progress?.Report(new ProgressGroup(PROGRESS_INITIAL, PROGRESS_TOTAL, $"Error: {ex.Message}"));
				Console.WriteLine($"Error downloading JSON: {ex.Message}");
				return false;
			}
		}


		public static bool CheckRatio(string _inputStr, string _compareStr)
		{
			var ratioFuzzy = Fuzz.Ratio(_inputStr, _compareStr);
			if (ratioFuzzy >= RATIO) return true;

			var l = new JaroWinkler();
			var ratio = l.Similarity(_inputStr, _compareStr) * MAX_RATIO;
			return ratio >= RATIO;
		}

		public static (bool, string) FuzzyContains(string input, string target)
		{
			int len = target.Length;
			for (int i = 0; i <= input.Length - len; i++)
			{
				var sub = input.Substring(i, len);
				if (CheckRatio(sub, target)) return (true, sub);
			}
			return (false, string.Empty);
		}

		public static T? GetSelectedValue<T>(this ComboBox _cbo)
		{
			if (_cbo.SelectedIndex < 0 || _cbo.SelectedIndex >= _cbo.Items.Count) return default;
			var currentValue = _cbo.Items[_cbo.SelectedIndex];
			return currentValue is T value ? value : default;
		}

		public static object? GetValue<T>(string _propertyName, object? _obj)
		{
			if (_obj == null) return default;

			var prop = typeof(T).GetProperty(_propertyName);
			return prop != null ? prop.GetValue(_obj) : default;
		}

		public static IEnumerable<T> CompareWithFallback<T>(this IEnumerable<T> _list, string _propertyName, string _compareStr)
		{
			var result = _list.Where(x =>
			{
				var value = GetValue<T>(_propertyName, x)?.ToString() ?? string.Empty;
				return value.Equals(_compareStr);
			});

			result = result.Any() ? result : _list.Where(x =>
			{
				var value = GetValue<T>(_propertyName, x)?.ToString() ?? string.Empty;
				return value.Contains(_compareStr);
			});

			result = result.Any() ? result : _list.Where(x =>
			{
				var value = GetValue<T>(_propertyName, x)?.ToString() ?? string.Empty;
				return CheckRatio(value, _compareStr);
			});

			return result;
		}

		public static IEnumerable<T> ListPredicate<T, K>(this IEnumerable<T> _list, IEnumerable<K> _inputList, Func<T, bool> _predicate)
		{
			return _inputList.Any() ? _list.Where(_predicate) : _list;
		}

		public static byte[] ToByteArray(this Bitmap _bitmap, ImageFormat? _format = null)
		{
			using var ms = new MemoryStream();
			_bitmap.Save(ms, _format ?? ImageFormat.Png);
			return ms.ToArray();
		}

		public static T? JsonToData<T>(this string _str)
		{
			if (string.IsNullOrWhiteSpace(_str)) return default;

			try
			{
				return JsonConvert.DeserializeObject<T>(_str);
			}
			catch (JsonException ex)
			{
				Console.WriteLine($"JSON parsing error: {ex.Message}");
				return default;
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Error parsing JSON: {ex.Message}");
				return default;
			}
		}

		public static string GetWindowsVersion()
		{
			return Environment.Is64BitOperatingSystem ? "win-x64" : "win-x86";
		}

		public static string CompleteText(string _str)
		{
			if (string.IsNullOrEmpty(_str)) return string.Empty;

			var input = _str.Replace(" ", "").Replace("-", "");
			if (string.IsNullOrEmpty(input)) return string.Empty;

			var str = new StringBuilder();

			foreach (var year in YEAR)
			{
				var (isContains, subStr) = FuzzyContains(input, year);
				if (isContains)
				{
					str.Append(year).Append(" Year ");
					input = input.Replace(subStr, "");
					break;
				}
			}

			foreach (var time in TIME)
			{
				var (isContains, subStr) = FuzzyContains(input, time);
				if (isContains)
				{
					str.Append(time).Append(' ');
					input = input.Replace(subStr, "");
					break;
				}
			}

			foreach (var month in MONTH)
			{
				var (isContains, subStr) = FuzzyContains(input, month);
				if (isContains)
				{
					str.Append(month).Append(' ');
					input = input.Replace(subStr, "");
					break;
				}
			}

			return str.ToString().Trim();
		}
	}
}
