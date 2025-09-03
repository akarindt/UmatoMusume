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
    public partial class FrmSetting : Form
    {
        private readonly Dictionary<string, string> _config = new Dictionary<string, string>();

        public FrmSetting()
        {
            InitializeComponent();

            _config = Helper.ReadConfig();

            chkCheckForUpdates.Checked = bool.Parse(Helper.GetConfigValue("AutoUpdate", "False"));
            chkRightMenu.Checked = bool.Parse(Helper.GetConfigValue("RightMenu", "False"));
            chkFullScreen.Checked = bool.Parse(Helper.GetConfigValue("FullScreen", "False"));
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
    }
}
