﻿namespace LiveSplit.HollowKnight {
    partial class HollowKnightSettings {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.components = new System.ComponentModel.Container();
            this.btnAddSplit = new System.Windows.Forms.Button();
            this.flowMain = new System.Windows.Forms.FlowLayoutPanel();
            this.flowOptions = new System.Windows.Forms.FlowLayoutPanel();
            this.Options_GroupBox = new System.Windows.Forms.GroupBox();
            this.versionLabel = new System.Windows.Forms.Label();
            this.RunBehaviour_GroupBox = new System.Windows.Forms.GroupBox();
            this.cboStartTriggerName = new System.Windows.Forms.ComboBox();
            this.chkAutosplitStartRuns = new System.Windows.Forms.CheckBox();
            this.chkOrdered = new System.Windows.Forms.CheckBox();
            this.chkAutosplitEndRuns = new System.Windows.Forms.CheckBox();
            this.SortBy_GroupBox = new System.Windows.Forms.GroupBox();
            this.rdAlpha = new System.Windows.Forms.RadioButton();
            this.rdType = new System.Windows.Forms.RadioButton();
            this.ToolTips = new System.Windows.Forms.ToolTip(this.components);
            this.flowMain.SuspendLayout();
            this.flowOptions.SuspendLayout();
            this.Options_GroupBox.SuspendLayout();
            this.RunBehaviour_GroupBox.SuspendLayout();
            this.SortBy_GroupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnAddSplit
            // 
            this.btnAddSplit.Location = new System.Drawing.Point(6, 92);
            this.btnAddSplit.Name = "btnAddSplit";
            this.btnAddSplit.Size = new System.Drawing.Size(57, 21);
            this.btnAddSplit.TabIndex = 0;
            this.btnAddSplit.Text = "增加";
            this.ToolTips.SetToolTip(this.btnAddSplit, "游戏结束时会自动结束计时，你不需要额外加一个“游戏结束”的事件");
            this.btnAddSplit.UseVisualStyleBackColor = true;
            this.btnAddSplit.Click += new System.EventHandler(this.btnAddSplit_Click);
            // 
            // flowMain
            // 
            this.flowMain.AllowDrop = true;
            this.flowMain.AutoSize = true;
            this.flowMain.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.flowMain.Controls.Add(this.flowOptions);
            this.flowMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowMain.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flowMain.Location = new System.Drawing.Point(0, 0);
            this.flowMain.Margin = new System.Windows.Forms.Padding(0);
            this.flowMain.Name = "flowMain";
            this.flowMain.Size = new System.Drawing.Size(456, 129);
            this.flowMain.TabIndex = 0;
            this.flowMain.WrapContents = false;
            this.flowMain.DragDrop += new System.Windows.Forms.DragEventHandler(this.flowMain_DragDrop);
            this.flowMain.DragEnter += new System.Windows.Forms.DragEventHandler(this.flowMain_DragEnter);
            this.flowMain.DragOver += new System.Windows.Forms.DragEventHandler(this.flowMain_DragOver);
            // 
            // flowOptions
            // 
            this.flowOptions.AutoSize = true;
            this.flowOptions.Controls.Add(this.Options_GroupBox);
            this.flowOptions.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flowOptions.Location = new System.Drawing.Point(0, 0);
            this.flowOptions.Margin = new System.Windows.Forms.Padding(0);
            this.flowOptions.Name = "flowOptions";
            this.flowOptions.Size = new System.Drawing.Size(456, 129);
            this.flowOptions.TabIndex = 0;
            // 
            // Options_GroupBox
            // 
            this.Options_GroupBox.Controls.Add(this.btnAddSplit);
            this.Options_GroupBox.Controls.Add(this.versionLabel);
            this.Options_GroupBox.Controls.Add(this.RunBehaviour_GroupBox);
            this.Options_GroupBox.Controls.Add(this.SortBy_GroupBox);
            this.Options_GroupBox.Location = new System.Drawing.Point(3, 3);
            this.Options_GroupBox.Name = "Options_GroupBox";
            this.Options_GroupBox.Size = new System.Drawing.Size(450, 123);
            this.Options_GroupBox.TabIndex = 6;
            this.Options_GroupBox.TabStop = false;
            this.Options_GroupBox.Text = "选项";
            // 
            // versionLabel
            // 
            this.versionLabel.Location = new System.Drawing.Point(262, 89);
            this.versionLabel.Name = "versionLabel";
            this.versionLabel.Size = new System.Drawing.Size(182, 24);
            this.versionLabel.TabIndex = 7;
            this.versionLabel.Text = "Autosplitter Version: x.x.x.x";
            this.versionLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // RunBehaviour_GroupBox
            // 
            this.RunBehaviour_GroupBox.Controls.Add(this.cboStartTriggerName);
            this.RunBehaviour_GroupBox.Controls.Add(this.chkAutosplitStartRuns);
            this.RunBehaviour_GroupBox.Controls.Add(this.chkOrdered);
            this.RunBehaviour_GroupBox.Controls.Add(this.chkAutosplitEndRuns);
            this.RunBehaviour_GroupBox.Location = new System.Drawing.Point(143, 15);
            this.RunBehaviour_GroupBox.Name = "RunBehaviour_GroupBox";
            this.RunBehaviour_GroupBox.Size = new System.Drawing.Size(301, 71);
            this.RunBehaviour_GroupBox.TabIndex = 7;
            this.RunBehaviour_GroupBox.TabStop = false;
            this.RunBehaviour_GroupBox.Text = "行为";
            // 
            // cboStartTriggerName
            // 
            this.cboStartTriggerName.Enabled = false;
            this.cboStartTriggerName.FormattingEnabled = true;
            this.cboStartTriggerName.Location = new System.Drawing.Point(148, 42);
            this.cboStartTriggerName.Name = "cboStartTriggerName";
            this.cboStartTriggerName.Size = new System.Drawing.Size(145, 21);
            this.cboStartTriggerName.TabIndex = 7;
            // 
            // chkAutosplitStartRuns
            // 
            this.chkAutosplitStartRuns.AutoSize = true;
            this.chkAutosplitStartRuns.Location = new System.Drawing.Point(6, 42);
            this.chkAutosplitStartRuns.Name = "chkAutosplitStartRuns";
            this.chkAutosplitStartRuns.Size = new System.Drawing.Size(139, 17);
            this.chkAutosplitStartRuns.TabIndex = 6;
            this.chkAutosplitStartRuns.Text = "自动触发开始计时：";
            this.ToolTips.SetToolTip(this.chkAutosplitStartRuns, "指定的事件会自动触发开始计时。用于非流程速通");
            this.chkAutosplitStartRuns.UseVisualStyleBackColor = true;
            this.chkAutosplitStartRuns.CheckedChanged += new System.EventHandler(this.AutosplitStartChanged);
            // 
            // chkOrdered
            // 
            this.chkOrdered.Location = new System.Drawing.Point(6, 17);
            this.chkOrdered.Name = "chkOrdered";
            this.chkOrdered.Size = new System.Drawing.Size(92, 19);
            this.chkOrdered.TabIndex = 4;
            this.chkOrdered.Text = "按顺序";
            this.ToolTips.SetToolTip(this.chkOrdered, "当进行万神殿速通或含有切换场景片段时必须勾选");
            this.chkOrdered.UseVisualStyleBackColor = true;
            this.chkOrdered.CheckedChanged += new System.EventHandler(this.ControlChanged);
            // 
            // chkAutosplitEndRuns
            // 
            this.chkAutosplitEndRuns.AutoEllipsis = true;
            this.chkAutosplitEndRuns.Location = new System.Drawing.Point(148, 17);
            this.chkAutosplitEndRuns.Name = "chkAutosplitEndRuns";
            this.chkAutosplitEndRuns.Size = new System.Drawing.Size(133, 19);
            this.chkAutosplitEndRuns.TabIndex = 5;
            this.chkAutosplitEndRuns.Text = "自动触发结束计时";
            this.ToolTips.SetToolTip(this.chkAutosplitEndRuns, "游戏结束时本来就会自动结束计时。勾上这个选项后，当进行到最后一个片段时，任意一个配置的事件都可以自动结束计时");
            this.chkAutosplitEndRuns.UseVisualStyleBackColor = true;
            this.chkAutosplitEndRuns.CheckedChanged += new System.EventHandler(this.AutosplitEndChanged);
            // 
            // SortBy_GroupBox
            // 
            this.SortBy_GroupBox.Controls.Add(this.rdAlpha);
            this.SortBy_GroupBox.Controls.Add(this.rdType);
            this.SortBy_GroupBox.Location = new System.Drawing.Point(6, 15);
            this.SortBy_GroupBox.Name = "SortBy_GroupBox";
            this.SortBy_GroupBox.Size = new System.Drawing.Size(131, 71);
            this.SortBy_GroupBox.TabIndex = 6;
            this.SortBy_GroupBox.TabStop = false;
            this.SortBy_GroupBox.Text = "筛选器排序方式";
            // 
            // rdAlpha
            // 
            this.rdAlpha.AutoSize = true;
            this.rdAlpha.Location = new System.Drawing.Point(6, 42);
            this.rdAlpha.Name = "rdAlpha";
            this.rdAlpha.Size = new System.Drawing.Size(83, 17);
            this.rdAlpha.TabIndex = 3;
            this.rdAlpha.Text = "相同的字放在一起";
            this.rdAlpha.UseVisualStyleBackColor = true;
            this.rdAlpha.CheckedChanged += new System.EventHandler(this.radio_CheckedChanged);
            // 
            // rdType
            // 
            this.rdType.AutoSize = true;
            this.rdType.Checked = true;
            this.rdType.Location = new System.Drawing.Point(6, 19);
            this.rdType.Name = "rdType";
            this.rdType.Size = new System.Drawing.Size(49, 17);
            this.rdType.TabIndex = 2;
            this.rdType.TabStop = true;
            this.rdType.Text = "按类型排序";
            this.rdType.UseVisualStyleBackColor = true;
            this.rdType.CheckedChanged += new System.EventHandler(this.radio_CheckedChanged);
            // 
            // ToolTips
            // 
            this.ToolTips.ShowAlways = true;
            // 
            // HollowKnightSettings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.Controls.Add(this.flowMain);
            this.Margin = new System.Windows.Forms.Padding(0);
            this.Name = "HollowKnightSettings";
            this.Size = new System.Drawing.Size(456, 129);
            this.Load += new System.EventHandler(this.Settings_Load);
            this.flowMain.ResumeLayout(false);
            this.flowMain.PerformLayout();
            this.flowOptions.ResumeLayout(false);
            this.Options_GroupBox.ResumeLayout(false);
            this.RunBehaviour_GroupBox.ResumeLayout(false);
            this.RunBehaviour_GroupBox.PerformLayout();
            this.SortBy_GroupBox.ResumeLayout(false);
            this.SortBy_GroupBox.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button btnAddSplit;
        private System.Windows.Forms.FlowLayoutPanel flowMain;
        private System.Windows.Forms.FlowLayoutPanel flowOptions;
        private System.Windows.Forms.RadioButton rdType;
        private System.Windows.Forms.RadioButton rdAlpha;
        private System.Windows.Forms.CheckBox chkOrdered;
        private System.Windows.Forms.CheckBox chkAutosplitEndRuns;
        private System.Windows.Forms.GroupBox Options_GroupBox;
        private System.Windows.Forms.ToolTip ToolTips;
        private System.Windows.Forms.Label versionLabel;
        private System.Windows.Forms.GroupBox RunBehaviour_GroupBox;
        private System.Windows.Forms.GroupBox SortBy_GroupBox;
        private System.Windows.Forms.CheckBox chkAutosplitStartRuns;
        private System.Windows.Forms.ComboBox cboStartTriggerName;
    }
}
