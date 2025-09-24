using System.Diagnostics;
using System.IO.Compression;
using UmatoMusume.Models;

namespace UmatoMusume.Utils
{
	public static class Updater
	{
		private const string VERSION_CONTROL_FILE = "version_control.json";
		private readonly static string[] EXCLUDE_FILES = new string[] { "restart.bat", "UmamusumePD.sqlite", "config.txt" };
		private readonly static string[] EXCLUDE_FOLDERS = new string[] { };
		private const int DOWNLOAD_BUFFER_SIZE = 8192;
		private const string OLD_FILE_POSTFIX = "_OLD";

		// Progress reporting constants
		private const int PROGRESS_INITIAL = 0;
		private const int PROGRESS_CHECKING = 5;
		private const int PROGRESS_DOWNLOADING = 10;
		private const int PROGRESS_EXTRACTING = 80;
		private const int PROGRESS_INSTALLING = 90;
		private const int PROGRESS_CLEANUP = 95;
		private const int PROGRESS_COMPLETE = 100;
		private const int PROGRESS_TOTAL = 100;

		private const int DOWNLOAD_PROGRESS_WEIGHT = 70;

		public static async Task<(VersionControl? currentVersion, VersionControl? latestVersion)> FetchMetadata()
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
				var (currentVersion, latestVersion) = await FetchMetadata();

				if (currentVersion == null || latestVersion == null) return false;

				var currentVersionCode = new Version(currentVersion.CurrentVersion.Replace("v", ""));
				var latestVersionCode = new Version(latestVersion.CurrentVersion.Replace("v", ""));

				if (currentVersionCode >= latestVersionCode) return false;

				return true;
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Error checking for updates: {ex.Message}");
				return false;
			}
		}

		public static async Task<bool> DownloadAndUpdate(IProgress<ProgressGroup>? _progress = null)
		{
			try
			{
				_progress?.Report(new ProgressGroup(PROGRESS_INITIAL, PROGRESS_TOTAL, "Checking for updates..."));

				var check = await CheckForUpdates();
				if (!check)
				{
					_progress?.Report(new ProgressGroup(PROGRESS_COMPLETE, PROGRESS_TOTAL, "No updates available"));
					return false;
				}

				_progress?.Report(new ProgressGroup(PROGRESS_CHECKING, PROGRESS_TOTAL, "Fetching update information..."));

				var (currentVersion, latestVersion) = await FetchMetadata();
				if (currentVersion == null || latestVersion == null)
				{
					_progress?.Report(new ProgressGroup(PROGRESS_INITIAL, PROGRESS_TOTAL, "Error: Unable to fetch update information"));
					return false;
				}

				string prefixFileName = latestVersion.PrefixFileName;
				string windowsVersion = Helper.GetWindowsVersion();
				string extension = latestVersion.Extension;

				string fileName = $"{prefixFileName}-{windowsVersion}.{extension}";
				string downloadUrl = latestVersion.DownloadUrl.Replace("#FILE_NAME", fileName).Replace("#TAG", latestVersion.CurrentVersion);

				string tempZipPath = Path.Combine(Path.GetTempPath(), fileName);
				string tempExtractFolder = Path.Combine(Path.GetTempPath(), Path.GetFileNameWithoutExtension(fileName));
				string baseDir = AppContext.BaseDirectory;

				_progress?.Report(new ProgressGroup(PROGRESS_DOWNLOADING, PROGRESS_TOTAL, $"Downloading update {latestVersion.CurrentVersion}..."));

				using (HttpClient client = new HttpClient())
				using (HttpResponseMessage response = await client.GetAsync(downloadUrl, HttpCompletionOption.ResponseHeadersRead))
				{
					response.EnsureSuccessStatusCode();
					var contentLength = response.Content.Headers.ContentLength;

					using (Stream contentStream = await response.Content.ReadAsStreamAsync())
					using (FileStream fileStream = new FileStream(tempZipPath, FileMode.Create, FileAccess.Write, FileShare.None, DOWNLOAD_BUFFER_SIZE, true))
					{
						var buffer = new byte[DOWNLOAD_BUFFER_SIZE];
						long totalRead = 0;
						int read;

						while ((read = await contentStream.ReadAsync(buffer, 0, buffer.Length)) > 0)
						{
							await fileStream.WriteAsync(buffer, 0, read);
							totalRead += read;

							if (contentLength.HasValue && contentLength.Value > 0)
							{
								int downloadPercent = (int)(totalRead * DOWNLOAD_PROGRESS_WEIGHT / contentLength.Value);
								int currentProgress = PROGRESS_DOWNLOADING + downloadPercent;
								_progress?.Report(new ProgressGroup(currentProgress, PROGRESS_TOTAL, $"Downloading... {currentProgress}%"));
							}
						}
					}
				}

				_progress?.Report(new ProgressGroup(PROGRESS_EXTRACTING, PROGRESS_TOTAL, "Extracting update files..."));
				ZipFile.ExtractToDirectory(tempZipPath, tempExtractFolder, true);

				_progress?.Report(new ProgressGroup(PROGRESS_INSTALLING, PROGRESS_TOTAL, "Installing update..."));

				foreach (var file in Directory.GetFiles(baseDir))
				{
					try
					{
						if (EXCLUDE_FILES.Contains(Path.GetFileName(file))) continue;

						string newName = Path.Combine(baseDir, Path.GetFileNameWithoutExtension(file) + OLD_FILE_POSTFIX + Path.GetExtension(file));
						if (File.Exists(newName)) File.Delete(newName);
						File.Move(file, newName);
					}
					catch { }
				}

				foreach (var dir in Directory.GetDirectories(baseDir))
				{
					try
					{
						if (EXCLUDE_FOLDERS.Contains(Path.GetFileName(dir))) continue;

						string newName = Path.Combine(baseDir, Path.GetFileName(dir) + OLD_FILE_POSTFIX);
						if (Directory.Exists(newName)) Directory.Delete(newName, true);
						Directory.Move(dir, newName);
					}
					catch { }
				}

				CopyAll(new DirectoryInfo(Path.Combine(tempExtractFolder, windowsVersion)), new DirectoryInfo(baseDir));

				_progress?.Report(new ProgressGroup(PROGRESS_CLEANUP, PROGRESS_TOTAL, "Cleaning up temporary files..."));
				DeleteObject(tempZipPath, true);
				DeleteObject(tempExtractFolder);

				_progress?.Report(new ProgressGroup(PROGRESS_COMPLETE, PROGRESS_TOTAL, "Update completed successfully!"));
				MessageBox.Show("Update completed successfully! The application now will be restarted", "Update Complete!", MessageBoxButtons.OK, MessageBoxIcon.Information);
				return true;
			}
			catch (Exception ex)
			{
				_progress?.Report(new ProgressGroup(PROGRESS_INITIAL, PROGRESS_TOTAL, $"Error during update: {ex.Message}"));
				return false;
			}
		}

		public static void RestartApplication()
		{
			string exePath = Application.ExecutablePath;
			string workDir = AppContext.BaseDirectory;

			string batPath = Path.Combine(workDir, "restart.bat");

			Process.Start(new ProcessStartInfo
			{
				FileName = batPath,
				Arguments = $"\"{workDir}\" \"{exePath}\"",
				CreateNoWindow = true,
				UseShellExecute = false
			});

			Application.Exit();
		}

		private static void DeleteObject(string _path, bool _isFile = false)
		{
			try
			{
				if (!_isFile)
				{
					Directory.Delete(_path, true);
					return;
				}

				File.Delete(_path);
				return;
			}
			catch { }
		}

		private static void CopyAll(DirectoryInfo _source, DirectoryInfo _target)
		{
			if (!_target.Exists) _target.Create();

			foreach (FileInfo fi in _source.GetFiles())
			{
				if (EXCLUDE_FILES.Contains(fi.Name)) continue;

				string targetFilePath = Path.Combine(_target.FullName, fi.Name);
				fi.CopyTo(targetFilePath, true);
			}

			foreach (DirectoryInfo di in _source.GetDirectories())
			{
				if (EXCLUDE_FOLDERS.Contains(di.Name)) continue;

				DirectoryInfo nextTargetSubDir = _target.CreateSubdirectory(di.Name);
				CopyAll(di, nextTargetSubDir);
			}
		}

		public static void CleanupOldFiles()
		{
			string baseDir = AppContext.BaseDirectory;

			foreach (var file in Directory.GetFiles(baseDir))
			{
				try
				{
					if (file.Contains(OLD_FILE_POSTFIX))
					{
						DeleteObject(file, true);
					}
				}
				catch { }
			}

			foreach (var dir in Directory.GetDirectories(baseDir))
			{
				try
				{
					if (dir.Contains(OLD_FILE_POSTFIX))
					{
						DeleteObject(dir);
					}
				}
				catch { }
			}
		}
	}
}
