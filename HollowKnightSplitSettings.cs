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
		[Description("Abyss Shriek (Skill)"), ToolTip("Splits when obtaining Abyss Shriek (Shadow Scream)")]
		AbyssShriek,
		[Description("Crystal Heart (Skill)"), ToolTip("Splits when obtaining Crystal Heart (Super Dash)")]
		CrystalHeart,
		[Description("Cyclone Slash (Skill)"), ToolTip("Splits when obtaining Cyclone Slash (Nail Art)")]
		CycloneSlash,
		[Description("Dash Slash (Skill)"), ToolTip("Splits when obtaining Dash Slash (Nail Art)")]
		DashSlash,
		[Description("Descending Dark (Skill)"), ToolTip("Splits when obtaining Descending Dark (Shadow Dive)")]
		DescendingDark,
		[Description("Desolate Dive (Skill)"), ToolTip("Splits when obtaining Desolate Dive")]
		DesolateDive,
		[Description("Dream Nail (Skill)"), ToolTip("Splits when obtaining Dream Nail")]
		DreamNail,
		[Description("Dream Nail - Awoken (Skill)"), ToolTip("Splits when Awkening the Dream Nail")]
		DreamNail2,
		[Description("Great Slash (Skill)"), ToolTip("Splits when obtaining Great Slash (Nail Art)")]
		GreatSlash,
		[Description("Howling Wraiths (Skill)"), ToolTip("Splits when obtaining Howling Wraiths (Scream)")]
		HowlingWraiths,
		[Description("Isma's Tear (Skill)"), ToolTip("Splits when obtaining Isma's Tear (Acid Armour)")]
		IsmasTear,
		[Description("Mantis Claw (Skill)"), ToolTip("Splits when obtaining Mantis Claw (Wall Jump)")]
		MantisClaw,
		[Description("Monarch Wings (Skill)"), ToolTip("Splits when obtaining Monarch Wings (Double Jump)")]
		MonarchWings,
		[Description("Mothwing Cloak (Skill)"), ToolTip("Splits when obtaining Mothwing Cloak (Dash)")]
		MothwingCloak,
		[Description("Shade Cloak (Skill)"), ToolTip("Splits when obtaining Shade Cloak (Shadow Dash)")]
		ShadeCloak,
		[Description("Shade Soul (Skill)"), ToolTip("Splits when obtaining Shade Soul (Vengeful Spirit 2)")]
		ShadeSoul,
		[Description("Vengeful Spirit (Skill)"), ToolTip("Splits when obtaining Vengeful Spirit")]
		VengefulSpirit,

		[Description("Kings Brand (Item)"), ToolTip("Splits when obtaining the Kings Brand")]
		KingsBrand,
		[Description("Lumafly Lantern (Item)"), ToolTip("Splits when obtaining the Lumafly Lantern")]
		LumaflyLantern,
		[Description("Pale Ore - First (Item)"), ToolTip("Splits when obtaining the first Pale Ore")]
		PaleOre,
		[Description("Tram Pass (Item)"), ToolTip("Splits when obtaining the Tram Pass")]
		TramPass,

		[Description("Mask 1 (Upgrade)"), ToolTip("Splits when getting first full Mask upgrade (6 HP)")]
		Mask1,
		[Description("Mask 2 (Upgrade)"), ToolTip("Splits when getting second full Mask upgrade (7 HP)")]
		Mask2,
		[Description("Mask 3 (Upgrade)"), ToolTip("Splits when getting third full Mask upgrade (8 HP)")]
		Mask3,
		[Description("Mask 4 (Upgrade)"), ToolTip("Splits when getting fourth full Mask upgrade (9 HP)")]
		Mask4,
		[Description("Nail 1 (Upgrade)"), ToolTip("Splits when getting Nail Upgrade 1")]
		NailUpgrade1,
		[Description("Nail 2 (Upgrade)"), ToolTip("Splits when getting Nail Upgrade 2")]
		NailUpgrade2,
		[Description("Nail 3 (Upgrade)"), ToolTip("Splits when getting Coil Nail (Upgrade 3)")]
		NailUpgrade3,
		[Description("Nail 4 (Upgrade)"), ToolTip("Splits when getting Pure Nail (Upgrade 4)")]
		NailUpgrade4,
		[Description("Vessel 1 (Upgrade)"), ToolTip("Splits when getting first full Vessel upgrade (1 extra MP)")]
		Vessel1,
		[Description("Vessel 2 (Upgrade)"), ToolTip("Splits when getting second full Vessel upgrade (2 extra MP)")]
		Vessel2,
		[Description("Vessel 3 (Upgrade)"), ToolTip("Splits when getting third full Vessel upgrade (3 extra MP)")]
		Vessel3,

		[Description("Broken Vessel (Boss)"), ToolTip("Splits when killing Broken Vessel")]
		BrokenVessel,
		[Description("Brooding Mawlek (Boss)"), ToolTip("Splits when killing Brooding Mawlek")]
		BroodingMawlek,
		[Description("Collector (Boss)"), ToolTip("Splits when killing Collector")]
		Collector,
		[Description("Crystal Guardian 1 (Boss)"), ToolTip("Splits when killing the Crystal Guardian for the first time")]
		CrystalGuardian1,
		[Description("Crystal Guardian 2 (Boss)"), ToolTip("Splits when killing the Crystal Guardian for the second time")]
		CrystalGuardian2,
		[Description("Dung Defender (Boss)"), ToolTip("Splits when killing Dung Defender")]
		DungDefender,
		[Description("Elder Hu (Boss)"), ToolTip("Splits when killing Elder Hu")]
		ElderHu,
		[Description("False Knight (Boss)"), ToolTip("Splits when killing False Knight")]
		FalseKnight,
		[Description("Failed Knight (Boss)"), ToolTip("Splits when killing Failed Knight (False Knight Dream)")]
		FailedKnight,
		[Description("Galien (Boss)"), ToolTip("Splits when killing Galien")]
		Galien,
		[Description("Gorb (Boss)"), ToolTip("Splits when killing Gorb")]
		Gorb,
		[Description("Gruz Mother (Boss)"), ToolTip("Splits when killing Gruz Mother")]
		GruzMother,
		[Description("Hollow Knight (Boss)"), ToolTip("Splits when killing Hollow Knight")]
		HollowKnight,
		[Description("Hornet 1 (Boss)"), ToolTip("Splits when killing Hornet for the first time")]
		Hornet1,
		[Description("Hornet 2 (Boss)"), ToolTip("Splits when killing Hornet for the second time")]
		Hornet2,
		[Description("Lost Kin (Boss)"), ToolTip("Splits when killing Lost Kin (Broken Vessel Dream)")]
		LostKin,
		[Description("Mantis Lords (Boss)"), ToolTip("Splits when killing Mantis Lords")]
		MantisLords,
		[Description("Markoth (Boss)"), ToolTip("Splits when killing Markoth")]
		Markoth,
		[Description("Marmu (Boss)"), ToolTip("Splits when killing Marmu")]
		Marmu,
		[Description("No Eyes (Boss)"), ToolTip("Splits when killing No Eyes")]
		NoEyes,
		[Description("Nosk (Boss)"), ToolTip("Splits when killing Nosk")]
		Nosk,
		[Description("Radiance (Boss)"), ToolTip("Splits when killing Radiance")]
		Radiance,
		[Description("Soul Master (Boss)"), ToolTip("Splits when killing Soul Master")]
		SoulMaster,
		[Description("Soul Tyrant (Boss)"), ToolTip("Splits when killing Soul Tyrant (Soul Master Dream)")]
		SoulTyrant,
		[Description("Traitor Lord (Boss)"), ToolTip("Splits when killing Traitor Lord")]
		TraitorLord,
		[Description("Uumuu (Boss)"), ToolTip("Splits when killing Uumuu")]
		Uumuu,
		[Description("Watcher Knight (Boss)"), ToolTip("Splits when killing Watcher Knight")]
		BlackKnight,
		[Description("Xero (Boss)"), ToolTip("Splits when killing Xero")]
		Xero,

		[Description("Herrah the Beast (Dreamer)"), ToolTip("Splits when you see the mask for Herrah (In Spider Area)")]
		Hegemol,
		[Description("Lurien the Watcher (Dreamer)"), ToolTip("Splits when you see the mask for Lurien (After killing Watcher Knight)")]
		Lurien,
		[Description("Monomon the Teacher (Dreamer)"), ToolTip("Splits when you see the mask for Monomon (After killing Uumuu)")]
		Monomon,
		[Description("Seer Departs (Event)"), ToolTip("Splits when the Seer Departs after bringing back 2400 essence")]
		SeerDeparts,

		[Description("Colosseum Fight 1 (Trial)"), ToolTip("Splits when beating the first Colosseum trial")]
		ColosseumBronze,
		[Description("Colosseum Fight 2 (Trial)"), ToolTip("Splits when beating the second Colosseum trial")]
		ColosseumSilver,
		[Description("Colosseum Fight 3 (Trial)"), ToolTip("Splits when beating the third Colosseum trial")]
		ColosseumGold,

		[Description("Aspid Hunter (Mini Boss)"), ToolTip("Splits when killing the final Aspid Hunter")]
		AspidHunter,
		[Description("Moss Knight (Mini Boss)"), ToolTip("Splits when killing Moss Knight")]
		MossKnight,
		[Description("Shrumal Ogre (Mini Boss)"), ToolTip("Splits when killing the final Shrumal Ogre")]
		MushroomBrawler,

		[Description("Distant Village (Stag Staion)"), ToolTip("Splits when obtaining Distant Village Stag Station")]
		DeepnestStation,
		[Description("Forgotten Crossroads (Stag Staion)"), ToolTip("Splits when opening the Forgotten Crossroads Stag Station")]
		CrossroadsStation,
		[Description("Kings Station (Stag Staion)"), ToolTip("Splits when obtaining Kings Station Stag Station")]
		KingsStationStation,
		[Description("Queens Station (Stag Staion)"), ToolTip("Splits when obtaining Queens Station Stag Staion")]
		QueensStationStation,

		[Description("City Of Tears (Area)"), ToolTip("Splits when entering City Of Tears text first appears")]
		CityOfTears,
		[Description("Crystal Peak (Area)"), ToolTip("Splits when entering Crystal Peak text first appears")]
		CrystalPeak,
		[Description("Deepnest (Area)"), ToolTip("Splits when entering Deepnest text first appears")]
		Deepnest,
		[Description("Deepnest Spa (Area)"), ToolTip("Splits when entering the Deepnest Spa area with bench")]
		DeepnestSpa,
		[Description("Fog Canyon (Area)"), ToolTip("Splits when entering Fog Canyon text first appears")]
		FogCanyon,
		[Description("Forgotten Crossroads (Area)"), ToolTip("Splits when entering Forgotten Crossroads text first appears")]
		ForgottenCrossroads,
		[Description("Fungal Wastes (Area)"), ToolTip("Splits when entering Fungal Wastes text first appears")]
		FungalWastes,
		[Description("Greenpath (Area)"), ToolTip("Splits when entering Greenpath text first appears")]
		Greenpath,
		[Description("Hive (Area)"), ToolTip("Splits when entering Hive text first appears")]
		Hive,
		[Description("Infected Crossroads (Area)"), ToolTip("Splits when entering Infected Crossroads text first appears")]
		InfectedCrossroads,
		[Description("Queens Gardens (Area)"), ToolTip("Splits when entering Queen's Gardens text first appears")]
		QueensGardens,
		[Description("Resting Grounds (Area)"), ToolTip("Splits when entering Resting Grounds text first appears")]
		RestingGrounds,
		[Description("Royal Waterways (Area)"), ToolTip("Splits when entering Royal Waterways text first appears")]
		RoyalWaterways,
		[Description("Teachers Archive (Area)"), ToolTip("Splits when entering Teachers Archive for the first time")]
		TeachersArchive,
		[Description("White Palace (Area)"), ToolTip("Splits when entering White Palace text for the first time")]
		WhitePalace,

		[Description("Baldur Shell (Charm)"), ToolTip("Splits when obtaining the Baldur Shell charm")]
		BaldurShell,
		[Description("Dashmaster (Charm)"), ToolTip("Splits when obtaining the Dashmaster charm")]
		Dashmaster,
		[Description("Deep Focus (Charm)"), ToolTip("Splits when obtaining the Deep Focus charm")]
		DeepFocus,
		[Description("Defenders Crest (Charm)"), ToolTip("Splits when obtaining the Defenders Crest charm")]
		DefendersCrest,
		[Description("Dream Wielder (Charm)"), ToolTip("Splits when obtaining the Dream Wielder charm")]
		DreamWielder,
		[Description("Flukenest (Charm)"), ToolTip("Splits when obtaining the Flukenest charm")]
		Flukenest,
		[Description("Fragile Greed (Charm)"), ToolTip("Splits when obtaining the Fragile Greed charm")]
		FragileGreed,
		[Description("Fragile Heart (Charm)"), ToolTip("Splits when obtaining the Fragile Heart charm")]
		FragileHeart,
		[Description("Fragile Strength (Charm)"), ToolTip("Splits when obtaining the Fragile Strength charm")]
		FragileStrength,
		[Description("Fury of the Fallen (Charm)"), ToolTip("Splits when obtaining the Fury of the Fallen charm")]
		FuryOfTheFallen,
		[Description("Gathering Swarm (Charm)"), ToolTip("Splits when obtaining the Gathering Swarm charm")]
		GatheringSwarm,
		[Description("Glowing Womb (Charm)"), ToolTip("Splits when obtaining the Glowing Womb charm")]
		GlowingWomb,
		[Description("Grubberfly's Elegy (Charm)"), ToolTip("Splits when obtaining the Grubberfly's Elegy charm")]
		GrubberflysElegy,
		[Description("Grubsong (Charm)"), ToolTip("Splits when obtaining the Grubsong charm")]
		Grubsong,
		[Description("Heavy Blow (Charm)"), ToolTip("Splits when obtaining the Heavy Blow charm")]
		HeavyBlow,
		[Description("Hiveblood (Charm)"), ToolTip("Splits when obtaining the Hiveblood charm")]
		Hiveblood,
		[Description("Joni's Blessing (Charm)"), ToolTip("Splits when obtaining the Joni's Blessing charm")]
		JonisBlessing,
		[Description("Kingsoul (Charm)"), ToolTip("Splits when obtaining the completed Kingsoul charm")]
		Kingsoul,
		[Description("Lifeblood Core (Charm)"), ToolTip("Splits when obtaining the Lifeblood Core charm")]
		LifebloodCore,
		[Description("Lifeblood Heart (Charm)"), ToolTip("Splits when obtaining the Lifeblood Heart charm")]
		LifebloodHeart,
		[Description("Longnail (Charm)"), ToolTip("Splits when obtaining the Longnail charm")]
		Longnail,
		[Description("Mark Of Pride (Charm)"), ToolTip("Splits when obtaining the Mark Of Pride charm")]
		MarkOfPride,
		[Description("Nailmaster's Glory (Charm)"), ToolTip("Splits when obtaining the Nailmaster's Glory charm")]
		NailmastersGlory,
		[Description("Quick Focus (Charm)"), ToolTip("Splits when obtaining the Quick Focus charm")]
		QuickFocus,
		[Description("Quick Slash (Charm)"), ToolTip("Splits when obtaining the Quick Slash charm")]
		QuickSlash,
		[Description("Shaman Stone (Charm)"), ToolTip("Splits when obtaining Shaman Stone charm")]
		ShamanStone,
		[Description("Shape of Unn (Charm)"), ToolTip("Splits when obtaining Shape of Unn charm")]
		ShapeOfUnn,
		[Description("Sharp Shadow (Charm)"), ToolTip("Splits when obtaining Sharp Shadow charm")]
		SharpShadow,
		[Description("Soul Catcher (Charm)"), ToolTip("Splits when obtaining the Soul Catcher charm")]
		SoulCatcher,
		[Description("Soul Eater (Charm)"), ToolTip("Splits when obtaining the Soul Eater charm")]
		SoulEater,
		[Description("Spell Twister (Charm)"), ToolTip("Splits when obtaining the Spell Twister charm")]
		SpellTwister,
		[Description("Spore Shroom (Charm)"), ToolTip("Splits when obtaining the Spore Shroom charm")]
		SporeShroom,
		[Description("Stalwart Shell (Charm)"), ToolTip("Splits when obtaining Stalwart Shell charm")]
		StalwartShell,
		[Description("Steady Body (Charm)"), ToolTip("Splits when obtaining the Steady Body charm")]
		SteadyBody,
		[Description("Thorns Of Agony (Charm)"), ToolTip("Splits when obtaining Thorns of Agony charm")]
		ThornsOfAgony,
		[Description("Void Heart (Charm)"), ToolTip("Splits when changing the Kingsoul to the Void Heart charm")]
		VoidHeart,
		[Description("Wayward Compass (Charm)"), ToolTip("Splits when obtaining Wayward Compass charm")]
		WaywardCompass,
	}
	public class ToolTipAttribute : Attribute {
		public string ToolTip { get; set; }
		public ToolTipAttribute(string text) {
			ToolTip = text;
		}
	}
}