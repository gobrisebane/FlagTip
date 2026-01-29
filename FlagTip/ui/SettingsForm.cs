using FlagTip.Caret;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FlagTip.UI
{
    public partial class SettingsForm : Form
    {

        private IndicatorForm _indicatorForm;
        private CaretController _caretController;
        private TableLayoutPanel optionLayout;

        public SettingsForm(IndicatorForm indicatorForm, CaretController caretController)
        {
            InitializeComponent();

            _indicatorForm = indicatorForm;
            _caretController = caretController;

            Text = "FlagTip";
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            ClientSize = new Size(640, 340);
            StartPosition = FormStartPosition.CenterScreen;

            InitializeAboutTab();
            InitializeOptionTab();


            trackOpacity.Value = (int)(Properties.Settings.Default.Opacity * 100);
            lblOpacity.Text = $"불투명도 (기본값:75): {trackOpacity.Value}%";
            trackOpacity.Scroll += TrackOpacity_Scroll;



            trackOffsetX.Value = Properties.Settings.Default.OffsetX;
            lblOffsetX.Text = $"가로 위치 (기본값:3): {trackOffsetX.Value}";
            trackOffsetX.Scroll += TrackOffsetX_Scroll;


            trackOffsetY.Value = Properties.Settings.Default.OffsetY;
            lblOffsetY.Text = $"세로 위치 (기본값:20): {trackOffsetY.Value}";
            trackOffsetY.Scroll += TrackOffsetY_Scroll;


        }

        private void TrackOpacity_Scroll(object sender, EventArgs e)
        {
            double opacity = trackOpacity.Value / 100.0;
            lblOpacity.Text = $"불투명도 (기본값:75): {trackOpacity.Value}%";

            // IndicatorForm에 즉시 적용
            _indicatorForm?.SetOpacity(opacity);

            // Settings에 저장
            Properties.Settings.Default.Opacity = opacity;
            Properties.Settings.Default.Save();
        }

        private void TrackOffsetX_Scroll(object sender, EventArgs e)
        {
            int value = trackOffsetX.Value;
            lblOffsetX.Text = $"가로 위치 (기본값:3): {value}";

            // IndicatorForm에 즉시 적용
            _indicatorForm.OFFSET_X = value;

            // Settings에 저장
            Properties.Settings.Default.OffsetX = value;
            Properties.Settings.Default.Save();
        }

        private void TrackOffsetY_Scroll(object sender, EventArgs e)
        {
            int value = trackOffsetY.Value;
            lblOffsetY.Text = $"세로 위치 (기본값:20): {value}";

            _indicatorForm.OFFSET_Y = value;

            Properties.Settings.Default.OffsetY = value;
            Properties.Settings.Default.Save();
        }

        private void InitializeAboutTab()
        {

            var version = Assembly
                .GetExecutingAssembly()
                .GetCustomAttribute<AssemblyFileVersionAttribute>()?
                .Version;

            labelVersion.Text = $"FlagTip {version}";
            labelVersion.TextAlign = ContentAlignment.MiddleCenter;
        }

        private void InitializeOptionTab()
        {
            chkFollowCursor.Checked = Properties.Settings.Default.FollowCursor;
            chkFollowCursor.CheckedChanged += ChkFollowCursor_CheckedChanged;
        }

        private void ChkFollowCursor_CheckedChanged(object sender, EventArgs e)
        {
            bool enabled = chkFollowCursor.Checked;

            Properties.Settings.Default.FollowCursor = enabled;
            Properties.Settings.Default.Save();

            _caretController.SetCursorFollowEnabled(enabled);
        }

 




        public void SelectAboutTab()
        {
            tabControl1.SelectedTab = tabAbout;
        }

        public void SelectOptionTab()
        {
            tabControl1.SelectedTab = tabOption;
        }

        private void tabPage2_Click(object sender, EventArgs e)
        {

        }

        private void tabPage1_Click(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void tableLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void labelVersion_Click(object sender, EventArgs e)
        {

        }

        private void label4_Click(object sender, EventArgs e)
        {

        }




        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            var url = linkLabelWebsite.Text;

            // http/https 없으면 자동 보정
            if (!url.StartsWith("http"))
                url = "https://" + url;

            Process.Start(new ProcessStartInfo
            {
                FileName = url,
                UseShellExecute = true
            });
        }

        private void linkLabel1_LinkClicked_1(object sender, LinkLabelLinkClickedEventArgs e)
        {
            var url = linkLabel1.Text;

            // http/https 없으면 자동 보정
            if (!url.StartsWith("http"))
                url = "https://" + url;

            Process.Start(new ProcessStartInfo
            {
                FileName = url,
                UseShellExecute = true
            });
        }

        private void labelEmail_Click(object sender, EventArgs e)
        {
            const string email = "gobrisebane@gmail.com";

            Clipboard.SetText(email);

            MessageBox.Show(
                "이메일 주소가 클립보드에 복사되었습니다.",
                "복사 완료",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information
            );
        }

    

        private void chromeErrorLink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {

            Process.Start(new ProcessStartInfo
            {
                FileName = "ms-settings:regionlanguage",
                UseShellExecute = true
            });
        }

      
    }
}
