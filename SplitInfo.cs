using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace LiveSplit.HollowKnight {
	/// <summary>
	/// 
	/// </summary>
	/// <remarks>
	/// C# 7 introduced pattern matching in switch statements, allowing us to pattern match against
	/// objects and their members. This allows us to ditch the Enum + Attribute + Reflection framework and
	/// use patterns in order to more efficiently match against values.
	/// </remarks>
	public sealed class SplitInfo {

		public static SplitInfo AbyssShriek { get; } = RegisterOrGetInfo("AbyssShriek", "Abyss Shriek (Skill)", "Splits when obtaining Abyss Shriek (Shadow Scream)");
		public static SplitInfo CrystalHeart{ get; } = RegisterOrGetInfo("CrystalHeart", "Crystal Heart (Skill)", "Splits when obtaining Crystal Heart (Super Dash)");
		public static SplitInfo CycloneSlash{ get; } = RegisterOrGetInfo("CycloneSlash", "Cyclone Slash (Skill)", "Splits when obtaining Cyclone Slash (Nail Art)");
		public static SplitInfo DashSlash{ get; } = RegisterOrGetInfo("DashSlash", "Dash Slash (Skill)", "Splits when obtaining Dash Slash (Nail Art)");
		public static SplitInfo DescendingDark{ get; } = RegisterOrGetInfo("DescendingDark", "Descending Dark (Skill)", "Splits when obtaining Descending Dark (Shadow Dive)");
		public static SplitInfo DesolateDive{ get; } = RegisterOrGetInfo("DesolateDive", "Desolate Dive (Skill)", "Splits when obtaining Desolate Dive");
		public static SplitInfo DreamNail{ get; } = RegisterOrGetInfo("DreamNail", "Dream Nail (Skill)", "Splits when obtaining Dream Nail");
		public static SplitInfo DreamNail2{ get; } = RegisterOrGetInfo("DreamNail2", "Dream Nail - Awoken (Skill)", "Splits when Awkening the Dream Nail");
		public static SplitInfo GreatSlash{ get; } = RegisterOrGetInfo("GreatSlash", "Great Slash (Skill)", "Splits when obtaining Great Slash (Nail Art)");
		public static SplitInfo HowlingWraiths{ get; } = RegisterOrGetInfo("HowlingWraiths", "Howling Wraiths (Skill)", "Splits when obtaining Howling Wraiths (Scream)");
		public static SplitInfo IsmasTear{ get; } = RegisterOrGetInfo("IsmasTear", "Isma's Tear (Skill)", "Splits when obtaining Isma's Tear (Acid Armour)");
		public static SplitInfo MantisClaw{ get; } = RegisterOrGetInfo("MantisClaw", "Mantis Claw (Skill)", "Splits when obtaining Mantis Claw (Wall Jump)");
		public static SplitInfo MonarchWings{ get; } = RegisterOrGetInfo("MonarchWings", "Monarch Wings (Skill)", "Splits when obtaining Monarch Wings (Double Jump)");
		public static SplitInfo MothwingCloak{ get; } = RegisterOrGetInfo("MothwingCloak", "Mothwing Cloak (Skill)", "Splits when obtaining Mothwing Cloak (Dash)");
		public static SplitInfo ShadeCloak{ get; } = RegisterOrGetInfo("ShadeCloak", "Shade Cloak (Skill)", "Splits when obtaining Shade Cloak (Shadow Dash)");
		public static SplitInfo ShadeSoul{ get; } = RegisterOrGetInfo("ShadeSoul", "Shade Soul (Skill)", "Splits when obtaining Shade Soul (Vengeful Spirit 2)");
		public static SplitInfo VengefulSpirit{ get; } = RegisterOrGetInfo("VengefulSpirit", "Vengeful Spirit (Skill)", "Splits when obtaining Vengeful Spirit");

		public static SplitInfo KingsBrand{ get; } = RegisterOrGetInfo("KingsBrand", "Kings Brand (Item)", "Splits when obtaining the Kings Brand");
		public static SplitInfo LoveKey{ get; } = RegisterOrGetInfo("LoveKey", "Love Key (Item)", "Splits when obtaining the Love Key");
		public static SplitInfo LumaflyLantern{ get; } = RegisterOrGetInfo("LumaflyLantern", "Lumafly Lantern (Item)", "Splits when obtaining the Lumafly Lantern");
		public static SplitInfo PaleOre{ get; } = RegisterOrGetInfo("PaleOre", "Pale Ore - First (Item)", "Splits when obtaining the first Pale Ore");
		public static SplitInfo SimpleKey{ get; } = RegisterOrGetInfo("SimpleKey", "Simple Key - First (Item)", "Splits when obtaining the first Simple Key");
		public static SplitInfo TramPass{ get; } = RegisterOrGetInfo("TramPass", "Tram Pass (Item)", "Splits when obtaining the Tram Pass");

		public static SplitInfo Mask1{ get; } = RegisterOrGetInfo("Mask1", "Mask 1 (Upgrade)", "Splits when getting first full Mask upgrade (6 HP)");
		public static SplitInfo Mask2{ get; } = RegisterOrGetInfo("Mask2", "Mask 2 (Upgrade)", "Splits when getting second full Mask upgrade (7 HP)");
		public static SplitInfo Mask3{ get; } = RegisterOrGetInfo("Mask3", "Mask 3 (Upgrade)", "Splits when getting third full Mask upgrade (8 HP)");
		public static SplitInfo Mask4{ get; } = RegisterOrGetInfo("Mask4", "Mask 4 (Upgrade)", "Splits when getting fourth full Mask upgrade (9 HP)");
		public static SplitInfo NailUpgrade1{ get; } = RegisterOrGetInfo("NailUpgrade1", "Nail 1 (Upgrade)", "Splits when getting Nail Upgrade 1");
		public static SplitInfo NailUpgrade2{ get; } = RegisterOrGetInfo("NailUpgrade2", "Nail 2 (Upgrade)", "Splits when getting Nail Upgrade 2");
		public static SplitInfo NailUpgrade3{ get; } = RegisterOrGetInfo("NailUpgrade3", "Nail 3 (Upgrade)", "Splits when getting Coil Nail (Upgrade 3)");
		public static SplitInfo NailUpgrade4{ get; } = RegisterOrGetInfo("NailUpgrade4", "Nail 4 (Upgrade)", "Splits when getting Pure Nail (Upgrade 4)");
		public static SplitInfo Vessel1{ get; } = RegisterOrGetInfo("Vessel1", "Vessel 1 (Upgrade)", "Splits when getting first full Vessel upgrade (1 extra MP)");
		public static SplitInfo Vessel2{ get; } = RegisterOrGetInfo("Vessel2", "Vessel 2 (Upgrade)", "Splits when getting second full Vessel upgrade (2 extra MP)");
		public static SplitInfo Vessel3{ get; } = RegisterOrGetInfo("Vessel3", "Vessel 3 (Upgrade)", "Splits when getting third full Vessel upgrade (3 extra MP)");

		public static SplitInfo BrokenVessel{ get; } = RegisterOrGetInfo("BrokenVessel", "Broken Vessel (Boss)", "Splits when killing Broken Vessel");
		public static SplitInfo BroodingMawlek{ get; } = RegisterOrGetInfo("BroodingMawlek", "Brooding Mawlek (Boss)", "Splits when killing Brooding Mawlek");
		public static SplitInfo Collector{ get; } = RegisterOrGetInfo("Collector", "Collector (Boss)", "Splits when killing Collector");
		public static SplitInfo CrystalGuardian1{ get; } = RegisterOrGetInfo("CrystalGuardian1", "Crystal Guardian 1 (Boss)", "Splits when killing the Crystal Guardian for the first time");
		public static SplitInfo CrystalGuardian2{ get; } = RegisterOrGetInfo("CrystalGuardian2", "Crystal Guardian 2 (Boss)", "Splits when killing the Crystal Guardian for the second time");
		public static SplitInfo DungDefender{ get; } = RegisterOrGetInfo("DungDefender", "Dung Defender (Boss)", "Splits when killing Dung Defender");
		public static SplitInfo ElderHu{ get; } = RegisterOrGetInfo("ElderHu", "Elder Hu (Boss)", "Splits when killing Elder Hu");
		public static SplitInfo FalseKnight{ get; } = RegisterOrGetInfo("FalseKnight", "False Knight (Boss)", "Splits when killing False Knight");
		public static SplitInfo FailedKnight{ get; } = RegisterOrGetInfo("FailedKnight", "Failed Knight (Boss)", "Splits when killing Failed Knight (False Knight Dream)");
		public static SplitInfo Flukemarm{ get; } = RegisterOrGetInfo("Flukemarm", "Flukemarm (Boss)", "Splits when killing Flukemarm");
		public static SplitInfo Galien{ get; } = RegisterOrGetInfo("Galien", "Galien (Boss)", "Splits when killing Galien");
		public static SplitInfo Gorb{ get; } = RegisterOrGetInfo("Gorb", "Gorb (Boss)", "Splits when killing Gorb");
		public static SplitInfo GruzMother{ get; } = RegisterOrGetInfo("GruzMother", "Gruz Mother (Boss)", "Splits when killing Gruz Mother");
		public static SplitInfo HollowKnight{ get; } = RegisterOrGetInfo("HollowKnight", "Hollow Knight - Dream Nail (Boss)", "Splits when going into the dream world for Hollow Knight to fight Radiance");
		public static SplitInfo Hornet1{ get; } = RegisterOrGetInfo("Hornet1", "Hornet 1 (Boss)", "Splits when killing Hornet for the first time");
		public static SplitInfo Hornet2{ get; } = RegisterOrGetInfo("Hornet2", "Hornet 2 (Boss)", "Splits when killing Hornet for the second time");
		public static SplitInfo LostKin{ get; } = RegisterOrGetInfo("LostKin", "Lost Kin (Boss)", "Splits when killing Lost Kin (Broken Vessel Dream)");
		public static SplitInfo MantisLords{ get; } = RegisterOrGetInfo("MantisLords", "Mantis Lords (Boss)", "Splits when killing Mantis Lords");
		public static SplitInfo Markoth{ get; } = RegisterOrGetInfo("Markoth", "Markoth (Boss)", "Splits when killing Markoth");
		public static SplitInfo Marmu{ get; } = RegisterOrGetInfo("Marmu", "Marmu (Boss)", "Splits when killing Marmu");
		public static SplitInfo MegaMossCharger{ get; } = RegisterOrGetInfo("MegaMossCharger", "Mega Moss Charger (Boss)", "Splits when killing Mega Moss Charger");
		public static SplitInfo NoEyes{ get; } = RegisterOrGetInfo("NoEyes", "No Eyes (Boss)", "Splits when killing No Eyes");
		public static SplitInfo Nosk{ get; } = RegisterOrGetInfo("Nosk", "Nosk (Boss)", "Splits when killing Nosk");
		public static SplitInfo SoulMaster{ get; } = RegisterOrGetInfo("SoulMaster", "Soul Master (Boss)", "Splits when killing Soul Master");
		public static SplitInfo SoulTyrant{ get; } = RegisterOrGetInfo("SoulTyrant", "Soul Tyrant (Boss)", "Splits when killing Soul Tyrant (Soul Master Dream)");
		public static SplitInfo TraitorLord{ get; } = RegisterOrGetInfo("TraitorLord", "Traitor Lord (Boss)", "Splits when killing Traitor Lord");
		public static SplitInfo Uumuu{ get; } = RegisterOrGetInfo("Uumuu", "Uumuu (Boss)", "Splits when killing Uumuu");
		public static SplitInfo BlackKnight{ get; } = RegisterOrGetInfo("BlackKnight", "Watcher Knight (Boss)", "Splits when killing Watcher Knight");
		public static SplitInfo Xero{ get; } = RegisterOrGetInfo("Xero", "Xero (Boss)", "Splits when killing Xero");

		public static SplitInfo Hegemol{ get; } = RegisterOrGetInfo("Hegemol", "Herrah the Beast (Dreamer)", "Splits when you see the mask for Herrah (In Spider Area)");
		public static SplitInfo Lurien{ get; } = RegisterOrGetInfo("Lurien", "Lurien the Watcher (Dreamer)", "Splits when you see the mask for Lurien (After killing Watcher Knight)");
		public static SplitInfo Monomon{ get; } = RegisterOrGetInfo("Monomon", "Monomon the Teacher (Dreamer)", "Splits when you see the mask for Monomon (After killing Uumuu)");
		public static SplitInfo SeerDeparts{ get; } = RegisterOrGetInfo("SeerDeparts", "Seer Departs (Event)", "Splits when the Seer Departs after bringing back 2400 essence");

		public static SplitInfo ColosseumBronze{ get; } = RegisterOrGetInfo("ColosseumBronze", "Colosseum Fight 1 (Trial)", "Splits when beating the first Colosseum trial");
		public static SplitInfo ColosseumSilver{ get; } = RegisterOrGetInfo("ColosseumSilver", "Colosseum Fight 2 (Trial)", "Splits when beating the second Colosseum trial");
		public static SplitInfo ColosseumGold{ get; } = RegisterOrGetInfo("ColosseumGold", "Colosseum Fight 3 (Trial)", "Splits when beating the third Colosseum trial");

		public static SplitInfo AspidHunter{ get; } = RegisterOrGetInfo("AspidHunter", "Aspid Hunter (Mini Boss)", "Splits when killing the final Aspid Hunter");
		public static SplitInfo MossKnight{ get; } = RegisterOrGetInfo("MossKnight", "Moss Knight (Mini Boss)", "Splits when killing Moss Knight");
		public static SplitInfo MushroomBrawler{ get; } = RegisterOrGetInfo("MushroomBrawler", "Shrumal Ogre (Mini Boss)", "Splits when killing the final Shrumal Ogre");
		public static SplitInfo Zote1{ get; } = RegisterOrGetInfo("Zote1", "Zote Rescued Vengefly King (Mini Boss)", "Splits when rescuing Zote from the Vengefly King");
		public static SplitInfo Zote2{ get; } = RegisterOrGetInfo("Zote2", "Zote Rescued Deepnest (Mini Boss)", "Splits when rescuing Zote in Deepnest");

		public static SplitInfo DeepnestStation{ get; } = RegisterOrGetInfo("DeepnestStation", "Distant Village (Stag Staion)", "Splits when obtaining Distant Village Stag Station");
		public static SplitInfo CrossroadsStation{ get; } = RegisterOrGetInfo("CrossroadsStation", "Forgotten Crossroads (Stag Staion)", "Splits when opening the Forgotten Crossroads Stag Station");
		public static SplitInfo KingsStationStation{ get; } = RegisterOrGetInfo("KingsStationStation", "Kings Station (Stag Staion)", "Splits when obtaining Kings Station Stag Station");
		public static SplitInfo QueensStationStation{ get; } = RegisterOrGetInfo("QueensStationStation", "Queens Station (Stag Staion)", "Splits when obtaining Queens Station Stag Staion");

		public static SplitInfo CityOfTears{ get; } = RegisterOrGetInfo("CityOfTears", "City Of Tears (Area)", "Splits when entering City Of Tears text first appears");
		public static SplitInfo CrystalPeak{ get; } = RegisterOrGetInfo("CrystalPeak", "Crystal Peak (Area)", "Splits when entering Crystal Peak text first appears");
		public static SplitInfo Deepnest{ get; } = RegisterOrGetInfo("Deepnest", "Deepnest (Area)", "Splits when entering Deepnest text first appears");
		public static SplitInfo DeepnestSpa{ get; } = RegisterOrGetInfo("DeepnestSpa", "Deepnest Spa (Area)", "Splits when entering the Deepnest Spa area with bench");
		public static SplitInfo FogCanyon{ get; } = RegisterOrGetInfo("FogCanyon", "Fog Canyon (Area)", "Splits when entering Fog Canyon text first appears");
		public static SplitInfo ForgottenCrossroads{ get; } = RegisterOrGetInfo("ForgottenCrossroads", "Forgotten Crossroads (Area)", "Splits when entering Forgotten Crossroads text first appears");
		public static SplitInfo FungalWastes{ get; } = RegisterOrGetInfo("FungalWastes", "Fungal Wastes (Area)", "Splits when entering Fungal Wastes text first appears");
		public static SplitInfo Greenpath{ get; } = RegisterOrGetInfo("Greenpath", "Greenpath (Area)", "Splits when entering Greenpath text first appears");
		public static SplitInfo Hive{ get; } = RegisterOrGetInfo("Hive", "Hive (Area)", "Splits when entering Hive text first appears");
		public static SplitInfo InfectedCrossroads{ get; } = RegisterOrGetInfo("InfectedCrossroads", "Infected Crossroads (Area)", "Splits when entering Infected Crossroads text first appears");
		public static SplitInfo QueensGardens{ get; } = RegisterOrGetInfo("QueensGardens", "Queens Gardens (Area)", "Splits when entering Queen's Gardens text first appears");
		public static SplitInfo RestingGrounds{ get; } = RegisterOrGetInfo("RestingGrounds", "Resting Grounds (Area)", "Splits when entering Resting Grounds text first appears");
		public static SplitInfo RoyalWaterways{ get; } = RegisterOrGetInfo("RoyalWaterways", "Royal Waterways (Area)", "Splits when entering Royal Waterways text first appears");
		public static SplitInfo TeachersArchive{ get; } = RegisterOrGetInfo("TeachersArchive", "Teachers Archive (Area)", "Splits when entering Teachers Archive for the first time");
		public static SplitInfo WhitePalace{ get; } = RegisterOrGetInfo("WhitePalace", "White Palace (Area)", "Splits when entering White Palace text for the first time");

		public static SplitInfo BaldurShell{ get; } = RegisterOrGetInfo("BaldurShell", "Baldur Shell (Charm)", "Splits when obtaining the Baldur Shell charm");
		public static SplitInfo Dashmaster{ get; } = RegisterOrGetInfo("Dashmaster", "Dashmaster (Charm)", "Splits when obtaining the Dashmaster charm");
		public static SplitInfo DeepFocus{ get; } = RegisterOrGetInfo("DeepFocus", "Deep Focus (Charm)", "Splits when obtaining the Deep Focus charm");
		public static SplitInfo DefendersCrest{ get; } = RegisterOrGetInfo("DefendersCrest", "Defenders Crest (Charm)", "Splits when obtaining the Defenders Crest charm");
		public static SplitInfo DreamWielder{ get; } = RegisterOrGetInfo("DreamWielder", "Dream Wielder (Charm)", "Splits when obtaining the Dream Wielder charm");
		public static SplitInfo Flukenest{ get; } = RegisterOrGetInfo("Flukenest", "Flukenest (Charm)", "Splits when obtaining the Flukenest charm");
		public static SplitInfo FragileGreed{ get; } = RegisterOrGetInfo("FragileGreed", "Fragile Greed (Charm)", "Splits when obtaining the Fragile Greed charm");
		public static SplitInfo FragileHeart{ get; } = RegisterOrGetInfo("FragileHeart", "Fragile Heart (Charm)", "Splits when obtaining the Fragile Heart charm");
		public static SplitInfo FragileStrength{ get; } = RegisterOrGetInfo("FragileStrength", "Fragile Strength (Charm)", "Splits when obtaining the Fragile Strength charm");
		public static SplitInfo FuryOfTheFallen{ get; } = RegisterOrGetInfo("FuryOfTheFallen", "Fury of the Fallen (Charm)", "Splits when obtaining the Fury of the Fallen charm");
		public static SplitInfo GatheringSwarm{ get; } = RegisterOrGetInfo("GatheringSwarm", "Gathering Swarm (Charm)", "Splits when obtaining the Gathering Swarm charm");
		public static SplitInfo GlowingWomb{ get; } = RegisterOrGetInfo("GlowingWomb", "Glowing Womb (Charm)", "Splits when obtaining the Glowing Womb charm");
		public static SplitInfo GrubberflysElegy{ get; } = RegisterOrGetInfo("GrubberflysElegy", "Grubberfly's Elegy (Charm)", "Splits when obtaining the Grubberfly's Elegy charm");
		public static SplitInfo Grubsong{ get; } = RegisterOrGetInfo("Grubsong", "Grubsong (Charm)", "Splits when obtaining the Grubsong charm");
		public static SplitInfo HeavyBlow{ get; } = RegisterOrGetInfo("HeavyBlow", "Heavy Blow (Charm)", "Splits when obtaining the Heavy Blow charm");
		public static SplitInfo Hiveblood{ get; } = RegisterOrGetInfo("Hiveblood", "Hiveblood (Charm)", "Splits when obtaining the Hiveblood charm");
		public static SplitInfo JonisBlessing{ get; } = RegisterOrGetInfo("JonisBlessing", "Joni's Blessing (Charm)", "Splits when obtaining the Joni's Blessing charm");
		public static SplitInfo Kingsoul{ get; } = RegisterOrGetInfo("Kingsoul", "Kingsoul (Charm)", "Splits when obtaining the completed Kingsoul charm");
		public static SplitInfo LifebloodCore{ get; } = RegisterOrGetInfo("LifebloodCore", "Lifeblood Core (Charm)", "Splits when obtaining the Lifeblood Core charm");
		public static SplitInfo LifebloodHeart{ get; } = RegisterOrGetInfo("LifebloodHeart", "Lifeblood Heart (Charm)", "Splits when obtaining the Lifeblood Heart charm");
		public static SplitInfo Longnail{ get; } = RegisterOrGetInfo("Longnail", "Longnail (Charm)", "Splits when obtaining the Longnail charm");
		public static SplitInfo MarkOfPride{ get; } = RegisterOrGetInfo("MarkOfPride", "Mark Of Pride (Charm)", "Splits when obtaining the Mark Of Pride charm");
		public static SplitInfo NailmastersGlory{ get; } = RegisterOrGetInfo("NailmastersGlory", "Nailmaster's Glory (Charm)", "Splits when obtaining the Nailmaster's Glory charm");
		public static SplitInfo QuickFocus{ get; } = RegisterOrGetInfo("QuickFocus", "Quick Focus (Charm)", "Splits when obtaining the Quick Focus charm");
		public static SplitInfo QuickSlash{ get; } = RegisterOrGetInfo("QuickSlash", "Quick Slash (Charm)", "Splits when obtaining the Quick Slash charm");
		public static SplitInfo ShamanStone{ get; } = RegisterOrGetInfo("ShamanStone", "Shaman Stone (Charm)", "Splits when obtaining Shaman Stone charm");
		public static SplitInfo ShapeOfUnn{ get; } = RegisterOrGetInfo("ShapeOfUnn", "Shape of Unn (Charm)", "Splits when obtaining Shape of Unn charm");
		public static SplitInfo SharpShadow{ get; } = RegisterOrGetInfo("SharpShadow", "Sharp Shadow (Charm)", "Splits when obtaining Sharp Shadow charm");
		public static SplitInfo SoulCatcher{ get; } = RegisterOrGetInfo("SoulCatcher", "Soul Catcher (Charm)", "Splits when obtaining the Soul Catcher charm");
		public static SplitInfo SoulEater{ get; } = RegisterOrGetInfo("SoulEater", "Soul Eater (Charm)", "Splits when obtaining the Soul Eater charm");
		public static SplitInfo SpellTwister{ get; } = RegisterOrGetInfo("SpellTwister", "Spell Twister (Charm)", "Splits when obtaining the Spell Twister charm");
		public static SplitInfo SporeShroom{ get; } = RegisterOrGetInfo("SporeShroom", "Spore Shroom (Charm)", "Splits when obtaining the Spore Shroom charm");
		public static SplitInfo StalwartShell{ get; } = RegisterOrGetInfo("StalwartShell", "Stalwart Shell (Charm)", "Splits when obtaining Stalwart Shell charm");
		public static SplitInfo SteadyBody{ get; } = RegisterOrGetInfo("SteadyBody", "Steady Body (Charm)", "Splits when obtaining the Steady Body charm");
		public static SplitInfo ThornsOfAgony{ get; } = RegisterOrGetInfo("ThornsOfAgony", "Thorns Of Agony (Charm)", "Splits when obtaining Thorns of Agony charm");
		public static SplitInfo VoidHeart{ get; } = RegisterOrGetInfo("VoidHeart", "Void Heart (Charm)", "Splits when changing the Kingsoul to the Void Heart charm");
		public static SplitInfo WaywardCompass{ get; } = RegisterOrGetInfo("WaywardCompass", "Wayward Compass (Charm)", "Splits when obtaining Wayward Compass charm");
		public static SplitInfo NotchShrumalOgres{ get; } = RegisterOrGetInfo("NotchShrumalOgres", "Shrumal Ogres (Charm Notch)", "Splits when obtaining the charm notch after defeating the Shrumal Ogres");
		public static SplitInfo NotchFogCanyon{ get; } = RegisterOrGetInfo("NotchFogCanyon", "Fog Canyon (Charm Notch)", "Splits when obtaining the charm notch in Fog Canyon");
		public static SplitInfo NotchSalubra1{ get; } = RegisterOrGetInfo("NotchSalubra1", "Salubra 1 (Charm Notch)", "Splits when obtaining the first charm notch from Salubra");
		public static SplitInfo NotchSalubra2{ get; } = RegisterOrGetInfo("NotchSalubra2", "Salubra 2 (Charm Notch)", "Splits when obtaining the second charm notch from Salubra");
		public static SplitInfo NotchSalubra3{ get; } = RegisterOrGetInfo("NotchSalubra3", "Salubra 3 (Charm Notch)", "Splits when obtaining the third charm notch from Salubra");
		public static SplitInfo NotchSalubra4{ get; } = RegisterOrGetInfo("NotchSalubra4", "Salubra 4 (Charm Notch)", "Splits when obtaining the fourth charm notch from Salubra");

		public static Dictionary<string, SplitInfo> RegisteredSplits { get; private set; }

		/// <summary>
		/// 
		/// </summary>
		/// <param name="id"></param>
		/// <param name="description"></param>
		/// <param name="tip"></param>
		/// <returns>The newly registered Info OR an already-registered object if the id is already present.</returns>
		public static SplitInfo RegisterOrGetInfo(string id, string description, string tip) {

			if (RegisteredSplits == null) {
				RegisteredSplits = new Dictionary<string, SplitInfo>();
			}

			if (RegisteredSplits.TryGetValue(id, out var info)) {
				return info;
			}

			info = new SplitInfo(id, description, tip);

			RegisteredSplits.Add(id, info);
			return info;
		}

		public static SplitInfo FromID(string id) {
			if (RegisteredSplits.TryGetValue(id, out var info)) {
				return info;
			}
			return null;
		}

		public static SplitInfo[] FromDescription(string description) {
			return (from kvp in RegisteredSplits
					where kvp.Value.Description.Equals(description)
					select kvp.Value).ToArray();
		}

		public static SplitInfo[] FromToolTip(string tip) {
			return (from kvp in RegisteredSplits
					where kvp.Value.ToolTip.Equals(tip)
					select kvp.Value).ToArray();
		}

		public string ID { get; private set; }
		public string Description { get; private set; }
		public string ToolTip { get; private set; }

		public SplitInfo(string id, string description, string tip) {
			ID = id;
			Description = description;
			ToolTip = tip;
		}

		public override bool Equals(object obj) {
			return GetHashCode().Equals(obj.GetHashCode());
		}

		public override int GetHashCode() {
			return ID.GetHashCode() ^ Description.GetHashCode() ^ ToolTip.GetHashCode();
		}
	}
}