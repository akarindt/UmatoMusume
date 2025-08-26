using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.Xml;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using UmatoMusume.Models;

namespace UmatoMusume.Utils
{
    public static class Updater
    {
        private const string VERSION_CONTROL_FILE = "version_control.json";

        public static async Task<(VersionControl? currentVersion, VersionControl? lastestVersion)> FetchMetadata()
        {
            var currentVersion = Helper.SingleLoadFromJson<VersionControl>(VERSION_CONTROL_FILE);
            if (currentVersion == null) return (null, null);

            using var client = new HttpClient();
            var jsonString = await client.GetStringAsync(currentVersion.VersionCheckUrl);
            var latestVersion = Helper.JsonToData<VersionControl>(jsonString);
            return (currentVersion, latestVersion);
        }

        public static async Task<bool> CheckForUpdates()
        {
            try
            {
                var (currentVersion, latestVersion) = await FetchMetadata().ConfigureAwait(false);

                if (currentVersion == null || latestVersion == null) return false;

                var currentVersionCode = new Version(currentVersion.CurrentVersion.Replace("v", ""));
                var lastestVersionCode = new Version(latestVersion.CurrentVersion.Replace("v", ""));

                if (currentVersionCode >= lastestVersionCode) return false;

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error checking for updates: {ex.Message}");
                return false;
            }
        }

        public static async Task DownloadAndUpdate()
        {
            var check = await CheckForUpdates();
            if (!check) return;

            var (currentVersion, latestVersion) = await FetchMetadata().ConfigureAwait(false);
            if (currentVersion == null || latestVersion == null) return;

            var prefixFileName = latestVersion.PrefixFileName;
            var windowsVersion = Helper.GetWindowsVersion();
            var extension = latestVersion.Extension;

            var fileName = $"{prefixFileName}-{windowsVersion}.{extension}";

            var downloadUrl = latestVersion.DownloadUrl.Replace("#FILE_NAME", fileName).Replace("#TAG", latestVersion.CurrentVersion);

            using HttpClient client = new HttpClient();
            using HttpResponseMessage response = await client.GetAsync(downloadUrl, HttpCompletionOption.ResponseHeadersRead);
            response.EnsureSuccessStatusCode();

            using Stream contentStream = await response.Content.ReadAsStreamAsync();
            using FileStream fileStream = new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.None, 8192, true);
            await contentStream.CopyToAsync(fileStream);
        }
    }
}
