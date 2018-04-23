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
			this.chkInfiniteHP = new System.Windows.Forms.CheckBox();
			this.chkInfiniteSoul = new System.Windows.Forms.CheckBox();
			this.chkInvincible = new System.Windows.Forms.CheckBox();
			this.lblPosition = new System.Windows.Forms.Label();
			this.lblTargetMode = new System.Windows.Forms.Label();
			this.chkCameraTarget = new System.Windows.Forms.CheckBox();
			this.chkLockZoom = new System.Windows.Forms.CheckBox();
			this.zoomValue = new System.Windows.Forms.TrackBar();
			this.chkEnemyInvincible = new System.Windows.Forms.CheckBox();
			this.btnDebugInfo = new System.Windows.Forms.Button();
			((System.ComponentModel.ISupportInitialize)(this.zoomValue)).BeginInit();
			this.SuspendLayout();
			// 
			// chkShowEnemyHP
			// 
			this.chkShowEnemyHP.AutoSize = true;
			this.chkShowEnemyHP.Font = new System.Drawing.Font("Courier New", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.chkShowEnemyHP.Location = new System.Drawing.Point(41, 152);
			this.chkShowEnemyHP.Name = "chkShowEnemyHP";
			this.chkShowEnemyHP.Size = new System.Drawing.Size(251, 20);
			this.chkShowEnemyHP.TabIndex = 8;
			this.chkShowEnemyHP.Text = "Show Enemy HP in Geo Counter";
			this.chkShowEnemyHP.UseVisualStyleBackColor = true;
			this.chkShowEnemyHP.Visible = false;
			this.chkShowEnemyHP.CheckedChanged += new System.EventHandler(this.chkShowEnemyHP_CheckedChanged);
			// 
			// lblNote
			// 
			this.lblNote.Dock = System.Windows.Forms.DockStyle.Fill;
			this.lblNote.Font = new System.Drawing.Font("Courier New", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lblNote.Location = new System.Drawing.Point(0, 0);
			this.lblNote.Name = "lblNote";
			this.lblNote.Size = new System.Drawing.Size(598, 271);
			this.lblNote.TabIndex = 0;
			this.lblNote.Text = "Not available";
			this.lblNote.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// lblSceneName
			// 
			this.lblSceneName.AutoSize = true;
			this.lblSceneName.Font = new System.Drawing.Font("Courier New", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lblSceneName.Location = new System.Drawing.Point(61, 109);
			this.lblSceneName.Name = "lblSceneName";
			this.lblSceneName.Size = new System.Drawing.Size(64, 16);
			this.lblSceneName.TabIndex = 6;
			this.lblSceneName.Text = "Scene: ";
			// 
			// lblUIState
			// 
			this.lblUIState.AutoSize = true;
			this.lblUIState.Font = new System.Drawing.Font("Courier New", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lblUIState.Location = new System.Drawing.Point(37, 9);
			this.lblUIState.Name = "lblUIState";
			this.lblUIState.Size = new System.Drawing.Size(80, 16);
			this.lblUIState.TabIndex = 1;
			this.lblUIState.Text = "UI State:";
			// 
			// lblMenuState
			// 
			this.lblMenuState.AutoSize = true;
			this.lblMenuState.Font = new System.Drawing.Font("Courier New", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lblMenuState.Location = new System.Drawing.Point(21, 29);
			this.lblMenuState.Name = "lblMenuState";
			this.lblMenuState.Size = new System.Drawing.Size(104, 16);
			this.lblMenuState.TabIndex = 2;
			this.lblMenuState.Text = "Hero State: ";
			// 
			// lblGameState
			// 
			this.lblGameState.AutoSize = true;
			this.lblGameState.Font = new System.Drawing.Font("Courier New", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lblGameState.Location = new System.Drawing.Point(21, 49);
			this.lblGameState.Name = "lblGameState";
			this.lblGameState.Size = new System.Drawing.Size(104, 16);
			this.lblGameState.TabIndex = 3;
			this.lblGameState.Text = "Game State: ";
			// 
			// lblCameraMode
			// 
			this.lblCameraMode.AutoSize = true;
			this.lblCameraMode.Font = new System.Drawing.Font("Courier New", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lblCameraMode.Location = new System.Drawing.Point(13, 69);
			this.lblCameraMode.Name = "lblCameraMode";
			this.lblCameraMode.Size = new System.Drawing.Size(112, 16);
			this.lblCameraMode.TabIndex = 4;
			this.lblCameraMode.Text = "Camera Mode: ";
			// 
			// btnEnablePause
			// 
			this.btnEnablePause.Font = new System.Drawing.Font("Courier New", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.btnEnablePause.Location = new System.Drawing.Point(41, 231);
			this.btnEnablePause.Name = "btnEnablePause";
			this.btnEnablePause.Size = new System.Drawing.Size(127, 27);
			this.btnEnablePause.TabIndex = 15;
			this.btnEnablePause.Text = "Enable Pause";
			this.btnEnablePause.UseVisualStyleBackColor = true;
			this.btnEnablePause.Click += new System.EventHandler(this.btnEnablePause_Click);
			// 
			// chkInfiniteHP
			// 
			this.chkInfiniteHP.AutoSize = true;
			this.chkInfiniteHP.Font = new System.Drawing.Font("Courier New", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.chkInfiniteHP.Location = new System.Drawing.Point(41, 177);
			this.chkInfiniteHP.Name = "chkInfiniteHP";
			this.chkInfiniteHP.Size = new System.Drawing.Size(115, 20);
			this.chkInfiniteHP.TabIndex = 9;
			this.chkInfiniteHP.Text = "Infinite HP";
			this.chkInfiniteHP.UseVisualStyleBackColor = true;
			// 
			// chkInfiniteSoul
			// 
			this.chkInfiniteSoul.AutoSize = true;
			this.chkInfiniteSoul.Font = new System.Drawing.Font("Courier New", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.chkInfiniteSoul.Location = new System.Drawing.Point(174, 177);
			this.chkInfiniteSoul.Name = "chkInfiniteSoul";
			this.chkInfiniteSoul.Size = new System.Drawing.Size(131, 20);
			this.chkInfiniteSoul.TabIndex = 11;
			this.chkInfiniteSoul.Text = "Infinite Soul";
			this.chkInfiniteSoul.UseVisualStyleBackColor = true;
			// 
			// chkInvincible
			// 
			this.chkInvincible.AutoSize = true;
			this.chkInvincible.Font = new System.Drawing.Font("Courier New", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.chkInvincible.Location = new System.Drawing.Point(41, 202);
			this.chkInvincible.Name = "chkInvincible";
			this.chkInvincible.Size = new System.Drawing.Size(107, 20);
			this.chkInvincible.TabIndex = 10;
			this.chkInvincible.Text = "Invincible";
			this.chkInvincible.UseVisualStyleBackColor = true;
			this.chkInvincible.CheckedChanged += new System.EventHandler(this.chkInvincible_CheckedChanged);
			// 
			// lblPosition
			// 
			this.lblPosition.AutoSize = true;
			this.lblPosition.Font = new System.Drawing.Font("Courier New", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lblPosition.Location = new System.Drawing.Point(37, 129);
			this.lblPosition.Name = "lblPosition";
			this.lblPosition.Size = new System.Drawing.Size(88, 16);
			this.lblPosition.TabIndex = 7;
			this.lblPosition.Text = "Position: ";
			// 
			// lblTargetMode
			// 
			this.lblTargetMode.AutoSize = true;
			this.lblTargetMode.Font = new System.Drawing.Font("Courier New", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lblTargetMode.Location = new System.Drawing.Point(13, 89);
			this.lblTargetMode.Name = "lblTargetMode";
			this.lblTargetMode.Size = new System.Drawing.Size(112, 16);
			this.lblTargetMode.TabIndex = 5;
			this.lblTargetMode.Text = "Target Mode: ";
			// 
			// chkCameraTarget
			// 
			this.chkCameraTarget.AutoSize = true;
			this.chkCameraTarget.Font = new System.Drawing.Font("Courier New", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.chkCameraTarget.Location = new System.Drawing.Point(174, 202);
			this.chkCameraTarget.Name = "chkCameraTarget";
			this.chkCameraTarget.Size = new System.Drawing.Size(187, 20);
			this.chkCameraTarget.TabIndex = 12;
			this.chkCameraTarget.Text = "Set Camera To Follow";
			this.chkCameraTarget.UseVisualStyleBackColor = true;
			this.chkCameraTarget.CheckedChanged += new System.EventHandler(this.chkCameraTarget_CheckedChanged);
			// 
			// chkLockZoom
			// 
			this.chkLockZoom.AutoSize = true;
			this.chkLockZoom.Font = new System.Drawing.Font("Courier New", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.chkLockZoom.Location = new System.Drawing.Point(174, 231);
			this.chkLockZoom.Name = "chkLockZoom";
			this.chkLockZoom.Size = new System.Drawing.Size(115, 20);
			this.chkLockZoom.TabIndex = 16;
			this.chkLockZoom.Text = "Zoom (1.00)";
			this.chkLockZoom.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.chkLockZoom.UseVisualStyleBackColor = true;
			this.chkLockZoom.CheckedChanged += new System.EventHandler(this.chkLockZoom_CheckedChanged);
			// 
			// zoomValue
			// 
			this.zoomValue.Enabled = false;
			this.zoomValue.Location = new System.Drawing.Point(281, 231);
			this.zoomValue.Maximum = 400;
			this.zoomValue.Minimum = 28;
			this.zoomValue.Name = "zoomValue";
			this.zoomValue.Size = new System.Drawing.Size(232, 45);
			this.zoomValue.TabIndex = 17;
			this.zoomValue.TickFrequency = 5;
			this.zoomValue.TickStyle = System.Windows.Forms.TickStyle.TopLeft;
			this.zoomValue.Value = 200;
			this.zoomValue.Scroll += new System.EventHandler(this.zoomValue_Scroll);
			// 
			// chkEnemyInvincible
			// 
			this.chkEnemyInvincible.AutoSize = true;
			this.chkEnemyInvincible.Enabled = false;
			this.chkEnemyInvincible.Font = new System.Drawing.Font("Courier New", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.chkEnemyInvincible.Location = new System.Drawing.Point(369, 152);
			this.chkEnemyInvincible.Name = "chkEnemyInvincible";
			this.chkEnemyInvincible.Size = new System.Drawing.Size(195, 20);
			this.chkEnemyInvincible.TabIndex = 13;
			this.chkEnemyInvincible.Text = "Make Enemy Invincible";
			this.chkEnemyInvincible.UseVisualStyleBackColor = true;
			this.chkEnemyInvincible.Visible = false;
			// 
			// btnDebugInfo
			// 
			this.btnDebugInfo.Font = new System.Drawing.Font("Courier New", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.btnDebugInfo.Location = new System.Drawing.Point(369, 173);
			this.btnDebugInfo.Name = "btnDebugInfo";
			this.btnDebugInfo.Size = new System.Drawing.Size(154, 27);
			this.btnDebugInfo.TabIndex = 14;
			this.btnDebugInfo.Text = "Toggle Debug Info";
			this.btnDebugInfo.UseVisualStyleBackColor = true;
			this.btnDebugInfo.Click += new System.EventHandler(this.btnDebugInfo_Click);
			// 
			// HollowKnightInfo
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(598, 271);
			this.Controls.Add(this.lblNote);
			this.Controls.Add(this.btnDebugInfo);
			this.Controls.Add(this.chkEnemyInvincible);
			this.Controls.Add(this.chkLockZoom);
			this.Controls.Add(this.zoomValue);
			this.Controls.Add(this.chkCameraTarget);
			this.Controls.Add(this.lblTargetMode);
			this.Controls.Add(this.lblPosition);
			this.Controls.Add(this.chkInvincible);
			this.Controls.Add(this.chkInfiniteSoul);
			this.Controls.Add(this.chkInfiniteHP);
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
			this.Name = "HollowKnightInfo";
			this.Text = "Hollow Knight Info";
			((System.ComponentModel.ISupportInitialize)(this.zoomValue)).EndInit();
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
		private System.Windows.Forms.CheckBox chkInfiniteHP;
		private System.Windows.Forms.CheckBox chkInfiniteSoul;
		private System.Windows.Forms.CheckBox chkInvincible;
		private System.Windows.Forms.Label lblPosition;
		private System.Windows.Forms.Label lblTargetMode;
		private System.Windows.Forms.CheckBox chkCameraTarget;
		private System.Windows.Forms.CheckBox chkLockZoom;
		private System.Windows.Forms.TrackBar zoomValue;
		private System.Windows.Forms.CheckBox chkEnemyInvincible;
		private System.Windows.Forms.Button btnDebugInfo;
	}
}