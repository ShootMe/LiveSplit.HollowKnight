using System;
using System.ComponentModel;
using System.Reflection;
using System.Windows.Forms;
namespace LiveSplit.HollowKnight {
	public partial class HollowKnightSplitSettings : UserControl {
		public string Split = "";
		public HollowKnightSplitSettings() {
			InitializeComponent();
		}
		private void cboName_SelectedIndexChanged(object sender, EventArgs e) {
			string splitDescription = cboName.SelectedValue.ToString();
			SplitName split = GetSplitName(splitDescription);
			Split = split.ToString();

			MemberInfo info = typeof(SplitName).GetMember(split.ToString())[0];
			DescriptionAttribute description = (DescriptionAttribute)info.GetCustomAttributes(typeof(DescriptionAttribute), false)[0];
			ToolTipAttribute tooltip = (ToolTipAttribute)info.GetCustomAttributes(typeof(ToolTipAttribute), false)[0];
			ToolTips.SetToolTip(cboName, tooltip.ToolTip);
		}
		public static SplitName GetSplitName(string text) {
			foreach (SplitName split in Enum.GetValues(typeof(SplitName))) {
				string name = split.ToString();
				MemberInfo info = typeof(SplitName).GetMember(name)[0];
				DescriptionAttribute description = (DescriptionAttribute)info.GetCustomAttributes(typeof(DescriptionAttribute), false)[0];

				if (name.Equals(text, StringComparison.OrdinalIgnoreCase) || description.Description.Equals(text, StringComparison.OrdinalIgnoreCase)) {
					return split;
				}
			}
			return SplitName.ForgottenCrossroads;
		}
	}
	public enum SplitName {
		[Description("Forgotten Crossroads (Area)"), ToolTip("Splits when entering Forgotten Crossroads text first appears")]
		ForgottenCrossroads,
		[Description("Aspid Hunter (Mini Boss)"), ToolTip("Splits when killing the final Aspid Hunter")]
		FlyingSpitter,
		[Description("Forgotten Crossroads (Stag Staion)"), ToolTip("Splits when opening the Forgotten Crossroads Stag Station")]
		CrossroadsStation,
		[Description("False Knight (Boss)"), ToolTip("Splits when killing False Knight")]
		FalseKnight,
		[Description("Vengeful Spirit"), ToolTip("Splits when obtaining Vengeful Spirit")]
		VengefulSpirit,
		[Description("Greenpath (Area)"), ToolTip("Splits when entering Greenpath text first appears")]
		Greenpath,
		[Description("Moss Knight (Mini Boss)"), ToolTip("Splits when killing Moss Knight")]
		MossKnight,
		[Description("Hornet 1 (Boss)"), ToolTip("Splits when killing Hornet for the first time")]
		Hornet1,
		[Description("Mothwing Cloak (Dash)"), ToolTip("Splits when obtaining Mothwing Cloak")]
		MothwingCloak,
		[Description("Thorns Of Agony (Charm)"), ToolTip("Splits when obtaining Thorns of Agony charm")]
		ThornsOfAgony,
		[Description("Fog Canyon (Area)"), ToolTip("Splits when entering Fog Canyon text first appears")]
		FogCanyon,
		[Description("Queens Station (Stag Staion)"), ToolTip("Splits when obtaining Queens Station Stag Staion")]
		QueensStationStation,
		[Description("Fungal Wastes (Area)"), ToolTip("Splits when entering Fungal Wastes text first appears")]
		FungalWastes,
		[Description("Shrumal Ogre (Mini Boss)"), ToolTip("Splits when killing the final Shrumal Ogre")]
		MushroomBrawler,
		[Description("Mantis Claw (Wall Jump)"), ToolTip("Splits when obtaining Mantis Claw")]
		MantisClaw,
		[Description("Deepnest (Area)"), ToolTip("Splits when entering Deepnest text first appears")]
		Deepnest,
		[Description("Deepnest Spa (Area)"), ToolTip("Splits when entering the Deepnest Spa area with bench")]
		DeepnestSpa,
		[Description("Distant Village (Stag Staion)"), ToolTip("Splits when obtaining Distant Village Stag Station")]
		DeepnestStation,
		[Description("Crystal Peak (Area)"), ToolTip("Splits when entering Crystal Peak text first appears")]
		CrystalPeak,
		[Description("Crystal Heart (Super Dash)"), ToolTip("Splits when obtaining Crystal Heart")]
		CrystalHeart,
		[Description("Gruz Mother (Boss)"), ToolTip("Splits when killing Gruz Mother")]
		GruzMother,
		[Description("Dream Nail"), ToolTip("Splits when obtaining Dream Nail")]
		DreamNail,
		[Description("Resting Grounds (Area)"), ToolTip("Splits when entering Resting Grounds text first appears")]
		RestingGrounds,
		[Description("City Of Tears (Area)"), ToolTip("Splits when entering City Of Tears text first appears")]
		CityOfTears,
		[Description("Nail Upgrade 1"), ToolTip("Splits when getting Nail Upgrade 1")]
		NailUpgrade1,
		[Description("Fragile Strength (Charm)"), ToolTip("Splits when obtaining the Fragile Strength charm")]
		FragileStrength,
		[Description("Watcher Knight (Boss)"), ToolTip("Splits when killing Watcher Knight")]
		BlackKnight,
		[Description("Lurien (Dreamer)"), ToolTip("Splits when you see the mask for Lurien")]
		Lurien,
		[Description("Kings Station (Stag Staion)"), ToolTip("Splits when obtaining Kings Station Stag Station")]
		KingsStation,
		[Description("Herrah (Dreamer)"), ToolTip("Splits when you see the mask for Herrah")]
		Hegemol,
		[Description("Teachers Archive (Area)"), ToolTip("Splits when entering Teachers Archive for the first time")]
		TeachersArchive,
		[Description("Uumuu (Boss)"), ToolTip("Splits when killing Uumuu")]
		Uumuu,
		[Description("Monomon (Dreamer)"), ToolTip("Splits when you see the mask for Monomon")]
		Monomon,
		[Description("Infected Crossroads (Area)"), ToolTip("Splits when entering Infected Crossroads text first appears")]
		InfectedCrossroads
	}
	public class ToolTipAttribute : Attribute {
		public string ToolTip { get; set; }
		public ToolTipAttribute(string text) {
			ToolTip = text;
		}
	}
}