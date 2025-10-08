using System.Data;
using UmatoMusume.Models;
using UmatoMusume.Utils;

namespace UmatoMusume
{
	public partial class FrmUpdater : Form
	{
		private const int PROGRESS_TOTAL = 100;
		public bool _shouldUpdate = false;

		public FrmUpdater()
		{
			InitializeComponent();
		}

		#region Functions
		private async Task CheckUpdate()
		{
			SetButtons(true);
			var dialogResult = MessageBox.Show("Do you want to check for updates?", "Check for Updates", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
			if (dialogResult == DialogResult.No) return;

			var check = await Updater.CheckForUpdates();
			if (!check)
			{
				MessageBox.Show("You are already using the latest version.", "No Updates", MessageBoxButtons.OK, MessageBoxIcon.Information);
				return;
			}

			dialogResult = MessageBox.Show("An update is available. Do you want to download and install it now?", "Update Available", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
			if (dialogResult == DialogResult.No) return;

			await StartUpdate();
		}

		public async Task StartUpdate()
		{
			SetButtons(false);

			foreach (Form f in Application.OpenForms.Cast<Form>().ToList())
			{
				if (f.Name != this.Name) f.Hide();
			}

			var progress = new Progress<ProgressGroup>(progressData =>
			{
				var (current, total, message) = progressData.Deconstruct();
				pUpdater.Value = Math.Min(current, PROGRESS_TOTAL);
				lblUpdate.Text = message;
			});

			var check = await Updater.DownloadAndUpdate(progress);
			if (!check)
			{
				var frmMain = new FrmMain();
				frmMain.Show();
				this.Hide();
				return;
			}

			SetButtons(true);
			Updater.RestartApplication();
			Application.Exit();
		}

		private void SetButtons(bool _isEnable = true)
		{
			btnCheckUpdate.Enabled = _isEnable;
			btnReDown.Enabled = _isEnable;
		}
		#endregion

		#region Event Handlers
		private async void btnCheckUpdate_Click(object sender, EventArgs e)
		{
			await CheckUpdate();
		}

		private async void btnReDown_Click(object sender, EventArgs e)
		{
			var dialogResult = MessageBox.Show("Do you want to re-download and install the update?", "Re-Download Update", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
			if (dialogResult == DialogResult.No) return;

			await StartUpdate();
		}

		private async void FrmUpdater_Load(object sender, EventArgs e)
		{
			if (_shouldUpdate)
			{
				await StartUpdate();
			}
		}
		#endregion

		protected override CreateParams CreateParams
		{
			get
			{
				var cp = base.CreateParams;
				var isFullScreen = bool.Parse(Helper.GetConfigValue("FullScreen", "False"));
				if (isFullScreen)
				{
					cp.ExStyle |= 0x80;
					cp.ExStyle |= 0x8;
				}

				return cp;
			}
		}
	}
}
