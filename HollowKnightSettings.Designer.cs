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
			this.flowMain = new System.Windows.Forms.FlowLayoutPanel();
			this.flowOptions = new System.Windows.Forms.FlowLayoutPanel();
			this.chkFalseKnight = new System.Windows.Forms.CheckBox();
			this.chkMothwingCloak = new System.Windows.Forms.CheckBox();
			this.chkThornsOfAgony = new System.Windows.Forms.CheckBox();
			this.chkMantisClaw = new System.Windows.Forms.CheckBox();
			this.chkDistantVillageStation = new System.Windows.Forms.CheckBox();
			this.chkCrystalHeart = new System.Windows.Forms.CheckBox();
			this.chkGruzMother = new System.Windows.Forms.CheckBox();
			this.chkDreamNail = new System.Windows.Forms.CheckBox();
			this.chkNailUpgrade1 = new System.Windows.Forms.CheckBox();
			this.chkWatcherKnight = new System.Windows.Forms.CheckBox();
			this.chkLurien = new System.Windows.Forms.CheckBox();
			this.chkHegemol = new System.Windows.Forms.CheckBox();
			this.chkMonomon = new System.Windows.Forms.CheckBox();
			this.chkUumuu = new System.Windows.Forms.CheckBox();
			this.toolTips = new System.Windows.Forms.ToolTip(this.components);
			this.flowMain.SuspendLayout();
			this.flowOptions.SuspendLayout();
			this.SuspendLayout();
			// 
			// flowMain
			// 
			this.flowMain.AutoSize = true;
			this.flowMain.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.flowMain.Controls.Add(this.flowOptions);
			this.flowMain.Dock = System.Windows.Forms.DockStyle.Fill;
			this.flowMain.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
			this.flowMain.Location = new System.Drawing.Point(0, 0);
			this.flowMain.Margin = new System.Windows.Forms.Padding(0);
			this.flowMain.Name = "flowMain";
			this.flowMain.Size = new System.Drawing.Size(279, 330);
			this.flowMain.TabIndex = 0;
			this.flowMain.WrapContents = false;
			// 
			// flowOptions
			// 
			this.flowOptions.Controls.Add(this.chkFalseKnight);
			this.flowOptions.Controls.Add(this.chkMothwingCloak);
			this.flowOptions.Controls.Add(this.chkThornsOfAgony);
			this.flowOptions.Controls.Add(this.chkMantisClaw);
			this.flowOptions.Controls.Add(this.chkDistantVillageStation);
			this.flowOptions.Controls.Add(this.chkCrystalHeart);
			this.flowOptions.Controls.Add(this.chkGruzMother);
			this.flowOptions.Controls.Add(this.chkDreamNail);
			this.flowOptions.Controls.Add(this.chkNailUpgrade1);
			this.flowOptions.Controls.Add(this.chkWatcherKnight);
			this.flowOptions.Controls.Add(this.chkLurien);
			this.flowOptions.Controls.Add(this.chkHegemol);
			this.flowOptions.Controls.Add(this.chkMonomon);
			this.flowOptions.Controls.Add(this.chkUumuu);
			this.flowOptions.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
			this.flowOptions.Location = new System.Drawing.Point(0, 0);
			this.flowOptions.Margin = new System.Windows.Forms.Padding(0);
			this.flowOptions.Name = "flowOptions";
			this.flowOptions.Size = new System.Drawing.Size(279, 330);
			this.flowOptions.TabIndex = 0;
			// 
			// chkFalseKnight
			// 
			this.chkFalseKnight.AutoSize = true;
			this.chkFalseKnight.Location = new System.Drawing.Point(3, 3);
			this.chkFalseKnight.Name = "chkFalseKnight";
			this.chkFalseKnight.Size = new System.Drawing.Size(116, 17);
			this.chkFalseKnight.TabIndex = 0;
			this.chkFalseKnight.Text = "False Knight (Boss)";
			this.toolTips.SetToolTip(this.chkFalseKnight, "Splits after killing False Knight");
			this.chkFalseKnight.UseVisualStyleBackColor = true;
			this.chkFalseKnight.CheckedChanged += new System.EventHandler(this.chkBox_CheckedChanged);
			// 
			// chkMothwingCloak
			// 
			this.chkMothwingCloak.AutoSize = true;
			this.chkMothwingCloak.Location = new System.Drawing.Point(3, 26);
			this.chkMothwingCloak.Name = "chkMothwingCloak";
			this.chkMothwingCloak.Size = new System.Drawing.Size(136, 17);
			this.chkMothwingCloak.TabIndex = 1;
			this.chkMothwingCloak.Text = "Mothwing Cloak (Dash)";
			this.toolTips.SetToolTip(this.chkMothwingCloak, "Splits after obtaining Mothwing Cloak");
			this.chkMothwingCloak.UseVisualStyleBackColor = true;
			this.chkMothwingCloak.CheckedChanged += new System.EventHandler(this.chkBox_CheckedChanged);
			// 
			// chkThornsOfAgony
			// 
			this.chkThornsOfAgony.AutoSize = true;
			this.chkThornsOfAgony.Location = new System.Drawing.Point(3, 49);
			this.chkThornsOfAgony.Name = "chkThornsOfAgony";
			this.chkThornsOfAgony.Size = new System.Drawing.Size(145, 17);
			this.chkThornsOfAgony.TabIndex = 2;
			this.chkThornsOfAgony.Text = "Thorns Of Agony (Charm)";
			this.toolTips.SetToolTip(this.chkThornsOfAgony, "Splits after obtaining Thorns Of Agony charm");
			this.chkThornsOfAgony.UseVisualStyleBackColor = true;
			// 
			// chkMantisClaw
			// 
			this.chkMantisClaw.AutoSize = true;
			this.chkMantisClaw.Location = new System.Drawing.Point(3, 72);
			this.chkMantisClaw.Name = "chkMantisClaw";
			this.chkMantisClaw.Size = new System.Drawing.Size(141, 17);
			this.chkMantisClaw.TabIndex = 3;
			this.chkMantisClaw.Text = "Mantis Claw (Wall Jump)";
			this.toolTips.SetToolTip(this.chkMantisClaw, "Splits after obtaining the Mantis Claw");
			this.chkMantisClaw.UseVisualStyleBackColor = true;
			// 
			// chkDistantVillageStation
			// 
			this.chkDistantVillageStation.AutoSize = true;
			this.chkDistantVillageStation.Location = new System.Drawing.Point(3, 95);
			this.chkDistantVillageStation.Name = "chkDistantVillageStation";
			this.chkDistantVillageStation.Size = new System.Drawing.Size(154, 17);
			this.chkDistantVillageStation.TabIndex = 4;
			this.chkDistantVillageStation.Text = "Distant Village Stag Station";
			this.toolTips.SetToolTip(this.chkDistantVillageStation, "Splits after entering the Distant Village Stag Station scene");
			this.chkDistantVillageStation.UseVisualStyleBackColor = true;
			// 
			// chkCrystalHeart
			// 
			this.chkCrystalHeart.AutoSize = true;
			this.chkCrystalHeart.Location = new System.Drawing.Point(3, 118);
			this.chkCrystalHeart.Name = "chkCrystalHeart";
			this.chkCrystalHeart.Size = new System.Drawing.Size(151, 17);
			this.chkCrystalHeart.TabIndex = 5;
			this.chkCrystalHeart.Text = "Crystal Heart (Super Dash)";
			this.toolTips.SetToolTip(this.chkCrystalHeart, "Splits after obtaining Crystal Heart");
			this.chkCrystalHeart.UseVisualStyleBackColor = true;
			// 
			// chkGruzMother
			// 
			this.chkGruzMother.AutoSize = true;
			this.chkGruzMother.Location = new System.Drawing.Point(3, 141);
			this.chkGruzMother.Name = "chkGruzMother";
			this.chkGruzMother.Size = new System.Drawing.Size(116, 17);
			this.chkGruzMother.TabIndex = 6;
			this.chkGruzMother.Text = "Gruz Mother (Boss)";
			this.toolTips.SetToolTip(this.chkGruzMother, "Splits after killing Gruz Mother");
			this.chkGruzMother.UseVisualStyleBackColor = true;
			// 
			// chkDreamNail
			// 
			this.chkDreamNail.AutoSize = true;
			this.chkDreamNail.Location = new System.Drawing.Point(3, 164);
			this.chkDreamNail.Name = "chkDreamNail";
			this.chkDreamNail.Size = new System.Drawing.Size(78, 17);
			this.chkDreamNail.TabIndex = 7;
			this.chkDreamNail.Text = "Dream Nail";
			this.toolTips.SetToolTip(this.chkDreamNail, "Splits after obtaining the Dream Nail");
			this.chkDreamNail.UseVisualStyleBackColor = true;
			// 
			// chkNailUpgrade1
			// 
			this.chkNailUpgrade1.AutoSize = true;
			this.chkNailUpgrade1.Location = new System.Drawing.Point(3, 187);
			this.chkNailUpgrade1.Name = "chkNailUpgrade1";
			this.chkNailUpgrade1.Size = new System.Drawing.Size(105, 17);
			this.chkNailUpgrade1.TabIndex = 8;
			this.chkNailUpgrade1.Text = "1st Nail Upgrade";
			this.toolTips.SetToolTip(this.chkNailUpgrade1, "Splits after buying the 1st Nail upgrade");
			this.chkNailUpgrade1.UseVisualStyleBackColor = true;
			// 
			// chkWatcherKnight
			// 
			this.chkWatcherKnight.AutoSize = true;
			this.chkWatcherKnight.Location = new System.Drawing.Point(3, 210);
			this.chkWatcherKnight.Name = "chkWatcherKnight";
			this.chkWatcherKnight.Size = new System.Drawing.Size(132, 17);
			this.chkWatcherKnight.TabIndex = 9;
			this.chkWatcherKnight.Text = "Watcher Knight (Boss)";
			this.toolTips.SetToolTip(this.chkWatcherKnight, "Splits after killing the Watcher Knight");
			this.chkWatcherKnight.UseVisualStyleBackColor = true;
			// 
			// chkLurien
			// 
			this.chkLurien.AutoSize = true;
			this.chkLurien.Location = new System.Drawing.Point(3, 233);
			this.chkLurien.Name = "chkLurien";
			this.chkLurien.Size = new System.Drawing.Size(104, 17);
			this.chkLurien.TabIndex = 10;
			this.chkLurien.Text = "Lurien (Dreamer)";
			this.toolTips.SetToolTip(this.chkLurien, "Splits after taking Lurien");
			this.chkLurien.UseVisualStyleBackColor = true;
			// 
			// chkHegemol
			// 
			this.chkHegemol.AutoSize = true;
			this.chkHegemol.Location = new System.Drawing.Point(3, 256);
			this.chkHegemol.Name = "chkHegemol";
			this.chkHegemol.Size = new System.Drawing.Size(117, 17);
			this.chkHegemol.TabIndex = 11;
			this.chkHegemol.Text = "Hegemol (Dreamer)";
			this.toolTips.SetToolTip(this.chkHegemol, "Splits after taking Hegemol");
			this.chkHegemol.UseVisualStyleBackColor = true;
			// 
			// chkMonomon
			// 
			this.chkMonomon.AutoSize = true;
			this.chkMonomon.Location = new System.Drawing.Point(3, 279);
			this.chkMonomon.Name = "chkMonomon";
			this.chkMonomon.Size = new System.Drawing.Size(122, 17);
			this.chkMonomon.TabIndex = 12;
			this.chkMonomon.Text = "Monomon (Dreamer)";
			this.toolTips.SetToolTip(this.chkMonomon, "Splits after taking Monomon");
			this.chkMonomon.UseVisualStyleBackColor = true;
			// 
			// chkUumuu
			// 
			this.chkUumuu.AutoSize = true;
			this.chkUumuu.Location = new System.Drawing.Point(3, 302);
			this.chkUumuu.Name = "chkUumuu";
			this.chkUumuu.Size = new System.Drawing.Size(92, 17);
			this.chkUumuu.TabIndex = 13;
			this.chkUumuu.Text = "Uumuu (Boss)";
			this.toolTips.SetToolTip(this.chkUumuu, "Splits after killing Uumuu");
			this.chkUumuu.UseVisualStyleBackColor = true;
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
			this.Size = new System.Drawing.Size(279, 330);
			this.Load += new System.EventHandler(this.Settings_Load);
			this.flowMain.ResumeLayout(false);
			this.flowOptions.ResumeLayout(false);
			this.flowOptions.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion
		private System.Windows.Forms.FlowLayoutPanel flowMain;
		private System.Windows.Forms.FlowLayoutPanel flowOptions;
		private System.Windows.Forms.ToolTip toolTips;
		private System.Windows.Forms.CheckBox chkMothwingCloak;
		private System.Windows.Forms.CheckBox chkFalseKnight;
		private System.Windows.Forms.CheckBox chkThornsOfAgony;
		private System.Windows.Forms.CheckBox chkMantisClaw;
		private System.Windows.Forms.CheckBox chkDistantVillageStation;
		private System.Windows.Forms.CheckBox chkCrystalHeart;
		private System.Windows.Forms.CheckBox chkGruzMother;
		private System.Windows.Forms.CheckBox chkDreamNail;
		private System.Windows.Forms.CheckBox chkNailUpgrade1;
		private System.Windows.Forms.CheckBox chkWatcherKnight;
		private System.Windows.Forms.CheckBox chkLurien;
		private System.Windows.Forms.CheckBox chkHegemol;
		private System.Windows.Forms.CheckBox chkMonomon;
		private System.Windows.Forms.CheckBox chkUumuu;
	}
}
