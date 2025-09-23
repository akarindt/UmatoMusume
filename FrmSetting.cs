using UmatoMusume.Utils;

namespace UmatoMusume
{
	public partial class FrmSetting : Form
	{

		public FrmSetting()
		{
			InitializeComponent();

			chkCheckForUpdates.Checked = bool.Parse(Helper.GetConfigValue("AutoUpdate", "False"));
			chkRightMenu.Checked = bool.Parse(Helper.GetConfigValue("RightMenu", "False"));
			chkFullScreen.Checked = bool.Parse(Helper.GetConfigValue("FullScreen", "False"));
			chkUsePaddle.Checked = bool.Parse(Helper.GetConfigValue("UsePaddleOCR", "False"));
			chkUseRapid.Checked = bool.Parse(Helper.GetConfigValue("UseRapidOCR", "False"));
		}

		private void chkCheckForUpdates_CheckedChanged(object sender, EventArgs e)
		{
			Helper.UpdateConfigValue("AutoUpdate", chkCheckForUpdates.Checked.ToString());
		}

		private void chkRightMenu_CheckedChanged(object sender, EventArgs e)
		{
			Helper.UpdateConfigValue("RightMenu", chkRightMenu.Checked.ToString());
		}

		private void chkFullScreen_CheckedChanged(object sender, EventArgs e)
		{
			Helper.UpdateConfigValue("FullScreen", chkFullScreen.Checked.ToString());
		}

		private void chkUsePaddle_CheckedChanged(object sender, EventArgs e)
		{
			Helper.UpdateConfigValue("UsePaddleOCR", chkUsePaddle.Checked.ToString());
			Helper.UpdateConfigValue("UseRapidOCR", (!chkUsePaddle.Checked).ToString());
			chkUseRapid.Checked = !chkUsePaddle.Checked;
		}

		private void chkUseRapid_CheckedChanged(object sender, EventArgs e)
		{

			Helper.UpdateConfigValue("UseRapidOCR", chkUseRapid.Checked.ToString());
			Helper.UpdateConfigValue("UsePaddleOCR", (!chkUseRapid.Checked).ToString());
			chkUsePaddle.Checked = !chkUseRapid.Checked;
		}
	}
}
