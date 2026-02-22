namespace FlagTip.UI
{
    partial class SettingsForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabOption = new System.Windows.Forms.TabPage();
            this.chkFollowCursor = new System.Windows.Forms.CheckBox();
            this.chromeErrorLink = new System.Windows.Forms.LinkLabel();
            this.trackOffsetY = new System.Windows.Forms.TrackBar();
            this.lblOffsetY = new System.Windows.Forms.Label();
            this.lblOffsetX = new System.Windows.Forms.Label();
            this.trackOffsetX = new System.Windows.Forms.TrackBar();
            this.lblOpacity = new System.Windows.Forms.Label();
            this.trackOpacity = new System.Windows.Forms.TrackBar();
            this.tabAbout = new System.Windows.Forms.TabPage();
            this.label2 = new System.Windows.Forms.Label();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.label3 = new System.Windows.Forms.Label();
            this.labelVersion = new System.Windows.Forms.Label();
            this.linkLabelWebsite = new System.Windows.Forms.LinkLabel();
            this.linkLabelEmail = new System.Windows.Forms.LinkLabel();
            this.linkLabel1 = new System.Windows.Forms.LinkLabel();
            this.tabControl1.SuspendLayout();
            this.tabOption.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackOffsetY)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackOffsetX)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackOpacity)).BeginInit();
            this.tabAbout.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabOption);
            this.tabControl1.Controls.Add(this.tabAbout);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(800, 450);
            this.tabControl1.TabIndex = 0;
            // 
            // tabOption
            // 
            this.tabOption.Controls.Add(this.chkFollowCursor);
            this.tabOption.Controls.Add(this.chromeErrorLink);
            this.tabOption.Controls.Add(this.trackOffsetY);
            this.tabOption.Controls.Add(this.lblOffsetY);
            this.tabOption.Controls.Add(this.lblOffsetX);
            this.tabOption.Controls.Add(this.trackOffsetX);
            this.tabOption.Controls.Add(this.lblOpacity);
            this.tabOption.Controls.Add(this.trackOpacity);
            this.tabOption.Location = new System.Drawing.Point(4, 22);
            this.tabOption.Name = "tabOption";
            this.tabOption.Padding = new System.Windows.Forms.Padding(3);
            this.tabOption.Size = new System.Drawing.Size(792, 424);
            this.tabOption.TabIndex = 0;
            this.tabOption.Text = "옵션";
            this.tabOption.UseVisualStyleBackColor = true;
            this.tabOption.Click += new System.EventHandler(this.tabPage1_Click);
            // 
            // chkFollowCursor
            // 
            this.chkFollowCursor.Location = new System.Drawing.Point(328, 49);
            this.chkFollowCursor.Name = "chkFollowCursor";
            this.chkFollowCursor.Size = new System.Drawing.Size(263, 43);
            this.chkFollowCursor.TabIndex = 9;
            this.chkFollowCursor.Text = "미지원프로그램에서 플래그가 커서를 따라가게 하기 (포토샵, 일러스트레이터, 왓츠앱)";
            this.chkFollowCursor.UseVisualStyleBackColor = true;
            // 
            // chromeErrorLink
            // 
            this.chromeErrorLink.AutoSize = true;
            this.chromeErrorLink.Location = new System.Drawing.Point(326, 104);
            this.chromeErrorLink.Name = "chromeErrorLink";
            this.chromeErrorLink.Size = new System.Drawing.Size(209, 12);
            this.chromeErrorLink.TabIndex = 8;
            this.chromeErrorLink.TabStop = true;
            this.chromeErrorLink.Text = "크롬, 엣지에 비정상 동작 시 대응방법";
            this.chromeErrorLink.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.chromeErrorLink_LinkClicked);
            // 
            // trackOffsetY
            // 
            this.trackOffsetY.Location = new System.Drawing.Point(40, 215);
            this.trackOffsetY.Maximum = 30;
            this.trackOffsetY.Name = "trackOffsetY";
            this.trackOffsetY.Size = new System.Drawing.Size(264, 45);
            this.trackOffsetY.TabIndex = 6;
            // 
            // lblOffsetY
            // 
            this.lblOffsetY.AutoSize = true;
            this.lblOffsetY.Location = new System.Drawing.Point(38, 197);
            this.lblOffsetY.Name = "lblOffsetY";
            this.lblOffsetY.Size = new System.Drawing.Size(38, 12);
            this.lblOffsetY.TabIndex = 5;
            this.lblOffsetY.Text = "label5";
            // 
            // lblOffsetX
            // 
            this.lblOffsetX.AutoSize = true;
            this.lblOffsetX.Location = new System.Drawing.Point(36, 111);
            this.lblOffsetX.Name = "lblOffsetX";
            this.lblOffsetX.Size = new System.Drawing.Size(38, 12);
            this.lblOffsetX.TabIndex = 4;
            this.lblOffsetX.Text = "label1";
            // 
            // trackOffsetX
            // 
            this.trackOffsetX.Location = new System.Drawing.Point(38, 131);
            this.trackOffsetX.Maximum = 30;
            this.trackOffsetX.Name = "trackOffsetX";
            this.trackOffsetX.Size = new System.Drawing.Size(266, 45);
            this.trackOffsetX.TabIndex = 3;
            // 
            // lblOpacity
            // 
            this.lblOpacity.AutoSize = true;
            this.lblOpacity.Location = new System.Drawing.Point(36, 30);
            this.lblOpacity.Name = "lblOpacity";
            this.lblOpacity.Size = new System.Drawing.Size(61, 12);
            this.lblOpacity.TabIndex = 1;
            this.lblOpacity.Text = "lblOpacity";
            // 
            // trackOpacity
            // 
            this.trackOpacity.Location = new System.Drawing.Point(40, 49);
            this.trackOpacity.Maximum = 100;
            this.trackOpacity.Minimum = 10;
            this.trackOpacity.Name = "trackOpacity";
            this.trackOpacity.Size = new System.Drawing.Size(266, 45);
            this.trackOpacity.TabIndex = 0;
            this.trackOpacity.Value = 70;
            this.trackOpacity.Scroll += new System.EventHandler(this.TrackOpacity_Scroll);
            // 
            // tabAbout
            // 
            this.tabAbout.Controls.Add(this.label2);
            this.tabAbout.Controls.Add(this.tableLayoutPanel1);
            this.tabAbout.Location = new System.Drawing.Point(4, 22);
            this.tabAbout.Name = "tabAbout";
            this.tabAbout.Padding = new System.Windows.Forms.Padding(3);
            this.tabAbout.Size = new System.Drawing.Size(792, 424);
            this.tabAbout.TabIndex = 1;
            this.tabAbout.Text = "About";
            this.tabAbout.UseVisualStyleBackColor = true;
            this.tabAbout.Click += new System.EventHandler(this.tabPage2_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(347, 177);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(0, 12);
            this.label2.TabIndex = 1;
            this.label2.Click += new System.EventHandler(this.label2_Click);
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Controls.Add(this.label3, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.labelVersion, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.linkLabelWebsite, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.linkLabelEmail, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.linkLabel1, 0, 4);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(3, 3);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 5;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 82.57262F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 17.42739F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 79F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 31F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 86F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(786, 418);
            this.tableLayoutPanel1.TabIndex = 7;
            this.tableLayoutPanel1.Paint += new System.Windows.Forms.PaintEventHandler(this.tableLayoutPanel1_Paint);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label3.Font = new System.Drawing.Font("굴림", 11F);
            this.label3.Location = new System.Drawing.Point(3, 183);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(780, 38);
            this.label3.TabIndex = 2;
            this.label3.Text = "최강규 제작 ";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.label3.Click += new System.EventHandler(this.label3_Click);
            // 
            // labelVersion
            // 
            this.labelVersion.Font = new System.Drawing.Font("굴림", 11F);
            this.labelVersion.Location = new System.Drawing.Point(3, 0);
            this.labelVersion.Name = "labelVersion";
            this.labelVersion.Size = new System.Drawing.Size(780, 183);
            this.labelVersion.TabIndex = 5;
            this.labelVersion.Text = "labelVersion";
            this.labelVersion.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.labelVersion.Click += new System.EventHandler(this.labelVersion_Click);
            // 
            // linkLabelWebsite
            // 
            this.linkLabelWebsite.AutoSize = true;
            this.linkLabelWebsite.Dock = System.Windows.Forms.DockStyle.Fill;
            this.linkLabelWebsite.Font = new System.Drawing.Font("굴림", 11F);
            this.linkLabelWebsite.Location = new System.Drawing.Point(3, 300);
            this.linkLabelWebsite.Name = "linkLabelWebsite";
            this.linkLabelWebsite.Size = new System.Drawing.Size(780, 31);
            this.linkLabelWebsite.TabIndex = 4;
            this.linkLabelWebsite.TabStop = true;
            this.linkLabelWebsite.Text = "https://flagtip.com/";
            this.linkLabelWebsite.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.linkLabelWebsite.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel1_LinkClicked);
            // 
            // linkLabelEmail
            // 
            this.linkLabelEmail.AutoSize = true;
            this.linkLabelEmail.Cursor = System.Windows.Forms.Cursors.Hand;
            this.linkLabelEmail.Dock = System.Windows.Forms.DockStyle.Fill;
            this.linkLabelEmail.LinkBehavior = System.Windows.Forms.LinkBehavior.AlwaysUnderline;
            this.linkLabelEmail.Location = new System.Drawing.Point(3, 221);
            this.linkLabelEmail.Name = "linkLabelEmail";
            this.linkLabelEmail.Size = new System.Drawing.Size(780, 79);
            this.linkLabelEmail.TabIndex = 7;
            this.linkLabelEmail.TabStop = true;
            this.linkLabelEmail.Text = "1@twepo.com";
            this.linkLabelEmail.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.linkLabelEmail.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.labelEmail_Click);
            // 
            // linkLabel1
            // 
            this.linkLabel1.AutoSize = true;
            this.linkLabel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.linkLabel1.Location = new System.Drawing.Point(3, 331);
            this.linkLabel1.Name = "linkLabel1";
            this.linkLabel1.Size = new System.Drawing.Size(780, 87);
            this.linkLabel1.TabIndex = 8;
            this.linkLabel1.TabStop = true;
            this.linkLabel1.Text = "https://github.com/gobrisebane/FlagTip";
            this.linkLabel1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.linkLabel1.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel1_LinkClicked_1);
            // 
            // SettingsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.tabControl1);
            this.Name = "SettingsForm";
            this.Text = "SettingsForm";
            this.tabControl1.ResumeLayout(false);
            this.tabOption.ResumeLayout(false);
            this.tabOption.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackOffsetY)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackOffsetX)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackOpacity)).EndInit();
            this.tabAbout.ResumeLayout(false);
            this.tabAbout.PerformLayout();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabOption;
        private System.Windows.Forms.TabPage tabAbout;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.LinkLabel linkLabelWebsite;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label labelVersion;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label lblOpacity;
        private System.Windows.Forms.TrackBar trackOpacity;
        private System.Windows.Forms.TrackBar trackOffsetX;
        private System.Windows.Forms.TrackBar trackOffsetY;
        private System.Windows.Forms.Label lblOffsetY;
        private System.Windows.Forms.Label lblOffsetX;
        private System.Windows.Forms.LinkLabel linkLabelEmail;
        private System.Windows.Forms.LinkLabel linkLabel1;
        private System.Windows.Forms.LinkLabel chromeErrorLink;
        private System.Windows.Forms.CheckBox chkFollowCursor;
    }
}