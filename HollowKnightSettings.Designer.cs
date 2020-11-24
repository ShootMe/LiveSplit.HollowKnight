namespace LiveSplit.HollowKnight {
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
            this.lblSort = new System.Windows.Forms.Label();
            this.chkOrdered = new System.Windows.Forms.CheckBox();
            this.rdAlpha = new System.Windows.Forms.RadioButton();
            this.chkAutosplitEndRuns = new System.Windows.Forms.CheckBox();
            this.rdType = new System.Windows.Forms.RadioButton();
            this.AutosplitEndSplits_ToolTip = new System.Windows.Forms.ToolTip(this.components);
            this.OrderedSplits_ToolTip = new System.Windows.Forms.ToolTip(this.components);
            this.flowMain.SuspendLayout();
            this.flowOptions.SuspendLayout();
            this.Options_GroupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnAddSplit
            // 
            this.btnAddSplit.Location = new System.Drawing.Point(3, 56);
            this.btnAddSplit.Name = "btnAddSplit";
            this.btnAddSplit.Size = new System.Drawing.Size(57, 21);
            this.btnAddSplit.TabIndex = 0;
            this.btnAddSplit.Text = "Add Split";
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
            this.flowMain.Size = new System.Drawing.Size(443, 80);
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
            this.flowOptions.Controls.Add(this.btnAddSplit);
            this.flowOptions.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flowOptions.Location = new System.Drawing.Point(0, 0);
            this.flowOptions.Margin = new System.Windows.Forms.Padding(0);
            this.flowOptions.Name = "flowOptions";
            this.flowOptions.Size = new System.Drawing.Size(443, 80);
            this.flowOptions.TabIndex = 0;
            // 
            // Options_GroupBox
            // 
            this.Options_GroupBox.Controls.Add(this.lblSort);
            this.Options_GroupBox.Controls.Add(this.chkOrdered);
            this.Options_GroupBox.Controls.Add(this.rdAlpha);
            this.Options_GroupBox.Controls.Add(this.chkAutosplitEndRuns);
            this.Options_GroupBox.Controls.Add(this.rdType);
            this.Options_GroupBox.Location = new System.Drawing.Point(3, 3);
            this.Options_GroupBox.Name = "Options_GroupBox";
            this.Options_GroupBox.Size = new System.Drawing.Size(437, 47);
            this.Options_GroupBox.TabIndex = 6;
            this.Options_GroupBox.TabStop = false;
            this.Options_GroupBox.Text = "Options";
            this.Options_GroupBox.Enter += new System.EventHandler(this.groupBox1_Enter);
            // 
            // lblSort
            // 
            this.lblSort.Location = new System.Drawing.Point(6, 15);
            this.lblSort.Name = "lblSort";
            this.lblSort.Size = new System.Drawing.Size(44, 23);
            this.lblSort.TabIndex = 1;
            this.lblSort.Text = "Sort By:";
            this.lblSort.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // chkOrdered
            // 
            this.chkOrdered.Location = new System.Drawing.Point(200, 19);
            this.chkOrdered.Name = "chkOrdered";
            this.chkOrdered.Size = new System.Drawing.Size(92, 19);
            this.chkOrdered.TabIndex = 4;
            this.chkOrdered.Text = "Ordered Splits";
            this.OrderedSplits_ToolTip.SetToolTip(this.chkOrdered, "Required for runs with Pantheon splits");
            this.chkOrdered.UseVisualStyleBackColor = true;
            this.chkOrdered.CheckedChanged += new System.EventHandler(this.ControlChanged);
            // 
            // rdAlpha
            // 
            this.rdAlpha.AutoSize = true;
            this.rdAlpha.Location = new System.Drawing.Point(111, 19);
            this.rdAlpha.Name = "rdAlpha";
            this.rdAlpha.Size = new System.Drawing.Size(83, 17);
            this.rdAlpha.TabIndex = 3;
            this.rdAlpha.Text = "Alphabetical";
            this.rdAlpha.UseVisualStyleBackColor = true;
            this.rdAlpha.CheckedChanged += new System.EventHandler(this.radio_CheckedChanged);
            // 
            // chkAutosplitEndRuns
            // 
            this.chkAutosplitEndRuns.AutoEllipsis = true;
            this.chkAutosplitEndRuns.Location = new System.Drawing.Point(298, 19);
            this.chkAutosplitEndRuns.Name = "chkAutosplitEndRuns";
            this.chkAutosplitEndRuns.Size = new System.Drawing.Size(133, 19);
            this.chkAutosplitEndRuns.TabIndex = 5;
            this.chkAutosplitEndRuns.Text = "End-triggering autosplit";
            this.AutosplitEndSplits_ToolTip.SetToolTip(this.chkAutosplitEndRuns, "Stop the timer on the last autosplit");
            this.chkAutosplitEndRuns.UseVisualStyleBackColor = true;
            this.chkAutosplitEndRuns.CheckedChanged += new System.EventHandler(this.AutosplitEndChanged);
            // 
            // rdType
            // 
            this.rdType.AutoSize = true;
            this.rdType.Checked = true;
            this.rdType.Location = new System.Drawing.Point(56, 19);
            this.rdType.Name = "rdType";
            this.rdType.Size = new System.Drawing.Size(49, 17);
            this.rdType.TabIndex = 2;
            this.rdType.TabStop = true;
            this.rdType.Text = "Type";
            this.rdType.UseVisualStyleBackColor = true;
            this.rdType.CheckedChanged += new System.EventHandler(this.radio_CheckedChanged);
            // 
            // AutosplitEndSplits_ToolTip
            // 
            this.AutosplitEndSplits_ToolTip.ShowAlways = true;
            this.AutosplitEndSplits_ToolTip.Popup += new System.Windows.Forms.PopupEventHandler(this.AutosplitEndPopup);
            // 
            // OrderedSplits_ToolTip
            // 
            this.OrderedSplits_ToolTip.ShowAlways = true;
            this.OrderedSplits_ToolTip.Popup += new System.Windows.Forms.PopupEventHandler(this.OrderedSplitsPopup);
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
            this.Size = new System.Drawing.Size(443, 80);
            this.Load += new System.EventHandler(this.Settings_Load);
            this.flowMain.ResumeLayout(false);
            this.flowMain.PerformLayout();
            this.flowOptions.ResumeLayout(false);
            this.Options_GroupBox.ResumeLayout(false);
            this.Options_GroupBox.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion
		private System.Windows.Forms.Button btnAddSplit;
		private System.Windows.Forms.FlowLayoutPanel flowMain;
		private System.Windows.Forms.FlowLayoutPanel flowOptions;
		private System.Windows.Forms.Label lblSort;
		private System.Windows.Forms.RadioButton rdType;
		private System.Windows.Forms.RadioButton rdAlpha;
		private System.Windows.Forms.CheckBox chkOrdered;
        private System.Windows.Forms.CheckBox chkAutosplitEndRuns;
        private System.Windows.Forms.ToolTip AutosplitEndSplits_ToolTip;
        private System.Windows.Forms.GroupBox Options_GroupBox;
        private System.Windows.Forms.ToolTip OrderedSplits_ToolTip;
    }
}
