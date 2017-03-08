namespace LiveSplit.HollowKnight {
	partial class HollowKnightInfo {
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

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(HollowKnightInfo));
			this.chkShowEnemyHP = new System.Windows.Forms.CheckBox();
			this.lblNote = new System.Windows.Forms.Label();
			this.lblSceneName = new System.Windows.Forms.Label();
			this.lblUIState = new System.Windows.Forms.Label();
			this.lblMenuState = new System.Windows.Forms.Label();
			this.lblGameState = new System.Windows.Forms.Label();
			this.lblCameraMode = new System.Windows.Forms.Label();
			this.btnEnablePause = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// chkShowEnemyHP
			// 
			this.chkShowEnemyHP.AutoSize = true;
			this.chkShowEnemyHP.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.chkShowEnemyHP.Location = new System.Drawing.Point(60, 112);
			this.chkShowEnemyHP.Name = "chkShowEnemyHP";
			this.chkShowEnemyHP.Size = new System.Drawing.Size(259, 24);
			this.chkShowEnemyHP.TabIndex = 6;
			this.chkShowEnemyHP.Text = "Show Enemy HP in Geo Counter";
			this.chkShowEnemyHP.UseVisualStyleBackColor = true;
			this.chkShowEnemyHP.CheckedChanged += new System.EventHandler(this.chkShowEnemyHP_CheckedChanged);
			// 
			// lblNote
			// 
			this.lblNote.Dock = System.Windows.Forms.DockStyle.Fill;
			this.lblNote.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lblNote.Location = new System.Drawing.Point(0, 0);
			this.lblNote.Name = "lblNote";
			this.lblNote.Size = new System.Drawing.Size(390, 175);
			this.lblNote.TabIndex = 0;
			this.lblNote.Text = "Not available";
			this.lblNote.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// lblSceneName
			// 
			this.lblSceneName.AutoSize = true;
			this.lblSceneName.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lblSceneName.Location = new System.Drawing.Point(56, 89);
			this.lblSceneName.Name = "lblSceneName";
			this.lblSceneName.Size = new System.Drawing.Size(63, 20);
			this.lblSceneName.TabIndex = 5;
			this.lblSceneName.Text = "Scene: ";
			// 
			// lblUIState
			// 
			this.lblUIState.AutoSize = true;
			this.lblUIState.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lblUIState.Location = new System.Drawing.Point(40, 9);
			this.lblUIState.Name = "lblUIState";
			this.lblUIState.Size = new System.Drawing.Size(73, 20);
			this.lblUIState.TabIndex = 1;
			this.lblUIState.Text = "UI State:";
			// 
			// lblMenuState
			// 
			this.lblMenuState.AutoSize = true;
			this.lblMenuState.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lblMenuState.Location = new System.Drawing.Point(18, 29);
			this.lblMenuState.Name = "lblMenuState";
			this.lblMenuState.Size = new System.Drawing.Size(100, 20);
			this.lblMenuState.TabIndex = 2;
			this.lblMenuState.Text = "Menu State: ";
			// 
			// lblGameState
			// 
			this.lblGameState.AutoSize = true;
			this.lblGameState.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lblGameState.Location = new System.Drawing.Point(15, 49);
			this.lblGameState.Name = "lblGameState";
			this.lblGameState.Size = new System.Drawing.Size(104, 20);
			this.lblGameState.TabIndex = 3;
			this.lblGameState.Text = "Game State: ";
			// 
			// lblCameraMode
			// 
			this.lblCameraMode.AutoSize = true;
			this.lblCameraMode.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lblCameraMode.Location = new System.Drawing.Point(3, 69);
			this.lblCameraMode.Name = "lblCameraMode";
			this.lblCameraMode.Size = new System.Drawing.Size(117, 20);
			this.lblCameraMode.TabIndex = 4;
			this.lblCameraMode.Text = "Camera Mode: ";
			// 
			// btnEnablePause
			// 
			this.btnEnablePause.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.btnEnablePause.Location = new System.Drawing.Point(60, 138);
			this.btnEnablePause.Name = "btnEnablePause";
			this.btnEnablePause.Size = new System.Drawing.Size(127, 27);
			this.btnEnablePause.TabIndex = 7;
			this.btnEnablePause.Text = "Enable Pause";
			this.btnEnablePause.UseVisualStyleBackColor = true;
			this.btnEnablePause.Click += new System.EventHandler(this.btnEnablePause_Click);
			// 
			// HollowKnightInfo
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(390, 175);
			this.Controls.Add(this.lblNote);
			this.Controls.Add(this.btnEnablePause);
			this.Controls.Add(this.lblCameraMode);
			this.Controls.Add(this.lblGameState);
			this.Controls.Add(this.lblMenuState);
			this.Controls.Add(this.lblUIState);
			this.Controls.Add(this.lblSceneName);
			this.Controls.Add(this.chkShowEnemyHP);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "HollowKnightInfo";
			this.Text = "Hollow Knight Info";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.CheckBox chkShowEnemyHP;
		private System.Windows.Forms.Label lblNote;
		private System.Windows.Forms.Label lblSceneName;
		private System.Windows.Forms.Label lblUIState;
		private System.Windows.Forms.Label lblMenuState;
		private System.Windows.Forms.Label lblGameState;
		private System.Windows.Forms.Label lblCameraMode;
		private System.Windows.Forms.Button btnEnablePause;
	}
}