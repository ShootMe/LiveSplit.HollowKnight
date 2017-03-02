using System;
using System.ComponentModel;
using System.Windows.Forms;
namespace LiveSplit.HollowKnight {
	public partial class HollowKnightSplitSettings : UserControl {
		public string ControlType = "";
		public HollowKnightSplitSettings() {
			InitializeComponent();
		}
		private void cboName_SelectedIndexChanged(object sender, EventArgs e) {
			this.ControlType = cboName.SelectedValue.ToString();
		}
	}
	public enum SplitName {
		[Description("False Knight (Boss)")]
		FalseKnight,
		[Description("Mothwing Cloak (Dash)")]
		MothwingCloak,
		[Description("Thorns Of Agony (Charm)")]
		ThornsOfAgony,
		[Description("Mantis Claw (Wall Jump)")]
		MantisClaw,
		[Description("Distant Village Stag Station")]
		DistantVillageStag,
		[Description("Crystal Heart (Super Dash)")]
		CrystalHeart,
		[Description("Gruz Mother (Boss)")]
		GruzMother,
		[Description("Dream Nail")]
		DreamNail,
		[Description("Nail Upgrade 1")]
		NailUpgrade1,
		[Description("Watcher Knight (Boss)")]
		WatcherKnight,
		[Description("Lurien (Dreamer)")]
		Lurien,
		[Description("Hegemol (Dreamer)")]
		Hegemol,
		[Description("Monomon (Dreamer)")]
		Monomon,
		[Description("Uumuu (Boss)")]
		Uumuu
	}
}