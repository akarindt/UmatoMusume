using F23.StringSimilarity;
using FuzzySharp;
using Newtonsoft.Json;
using OpenQA.Selenium;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Xml;
using System.Xml.Linq;

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

        public static IWebElement? FindElementSafe(ISearchContext driver, By by)
        {
            try
            {
                return driver.FindElement(by);
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


        public static IReadOnlyCollection<IWebElement> FindElementsSafe(ISearchContext driver, By by)
        {
            try
            {
                return driver.FindElements(by);
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
        public static bool SaveAsJson<T>(List<T> items, string filePath, Newtonsoft.Json.Formatting formatting = Newtonsoft.Json.Formatting.Indented)
        {
            try
            {
                string? directory = Path.GetDirectoryName(filePath);
                if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                string json = JsonConvert.SerializeObject(items, formatting);
                File.WriteAllText(filePath, json, Encoding.UTF8);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving JSON file: {ex.Message}");
                return false;
            }
        }

        public static List<T> LoadFromJson<T>(string filePath)
        {
            try
            {
                if (!File.Exists(filePath))
                {
                    return new List<T>();
                }

                string json = File.ReadAllText(filePath, Encoding.UTF8);
                return JsonConvert.DeserializeObject<List<T>>(json) ?? new List<T>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading JSON file: {ex.Message}");
                return new List<T>();
            }
        }

        public static async Task<bool> DownloadJsonAsync(string url, string filePath, IProgress<(int Current, int Total, string Message)>? progress = null)
        {
            try
            {
                using var httpClient = new HttpClient();
                progress?.Report((PROGRESS_INITIAL, PROGRESS_TOTAL, "Starting download..."));
                using var response = await httpClient.GetAsync(url, HttpCompletionOption.ResponseHeadersRead);
                response.EnsureSuccessStatusCode();
                var contentLength = response.Content.Headers.ContentLength;
                using var stream = await response.Content.ReadAsStreamAsync();
                string? directory = Path.GetDirectoryName(filePath);

                if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                using (var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None))
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
                            progress?.Report((percent, PROGRESS_TOTAL, $"Downloading... {percent}%"));
                        }
                    }
                    progress?.Report((PROGRESS_TOTAL, PROGRESS_TOTAL, "Download complete!"));
                    return true;
                }
            }
            catch (Exception ex)
            {
                progress?.Report((PROGRESS_INITIAL, PROGRESS_TOTAL, $"Error: {ex.Message}"));
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

        public static T? GetSelectedValue<T>(this ComboBox _cbo)
        {
            if (_cbo.SelectedIndex < 0 || _cbo.SelectedIndex >= _cbo.Items.Count) return default;
            var currentValue = _cbo.Items[_cbo.SelectedIndex];
            return currentValue is T value ? value : default;
        }

        public static object? GetValue<T>(string _propertyName, object? _obj)
        {
            if(_obj == null) return default;

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
    }
}
