using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using UmatoMusume.Utils;

namespace UmatoMusume
{
    public partial class FrmDownload : Form
    {
        private Label lblProgress;

        private const string DEFAULT_FOLDER = "Assets";
        private const string SUPPORT_CARD_DOWNLOAD_URL = "https://raw.githubusercontent.com/akarindt/UmatoMusume/refs/heads/master/Assets/support_card.json";
        private const string UMA_DATA_DOWNLOAD_URL = "https://raw.githubusercontent.com/akarindt/UmatoMusume/refs/heads/master/Assets/uma_data.json";
        private const string CAREER_DATA_DOWNLOAD_URL = "https://raw.githubusercontent.com/akarindt/UmatoMusume/refs/heads/master/Assets/career_data.json";
        private const int PROGRESS_INITIAL = 0;
        private const int PROGRESS_TOTAL = 100;
        private const int Y_OFFSET = 20;

        public FrmDownload()
        {
            InitializeComponent();

            lblProgress = new Label
            {
                AutoSize = true,
                Location = new Point(pbDownload.Location.X, pbDownload.Location.Y - Y_OFFSET),
                Name = "lblProgress",
                Size = new Size(pbDownload.Width, Y_OFFSET),
                Text = "Ready"
            };
            groupBox1.Controls.Add(lblProgress);
        }

        private async void btnCrawlUma_Click(object sender, EventArgs e)
        {
            SetControlsEnabled(false);
            pbDownload.Value = PROGRESS_INITIAL;

            try
            {
                var progress = new Progress<(int Current, int Total, string Message)>(progressData =>
                {
                    var (current, total, message) = progressData;
                    pbDownload.Value = Math.Min(current, PROGRESS_TOTAL);
                    lblProgress.Text = message;
                });

                await GameTora.DownloadUmaData(progress);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error occurred: {ex.Message}", "Download Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                SetControlsEnabled(true);
            }
        }

        private async void btnCrawlSupport_Click(object sender, EventArgs e)
        {
            SetControlsEnabled(false);
            pbDownload.Value = PROGRESS_INITIAL;

            try
            {
                var progress = new Progress<(int Current, int Total, string Message)>(progressData =>
                {
                    var (current, total, message) = progressData;
                    pbDownload.Value = Math.Min(current, PROGRESS_TOTAL);
                    lblProgress.Text = message;
                });

                await GameTora.DownloadSupportData(progress);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error occurred: {ex.Message}", "Download Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                SetControlsEnabled(true);
            }
        }

        private async void btnDownloadUma_Click(object sender, EventArgs e)
        {
            SetControlsEnabled(false);
            pbDownload.Value = PROGRESS_INITIAL;

            try
            {
                var progress = new Progress<(int Current, int Total, string Message)>(progressData =>
                {
                    var (current, total, message) = progressData;
                    pbDownload.Value = Math.Min(current, PROGRESS_TOTAL);
                    lblProgress.Text = message;
                });

                bool result = await Helper.DownloadJsonAsync(UMA_DATA_DOWNLOAD_URL, DEFAULT_FOLDER + "/uma_data.json", progress);
                if (!result)
                {
                    MessageBox.Show("Failed to download Uma data.", "Download Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                MessageBox.Show("Uma data downloaded successfully. Please restart the application to load the newest data", "Download Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            finally
            {
                SetControlsEnabled(true);
            }
        }

        private async void btnDownloadSupport_Click(object sender, EventArgs e)
        {
            SetControlsEnabled(false);
            pbDownload.Value = PROGRESS_INITIAL;

            try
            {
                var progress = new Progress<(int Current, int Total, string Message)>(progressData =>
                {
                    var (current, total, message) = progressData;
                    pbDownload.Value = Math.Min(current, PROGRESS_TOTAL);
                    lblProgress.Text = message;
                });

                bool result = await Helper.DownloadJsonAsync(SUPPORT_CARD_DOWNLOAD_URL, DEFAULT_FOLDER + "/support_card.json", progress);
                if (!result)
                {
                    MessageBox.Show("Failed to download Support Card data.", "Download Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                MessageBox.Show("Support Card data downloaded successfully. Please restart the application to load the newest data", "Download Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            finally
            {
                SetControlsEnabled(true);
            }
        }

        private void SetControlsEnabled(bool enabled)
        {
            btnCrawlUma.Enabled = enabled;
            btnCrawlSupport.Enabled = enabled;
            btnDownloadUma.Enabled = enabled;
            btnDownloadSupport.Enabled = enabled;
            btnDownloadCareer.Enabled = enabled;
            btnCrawlCareer.Enabled = enabled;
        }

        private async void btnDownloadCareer_Click(object sender, EventArgs e)
        {
            SetControlsEnabled(false);
            pbDownload.Value = PROGRESS_INITIAL;

            try
            {
                var progress = new Progress<(int Current, int Total, string Message)>(progressData =>
                {
                    var (current, total, message) = progressData;
                    pbDownload.Value = Math.Min(current, PROGRESS_TOTAL);
                    lblProgress.Text = message;
                });

                bool result = await Helper.DownloadJsonAsync(CAREER_DATA_DOWNLOAD_URL, DEFAULT_FOLDER + "/career.json", progress);
                if (!result)
                {
                    MessageBox.Show("Failed to download Career data.", "Download Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                MessageBox.Show("Career data downloaded successfully. Please restart the application to load the newest data", "Download Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            finally
            {
                SetControlsEnabled(true);
            }
        }

        private async void btnCrawlCareer_Click(object sender, EventArgs e)
        {
            SetControlsEnabled(false);
            pbDownload.Value = PROGRESS_INITIAL;

            try
            {
                var progress = new Progress<(int Current, int Total, string Message)>(progressData =>
                {
                    var (current, total, message) = progressData;
                    pbDownload.Value = Math.Min(current, PROGRESS_TOTAL);
                    lblProgress.Text = message;
                });

                await GameTora.DownloadAllCareerData(progress);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error occurred: {ex.Message}", "Download Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                SetControlsEnabled(true);
            }
        }
    }
}
