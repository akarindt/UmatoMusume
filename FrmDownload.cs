using UmatoMusume.Models;
using UmatoMusume.Utils;

namespace UmatoMusume
{
	public partial class FrmDownload : Form
	{
		private const string DEFAULT_FOLDER = "Assets";
		private const string SUPPORT_CARD_DOWNLOAD_URL = "https://raw.githubusercontent.com/akarindt/UmatoMusume/refs/heads/master/Assets/support_card.json";
		private const string UMA_DATA_DOWNLOAD_URL = "https://raw.githubusercontent.com/akarindt/UmatoMusume/refs/heads/master/Assets/uma_data.json";
		private const string CAREER_DATA_DOWNLOAD_URL = "https://raw.githubusercontent.com/akarindt/UmatoMusume/refs/heads/master/Assets/career.json";
		private const string RACES_DATA_DOWNLOAD_URL = "https://raw.githubusercontent.com/akarindt/UmatoMusume/refs/heads/master/Assets/races.json";
		private const int PROGRESS_INITIAL = 0;
		private const int PROGRESS_TOTAL = 100;

		public FrmDownload()
		{
			InitializeComponent();
		}

		#region Functions
		private void SetControlsEnabled(bool enabled)
		{
			btnCrawlUma.Enabled = enabled;
			btnCrawlSupport.Enabled = enabled;
			btnDownloadUma.Enabled = enabled;
			btnDownloadSupport.Enabled = enabled;
			btnDownloadCareer.Enabled = enabled;
			btnCrawlCareer.Enabled = enabled;
			btnCrawlRaces.Enabled = enabled;
			btnDownloadRaces.Enabled = enabled;
		}

		private async Task InitAction(Label _label, ProgressBar _progressBar, Func<Progress<ProgressGroup>, Task> _func, string _type)
		{
			SetControlsEnabled(false);
			_progressBar.Value = PROGRESS_INITIAL;

			try
			{
				var progress = new Progress<ProgressGroup>(data =>
				{
					_progressBar.Value = Math.Min(data.Current, PROGRESS_TOTAL);
					_label.Text = data.Message;
				});

				await _func(progress);

				MessageBox.Show(
					$"{_type} data downloaded successfully. Please restart the application to load the newest data.",
					"Download Complete",
					MessageBoxButtons.OK,
					MessageBoxIcon.Information
				);
			}
			catch (Exception ex)
			{
				MessageBox.Show($"Error occurred: {ex.Message}", "Download Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
			finally
			{
				SetControlsEnabled(true);
			}
		}
		#endregion

		#region Event Handlers
		private async void btnCrawlUma_Click(object sender, EventArgs e)
		{
			await InitAction(lblProgress, pbDownload, progress => GameTora.DownloadUmaData(progress), "Uma");
		}

		private async void btnCrawlSupport_Click(object sender, EventArgs e)
		{
			await InitAction(lblProgress, pbDownload, progress => GameTora.DownloadSupportData(progress), "Support Card");
		}

		private async void btnDownloadUma_Click(object sender, EventArgs e)
		{
			await InitAction(lblProgress, pbDownload, progress => Helper.DownloadJsonAsync(UMA_DATA_DOWNLOAD_URL, DEFAULT_FOLDER + "/uma_data.json", progress), "Uma");
		}

		private async void btnDownloadSupport_Click(object sender, EventArgs e)
		{
			await InitAction(lblProgress, pbDownload, progress => Helper.DownloadJsonAsync(SUPPORT_CARD_DOWNLOAD_URL, DEFAULT_FOLDER + "/support_card.json", progress), "Support Card");
		}

		private async void btnDownloadCareer_Click(object sender, EventArgs e)
		{
			await InitAction(lblProgress, pbDownload, progress => Helper.DownloadJsonAsync(CAREER_DATA_DOWNLOAD_URL, DEFAULT_FOLDER + "/career.json", progress), "Career");
		}

		private async void btnCrawlCareer_Click(object sender, EventArgs e)
		{
			await InitAction(lblProgress, pbDownload, progress => GameTora.DownloadAllCareerData(progress), "Career");
		}

		private async void btnCrawlRaces_Click(object sender, EventArgs e)
		{
			await InitAction(lblProgress, pbDownload, progress => GameTora.DownloadRacesData(progress), "Races");
		}

		private async void btnDownloadRaces_Click(object sender, EventArgs e)
		{
			await InitAction(lblProgress, pbDownload, progress => Helper.DownloadJsonAsync(RACES_DATA_DOWNLOAD_URL, DEFAULT_FOLDER + "/races.json", progress), "Races");
		}

		private void FrmDownload_FormClosed(object sender, FormClosedEventArgs e)
		{
			GameTora.DisposeResources();
		}
		#endregion
	}
}
