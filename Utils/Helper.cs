using OpenQA.Selenium;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using FuzzySharp;
using System.Runtime.CompilerServices;

namespace UmatoMusume.Utils
{
    public static class Helper
    {
        private const int DEFAULT_BUFFER_SIZE = 8192;
        private const int DEFAULT_OFFSET = 0;
        private const int PROGRESS_INITIAL = 0;
        private const int PROGRESS_TOTAL = 100;
        private const int RATIO = 80;

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
        public static bool SaveAsJson<T>(List<T> items, string filePath, Formatting formatting = Formatting.Indented)
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
            return Fuzz.Ratio(_inputStr.Trim().ToLower(), _compareStr.ToLower().ToString()) >= RATIO;
        }

        public static T? GetSelectedValue<T>(this ComboBox _cbo)
        {
            var currentValue = _cbo.Items[_cbo.SelectedIndex];
            return currentValue is T value ? value : default;
        }
    }
}
