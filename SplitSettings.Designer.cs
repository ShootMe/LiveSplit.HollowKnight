namespace LiveSplit.HollowKnight {
	partial class SplitSettings {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SplitSettings));
			this.btnRemove = new System.Windows.Forms.Button();
			this.cboName = new System.Windows.Forms.ComboBox();
			this.ToolTips = new System.Windows.Forms.ToolTip(this.components);
			this.btnMoveUp = new System.Windows.Forms.Button();
			this.btnMoveDown = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// btnRemove
			// 
			this.btnRemove.Image = ((System.Drawing.Image)(resources.GetObject("btnRemove.Image")));
			this.btnRemove.Location = new System.Drawing.Point(255, 2);
			this.btnRemove.Name = "btnRemove";
			this.btnRemove.Size = new System.Drawing.Size(26, 23);
			this.btnRemove.TabIndex = 4;
			this.ToolTips.SetToolTip(this.btnRemove, "Remove this setting");
			this.btnRemove.UseVisualStyleBackColor = true;
			// 
			// cboName
			// 
			this.cboName.FormattingEnabled = true;
			this.cboName.Location = new System.Drawing.Point(3, 3);
			this.cboName.Name = "cboName";
			this.cboName.Size = new System.Drawing.Size(246, 21);
			this.cboName.TabIndex = 0;
			this.cboName.SelectedIndexChanged += new System.EventHandler(this.cboName_SelectedIndexChanged);
			// 
			// btnMoveUp
			// 
			this.btnMoveUp.BackgroundImage = global::LiveSplit.HollowKnight.Properties.Resources.up32;
			this.btnMoveUp.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
			this.btnMoveUp.ForeColor = System.Drawing.SystemColors.ControlText;
			this.btnMoveUp.Location = new System.Drawing.Point(287, 2);
			this.btnMoveUp.Name = "btnMoveUp";
			this.btnMoveUp.Size = new System.Drawing.Size(26, 23);
			this.btnMoveUp.TabIndex = 5;
			this.ToolTips.SetToolTip(this.btnMoveUp, "Remove this setting");
			this.btnMoveUp.UseVisualStyleBackColor = true;
			// 
			// btnMoveDown
			// 
			this.btnMoveDown.BackgroundImage = global::LiveSplit.HollowKnight.Properties.Resources.down32;
			this.btnMoveDown.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
			this.btnMoveDown.Location = new System.Drawing.Point(319, 2);
			this.btnMoveDown.Name = "btnMoveDown";
			this.btnMoveDown.Size = new System.Drawing.Size(26, 23);
			this.btnMoveDown.TabIndex = 6;
			this.ToolTips.SetToolTip(this.btnMoveDown, "Remove this setting");
			this.btnMoveDown.UseVisualStyleBackColor = true;
			// 
			// SplitSettings
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.AutoSize = true;
			this.Controls.Add(this.btnMoveDown);
			this.Controls.Add(this.btnMoveUp);
			this.Controls.Add(this.btnRemove);
			this.Controls.Add(this.cboName);
			this.Margin = new System.Windows.Forms.Padding(0);
			this.Name = "SplitSettings";
			this.Size = new System.Drawing.Size(350, 29);
			this.ResumeLayout(false);

		}

		#endregion
		public System.Windows.Forms.Button btnRemove;
		public System.Windows.Forms.ComboBox cboName;
		private System.Windows.Forms.ToolTip ToolTips;
		public System.Windows.Forms.Button btnMoveUp;
		public System.Windows.Forms.Button btnMoveDown;
	}
}
