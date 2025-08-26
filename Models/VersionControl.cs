using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UmatoMusume.Models
{
    public class VersionControl
    {
        public VersionControl() { }

        public VersionControl(string _currentVersion, string _versionCheckUrl, string _downloadUrl, string _prefixFileName, string _extension) 
        {
            CurrentVersion = _currentVersion;
            VersionCheckUrl = _versionCheckUrl;
            DownloadUrl = _downloadUrl;
            PrefixFileName = _prefixFileName;
            Extension = _extension;
        }

        [JsonProperty("CurrentVersion")]
        public string CurrentVersion { get; set; } = string.Empty;

        [JsonProperty("VersionCheckUrl")]
        public string VersionCheckUrl { get; set; } = string.Empty;

        [JsonProperty("DownloadUrl")]
        public string DownloadUrl { get; set; } = string.Empty;

        [JsonProperty("PrefixFileName")]
        public string PrefixFileName { get; set; } = string.Empty;

        [JsonProperty("Extension")]
        public string Extension { get; set; } = string.Empty;

    }
}
