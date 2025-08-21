﻿using System;
using System.ComponentModel;
using System.Reflection;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;

namespace LiveSplit.HollowKnight {
    public partial class HollowKnightSplitSettings : UserControl {
        public string Split { get; set; } = "";
        private int mX = 0;
        private int mY = 0;
        private bool isDragging = false;
        public HollowKnightSplitSettings() {
            InitializeComponent();
            cboName.MouseWheel += (o, e) => ((HandledMouseEventArgs)e).Handled = true;
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
            return SplitName.ManualSplit;
        }

        private void picHandle_MouseMove(object sender, MouseEventArgs e) {
            if (!isDragging) {
                if (e.Button == MouseButtons.Left) {
                    int num1 = mX - e.X;
                    int num2 = mY - e.Y;
                    if (((num1 * num1) + (num2 * num2)) > 20) {
                        DoDragDrop(this, DragDropEffects.All);
                        isDragging = true;
                        return;
                    }
                }
            }
        }
        private void picHandle_MouseDown(object sender, MouseEventArgs e) {
            mX = e.X;
            mY = e.Y;
            isDragging = false;
        }
    }
    public enum SplitName {

        [Description("Start New Game (Start)"), ToolTip("Splits when starting a new game, including Normal, Steel Soul, and Godseeker mode")]
        StartNewGame,
        [Description("Start Pantheon (Start)"), ToolTip("Splits when starting a Pantheon run")]
        StartPantheon,
        [Description("Rando Wake (Start)"), ToolTip("Splits when gaining control after waking up in Rando")]
        RandoWake,

        [Description("Credits Roll (Ending)"), ToolTip("Splits on any credits rolling, any ending")]
        EndingSplit,
        [Description("The Hollow Knight (Ending)"), ToolTip("Splits on The Hollow Knight ending")]
        EndingA,
        [Description("Sealed Siblings (Ending)"), ToolTip("Splits on Sealed Siblings ending")]
        EndingB,
        [Description("Dream No More (Ending)"), ToolTip("Splits on Dream No More ending, True Ending")]
        EndingC,
        [Description("Embrace the Void (Ending)"), ToolTip("Splits on Embrace the Void ending")]
        EndingD,
        [Description("Delicate Flower (Ending)"), ToolTip("Splits on Delicate Flower ending")]
        EndingE,

        [Description("Abyss Shriek (Skill)"), ToolTip("Splits when obtaining Abyss Shriek")]
        AbyssShriek,
        [Description("Crystal Heart (Skill)"), ToolTip("Splits when obtaining Crystal Heart")]
        CrystalHeart,
        [Description("Cyclone Slash (Skill)"), ToolTip("Splits when obtaining Cyclone Slash")]
        CycloneSlash,
        [Description("Dash Slash (Skill)"), ToolTip("Splits when obtaining Dash Slash")]
        DashSlash,
        [Description("Descending Dark (Skill)"), ToolTip("Splits when obtaining Descending Dark")]
        DescendingDark,
        [Description("Desolate Dive (Skill)"), ToolTip("Splits when obtaining Desolate Dive")]
        DesolateDive,
        [Description("Dream Nail (Skill)"), ToolTip("Splits when obtaining Dream Nail")]
        DreamNail,
        [Description("Dream Nail - Awoken (Skill)"), ToolTip("Splits when Awakening the Dream Nail")]
        DreamNail2,
        [Description("Dream Gate (Skill)"), ToolTip("Splits when obtaining Dream Gate")]
        DreamGate,
        [Description("Great Slash (Skill)"), ToolTip("Splits when obtaining Great Slash")]
        GreatSlash,
        [Description("Howling Wraiths (Skill)"), ToolTip("Splits when obtaining Howling Wraiths")]
        HowlingWraiths,
        [Description("Isma's Tear (Skill)"), ToolTip("Splits when obtaining Isma's Tear")]
        IsmasTear,
        [Description("Mantis Claw (Skill)"), ToolTip("Splits when obtaining Mantis Claw")]
        MantisClaw,
        [Description("Monarch Wings (Skill)"), ToolTip("Splits when obtaining Monarch Wings")]
        MonarchWings,
        [Description("Mothwing Cloak (Skill)"), ToolTip("Splits when obtaining Mothwing Cloak")]
        MothwingCloak,
        [Description("Shade Cloak (Skill)"), ToolTip("Splits when obtaining Shade Cloak")]
        ShadeCloak,
        [Description("Shade Soul (Skill)"), ToolTip("Splits when obtaining Shade Soul")]
        ShadeSoul,
        [Description("Vengeful Spirit (Skill)"), ToolTip("Splits when obtaining Vengeful Spirit")]
        VengefulSpirit,

        [Description("City Crest (Item)"), ToolTip("Splits when obtaining the City Crest")]
        CityKey,
        [Description("Delicate Flower (Item)"), ToolTip("Splits when flower is in inventory")]
        HasDelicateFlower,
        [Description("Elegant Key (Item)"), ToolTip("Splits when obtaining the Elegant Key")]
        ElegantKey,
        [Description("Elegant Key Shoptimisation (Item)"), ToolTip("Splits when obtaining both Mask Shard 1 and Elegant Key")]
        ElegantKeyShoptimised,
        [Description("God Tuner (Item)"), ToolTip("Splits when obtaining the God Tuner")]
        GodTuner,
        [Description("Hunter's Mark (Item)"), ToolTip("Splits when obtaining the Hunter's Mark")]
        HuntersMark,
        [Description("King's Brand (Item)"), ToolTip("Splits when obtaining the King's Brand")]
        KingsBrand,
        [Description("Love Key (Item)"), ToolTip("Splits when obtaining the Love Key")]
        LoveKey,
        [Description("Lumafly Lantern (Item)"), ToolTip("Splits when obtaining the Lumafly Lantern")]
        LumaflyLantern,
        [Description("Pale Lurker Key (Item)"), ToolTip("Splits when obtaining the Simple Key from the Pale Lurker")]
        PaleLurkerKey,
        [Description("Pale Ore - Any (Item)"), ToolTip("Splits if you've obtained any Pale Ore")]
        PaleOre,
        [Description("Salubra's Blessing (Item)"), ToolTip("Splits when obtaining Salubra's Blessing")]
        SalubrasBlessing,
        [Description("Simple Key - First (Item)"), ToolTip("Splits when obtaining the first Simple Key")]
        SimpleKey,
        [Description("Shopkeeper's Key (Item)"), ToolTip("Splits when obtaining the Shopkeeper's Key")]
        SlyKey,
        [Description("Tram Pass (Item)"), ToolTip("Splits when obtaining the Tram Pass")]
        TramPass,

        [Description("Mask Shard 1 (Fragment)"), ToolTip("Splits when getting 1st Mask Shard")]
        MaskFragment1,
        [Description("Mask Shard 2 (Fragment)"), ToolTip("Splits when getting 2nd Mask Shard")]
        MaskFragment2,
        [Description("Mask Shard 3 (Fragment)"), ToolTip("Splits when getting 3rd Mask Shard")]
        MaskFragment3,
        [Description("Mask Upgrade 4 (Upgrade)"), ToolTip("Splits when getting 1 extra Mask (6 base HP)")]
        Mask1,
        [Description("Mask Shard 5 (Fragment)"), ToolTip("Splits when getting 5th Mask Shard")]
        MaskFragment5,
        [Description("Mask Shard 6 (Fragment)"), ToolTip("Splits when getting 6th Mask Shard")]
        MaskFragment6,
        [Description("Mask Shard 7 (Fragment)"), ToolTip("Splits when getting 7th Mask Shard")]
        MaskFragment7,
        [Description("Mask Upgrade 8 (Upgrade)"), ToolTip("Splits when getting 2 extra Masks (7 base HP)")]
        Mask2,
        [Description("Mask Shard 9 (Fragment)"), ToolTip("Splits when getting 9th Mask Shard")]
        MaskFragment9,
        [Description("Mask Shard 10 (Fragment)"), ToolTip("Splits when getting 10th Mask Shard")]
        MaskFragment10,
        [Description("Mask Shard 11 (Fragment)"), ToolTip("Splits when getting 11th Mask Shard")]
        MaskFragment11,
        [Description("Mask Upgrade 12 (Upgrade)"), ToolTip("Splits when getting 3 extra Masks (8 base HP)")]
        Mask3,
        [Description("Mask Shard 13 (Fragment)"), ToolTip("Splits when getting 13th Mask Shard")]
        MaskFragment13,
        [Description("Mask Shard 14 (Fragment)"), ToolTip("Splits when getting 14th Mask Shard")]
        MaskFragment14,
        [Description("Mask Shard 15 (Fragment)"), ToolTip("Splits when getting 15th Mask Shard")]
        MaskFragment15,
        [Description("Mask Upgrade 16 (Upgrade)"), ToolTip("Splits when getting 4 extra Masks (9 base HP)")]
        Mask4,
        [Description("Mask Upgrade 20 (Upgrade)"), ToolTip("(Glitched Only) Splits when getting 5 extra Masks (10 base HP)")]
        Mask5,
        [Description("Nail 1 (Upgrade)"), ToolTip("Splits upon upgrading to the Sharpened Nail")]
        NailUpgrade1,
        [Description("Nail 2 (Upgrade)"), ToolTip("Splits upon upgrading to the Channeled Nail")]
        NailUpgrade2,
        [Description("Nail 3 (Upgrade)"), ToolTip("Splits upon upgrading to the Coiled Nail")]
        NailUpgrade3,
        [Description("Nail 4 (Upgrade)"), ToolTip("Splits upon upgrading to the Pure Nail")]
        NailUpgrade4,
        [Description("Vessel Fragment 1 (Fragment)"), ToolTip("Splits when getting 1st Soul Vessel Fragment")]
        VesselFragment1,
        [Description("Vessel Fragment 2 (Fragment)"), ToolTip("Splits when getting 2nd Soul Vessel Fragment")]
        VesselFragment2,
        [Description("Soul Vessel 1 (Upgrade)"), ToolTip("Splits when upgrading to 1 Soul Vessel (3 Soul Vessel Fragments)")]
        Vessel1,
        [Description("Vessel Fragment 4 (Fragment)"), ToolTip("Splits when getting 4th Soul Vessel Fragment")]
        VesselFragment4,
        [Description("Vessel Fragment 5 (Fragment)"), ToolTip("Splits when getting 5th Soul Vessel Fragment")]
        VesselFragment5,
        [Description("Soul Vessel 2 (Upgrade)"), ToolTip("Splits when upgrading to 2 Soul Vessels (6 Soul Vessel Fragments)")]
        Vessel2,
        [Description("Vessel Fragment 7 (Fragment)"), ToolTip("Splits when getting 7th Soul Vessel Fragment")]
        VesselFragment7,
        [Description("Vessel Fragment 8 (Fragment)"), ToolTip("Splits when getting 8th Soul Vessel Fragment")]
        VesselFragment8,
        [Description("Soul Vessel 3 (Upgrade)"), ToolTip("Splits when upgrading to 3 Soul Vessels (9 Soul Vessel Fragments)")]
        Vessel3,

        [Description("Brooding Mawlek Mask Shard (Obtain)"), ToolTip("Splits when getting the Mask Shard from Brooding Mawlek")]
        MaskShardMawlek,
        [Description("Grub Reward Mask Shard (Obtain)"), ToolTip("Splits when getting the Mask Shard given by Grubfather")]
        MaskShardGrubfather,
        [Description("Goam Mask Shard (Obtain)"), ToolTip("Splits when getting the Goam Mask Shard in Forgotten Crossroads")]
        MaskShardGoam,
        [Description("Queen's Station Mask Shard (Obtain)"), ToolTip("Splits when getting the Mask Shard in Queen's Station")]
        MaskShardQueensStation,
        [Description("Bretta Mask Shard (Obtain)"), ToolTip("Splits when getting the Mask Shard in Bretta's hut in Dirtmouth")]
        MaskShardBretta,
        [Description("Stone Sanctuary Mask Shard (Obtain)"), ToolTip("Splits when getting the Mask Shard in Stone Sanctuary")]
        MaskShardStoneSanctuary,
        [Description("Waterways Mask Shard (Obtain)"), ToolTip("Splits when getting the Mask Shard in Royal Waterways")]
        MaskShardWaterways,
        [Description("Fungal Core Mask Shard (Obtain)"), ToolTip("Splits when getting the Mask Shard below Fungal Core")]
        MaskShardFungalCore,
        [Description("Enraged Guardian Mask Shard (Obtain)"), ToolTip("Splits when getting the Mask Shard from Enraged Guardian")]
        MaskShardEnragedGuardian,
        [Description("Hive Mask Shard (Obtain)"), ToolTip("Splits when getting the Mask Shard in the Hive")]
        MaskShardHive,
        [Description("Seer Mask Shard (Obtain)"), ToolTip("Splits when getting the Mask Shard from Seer")]
        MaskShardSeer,
        [Description("Grey Mourner Mask Shard (Obtain)"), ToolTip("Splits when getting the Mask Shard from Grey Mourner")]
        MaskShardFlower,

        [Description("Greenpath Vessel Fragment (Obtain)"), ToolTip("Splits when getting Vessel Fragment in Greenpath")]
        VesselFragGreenpath,
        [Description("Crossroads Vessel Fragment (Obtain)"), ToolTip("Splits when getting the Vessel Fragment in Forgotten Crossroads")]
        VesselFragCrossroadsLift,
        [Description("King's Station Vessel Fragment (Obtain)"), ToolTip("Splits when getting the Vessel Fragment after the arena above King's Station")]
        VesselFragKingsStation,
        [Description("Deepnest Vessel Fragment (Obtain)"), ToolTip("Splits when getting the Vessel Fragment in Deepnest")]
        VesselFragGarpedes,
        [Description("Stag Nest Vessel Fragment (Obtain)"), ToolTip("Splits when getting the Vessel Fragment in Stag Nest")]
        VesselFragStagNest,
        [Description("Seer Vessel Fragment (Obtain)"), ToolTip("Splits when getting the Vessel Fragment from Seer")]
        VesselFragSeer,
        [Description("Basin Fountain Vessel Fragment (Obtain)"), ToolTip("Splits when getting the Vessel Fragment from the fountain in Ancient Basin")]
        VesselFragFountain,

        [Description("Broken Vessel (Boss)"), ToolTip("Splits when killing Broken Vessel")]
        BrokenVessel,
        [Description("Broken Vessel (Transition)"), ToolTip("Splits on any non-death transition after defeating Broken Vessel")]
        BrokenVesselTrans,
        [Description("Brooding Mawlek (Boss)"), ToolTip("Splits when killing Brooding Mawlek")]
        BroodingMawlek,
        [Description("Enter Brooding Mawlek (Transition)"), ToolTip("Splits when entering the Brooding Mawlek arena transition in Forgotten Crossroads")]
        EnterBroodingMawlek,
        [Description("Collector (Boss)"), ToolTip("Splits when killing Collector")]
        Collector,
        [Description("Collector Defeated (Transition)"), ToolTip("Splits on transition after defeating the Collector")]
        TransCollector,
        [Description("Crystal Guardian (Boss)"), ToolTip("Splits when killing the Crystal Guardian")]
        CrystalGuardian1,
        [Description("Enraged Guardian (Boss)"), ToolTip("Splits when killing the Enraged Guardian")]
        CrystalGuardian2,
        [Description("Dung Defender (Boss)"), ToolTip("Splits when killing Dung Defender")]
        DungDefender,
        [Description("Dung Defender Idol (Item)"), ToolTip("Splits when picking up Dung Defender idol as the first idol")]
        DungDefenderIdol,
        [Description("Glade Idol (Item)"), ToolTip("Splits when picking up the King's Idol in the Spirits' Glade")]
        GladeIdol,
        [Description("Elder Hu (Boss)"), ToolTip("Splits when killing Elder Hu")]
        ElderHu,
        [Description("Elder Hu (Essence)"), ToolTip("Splits when absorbing essence from Elder Hu")]
        ElderHuEssence,
        [Description("Elder Hu Killed (Transition)"), ToolTip("Splits on the transition after killing Elder Hu")]
        ElderHuTrans,
        [Description("False Knight (Boss)"), ToolTip("Splits when killing False Knight")]
        FalseKnight,
        [Description("Failed Champion (Boss)"), ToolTip("Splits when killing Failed Champion")]
        FailedKnight,
        [Description("Failed Champion (Essence)"), ToolTip("Splits when getting Failed Champion essence")]
        FailedChampionEssence,
        [Description("Flukemarm (Boss)"), ToolTip("Splits when killing Flukemarm")]
        Flukemarm,
        [Description("Galien (Boss)"), ToolTip("Splits when killing Galien")]
        Galien,
        [Description("Galien (Essence)"), ToolTip("Splits when absorbing essence from Galien")]
        GalienEssence,
        [Description("God Tamer (Boss)"), ToolTip("Splits when killing the God Tamer")]
        GodTamer,
        [Description("Gorb (Boss)"), ToolTip("Splits when killing Gorb")]
        Gorb,
        [Description("Gorb (Essence)"), ToolTip("Splits when absorbing essence from Gorb")]
        GorbEssence,
        [Description("Grey Prince Zote (Boss)"), ToolTip("Splits when killing Grey Prince")]
        GreyPrince,
        [Description("Grey Prince Zote (Essence)"), ToolTip("Splits when getting Grey Prince Zote essence")]
        GreyPrinceEssence,
        [Description("Grey Prince Zote Level (Boss)"), ToolTip("Splits each time defeating Grey Prince Zote in Bretta's dream")]
        OnDefeatGPZ,
        [Description("Gruz Mother (Boss)"), ToolTip("Splits when killing Gruz Mother")]
        GruzMother,
        [Description("Hive Knight (Boss)"), ToolTip("Splits when killing Hive Knight")]
        HiveKnight,
        [Description("Enter Hive Knight (Transition)"), ToolTip("Splits when entering Hive Knight boss arena transition")]
        EnterHiveKnight,
        [Description("Hornet 1 (Boss)"), ToolTip("Splits when killing Hornet Protector in Greenpath")]
        Hornet1,
        [Description("Enter Hornet 1 (Transition)"), ToolTip("Splits when entering Hornet boss arena transition in Greenpath")]
        EnterHornet1,
        [Description("Hornet 2 (Boss)"), ToolTip("Splits when killing Hornet Sentinel in Kingdom's Edge")]
        Hornet2,
        [Description("Enter Hornet 2 (Transition)"), ToolTip("Splits when entering Hornet boss arena transition in Kingdom's Edge")]
        EnterHornet2,
        [Description("Lost Kin (Boss)"), ToolTip("Splits when killing Lost Kin")]
        LostKin,
        [Description("Lost Kin (Essence)"), ToolTip("Splits when getting Lost Kin essence")]
        LostKinEssence,
        [Description("Mantis Lords (Boss)"), ToolTip("Splits when killing Mantis Lords")]
        MantisLords,
        [Description("Markoth (Boss)"), ToolTip("Splits when killing Markoth")]
        Markoth,
        [Description("Markoth (Essence)"), ToolTip("Splits when absorbing essence from Markoth")]
        MarkothEssence,
        [Description("Marmu (Boss)"), ToolTip("Splits when killing Marmu")]
        Marmu,
        [Description("Marmu (Essence)"), ToolTip("Splits when absorbing essence from Marmu")]
        MarmuEssence,
        [Description("Massive Moss Charger (Boss)"), ToolTip("Splits when killing Massive Moss Charger")]
        MegaMossCharger,
        [Description("Massive Moss Charger Killed (Transition)"), ToolTip("Splits on transition after Massive Moss Charger is killed")]
        MegaMossChargerTrans,
        [Description("Nightmare King Grimm (Boss)"), ToolTip("Splits when killing Nightmare King Grimm")]
        NightmareKingGrimm,
        [Description("No Eyes (Boss)"), ToolTip("Splits when killing No Eyes")]
        NoEyes,
        [Description("No Eyes (Essence)"), ToolTip("Splits when absorbing essence from No Eyes")]
        NoEyesEssence,
        [Description("Nosk (Boss)"), ToolTip("Splits when killing Nosk")]
        Nosk,
        [Description("Nosk (Transition)"), ToolTip("Splits when entering Nosk boss arena transition")]
        EnterNosk,
        [Description("Oblobbles (Boss)"), ToolTip("Splits when 2 Oblobbles are defeated (ideally the first pair you encounter in Colo 2)")]
        KilledOblobbles,
        [Description("Oro & Mato Nail Bros (Boss)"), ToolTip("Splits when defeating Brothers Oro & Mato")]
        MatoOroNailBros,
        [Description("Pure Vessel (Boss)"), ToolTip("Splits when killing Pure Vessel")]
        PureVessel,
        [Description("Segment Practice - Radiance (Boss)"), ToolTip("Splits when killing The Radiance")]
        RadianceBoss,
        [Description("Segment Practice - THK (Boss)"), ToolTip("Splits when killing The Hollow Knight")]
        HollowKnightBoss,
        [Description("Paintmaster Sheo (Boss)"), ToolTip("Splits when killing Paintmaster Sheo")]
        SheoPaintmaster,
        [Description("Great Nailsage Sly (Boss)"), ToolTip("Splits when killing Great Nailsage Sly")]
        SlyNailsage,
        [Description("Soul Master (Boss)"), ToolTip("Splits when killing Soul Master")]
        SoulMaster,
        [Description("Soul Master - Fake Spell Pickup (Boss)"), ToolTip("Splits when triggering Soul Master phase 2 the first time")]
        SoulMasterPhase1,
        [Description("Enter Soul Master (Transition)"), ToolTip("Splits when entering Soul Master boss arena transition")]
        EnterSoulMaster,
        [Description("Soul Master Encountered (Boss)"), ToolTip("Splits when Soul Master is activated the first time as the gate closes")]
        SoulMasterEncountered,
        [Description("Soul Sanctum Hallownest Seal (Item)"), ToolTip("Splits when the Hallownest Seal in Soul Sanctum is collected")]
        SoulSanctumSeal,
        [Description("Soul Tyrant (Boss)"), ToolTip("Splits when killing Soul Tyrant")]
        SoulTyrant,
        [Description("Soul Tyrant (Essence)"), ToolTip("Splits when getting Soul Tyrant essence")]
        SoulTyrantEssence,
        [Description("Soul Tyrant w/ Sanctum Grub (Essence)"), ToolTip("Splits when getting Soul Tyrant essence and Sanctum fakedive grub")]
        SoulTyrantEssenceWithSanctumGrub,
        [Description("Traitor Lord (Boss)"), ToolTip("Splits when killing Traitor Lord")]
        TraitorLord,
        [Description("Troupe Master Grimm (Boss)"), ToolTip("Splits when killing Troupe Master Grimm")]
        TroupeMasterGrimm,
        [Description("Enter Troupe Master Grimm (Transition)"), ToolTip("Splits when entering Grimm tent with requirements to trigger Troupe Master Grimm boss")]
        EnterTMG,
        [Description("Uumuu (Boss)"), ToolTip("Splits when killing Uumuu")]
        Uumuu,
        [Description("Uumuu Encountered (Boss)"), ToolTip("Splits Uumuu is activated the first time as the gate closes")]
        UumuuEncountered,
        [Description("Watcher Knight (Boss)"), ToolTip("Splits when killing Watcher Knights")]
        BlackKnight,
        [Description("Watcher Knight Killed (Transition)"), ToolTip("Splits on the transition after killing Watcher Knights")]
        BlackKnightTrans,
        [Description("White Defender (Boss)"), ToolTip("Splits when killing White Defender")]
        WhiteDefender,
        [Description("White Defender (Essence)"), ToolTip("Splits when getting White Defender essence")]
        WhiteDefenderEssence,
        [Description("White Defender Level (Boss)"), ToolTip("Splits each time defeating White Defender in Dung Defender's dream")]
        OnDefeatWhiteDefender,
        [Description("Xero (Boss)"), ToolTip("Splits when killing Xero")]
        Xero,
        [Description("Xero (Essence)"), ToolTip("Splits when absorbing essence from Xero")]
        XeroEssence,

        [Description("Vengefly King (Pantheon)"), ToolTip("Splits after killing Vengefly King in Pantheon 1 or Pantheon 5")]
        VengeflyKingP,
        [Description("Gruz Mother (Pantheon)"), ToolTip("Splits after killing Gruz Mother in Pantheon 1 or Pantheon 5")]
        GruzMotherP,
        [Description("False Knight (Pantheon)"), ToolTip("Splits after killing False Knight in Pantheon 1 or Pantheon 5")]
        FalseKnightP,
        [Description("Massive Moss Charger (Pantheon)"), ToolTip("Splits after killing Massive Moss Charger in Pantheon 1 or Pantheon 5")]
        MassiveMossChargerP,
        [Description("Hornet 1 (Pantheon)"), ToolTip("Splits after killing Hornet Protector in Pantheon 1 or Pantheon 5")]
        Hornet1P,
        [Description("Gorb (Pantheon)"), ToolTip("Splits after killing Gorb in Pantheon 1 or Pantheon 5")]
        GorbP,
        [Description("Dung Defender (Pantheon)"), ToolTip("Splits after killing Dung Defender in Pantheon 1 or Pantheon 5")]
        DungDefenderP,
        [Description("Soul Warrior (Pantheon)"), ToolTip("Splits after killing Soul Warrior in Pantheon 1 or Pantheon 5")]
        SoulWarriorP,
        [Description("Brooding Mawlek (Pantheon)"), ToolTip("Splits after killing Brooding Mawlek in Pantheon 1 or Pantheon 5")]
        BroodingMawlekP,
        [Description("Oro & Mato Nail Bros (Pantheon)"), ToolTip("Splits after killing Brothers Oro & Mato in Pantheon 1 or Pantheon 5")]
        OroMatoNailBrosP,

        [Description("Xero (Pantheon)"), ToolTip("Splits after killing Xero in Pantheon 2 or Pantheon 5")]
        XeroP,
        [Description("Crystal Guardian (Pantheon)"), ToolTip("Splits after killing Crystal Guardian in Pantheon 2 or Pantheon 5")]
        CrystalGuardianP,
        [Description("Soul Master (Pantheon)"), ToolTip("Splits after killing Soul Master in Pantheon 2 or Pantheon 5")]
        SoulMasterP,
        [Description("Oblobbles (Pantheon)"), ToolTip("Splits after killing Oblobbles in Pantheon 2 or Pantheon 5")]
        OblobblesP,
        [Description("Mantis Lords (Pantheon)"), ToolTip("Splits after killing Mantis Lords in Pantheon 2 or Sisters of Battle Pantheon 5")]
        MantisLordsP,
        [Description("Marmu (Pantheon)"), ToolTip("Splits after killing Marmu in Pantheon 2 or Pantheon 5")]
        MarmuP,
        [Description("Nosk (Pantheon)"), ToolTip("Splits after killing Nosk in Pantheon 2")]
        NoskP,
        [Description("Flukemarm (Pantheon)"), ToolTip("Splits after killing Flukemarm in Pantheon 2 or Pantheon 5")]
        FlukemarmP,
        [Description("Broken Vessel (Pantheon)"), ToolTip("Splits after killing Broken Vessel in Pantheon 2 or Pantheon 5")]
        BrokenVesselP,
        [Description("Paintmaster Sheo (Pantheon)"), ToolTip("Splits after killing Paintmaster Sheo in Pantheon 2 or Pantheon 5")]
        SheoPaintmasterP,

        [Description("Hive Knight (Pantheon)"), ToolTip("Splits after killing Hive Knight in Pantheon 3 or Pantheon 5")]
        HiveKnightP,
        [Description("Elder Hu (Pantheon)"), ToolTip("Splits after killing Elder Hu in Pantheon 3 or Pantheon 5")]
        ElderHuP,
        [Description("Collector (Pantheon)"), ToolTip("Splits after killing The Collector in Pantheon 3 or Pantheon 5")]
        CollectorP,
        [Description("God Tamer (Pantheon)"), ToolTip("Splits after killing God Tamer in Pantheon 3 or Pantheon 5")]
        GodTamerP,
        [Description("Troupe Master Grimm (Pantheon)"), ToolTip("Splits after killing Troupe Master Grimm in Pantheon 3 or Pantheon 5")]
        TroupeMasterGrimmP,
        [Description("Galien (Pantheon)"), ToolTip("Splits after killing Galien in Pantheon 3 or Pantheon 5")]
        GalienP,
        [Description("Grey Prince Zote (Pantheon)"), ToolTip("Splits after killing Grey Prince Zote in Pantheon 3 or Pantheon 5")]
        GreyPrinceZoteP,
        [Description("Uumuu (Pantheon)"), ToolTip("Splits after killing Uumuu in Pantheon 3 or Pantheon 5")]
        UumuuP,
        [Description("Hornet 2 (Pantheon)"), ToolTip("Splits after killing Hornet Sentinel in Pantheon 3 or Pantheon 5")]
        Hornet2P,
        [Description("Great Nailsage Sly (Pantheon)"), ToolTip("Splits after killing Great Nailsage Sly in Pantheon 3 or Pantheon 5")]
        SlyP,

        [Description("Enraged Guardian (Pantheon)"), ToolTip("Splits after killing Enraged Guardian in Pantheon 4 or Pantheon 5")]
        EnragedGuardianP,
        [Description("Lost Kin (Pantheon)"), ToolTip("Splits after killing Lost Kin in Pantheon 4 or Pantheon 5")]
        LostKinP,
        [Description("No Eyes (Pantheon)"), ToolTip("Splits after killing No Eyes in Pantheon 4 or Pantheon 5")]
        NoEyesP,
        [Description("Traitor Lord (Pantheon)"), ToolTip("Splits after killing Traitor Lord in Pantheon 4 or Pantheon 5")]
        TraitorLordP,
        [Description("White Defender (Pantheon)"), ToolTip("Splits after killing White Defender in Pantheon 4 or Pantheon 5")]
        WhiteDefenderP,
        [Description("Failed Champion (Pantheon)"), ToolTip("Splits after killing Failed Champion in Pantheon 4 or Pantheon 5")]
        FailedChampionP,
        [Description("Markoth (Pantheon)"), ToolTip("Splits after killing Markoth in Pantheon 4 or Pantheon 5")]
        MarkothP,
        [Description("Watcher Knights (Pantheon)"), ToolTip("Splits after killing Watcher Knights in Pantheon 4 or Pantheon 5")]
        WatcherKnightsP,
        [Description("Soul Tyrant (Pantheon)"), ToolTip("Splits after killing Soul Tyrant in Pantheon 4 or Pantheon 5")]
        SoulTyrantP,
        [Description("Pure Vessel (Pantheon)"), ToolTip("Splits after killing Pure Vessel in Pantheon 4 or Pantheon 5")]
        PureVesselP,

        [Description("Winged Nosk (Pantheon)"), ToolTip("Splits after killing Winged Nosk in Pantheon 5")]
        NoskHornetP,
        [Description("Nightmare King Grimm (Pantheon)"), ToolTip("Splits after killing Nightmare King Grimm in Pantheon 5")]
        NightmareKingGrimmP,
        [Description("Absolute Radiance (Ending)"), ToolTip("Splits after killing Absolute Radiance in Pantheon 5, Godhome Ending")]
        RadianceP,

        [Description("Herrah the Beast (Dreamer)"), ToolTip("Splits when you see the mask for Herrah")]
        Hegemol,
        [Description("Lurien the Watcher (Dreamer)"), ToolTip("Splits when you see the mask for Lurien")]
        Lurien,
        [Description("Monomon the Teacher (Dreamer)"), ToolTip("Splits when you see the mask for Monomon")]
        Monomon,

        [Description("Herrah (Old Dreamer Timing)"), ToolTip("For Dreamer Quit-outs on older patches. Splits when Herrah is registered as defeated (In Spider Area)")]
        HegemolDreamer,
        [Description("Lurien (Old Dreamer Timing)"), ToolTip("For Dreamer Quit-outs on older patches. Splits when Lurien is registered as defeated (After killing Watcher Knight)")]
        LurienDreamer,
        [Description("Monomon (Old Dreamer Timing)"), ToolTip("For Dreamer Quit-outs on older patches. Splits when Monomon is registered as defeated (After killing Uumuu)")]
        MonomonDreamer,

        [Description("First Dreamer (Dreamer)"), ToolTip("Splits when you see the mask for the first dreamer killed")]
        Dreamer1,
        [Description("Second Dreamer (Dreamer)"), ToolTip("Splits when you see the mask for the second dreamer killed")]
        Dreamer2,
        [Description("Third Dreamer (Dreamer)"), ToolTip("Splits when you see the mask for the third dreamer killed")]
        Dreamer3,

        [Description("106% Pre-Grimm Shop (Event)"), ToolTip("Splits when Lantern + Vessel Fragment (5) + Mask Shard (4) have been acquired")]
        PreGrimmShop,
        [Description("106% Pre-Grimm Shop (Transition)"), ToolTip("Splits when Lantern + Vessel Fragment (5) + Mask Shard (4) have been acquired")]
        PreGrimmShopTrans,
        [Description("1xx% Sly Final Shop (Transition)"), ToolTip("Splits on leaving Sly's shop after having bought Sprintmaster and Vessel Fragment 8")]
        SlyShopFinished,
        [Description("Abyss Door (Event)"), ToolTip("Splits on the Abyss door opening")]
        AbyssDoor,
        [Description("Abyss Lighthouse (Event)"), ToolTip("Splits on the Abyss Lighthouse being lit")]
        AbyssLighthouse,
        [Description("Can Overcharm (Event)"), ToolTip("Splits when overcharming is enabled")]
        CanOvercharm,
        [Description("Chains Broken - Hollow Knight (Event)"), ToolTip("Splits at the end of the first Hollow Knight scream after the chains are broken")]
        UnchainedHollowKnight,
        [Description("Chandelier - Watcher Knights (Event)"), ToolTip("Splits when dropping the chandelier on one of the Watcher Knights")]
        WatcherChandelier,
        [Description("City Gate (Event)"), ToolTip("Splits when using the City Crest to open the gate")]
        CityGateOpen,
        [Description("City Gate w/ Mantis Lords defeated (Event)"), ToolTip("To make sure you don't forget Mantis Lords")]
        CityGateAndMantisLords,
        [Description("Death (Event)"), ToolTip("Splits when player HP is 0")]
        PlayerDeath,
        [Description("Shade Killed (Event)"), ToolTip("Splits when the Shade is killed")]
        ShadeKilled,
        [Description("Shop Lumafly Lantern (Transition)"), ToolTip("Splits on transition after Lantern has been acquired")]
        LumaflyLanternTransition,
        [Description("Flower Quest (Event)"), ToolTip("Splits when placing the flower at the grave of the Traitors' Child")]
        FlowerQuest,
        [Description("Flower Quest Reward (Event)"), ToolTip("Splits when Grey Mourner gives you the Flower Quest reward")]
        FlowerRewardGiven,
        [Description("Happy Couple (Event)"), ToolTip("Splits when talking to Nailsmith in Sheo's hut for the first time")]
        HappyCouplePlayerDataEvent,
        [Description("Stinky (Event)"), ToolTip("Splits when seeing the Dung Defender's statue of the Knight")]
        WhiteDefenderStatueUnlocked,
        [Description("Nailsmith Killed (Event)"), ToolTip("Splits when Nailsmith is killed")]
        NailsmithKilled,
        [Description("Nailsmith Killed/Spared (Event)"), ToolTip("Splits when Nailsmith is killed\nSkips when nailsmith is spared (requires ordered splits)")]
        NailsmithChoice,
        [Description("Nightmare Lantern Lit (Event)"), ToolTip("Splits when initially lighting the Nightmare Lantern")]
        NightmareLantern,
        [Description("Nightmare Lantern Destroyed (Event)"), ToolTip("Splits when destroying the Nightmare Lantern")]
        NightmareLanternDestroyed,
        [Description("Radiance Dream Entry (Event)"), ToolTip("Splits upon entering the Radiance dream\nSkips upon killing the Hollow Knight (requires ordered splits)")]
        HollowKnightDreamnail,
        [Description("Seer Departs (Event)"), ToolTip("Splits when the Seer Departs after bringing back 2400 essence")]
        SeerDeparts,
        [Description("Spirit Glade Door (Event)"), ToolTip("Splits when the Seer opens the Spirits' Glade after bringing back 200 essence")]
        SpiritGladeOpen,
        [Description("Trap Bench (Event)"), ToolTip("Splits when getting the trap bench in Beasts Den")]
        BeastsDenTrapBench,
        [Description("Eternal Ordeal Unlocked (Event)"), ToolTip("Splits when breaking the wall to the Zote statue in Godhome")]
        EternalOrdealUnlocked,
        [Description("Eternal Ordeal Achieved (Event)"), ToolTip("Splits when achieving the ordeal (57th Zote killed)")]
        EternalOrdealAchieved,
        [Description("Riding Stag (Event)"), ToolTip("Splits while riding the stag")]
        RidingStag,
        [Description("Stag Position Updated (Event)"), ToolTip("Splits when the stag is called")]
        StagMoved,
        [Description("Saved Cloth (Event)"), ToolTip("Splits when saving Cloth in Ancient Basin")]
        SavedCloth,
        [Description("Crystal Peak Lift Opened (Event)"), ToolTip("Splits when opening the lever for the lift between Dirtmouth and Crystal Peak")]
        MineLiftOpened,
        [Description("Shape of Unn Synergies / Pure Snail (Event)"), ToolTip("Splits when recovering health with Spore Shroom, Quick Focus, Baldur Shell, and Shape of Unn equipped")]
        PureSnail,
        [Description("Colosseum Unlocked 1 (Trial)"), ToolTip("Splits when the knight unlocks the Trial of the Warrior at Little Fool")]
        ColosseumBronzeUnlocked,
        [Description("Colosseum Unlocked 2 (Trial)"), ToolTip("Splits when the knight unlocks the Trial of the Conqueror at Little Fool")]
        ColosseumSilverUnlocked,
        [Description("Colosseum Unlocked 3 (Trial)"), ToolTip("Splits when the knight unlocks the Trial of the Fool at Little Fool")]
        ColosseumGoldUnlocked,
        [Description("Colosseum Fight 1 (Trial)"), ToolTip("Splits when beating the Trial of the Warrior")]
        ColosseumBronze,
        [Description("Colosseum Fight 2 (Trial)"), ToolTip("Splits when beating the Trial of the Conqueror")]
        ColosseumSilver,
        [Description("Colosseum Fight 3 (Trial)"), ToolTip("Splits when beating the Trial of the Warrior")]
        ColosseumGold,
        [Description("Colosseum Entrance 1 (Transition)"), ToolTip("Splits on the transition into the Trial of the Warrior")]
        ColosseumBronzeEntry,
        [Description("Colosseum Entrance 2 (Transition)"), ToolTip("Splits on the transition into the Trial of the Conqueror")]
        ColosseumSilverEntry,
        [Description("Colosseum Entrance 3 (Transition)"), ToolTip("Splits on the transition into the Trial of the Warrior")]
        ColosseumGoldEntry,
        [Description("Colosseum Exit 1 (Transition)"), ToolTip("Splits on the transition out of the trial, or in the load-in after quitout")]
        ColosseumBronzeExit,
        [Description("Colosseum Exit 2 (Transition)"), ToolTip("Splits on the transition out of the trial, or in the load-in after quitout")]
        ColosseumSilverExit,
        [Description("Colosseum Exit 3 (Transition)"), ToolTip("Splits on the transition out of the trial, or in the load-in after quitout")]
        ColosseumGoldExit,
        [Description("Pantheon 1 (Trial)"), ToolTip("Splits when beating the Pantheon of the Master")]
        Pantheon1,
        [Description("Pantheon 2 (Trial)"), ToolTip("Splits when beating the Pantheon of the Artist")]
        Pantheon2,
        [Description("Pantheon 3 (Trial)"), ToolTip("Splits when beating the Pantheon of the Sage")]
        Pantheon3,
        [Description("Pantheon 4 (Trial)"), ToolTip("Splits when beating the Pantheon of the Knight")]
        Pantheon4,
        [Description("Pantheon 5 (Trial)"), ToolTip("Splits when beating the Pantheon of Hallownest")]
        Pantheon5,
        [Description("Path of Pain (Completed)"), ToolTip("Splits when completing the Path of Pain in White Palace")]
        PathOfPain,

        [Description("Aspid Hunter (Mini Boss)"), ToolTip("Splits when killing 3 Aspid Hunters in a row")]
        AspidHunter,
        [Description("Aluba (Killed)"), ToolTip("Splits when killing an Aluba")]
        Aluba,
        [Description("Crystal Peak Hunter's Notes (Killed)"), ToolTip("Splits when killing all Crystal Peak enemies needed for Hunter's Notes journal completion\n(Includes Crystal Crawler, Crystal Guardian 1&2, Crystal Hunter, Crystallised Husk, Glimback, Husk Miner, and Shardmite. Excludes Bluggsac, Grimmkin Novice, and Grub Mimic.)")]
        CrystalPeakHuntersNotes,
        [Description("Little Baldur Hunter's Notes (Killed)"), ToolTip("Splits when killing all little Baldurs needed for Hunter's Notes journal completion")]
        RollerHuntersNotes,
        [Description("Maggots (Killed)"), ToolTip("Splits when killing both Maggots")]
        Maggots,
        [Description("Husk Miner (Killed)"), ToolTip("Splits when killing a Husk Miner")]
        HuskMiner,
        [Description("Great Hopper (Killed)"), ToolTip("Splits when killing a Great Hopper")]
        GreatHopper,
        [Description("Great Husk Sentry (Killed)"), ToolTip("Splits when killing a Great Husk Sentry")]
        GreatHuskSentry,
        [Description("Gorgeous Husk (Killed)"), ToolTip("Splits when killing Gorgeous Husk")]
        GorgeousHusk,
        [Description("Menderbug (Killed)"), ToolTip("Splits when killing Menderbug")]
        MenderBug,
        [Description("Soul Warrior (Killed)"), ToolTip("Splits on first Soul Warrior kill")]
        killedSanctumWarrior,
        [Description("Soul Twister (Killed)"), ToolTip("Splits on first Soul Twister kill")]
        killedSoulTwister,
        [Description("Moss Knight (Mini Boss)"), ToolTip("Splits when killing Moss Knight")]
        MossKnight,
        [Description("Shrumal Ogres (Mini Boss)"), ToolTip("Splits when killing 2 Shrumal Ogres in a row")]
        MushroomBrawler,
        [Description("Petra Arena (Mini Boss)"), ToolTip("Splits when killing 3 Petras in a row")]
        PetraArena,
        [Description("Zote Rescued - Vengefly King (Mini Boss)"), ToolTip("Splits when rescuing Zote from the Vengefly King")]
        Zote1,
        [Description("Vengefly King Killed (Transition)"), ToolTip("Splits on transition after Vengefly King in Greenpath killed")]
        VengeflyKingTrans,
        [Description("Zote Rescued - Deepnest (Mini Boss)"), ToolTip("Splits when rescuing Zote in Deepnest")]
        Zote2,
        [Description("Zote Defeated - Colosseum (Mini Boss)"), ToolTip("Splits when defeating Zote in the Colosseum")]
        ZoteKilled,

        [Description("Forgotten Crossroads (Stag Station)"), ToolTip("Splits when opening the Forgotten Crossroads Stag Station")]
        CrossroadsStation,
        [Description("Greenpath (Stag Station)"), ToolTip("Splits when obtaining Greenpath Stag Station")]
        GreenpathStation,
        [Description("Queen's Station (Stag Station)"), ToolTip("Splits when obtaining Queen's Station Stag Station")]
        QueensStationStation,
        [Description("Queen's Gardens (Stag Station)"), ToolTip("Splits when obtaining Queen's Gardens Stag Station")]
        QueensGardensStation,
        [Description("City Storerooms (Stag Station)"), ToolTip("Splits when obtaining City Storerooms Stag Station")]
        StoreroomsStation,
        [Description("King's Station (Stag Station)"), ToolTip("Splits when obtaining King's Station Stag Station")]
        KingsStationStation,
        [Description("Resting Grounds (Stag Station)"), ToolTip("Splits when obtaining Resting Grounds Stag Station")]
        RestingGroundsStation,
        [Description("Distant Village (Stag Station)"), ToolTip("Splits when obtaining Distant Village Stag Station")]
        DeepnestStation,
        [Description("Hidden Station (Stag Station)"), ToolTip("Splits when obtaining Hidden Station Stag Station")]
        HiddenStationStation,
        [Description("Stagnest (Stag Station)"), ToolTip("Splits when traveling to Stagnest (Requires Ordered Splits)")]
        StagnestStation,
        [Description("Stagnest Buy (Stag Station)"), ToolTip("Splits when obtaining Stagnest Station (Not for All Stag Stations)")]
        StagnestStationBuy,

        [Description("Mr. Mushroom 1 (Spot)"), ToolTip("Splits when talking to Mister Mushroom in Fungal Wastes")]
        MrMushroom1,
        [Description("Mr. Mushroom 2 (Spot)"), ToolTip("Splits when talking to Mister Mushroom in Kingdom's Edge")]
        MrMushroom2,
        [Description("Mr. Mushroom 3 (Spot)"), ToolTip("Splits when talking to Mister Mushroom in Deepnest")]
        MrMushroom3,
        [Description("Mr. Mushroom 4 (Spot)"), ToolTip("Splits when talking to Mister Mushroom in Mato's Hut")]
        MrMushroom4,
        [Description("Mr. Mushroom 5 (Spot)"), ToolTip("Splits when talking to Mister Mushroom in Ancient Basin")]
        MrMushroom5,
        [Description("Mr. Mushroom 6 (Spot)"), ToolTip("Splits when talking to Mister Mushroom by Overgrown Mound")]
        MrMushroom6,
        [Description("Mr. Mushroom 7 (Spot)"), ToolTip("Splits when talking to Mister Mushroom in King's Pass")]
        MrMushroom7,

        [Description("Ancient Basin (Area)"), ToolTip("Splits when entering Ancient Basin text first appears")]
        Abyss,
        [Description("City of Tears (Area)"), ToolTip("Splits when entering City of Tears text first appears")]
        CityOfTears,
        [Description("Colosseum (Area)"), ToolTip("Splits when entering Colosseum text first appears")]
        Colosseum,
        [Description("Crystal Peak (Area)"), ToolTip("Splits when entering Crystal Peak text first appears")]
        CrystalPeak,
        [Description("Deepnest (Area)"), ToolTip("Splits when entering Deepnest text first appears")]
        Deepnest,
        [Description("Deepnest Spa (Area)"), ToolTip("Splits when entering the Deepnest Spa area with bench")]
        DeepnestSpa,
        [Description("Dirtmouth (Area)"), ToolTip("Splits when entering Dirtmouth text first appears")]
        Dirtmouth,
        [Description("Fog Canyon (Area)"), ToolTip("Splits when entering Fog Canyon text first appears")]
        FogCanyon,
        [Description("Forgotten Crossroads (Area)"), ToolTip("Splits when entering Forgotten Crossroads text first appears")]
        ForgottenCrossroads,
        [Description("Fungal Wastes (Area)"), ToolTip("Splits when entering Fungal Wastes text first appears")]
        FungalWastes,
        [Description("Godhome (Area)"), ToolTip("Splits when entering Godhome text first appears")]
        Godhome,
        [Description("Greenpath (Area)"), ToolTip("Splits when entering Greenpath text first appears")]
        Greenpath,
        [Description("Hive (Area)"), ToolTip("Splits when entering Hive text first appears")]
        Hive,
        [Description("Infected Crossroads (Area)"), ToolTip("Splits when entering Infected Crossroads text first appears")]
        InfectedCrossroads,
        [Description("Kingdom's Edge (Area)"), ToolTip("Splits when entering Kingdom's Edge text first appears")]
        KingdomsEdge,
        [Description("Queen's Gardens (Area)"), ToolTip("Splits when entering Queen's Gardens text first appears")]
        QueensGardens,
        [Description("Resting Grounds (Area)"), ToolTip("Splits when entering Resting Grounds text first appears")]
        RestingGrounds,
        [Description("Royal Waterways (Area)"), ToolTip("Splits when entering Royal Waterways text first appears")]
        RoyalWaterways,
        [Description("Teachers Archive (Area)"), ToolTip("Splits when entering Teachers Archive for the first time")]
        TeachersArchive,
        [Description("White Palace (Area)"), ToolTip("Splits when entering White Palace text for the first time")]
        WhitePalace,
        [Description("White Palace - Workshop (Area)"), ToolTip("Splits when visiting the secret room in White Palace")]
        WhitePalaceSecretRoom,

        [Description("Ancestral Mound (Transition)"), ToolTip("Splits on transition into Ancestral Mound")]
        AncestralMound,
        [Description("Ancient Basin (Transition)"), ToolTip("Splits on transition to Ancient Basin")]
        BasinEntry,
        [Description("Basin Spike Pit Exit (Transition)"), ToolTip("Splits on transition after crossing the Spike Pit from Basin Toll Bench towards Broken Vessel")]
        BasinSpikePitExit,
        [Description("Beast Den (Transition)"), ToolTip("Splits on transition into Beast Den")]
        EnterBeastDen,
        [Description("Blue Lake (Transition)"), ToolTip("Splits on transition to Blue Lake from either side")]
        BlueLake,
        [Description("Bretta House (Transition)"), ToolTip("Splits on transition to Bretta's House")]
        BrettaHouse,
        [Description("Modded: Bretta House Bubbles (Transition)"), ToolTip("Splits on transition to the fourth room of Bretta's House C-Side ('Scattered and Lost' mod)")]
        BrettaHouseBubbles,
        [Description("Modded: Bretta House Bumpers (Transition)"), ToolTip("Splits on transition to the third room of Bretta's House C-Side ('Scattered and Lost' mod)")]
        BrettaHouseBumpers,
        [Description("Modded: Bretta House Switches (Transition)"), ToolTip("Splits on transition to the second room of Bretta's House C-Side ('Scattered and Lost' mod)")]
        BrettaHouseSwitches,
        [Description("Modded: Bretta House Zippers (Transition)"), ToolTip("Splits on transition to the first room of Bretta's House C-Side ('Scattered and Lost' mod)")]
        BrettaHouseZippers,
        [Description("Catacombs Entry (Transition)"), ToolTip("Splits on entry to the catacombs below Resting Grounds")]
        CatacombsEntry,
        [Description("Crystal Peak Entry (Transition)"), ToolTip("Splits on transition to the room where the dive and toll entrances meet, or the room right of Dirtmouth")]
        CrystalPeakEntry,
        [Description("Crystal Mound Exit (Transition)"), ToolTip("Splits on transition from Crystal Mound")]
        CrystalMoundExit,
        [Description("Crossroads (Transition)"), ToolTip("Splits on transition through the Well from Dirtmouth to Crossroads")]
        EnterCrossroads,
        [Description("Deepnest (Transition)"), ToolTip("Splits on transition into Deepnest")]
        EnterDeepnest,
        [Description("Dirtmouth (Transition)"), ToolTip("Splits on any transition into Dirtmouth Town")]
        EnterDirtmouth,
        [Description("Enter Any Dream (Transition)"), ToolTip("Splits when entering any dream world")]
        EnterAnyDream,
        [Description("Fog Canyon (Transition)"), ToolTip("Splits on transition to Fog Canyon\n(Room below Greenpath, left of Queen's Station, right of Overgrown Mound, or below Crossroads)")]
        FogCanyonEntry,
        [Description("Fungal Wastes Entry (Transition)"), ToolTip("Splits on transition to Fungal Wastes\n(Room below Crossroads, right of Queen's Station, left of Waterways or Spore Shroom room)")]
        FungalWastesEntry,
        [Description("Gorgeous Husk Killed (Transition)"), ToolTip("Splits on transition after Gorgeous Husk defeated")]
        TransGorgeousHusk,
        [Description("Godhome (Transition)"), ToolTip("Splits on transition to Godhome")]
        EnterGodhome,
        [Description("Greenpath (Transition)"), ToolTip("Splits when entering Greenpath")]
        EnterGreenpath,
        [Description("Greenpath w/ Unlocked Overcharm (Transition)"), ToolTip("Splits when entering Greenpath with overcharming unlocked")]
        EnterGreenpathWithOvercharm,
        [Description("Hallownest's Crown (Transition)"), ToolTip("Splits on transition into the room with the Whispering Root at the base of Hallownest's Crown")]
        EnterCrown,
        [Description("Rafters (Transition)"), ToolTip("Splits on any transition into the City Rafters room")]
        EnterRafters,
        [Description("Lemm Shop Exit (Transition)"), ToolTip("Splits on transition after talking to Lemm in the shop")]
        LemmShopExit,
        [Description("Salubra Exit (Transition)"), ToolTip("Splits on the transition out of Salubra's Hut")]
        SalubraExit,
        [Description("Spire Bench Exit (Transition)"), ToolTip("Splits on the transition out of the bench room in Watcher's Spire")]
        SpireBenchExit,

        [Description("Has Claw (Transition)"), ToolTip("Splits on transition after Mantis Claw acquired")]
        TransClaw,
        [Description("Has Vengeful Spirit (Transition)"), ToolTip("Splits on transition after Vengeful Spirit acquired")]
        TransVS,
        [Description("Has Shade Soul (Transition)"), ToolTip("Splits on transition after Shade Soul acquired (Room Dupe safe)")]
        TransShadeSoul,
        [Description("Has Descending Dark (Transition)"), ToolTip("Splits on transition after Descending Dark acquired")]
        TransDescendingDark,
        [Description("Has Map Crossroads (Transition)"), ToolTip("Splits on transition after Crossroads Map acquired")]
        TransMapCrossroads,
        [Description("Has Isma's Tear (Transition)"), ToolTip("Splits on transition after Isma's Tear acquired")]
        TransTear,
        [Description("Isma's Tear with Grub (Transition)"), ToolTip("Splits on transition after collecting Isma's Tear and saving the grub in Isma's Grove")]
        TransTearWithGrub,
        [Description("Junk Pit (Transition)"), ToolTip("Splits on transition into Junk Pit")]
        EnterJunkPit,
        [Description("Hive (Transition)"), ToolTip("Splits on transition to Hive")]
        HiveEntry,
        [Description("King's Pass (Transition)"), ToolTip("Splits when entering Dirtmouth from King's Pass")]
        KingsPass,
        [Description("King's Pass from Town (Transition)"), ToolTip("Splits when entering King's Pass from Dirtmouth")]
        KingsPassEnterFromTown,
        [Description("Kingdom's Edge (Transition)"), ToolTip("Splits on transition to Kingdom's Edge from King's Station")]
        KingdomsEdgeEntry,
        [Description("Kingdom's Edge Overcharmed (Transition)"), ToolTip("Splits on transition to Kingdom's Edge from King's Station while overcharmed")]
        KingdomsEdgeOvercharmedEntry,
        [Description("NKG Dream (Transition)"), ToolTip("Splits on transition into Nightmare King Grimm dream")]
        EnterNKG,
        [Description("Queen's Gardens or Deepnest (Transition)"), ToolTip("Splits on transition into either Queen's Gardens or Deepnest")]
        EnterQueensGardensOrDeepnest,
        [Description("Queen's Gardens Entry (Transition)"), ToolTip("Splits on transition to QG scene following QGA or above Deepnest")]
        QueensGardensEntry,
        [Description("Queen's Gardens - Frogs (Transition)"), ToolTip("Splits on transition to QG frogs scene")]
        QueensGardensFrogsTrans,
        [Description("Queen's Gardens - Post-Upper Arena (Transition)"), ToolTip("Splits on transition to room after upper arena in QG")]
        QueensGardensPostArenaTransition,
        [Description("Soul Sanctum (Transition)"), ToolTip("Splits when entering Soul Sanctum")]
        EnterSanctum,
        [Description("Soul Sanctum w/ Shade Soul (Transition)"), ToolTip("Splits when entering Soul Sanctum after obtaining Shade Soul")]
        EnterSanctumWithShadeSoul,
        [Description("Tower of Love (Transition)"), ToolTip("Splits when entering the Tower of Love")]
        EnterLoveTower,
        [Description("Waterways (Transition)"), ToolTip("Splits on transition to Waterways\n(Simple Key manhole or Spike-tunnel)")]
        WaterwaysEntry,
        [Description("White Palace Entry (Transition)"), ToolTip("Splits when entering the first White Palace scene")]
        WhitePalaceEntry,
        [Description("City Toll Bench Room (Transition)"), ToolTip("Splits when entering the city toll bench room")]
        EnterCityTollBenchRoom,

        [Description("Baldur Shell (Charm)"), ToolTip("Splits when obtaining the Baldur Shell charm")]
        BaldurShell,
        [Description("Dashmaster (Charm)"), ToolTip("Splits when obtaining the Dashmaster charm")]
        Dashmaster,
        [Description("Deep Focus (Charm)"), ToolTip("Splits when obtaining the Deep Focus charm")]
        DeepFocus,
        [Description("Defenders Crest (Charm)"), ToolTip("Splits when obtaining the Defenders Crest charm")]
        DefendersCrest,
        [Description("Dreamshield (Charm)"), ToolTip("Splits when obtaining the Dreamshield charm")]
        Dreamshield,
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
        [Description("Grimmchild (Charm)"), ToolTip("Splits when obtaining the Grimmchild charm")]
        Grimmchild,
        [Description("Grimmchild Lvl 2 (Charm)"), ToolTip("Splits when upgrading Grimmchild to level 2")]
        Grimmchild2,
        [Description("Grimmchild Lvl 3 (Charm)"), ToolTip("Splits when upgrading Grimmchild to level 3")]
        Grimmchild3,
        [Description("Grimmchild Lvl 4 (Charm)"), ToolTip("Splits when upgrading Grimmchild to level 4")]
        Grimmchild4,
        [Description("Carefree Melody (Charm)"), ToolTip("Splits when obtaining the Carefree Melody charm")]
        CarefreeMelody,
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
        [Description("White Fragment - Queen's (Charm)"), ToolTip("Splits on picking up the left White Fragment from the White Lady")]
        WhiteFragmentLeft,
        [Description("White Fragment - King's (Charm)"), ToolTip("Splits on picking up the right White Fragment from the Pale King")]
        WhiteFragmentRight,
        [Description("Kingsoul (Charm)"), ToolTip("Splits when obtaining the completed Kingsoul charm")]
        Kingsoul,
        [Description("Lifeblood Core (Charm)"), ToolTip("Splits when obtaining the Lifeblood Core charm")]
        LifebloodCore,
        [Description("Lifeblood Heart (Charm)"), ToolTip("Splits when obtaining the Lifeblood Heart charm")]
        LifebloodHeart,
        [Description("Longnail (Charm)"), ToolTip("Splits when obtaining the Longnail charm")]
        Longnail,
        [Description("Mark of Pride (Charm)"), ToolTip("Splits when obtaining the Mark of Pride charm")]
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
        [Description("Sprintmaster (Charm)"), ToolTip("Splits when obtaining the Sprintmaster charm")]
        Sprintmaster,
        [Description("Stalwart Shell (Charm)"), ToolTip("Splits when obtaining Stalwart Shell charm")]
        StalwartShell,
        [Description("Steady Body (Charm)"), ToolTip("Splits when obtaining the Steady Body charm")]
        SteadyBody,
        [Description("Thorns of Agony (Charm)"), ToolTip("Splits when obtaining Thorns of Agony charm")]
        ThornsOfAgony,
        [Description("Unbreakable Greed (Charm)"), ToolTip("Splits when obtaining the Unbreakable Greed charm")]
        UnbreakableGreed,
        [Description("Unbreakable Heart (Charm)"), ToolTip("Splits when obtaining the Unbreakable Heart charm")]
        UnbreakableHeart,
        [Description("Unbreakable Strength (Charm)"), ToolTip("Splits when obtaining the Unbreakable Strength charm")]
        UnbreakableStrength,
        [Description("Void Heart (Charm)"), ToolTip("Splits when changing the Kingsoul to the Void Heart charm")]
        VoidHeart,
        [Description("Wayward Compass (Charm)"), ToolTip("Splits when obtaining Wayward Compass charm")]
        WaywardCompass,
        [Description("Weaversong (Charm)"), ToolTip("Splits when obtaining the Weaversong charm")]
        Weaversong,
        [Description("Shrumal Ogres (Charm Notch)"), ToolTip("Splits when obtaining the charm notch after defeating the Shrumal Ogres")]
        NotchShrumalOgres,
        [Description("Fog Canyon (Charm Notch)"), ToolTip("Splits when obtaining the charm notch in Fog Canyon")]
        NotchFogCanyon,
        [Description("Grimm (Charm Notch)"), ToolTip("Splits when obtaining the charm notch after Grimm")]
        NotchGrimm,
        [Description("Salubra 1 (Charm Notch)"), ToolTip("Splits when obtaining the first charm notch from Salubra")]
        NotchSalubra1,
        [Description("Salubra 2 (Charm Notch)"), ToolTip("Splits when obtaining the second charm notch from Salubra")]
        NotchSalubra2,
        [Description("Salubra 3 (Charm Notch)"), ToolTip("Splits when obtaining the third charm notch from Salubra")]
        NotchSalubra3,
        [Description("Salubra 4 (Charm Notch)"), ToolTip("Splits when obtaining the fourth charm notch from Salubra")]
        NotchSalubra4,

        [Description("Lemm Shop (NPC)"), ToolTip("Splits when talking to Lemm in the shop for the first time")]
        Lemm2,
        [Description("Lemm - ACN (Event)"), ToolTip("Splits on having sold a total: 1 journal, 6 seals, and 4 idols to Lemm")]
        AllCharmNotchesLemm2CP,
        [Description("Met Grey Mourner (NPC)"), ToolTip("Splits when talking to Grey Mourner for the first time")]
        MetGreyMourner,
        [Description("Mourner w/ Seer Ascended (NPC)"), ToolTip("Splits when both talked to Grey Mourner and Seer has ascended")]
        GreyMournerSeerAscended,
        [Description("Elderbug Flower Quest (NPC)"), ToolTip("Splits when giving the flower to the Elderbug")]
        ElderbugFlower,
        [Description("Godseeker Flower (NPC)"), ToolTip("Splits when giving Godseeker a flower")]
        givenGodseekerFlower,
        [Description("Oro Flower (NPC)"), ToolTip("Splits when giving Oro a flower")]
        givenOroFlower,
        [Description("White Lady Flower (NPC)"), ToolTip("Splits when giving White Lady a flower")]
        givenWhiteLadyFlower,
        [Description("Emilitia Flower (NPC)"), ToolTip("Splits when giving Emilita a flower")]
        givenEmilitiaFlower,
        [Description("Bretta Rescued (NPC)"), ToolTip("Splits when saving Bretta")]
        BrettaRescued,
        [Description("Brumm Flame (NPC)"), ToolTip("Splits when collecting Brumm's flame in Deepnest")]
        BrummFlame,
        [Description("Little Fool (NPC)"), ToolTip("Splits when talking to the Little Fool for the first time")]
        LittleFool,
        [Description("Sly Rescued (NPC)"), ToolTip("Splits when saving Sly")]
        SlyRescued,

        [Description("Grimm Flame 1 (Flame)"), ToolTip("Splits after obtaining the first flame.")]
        Flame1,
        [Description("Grimm Flame 2 (Flame)"), ToolTip("Splits after obtaining the second flame.")]
        Flame2,
        [Description("Grimm Flame 3 (Flame)"), ToolTip("Splits after obtaining the third flame.")]
        Flame3,

        [Description("Grimm Flame 1 (Transition)"), ToolTip("Splits on transition after obtaining the first flame on current Grimmchild cycle.")]
        TransFlame1,
        [Description("Grimm Flame 2 (Transition)"), ToolTip("Splits on transition after obtaining the second flame on current Grimmchild cycle.")]
        TransFlame2,
        [Description("Grimm Flame 3 (Transition)"), ToolTip("Splits on transition after obtaining the third flame on current Grimmchild cycle.")]
        TransFlame3,

        [Description("Pale Ore 1 (Ore)"), ToolTip("Splits after obtaining the first pale ore.")]
        Ore1,
        [Description("Pale Ore 2 (Ore)"), ToolTip("Splits after obtaining the second pale ore.")]
        Ore2,
        [Description("Pale Ore 3 (Ore)"), ToolTip("Splits after obtaining the third pale ore.")]
        Ore3,
        [Description("Pale Ore 4 (Ore)"), ToolTip("Splits after obtaining the fourth pale ore.")]
        Ore4,
        [Description("Pale Ore 5 (Ore)"), ToolTip("Splits after obtaining the fifth pale ore.")]
        Ore5,
        [Description("Pale Ore 6 (Ore)"), ToolTip("Splits after obtaining the sixth pale ore.")]
        Ore6,

        [Description("Rescued Grub 1 (Grub)"), ToolTip("Splits when rescuing grub #1")]
        Grub1,
        [Description("Rescued Grub 2 (Grub)"), ToolTip("Splits when rescuing grub #2")]
        Grub2,
        [Description("Rescued Grub 3 (Grub)"), ToolTip("Splits when rescuing grub #3")]
        Grub3,
        [Description("Rescued Grub 4 (Grub)"), ToolTip("Splits when rescuing grub #4")]
        Grub4,
        [Description("Rescued Grub 5 (Grub)"), ToolTip("Splits when rescuing grub #5")]
        Grub5,
        [Description("Rescued Grub 6 (Grub)"), ToolTip("Splits when rescuing grub #6")]
        Grub6,
        [Description("Rescued Grub 7 (Grub)"), ToolTip("Splits when rescuing grub #7")]
        Grub7,
        [Description("Rescued Grub 8 (Grub)"), ToolTip("Splits when rescuing grub #8")]
        Grub8,
        [Description("Rescued Grub 9 (Grub)"), ToolTip("Splits when rescuing grub #9")]
        Grub9,
        [Description("Rescued Grub 10 (Grub)"), ToolTip("Splits when rescuing grub #10")]
        Grub10,
        [Description("Rescued Grub 11 (Grub)"), ToolTip("Splits when rescuing grub #11")]
        Grub11,
        [Description("Rescued Grub 12 (Grub)"), ToolTip("Splits when rescuing grub #12")]
        Grub12,
        [Description("Rescued Grub 13 (Grub)"), ToolTip("Splits when rescuing grub #13")]
        Grub13,
        [Description("Rescued Grub 14 (Grub)"), ToolTip("Splits when rescuing grub #14")]
        Grub14,
        [Description("Rescued Grub 15 (Grub)"), ToolTip("Splits when rescuing grub #15")]
        Grub15,
        [Description("Rescued Grub 16 (Grub)"), ToolTip("Splits when rescuing grub #16")]
        Grub16,
        [Description("Rescued Grub 17 (Grub)"), ToolTip("Splits when rescuing grub #17")]
        Grub17,
        [Description("Rescued Grub 18 (Grub)"), ToolTip("Splits when rescuing grub #18")]
        Grub18,
        [Description("Rescued Grub 19 (Grub)"), ToolTip("Splits when rescuing grub #19")]
        Grub19,
        [Description("Rescued Grub 20 (Grub)"), ToolTip("Splits when rescuing grub #20")]
        Grub20,
        [Description("Rescued Grub 21 (Grub)"), ToolTip("Splits when rescuing grub #21")]
        Grub21,
        [Description("Rescued Grub 22 (Grub)"), ToolTip("Splits when rescuing grub #22")]
        Grub22,
        [Description("Rescued Grub 23 (Grub)"), ToolTip("Splits when rescuing grub #23")]
        Grub23,
        [Description("Rescued Grub 24 (Grub)"), ToolTip("Splits when rescuing grub #24")]
        Grub24,
        [Description("Rescued Grub 25 (Grub)"), ToolTip("Splits when rescuing grub #25")]
        Grub25,
        [Description("Rescued Grub 26 (Grub)"), ToolTip("Splits when rescuing grub #26")]
        Grub26,
        [Description("Rescued Grub 27 (Grub)"), ToolTip("Splits when rescuing grub #27")]
        Grub27,
        [Description("Rescued Grub 28 (Grub)"), ToolTip("Splits when rescuing grub #28")]
        Grub28,
        [Description("Rescued Grub 29 (Grub)"), ToolTip("Splits when rescuing grub #29")]
        Grub29,
        [Description("Rescued Grub 30 (Grub)"), ToolTip("Splits when rescuing grub #30")]
        Grub30,
        [Description("Rescued Grub 31 (Grub)"), ToolTip("Splits when rescuing grub #31")]
        Grub31,
        [Description("Rescued Grub 32 (Grub)"), ToolTip("Splits when rescuing grub #32")]
        Grub32,
        [Description("Rescued Grub 33 (Grub)"), ToolTip("Splits when rescuing grub #33")]
        Grub33,
        [Description("Rescued Grub 34 (Grub)"), ToolTip("Splits when rescuing grub #34")]
        Grub34,
        [Description("Rescued Grub 35 (Grub)"), ToolTip("Splits when rescuing grub #35")]
        Grub35,
        [Description("Rescued Grub 36 (Grub)"), ToolTip("Splits when rescuing grub #36")]
        Grub36,
        [Description("Rescued Grub 37 (Grub)"), ToolTip("Splits when rescuing grub #37")]
        Grub37,
        [Description("Rescued Grub 38 (Grub)"), ToolTip("Splits when rescuing grub #38")]
        Grub38,
        [Description("Rescued Grub 39 (Grub)"), ToolTip("Splits when rescuing grub #39")]
        Grub39,
        [Description("Rescued Grub 40 (Grub)"), ToolTip("Splits when rescuing grub #40")]
        Grub40,
        [Description("Rescued Grub 41 (Grub)"), ToolTip("Splits when rescuing grub #41")]
        Grub41,
        [Description("Rescued Grub 42 (Grub)"), ToolTip("Splits when rescuing grub #42")]
        Grub42,
        [Description("Rescued Grub 43 (Grub)"), ToolTip("Splits when rescuing grub #43")]
        Grub43,
        [Description("Rescued Grub 44 (Grub)"), ToolTip("Splits when rescuing grub #44")]
        Grub44,
        [Description("Rescued Grub 45 (Grub)"), ToolTip("Splits when rescuing grub #45")]
        Grub45,
        [Description("Rescued Grub 46 (Grub)"), ToolTip("Splits when rescuing grub #46")]
        Grub46,

        [Description("Rescued Grub Basin Dive (Grub)"), ToolTip("Splits when rescuing the grub in Abyss_17")]
        GrubBasinDive,
        [Description("Rescued Grub Basin Wings (Grub)"), ToolTip("Splits when rescuing the grub in Abyss_19")]
        GrubBasinWings,
        [Description("Rescued Grub City Below Love Tower (Grub)"), ToolTip("Splits when rescuing the grub in Ruins2_07")]
        GrubCityBelowLoveTower,
        [Description("Rescued Grub City Below Sanctum (Grub)"), ToolTip("Splits when rescuing the grub in Ruins1_05")]
        GrubCityBelowSanctum,
        [Description("Rescued Grub City Collector All (Grub)"), ToolTip("Splits when rescuing all three grubs in Ruins2_11. (On 1221, splits for right grub)")]
        GrubCityCollectorAll,
        [Description("Rescued Grub City Collector (Grub)"), ToolTip("Splits when rescuing any grub in Ruins2_11")]
        GrubCityCollector,
        [Description("Rescued Grub City Guard House (Grub)"), ToolTip("Splits when rescuing the grub in Ruins_House_01")]
        GrubCityGuardHouse,
        [Description("Rescued Grub City Sanctum (Grub)"), ToolTip("Splits when rescuing the grub in Ruins1_32")]
        GrubCitySanctum,
        [Description("Rescued Grub City Spire (Grub)"), ToolTip("Splits when rescuing the grub in Ruins2_03")]
        GrubCitySpire,
        [Description("Rescued Grub Cliffs Baldur Shell (Grub)"), ToolTip("Splits when rescuing the grub in Fungus1_28")]
        GrubCliffsBaldurShell,
        [Description("Rescued Grub Crossroads Acid (Grub)"), ToolTip("Splits when rescuing the grub in Crossroads_35")]
        GrubCrossroadsAcid,
        [Description("Rescued Grub Crossroads Guarded (Grub)"), ToolTip("Splits when rescuing the grub in Crossroads_48")]
        GrubCrossroadsGuarded,
        [Description("Rescued Grub Crossroads Spikes (Grub)"), ToolTip("Splits when rescuing the grub in Crossroads_31")]
        GrubCrossroadsSpikes,
        [Description("Rescued Grub Crossroads Vengefly (Grub)"), ToolTip("Splits when rescuing the grub in Crossroads_05")]
        GrubCrossroadsVengefly,
        [Description("Rescued Grub Crossroads Wall (Grub)"), ToolTip("Splits when rescuing the grub in Crossroads_03")]
        GrubCrossroadsWall,
        [Description("Rescued Grub Crystal Peak Bottom Lever (Grub)"), ToolTip("Splits when rescuing the grub in Mines_04")]
        GrubCrystalPeaksBottomLever,
        [Description("Rescued Grub Crystal Peak Crown (Grub)"), ToolTip("Splits when rescuing the grub in Mines_24")]
        GrubCrystalPeaksCrown,
        [Description("Rescued Grub Crystal Peak Crushers (Grub)"), ToolTip("Splits when rescuing the grub in Mines_19")]
        GrubCrystalPeaksCrushers,
        [Description("Rescued Grub Crystal Peak Crystal Heart (Grub)"), ToolTip("Splits when rescuing the grub in Mines_31")]
        GrubCrystalPeaksCrystalHeart,
        [Description("Rescued Grub Crystal Peak Mimic (Grub)"), ToolTip("Splits when rescuing the grub in Mines_16")]
        GrubCrystalPeaksMimics,
        [Description("Rescued Grub Crystal Peak Mound (Grub)"), ToolTip("Splits when rescuing the grub in Mines_35")]
        GrubCrystalPeaksMound,
        [Description("Rescued Grub Crystal Peak Spikes (Grub)"), ToolTip("Splits when rescuing the grub in Mines_03")]
        GrubCrystalPeaksSpikes,
        [Description("Rescued Grub Deepnest Beast's Den (Grub)"), ToolTip("Splits when rescuing the grub in Deepnest_Spider_Town")]
        GrubDeepnestBeastsDen,
        [Description("Rescued Grub Deepnest Dark (Grub)"), ToolTip("Splits when rescuing the grub in Deepnest_39")]
        GrubDeepnestDark,
        [Description("Rescued Grub Deepnest Mimics (Grub)"), ToolTip("Splits when rescuing the grub in Deepnest_36")]
        GrubDeepnestMimics,
        [Description("Rescued Grub Deepnest Nosk (Grub)"), ToolTip("Splits when rescuing the grub in Deepnest_31")]
        GrubDeepnestNosk,
        [Description("Rescued Grub Deepnest Spikes (Grub)"), ToolTip("Splits when rescuing the grub in Deepnest_03")]
        GrubDeepnestSpikes,
        [Description("Rescued Grub Fog Canyon Archives (Grub)"), ToolTip("Splits when rescuing the grub in Fungus3_47")]
        GrubFogCanyonArchives,
        [Description("Rescued Grub Fungal Bouncy (Grub)"), ToolTip("Splits when rescuing the grub in Fungus2_18")]
        GrubFungalBouncy,
        [Description("Rescued Grub Fungal Spore Shroom (Grub)"), ToolTip("Splits when rescuing the grub in Fungus2_20")]
        GrubFungalSporeShroom,
        [Description("Rescued Grub Greenpath Cornifer (Grub)"), ToolTip("Splits when rescuing the grub in Fungus1_06")]
        GrubGreenpathCornifer,
        [Description("Rescued Grub Greenpath Hunter (Grub)"), ToolTip("Splits when rescuing the grub in Fungus1_07")]
        GrubGreenpathHunter,
        [Description("Rescued Grub Greenpath Moss Knight (Grub)"), ToolTip("Splits when rescuing the grub in Fungus1_21")]
        GrubGreenpathMossKnight,
        [Description("Rescued Grub Greenpath Vessel Fragment (Grub)"), ToolTip("Splits when rescuing the grub in Fungus1_13")]
        GrubGreenpathVesselFragment,
        [Description("Rescued Grub Hive External (Grub)"), ToolTip("Splits when rescuing the grub in Hive_03")]
        GrubHiveExternal,
        [Description("Rescued Grub Hive Internal (Grub)"), ToolTip("Splits when rescuing the grub in Hive_04")]
        GrubHiveInternal,
        [Description("Rescued Grub Kingdom's Edge Center (Grub)"), ToolTip("Splits when rescuing the grub in Deepnest_East_11")]
        GrubKingdomsEdgeCenter,
        [Description("Rescued Grub Kingdom's Edge Oro (Grub)"), ToolTip("Splits when rescuing the grub in Deepnest_East_14")]
        GrubKingdomsEdgeOro,
        [Description("Rescued Grub Queen's Gardens Below Stag (Grub)"), ToolTip("Splits when rescuing the grub in Fungus3_10")]
        GrubQueensGardensBelowStag,
        [Description("Rescued Grub Queen's Gardens Upper (Grub)"), ToolTip("Splits when rescuing the grub in Fungus3_22")]
        GrubQueensGardensUpper,
        [Description("Rescued Grub Queen's Gardens White Lady (Grub)"), ToolTip("Splits when rescuing the grub in Fungus3_48")]
        GrubQueensGardensWhiteLady,
        [Description("Rescued Grub Resting Grounds Crypts (Grub)"), ToolTip("Splits when rescuing the grub in RestingGrounds_10")]
        GrubRestingGroundsCrypts,
        [Description("Rescued Grub Waterways Center (Grub)"), ToolTip("Splits when rescuing the grub in Waterways_04")]
        GrubWaterwaysCenter,
        [Description("Rescued Grub Waterways Hwurmps (Grub)"), ToolTip("Splits when rescuing the grub in Waterways_14")]
        GrubWaterwaysHwurmps,
        [Description("Rescued Grub Waterways Isma (Grub)"), ToolTip("Splits when rescuing the grub in Waterways_13")]
        GrubWaterwaysIsma,

        [Description("Mimic 1 (Killed)"), ToolTip("Splits when rescuing mimic #1")]
        Mimic1,
        [Description("Mimic 2 (Killed)"), ToolTip("Splits when rescuing mimic #2")]
        Mimic2,
        [Description("Mimic 3 (Killed)"), ToolTip("Splits when rescuing mimic #3")]
        Mimic3,
        [Description("Mimic 4 (Killed)"), ToolTip("Splits when rescuing mimic #4")]
        Mimic4,
        [Description("Mimic 5 (Killed)"), ToolTip("Splits when rescuing mimic #5")]
        Mimic5,

        [Description("Whispering Root (Ancestral Mound)"), ToolTip("Splits upon completing the whispering root in the Ancestral Mound")]
        TreeMound,
        [Description("Whispering Root (City of Tears)"), ToolTip("Splits upon completing the whispering root in the City of Tears")]
        TreeCity,
        [Description("Whispering Root (Crystal Peak)"), ToolTip("Splits upon completing the whispering root in Crystal Peak")]
        TreePeak,
        [Description("Whispering Root (Deepnest)"), ToolTip("Splits upon completing the whispering root in Deepnest")]
        TreeDeepnest,
        [Description("Whispering Root (Forgotten Crossroads)"), ToolTip("Splits upon completing the whispering root in the Forgotten Crossroads")]
        TreeCrossroads,
        [Description("Whispering Root (Leg Eater)"), ToolTip("Splits upon completing the whispering root left from Leg Eater")]
        TreeLegEater,
        [Description("Whispering Root (Mantis Village)"), ToolTip("Splits upon completing the whispering root above the Mantis Village")]
        TreeMantisVillage,
        [Description("Whispering Root (Greenpath)"), ToolTip("Splits upon completing the whispering root in Greenpath")]
        TreeGreenpath,
        [Description("Whispering Root (Hive)"), ToolTip("Splits upon completing the whispering root in the Hive")]
        TreeHive,
        [Description("Whispering Root (Howling Cliffs)"), ToolTip("Splits upon completing the whispering root in the Howling Cliifs")]
        TreeCliffs,
        [Description("Whispering Root (Kingdom's Edge)"), ToolTip("Splits upon completing the whispering root in the Kingdom's Edge")]
        TreeKingdomsEdge,
        [Description("Whispering Root (Queen's Gardens)"), ToolTip("Splits upon completing the whispering root in the Queen's Gardens")]
        TreeQueensGardens,
        [Description("Whispering Root (Resting Grounds)"), ToolTip("Splits upon completing the whispering root in the Resting Grounds")]
        TreeRestingGrounds,
        [Description("Whispering Root (Royal Waterways)"), ToolTip("Splits upon completing the whispering root in the Royal Waterways")]
        TreeWaterways,
        [Description("Whispering Root (Spirits' Glade)"), ToolTip("Splits upon completing the whispering root in the Spirits' Glade")]
        TreeGlade,

        [Description("100 Essence (Essence)"), ToolTip("Splits upon obtaining 100 Essence")]
        Essence100,
        [Description("200 Essence (Essence)"), ToolTip("Splits upon obtaining 200 Essence")]
        Essence200,
        [Description("300 Essence (Essence)"), ToolTip("Splits upon obtaining 300 Essence")]
        Essence300,
        [Description("400 Essence (Essence)"), ToolTip("Splits upon obtaining 400 Essence")]
        Essence400,
        [Description("500 Essence (Essence)"), ToolTip("Splits upon obtaining 500 Essence")]
        Essence500,
        [Description("600 Essence (Essence)"), ToolTip("Splits upon obtaining 600 Essence")]
        Essence600,
        [Description("700 Essence (Essence)"), ToolTip("Splits upon obtaining 700 Essence")]
        Essence700,
        [Description("800 Essence (Essence)"), ToolTip("Splits upon obtaining 800 Essence")]
        Essence800,
        [Description("900 Essence (Essence)"), ToolTip("Splits upon obtaining 900 Essence")]
        Essence900,
        [Description("1000 Essence (Essence)"), ToolTip("Splits upon obtaining 1000 Essence")]
        Essence1000,
        [Description("1100 Essence (Essence)"), ToolTip("Splits upon obtaining 1100 Essence")]
        Essence1100,
        [Description("1200 Essence (Essence)"), ToolTip("Splits upon obtaining 1200 Essence")]
        Essence1200,
        [Description("1300 Essence (Essence)"), ToolTip("Splits upon obtaining 1300 Essence")]
        Essence1300,
        [Description("1400 Essence (Essence)"), ToolTip("Splits upon obtaining 1400 Essence")]
        Essence1400,
        [Description("1500 Essence (Essence)"), ToolTip("Splits upon obtaining 1500 Essence")]
        Essence1500,
        [Description("1600 Essence (Essence)"), ToolTip("Splits upon obtaining 1600 Essence")]
        Essence1600,
        [Description("1700 Essence (Essence)"), ToolTip("Splits upon obtaining 1700 Essence")]
        Essence1700,
        [Description("1800 Essence (Essence)"), ToolTip("Splits upon obtaining 1800 Essence")]
        Essence1800,
        [Description("1900 Essence (Essence)"), ToolTip("Splits upon obtaining 1900 Essence")]
        Essence1900,
        [Description("2000 Essence (Essence)"), ToolTip("Splits upon obtaining 2000 Essence")]
        Essence2000,
        [Description("2100 Essence (Essence)"), ToolTip("Splits upon obtaining 2100 Essence")]
        Essence2100,
        [Description("2200 Essence (Essence)"), ToolTip("Splits upon obtaining 2200 Essence")]
        Essence2200,
        [Description("2300 Essence (Essence)"), ToolTip("Splits upon obtaining 2300 Essence")]
        Essence2300,
        [Description("2400 Essence (Essence)"), ToolTip("Splits upon obtaining 2400 Essence")]
        Essence2400,

        [Description("Any Bench (Bench)"), ToolTip("Splits when sitting on a bench")]
        BenchAny,
        /*
        [Description("Dirtmouth (Bench)"), ToolTip("Splits when sitting on the bench in Dirtmouth")]
        BenchDirtmouth,
        [Description("Mato (Bench)"), ToolTip("Splits when sitting on the bench in Mato's Hut")]
        BenchMato,
        [Description("Crossroads Hotsprings (Bench)"), ToolTip("Splits when sitting on the hotsprings bench in Crossroads")]
        BenchCrossroadsHotsprings,
        */
        [Description("Crossroads Stag (Bench)"), ToolTip("Splits when sitting on the bench at Crossroads Stag")]
        BenchCrossroadsStag,
        /*
        [Description("Salubra (Bench)"), ToolTip("Splits when sitting on the bench by Salubra")]
        BenchSalubra,
        [Description("Ancestral Mound (Bench)"), ToolTip("Splits when sitting on the bench in Ancestral Mound")]
        BenchAncestralMound,
        [Description("Black Egg Temple (Bench)"), ToolTip("Splits when sitting on the bench in Black Egg Temple")]
        BenchBlackEgg,
        [Description("Waterfall (Bench)"), ToolTip("Splits when sitting on the waterfall bench in Greenpath")]
        BenchWaterfall,
        [Description("Stone Sanctuary (Bench)"), ToolTip("Splits when sitting on the bench by Stone Sanctuary")]
        BenchStoneSanctuary,
        [Description("Greenpath Toll (Bench)"), ToolTip("Splits when sitting on the toll bench in Greenpath")]
        BenchGreenpathToll,
        */
        [Description("Greenpath Stag (Bench)"), ToolTip("Splits when sitting on the bench at Greenpath Stag")]
        BenchGreenpathStag,
        /*
        [Description("Lake of Unn (Bench)"), ToolTip("Splits when sitting on the bench at the Lake of Unn")]
        BenchLakeOfUnn,
        [Description("Sheo (Bench)"), ToolTip("Splits when sitting on the bench by Sheo's hut")]
        BenchSheo,
        [Description("Teacher's Archives (Bench)"), ToolTip("Splits when sitting on the bench in Teacher's Archives")]
        BenchArchives,
        */
        [Description("Queen's Station (Bench)"), ToolTip("Splits when sitting on the bench in Queen's Station")]
        BenchQueensStation,
        /*
        [Description("Leg Eater (Bench)"), ToolTip("Splits when sitting on the bench at Leg Eater")]
        BenchLegEater,
        [Description("Bretta (Bench)"), ToolTip("Splits when sitting on the bench by Bretta")]
        BenchBretta,
        [Description("Mantis Village (Bench)"), ToolTip("Splits when sitting on the bench in Mantis Village")]
        BenchMantisVillage,
        [Description("Quirrel (Bench)"), ToolTip("Splits when sitting on the bench Quirrel sits at in City of Tears")]
        BenchQuirrel,
        [Description("City Toll (Bench)"), ToolTip("Splits when sitting on the toll bench in City of Tears")]
        BenchCityToll,
        */
        [Description("Storerooms (Bench)"), ToolTip("Splits when sitting on the bench in City Storerooms")]
        BenchStorerooms,
        [Description("Watcher's Spire (Bench)"), ToolTip("Splits when sitting on the bench in Watcher's Spire")]
        BenchSpire,
        [Description("Watcher's Spire + Killed Great Husk Sentry (Bench)"), ToolTip("Splits when sitting on the bench in Watcher's Spire  after killing a Great Husk Sentry")]
        BenchSpireGHS,
        [Description("King's Station (Bench)"), ToolTip("Splits when sitting on the bench in King's Station")]
        BenchKingsStation,
        /*
        [Description("Pleasure House (Bench)"), ToolTip("Splits when sitting on the bench in Pleasure House")]
        BenchPleasureHouse,
        [Description("Waterways (Bench)"), ToolTip("Splits when sitting on the bench in Royal Waterways")]
        BenchWaterways,
        [Description("Deepnest Hotsprings (Bench)"), ToolTip("Splits when sitting on the hotsprings bench in Deepnest")]
        BenchDeepnestHotsprings,
        [Description("Failed Tramway (Bench)"), ToolTip("Splits when sitting on the bench at the Failed Tramway")]
        BenchFailedTramway,
        [Description("Beast's Den (Bench)"), ToolTip("Splits when sitting on the bench in Beast's Den")]
        BenchDeepnestSpiderTown,
        [Description("Basin Toll (Bench)"), ToolTip("Splits when sitting on the toll bench in Ancient Basin")]
        BenchBasinToll,
        */
        [Description("Hidden Station (Bench)"), ToolTip("Splits when sitting on the bench in Hidden Station")]
        BenchHiddenStation,
        /*
        [Description("Oro (Bench)"), ToolTip("Splits when sitting on the bench at Oro's hut")]
        BenchOro,
        [Description("Camp (Bench)"), ToolTip("Splits when sitting on the bench at the camp in Kingdom's Edge")]
        BenchCamp,
        [Description("Colosseum (Bench)"), ToolTip("Splits when sitting on the bench at the Colosseum")]
        BenchColosseum,
        [Description("Hive (Bench)"), ToolTip("Splits when sitting on the bench in the Hive")]
        BenchHive,
        */
        [Description("Resting Grounds Stag (Bench)"), ToolTip("Splits when sitting on the bench at Resting Grounds Stag")]
        BenchRGStag,
        /*
        [Description("Crystal Peak Dark Room (Bench)"), ToolTip("Splits when sitting on the dark room bench in Crystal Peak")]
        BenchDarkRoom,
        [Description("Crystal Guardian (Bench)"), ToolTip("Splits when sitting on the Crystal Guardian's bench")]
        BenchCG1,
        [Description("Grey Mourner (Bench)"), ToolTip("Splits when sitting on the bench by the Grey Mourner")]
        BenchFlowerQuest,
        [Description("Queen's Gardens Cornifer (Bench)"), ToolTip("Splits when sitting on the bench by Cornifer in Queen's Gardens")]
        BenchQGCornifer,
        [Description("Queen's Gardens Toll (Bench)"), ToolTip("Splits when sitting on the toll bench in Queen's Gardens")]
        BenchQGToll,
        */
        [Description("Queen's Gardens Stag (Bench)"), ToolTip("Splits when sitting on the bench at Queen's Gardens Stag")]
        BenchQGStag,
        /*
        [Description("Tram (Bench)"), ToolTip("Splits when sitting on the bench in a tram")]
        BenchTram,
        [Description("White Palace Entrance (Bench)"), ToolTip("Splits when sitting on the bench inside the entrance to White Palace")]
        BenchWhitePalaceEntrance,
        [Description("White Palace Atrium (Bench)"), ToolTip("Splits when sitting on the bench in the White Palace Atrium")]
        BenchWhitePalaceAtrium,
        [Description("White Palace Balcony (Bench)"), ToolTip("Splits when sitting on the bench at the White Palace Balcony")]
        BenchWhitePalaceBalcony,
        [Description("Godhome Atrium (Bench)"), ToolTip("Splits when sitting on the bench in the Godhome Atrium")]
        BenchGodhomeAtrium,
        [Description("Hall of Gods (Bench)"), ToolTip("Splits when sitting on the bench in the Godhome Atrium")]
        BenchHallOfGods,
        */

        [Description("Queen's Gardens Bench (Toll)"), ToolTip("Splits when buying Queen's Gardens toll bench")]
        TollBenchQG,
        [Description("Sanctum Bench (Toll)"), ToolTip("Splits when buying City/Sanctum toll bench by Cornifer's location")]
        TollBenchCity,
        [Description("Basin Bench (Toll)"), ToolTip("Splits when buying Ancient Basin toll bench")]
        TollBenchBasin,
        [Description("Waterways Manhole (Toll)"), ToolTip("Splits when opening the Waterways Manhole")]
        WaterwaysManhole,
        [Description("Tram Deepnest (Tram)"), ToolTip("Splits when unlocking the tram in Deepnest")]
        TramDeepnest,

        [Description("White Palace - Lower Orb (Lever)"), ToolTip("Splits when lighting the orb in White Palace lowest floor")]
        WhitePalaceOrb1,
        [Description("White Palace - Left Orb (Lever)"), ToolTip("Splits when lighting the orb in White Palace left wing")]
        WhitePalaceOrb3,
        [Description("White Palace - Right Orb (Lever)"), ToolTip("Splits when lighting the orb in White Palace right wing")]
        WhitePalaceOrb2,

        [Description("White Palace - Lower Entry (Room)"), ToolTip("Splits on transition to White_Palace_01")]
        WhitePalaceLowerEntry,
        [Description("White Palace - Lower Orb (Room)"), ToolTip("Splits on transition to White_Palace_02")]
        WhitePalaceLowerOrb,
        [Description("White Palace - Left Entry (Room)"), ToolTip("Splits on transition to White_Palace_04")]
        WhitePalaceLeftEntry,
        [Description("White Palace - Left Midpoint (Room)"), ToolTip("Splits on transition between White_Palace_04 and _14")]
        WhitePalaceLeftWingMid,
        [Description("White Palace - Right Side Entry (Room)"), ToolTip("Splits on transition between White_Palace_03_Hub and _15")]
        WhitePalaceRightEntry,
        [Description("White Palace - Right Side Climb (Room)"), ToolTip("Splits on transition between White_Palace_05 and _16")]
        WhitePalaceRightClimb,
        [Description("White Palace - Right Side Saw Squeeze (Room)"), ToolTip("Splits on transition between White_Palace_16 and _05")]
        WhitePalaceRightSqueeze,
        [Description("White Palace - Right Side Exit (Room)"), ToolTip("Splits on transition between White_Palace_05 and _15")]
        WhitePalaceRightDone,
        [Description("White Palace - Top Entry (Room)"), ToolTip("Splits on transition between White_Palace_03_Hub and _06")]
        WhitePalaceTopEntry,
        [Description("White Palace - Top Cursed Cycle (Room)"), ToolTip("Splits on transition between White_Palace_06 and _07")]
        WhitePalaceTopClimb,
        [Description("White Palace - Top Lever (Room)"), ToolTip("Splits on transition between White_Palace_07 and _12")]
        WhitePalaceTopLeverRoom,
        [Description("White Palace - Top Final Platforming (Room)"), ToolTip("Splits on transition between White_Palace_12 and _13")]
        WhitePalaceTopLastPlats,
        [Description("White Palace - Throne Room (Room)"), ToolTip("Splits on transition between White_Palace_13 and _09")]
        WhitePalaceThroneRoom,
        [Description("White Palace - Atrium (Room)"), ToolTip("Splits on any transition to White_Palace_03_Hub")]
        WhitePalaceAtrium,

        [Description("Path of Pain Room 1 (Room)"), ToolTip("Splits on transition to the first room in PoP (entry to PoP)")]
        PathOfPainEntry,
        [Description("Path of Pain Room 2 (Room)"), ToolTip("Splits on transition to the second room in PoP")]
        PathOfPainTransition1,
        [Description("Path of Pain Room 3 (Room)"), ToolTip("Splits on transition to the third room in PoP")]
        PathOfPainTransition2,
        [Description("Path of Pain Room 4 (Room)"), ToolTip("Splits on transition to the fourth room in PoP (Final room)")]
        PathOfPainTransition3,
        [Description("Path of Pain Room 4 DDark (Event)"), ToolTip("Splits on landing with Descending Dark in fourth room of PoP")]
        PathOfPainRoom4DDark,

        [Description("Kingdom's Edge Acid (Dreamgate)"), ToolTip("Splits when placing Dreamgate by KE Acid (hopefully)")]
        DgateKingdomsEdgeAcid,

        [Description("Godhome Bench (Transition)"), ToolTip("Splits when leaving a Godhome Bench room")]
        GodhomeBench,
        [Description("Godhome Lore Room (Transition)"), ToolTip("Splits when leaving a Godhome lore room")]
        GodhomeLoreRoom,
        [Description("Pantheon 1-4 (Transition)"), ToolTip("Splits on entry to any of Pantheon 1 - 4")]
        Pantheon1to4Entry,
        [Description("Pantheon 5 (Transition)"), ToolTip("Splits on entry to Pantheon 5")]
        Pantheon5Entry,

        [Description("Main Menu (Menu)"), ToolTip("Splits on the main menu")]
        Menu,
        [Description("Main Menu w/ Claw (Menu)"), ToolTip("Splits on transition to the main menu after Mantis Claw acquired")]
        MenuClaw,
        [Description("Main Menu w/ Mothwing Cloak (Menu)"), ToolTip("Splits on transition to the main menu after Mothwing Cloak acquired")]
        MenuCloak,
        [Description("Main Menu w/ Dashmaster (Menu)"), ToolTip("Splits on transition to the main menu after Dashmaster acquired")]
        MenuDashmaster,
        [Description("Main Menu w/ Dream Nail (Menu)"), ToolTip("Splits on transition to the main menu after Dream Nail acquired")]
        MenuDreamNail,
        [Description("Main Menu w/ Dream Gate (Menu)"), ToolTip("Splits on transition to the main menu after Dream Gate acquired")]
        MenuDreamGate,
        [Description("Main Menu w/ 3 Dreamers (Menu)"), ToolTip("Splits on transition to the main menu after 3 Dreamers acquired")]
        MenuDreamer3,
        [Description("Main Menu w/ Ghusk (Menu)"), ToolTip("Splits on transition to the main menu after Gorgeous Husk defeated")]
        MenuGorgeousHusk,
        [Description("Main Menu w/ Isma's Tear (Menu)"), ToolTip("Splits on transition to the main menu after Isma's Tear acquired")]
        MenuIsmasTear,
        [Description("Main Menu w/ Shade Soul (Menu)"), ToolTip("Splits on transition to the main menu after Shade Soul acquired")]
        MenuShadeSoul,
        [Description("Main Menu w/ Void Heart (Menu)"), ToolTip("Splits on transition to the main menu after Void Heart acquired")]
        MenuVoidHeart,

        [Description("Main Menu w/ Herrah (Menu)"), ToolTip("For Dreamer Quit-outs on older patches. Splits on transition to the main menu after Herrah is registered as defeated")]
        MenuHegemol,
        [Description("Main Menu w/ Lurien (Menu)"), ToolTip("For Dreamer Quit-outs on older patches. Splits on transition to the main menu after Lurien is registered as defeated")]
        MenuLurien,
        [Description("Main Menu w/ Monomon (Menu)"), ToolTip("For Dreamer Quit-outs on older patches. Splits on transition to the main menu after Monomon is registered as defeated")]
        MenuMonomon,

        [Description("Cornifer at Home (Transition)"), ToolTip("Splits when entering Iselda's hut while Cornifer is sleeping")]
        CorniferAtHome,
        [Description("All Seals (Item)"), ToolTip("Splits when 17 Hallownest Seals have been collected")]
        AllSeals,
        [Description("All Eggs (Item)"), ToolTip("Splits when 21 Rancid Eggs have been collected")]
        AllEggs,
        [Description("Sly Simple Key (Item)"), ToolTip("Splits when buying the simple key from Sly")]
        SlySimpleKey,
        [Description("All Breakables (Event)"), ToolTip("Splits when all 3 fragile charms are broken")]
        AllBreakables,
        [Description("All Unbreakables (Charm)"), ToolTip("Splits when all 3 unbreakable charms are obtained")]
        AllUnbreakables,
        [Description("Met Emilitia (Event)"), ToolTip("Splits when talking to Emilitia for the first time")]
        MetEmilitia,

        [Description("Map Dirtmouth (Item)"), ToolTip("Splits when acquiring the Dirtmouth map")]
        mapDirtmouth,
        [Description("Map Crossroads (Item)"), ToolTip("Splits when acquiring the Crossroads map")]
        mapCrossroads,
        [Description("Map Greenpath (Item)"), ToolTip("Splits when acquiring the Greenpath map")]
        mapGreenpath,
        [Description("Map Fog Canyon (Item)"), ToolTip("Splits when acquiring the Fog Canyon map")]
        mapFogCanyon,
        [Description("Map Queen's Gardens (Item)"), ToolTip("Splits when acquiring the QG map")]
        mapRoyalGardens,
        [Description("Map Fungal Wastes (Item)"), ToolTip("Splits when acquiring the Fungal Wastes map")]
        mapFungalWastes,
        [Description("Map City of Tears (Item)"), ToolTip("Splits when acquiring the City map")]
        mapCity,
        [Description("Map Waterways (Item)"), ToolTip("Splits when acquiring the Waterways map")]
        mapWaterways,
        [Description("Map Crystal Peak (Item)"), ToolTip("Splits when acquiring the Crystal Peak map")]
        mapMines,
        [Description("Map Deepnest (Item)"), ToolTip("Splits when acquiring the Deepnest map")]
        mapDeepnest,
        [Description("Map Howling Cliffs (Item)"), ToolTip("Splits when acquiring the Howling Cliffs map")]
        mapCliffs,
        [Description("Map Kingdom's Edge (Item)"), ToolTip("Splits when acquiring the KE map")]
        mapOutskirts,
        [Description("Map Resting Grounds (Item)"), ToolTip("Splits when acquiring the Resting Grounds map")]
        mapRestingGrounds,
        [Description("Map Ancient Basin (Item)"), ToolTip("Splits when acquiring the Abyss map")]
        mapAbyss,

        [Description("Dream Nail Marissa (Obtain)"), ToolTip("Splits when obtaining the essence from Marissa")]
        OnObtainGhostMarissa,
        [Description("Dream Nail Caelif and Fera (Obtain)"), ToolTip("Splits when obtaining the essence from Caelif and Fera Orthop")]
        OnObtainGhostCaelifFera,
        [Description("Dream Nail Poggy (Obtain)"), ToolTip("Splits when obtaining the essence from Poggy Thorax")]
        OnObtainGhostPoggy,
        [Description("Dream Nail Gravedigger (Obtain)"), ToolTip("Splits when obtaining the essence from Gravedigger")]
        OnObtainGhostGravedigger,
        [Description("Dream Nail Joni (Obtain)"), ToolTip("Splits when obtaining the essence from Blue Child Joni")]
        OnObtainGhostJoni,
        [Description("Dream Nail Cloth (Obtain)"), ToolTip("Splits when obtaining the essence from Cloth")]
        OnObtainGhostCloth,
        [Description("Dream Nail Vespa (Obtain)"), ToolTip("Splits when obtaining the essence from Hive Queen Vespa")]
        OnObtainGhostVespa,
        [Description("Dream Nail Revek (Obtain)"), ToolTip("Splits when 19 essence has been obtained in Spirits' Glade")]
        OnObtainGhostRevek,

        [Description("Wanderer's Journal (Obtain)"), ToolTip("Splits when obtaining a Wanderer's Journal")]
        OnObtainWanderersJournal,
        [Description("Hallownest Seal (Obtain)"), ToolTip("Splits when obtaining a Hallownest Seal")]
        OnObtainHallownestSeal,
        [Description("King's Idol (Obtain)"), ToolTip("Splits when obtaining a King's Idol")]
        OnObtainKingsIdol,
        [Description("Arcane Egg 8 (Obtain)"), ToolTip("Splits when obtaining 8 Arcane Eggs")]
        ArcaneEgg8,
        [Description("Arcane Egg (Obtain)"), ToolTip("Splits when obtaining an Arcane Egg")]
        OnObtainArcaneEgg,
        [Description("Charm Notch (Obtain)"), ToolTip("Splits when obtaining a new Charm Slot")]
        OnObtainCharmNotch,
        [Description("Rancid Egg (Obtain)"), ToolTip("Splits when obtaining a Rancid Egg")]
        OnObtainRancidEgg,
        [Description("Mask Shard (Obtain)"), ToolTip("Splits when obtaining a Mask Shard or upgrade for complete Mask")]
        OnObtainMaskShard,
        [Description("Vessel Fragment (Obtain)"), ToolTip("Splits when obtaining a Vessel Fragment or on upgrade for full Soul Vessel")]
        OnObtainVesselFragment,
        [Description("Simple Key (Obtain)"), ToolTip("Splits when obtaining a Simple Key")]
        OnObtainSimpleKey,
        [Description("Use Simple Key (Obtain)"), ToolTip("Splits when using a Simple Key")]
        OnUseSimpleKey,
        [Description("Grub (Obtain)"), ToolTip("Splits when saving a Grub")]
        OnObtainGrub,
        [Description("Pale Ore (Obtain)"), ToolTip("Splits when obtaining a Pale Ore")]
        OnObtainPaleOre,
        [Description("White Fragment (Obtain)"), ToolTip("Splits when obtaining any White Fragment, or Void Heart")]
        OnObtainWhiteFragment,

        [Description("Colo 1 Wave 1a"), ToolTip("Splits upon killing the first Sheilded Fool in wave 1\nRecommended for use with a pre-set save file")]
        Bronze1a,
        [Description("Colo 1 Wave 1b"), ToolTip("Splits upon killing the pair of Sheilded fools in wave 1\nRecommended for use with a pre-set save file")]
        Bronze1b,
        [Description("Colo 1 Wave 1c"), ToolTip("Splits upon killing the pair of Sharp Baldurs at the end of wave 1\nRecommended for use with a pre-set save file")]
        Bronze1c,
        [Description("Colo 1 Wave 2"), ToolTip("Splits upon killing all five Sharp Baldurs in wave 2\nRecommended for use with a pre-set save file")]
        Bronze2,
        [Description("Colo 1 Wave 3a"), ToolTip("Splits upon killing the first Sturdy Fool in wave 3\nRecommended for use with a pre-set save file")]
        Bronze3a,
        [Description("Colo 1 Wave 3b"), ToolTip("Splits upon killing the pair of Sturdy Fools at the end of wave 3\nRecommended for use with a pre-set save file")]
        Bronze3b,
        [Description("Colo 1 Wave 4"), ToolTip("Splits upon killing the pair of Primal Aspids in wave 4\nRecommended for use with a pre-set save file")]
        Bronze4,
        [Description("Colo 1 Wave 5"), ToolTip("Splits upon killing the pair of Primal Aspids in wave 5\nRecommended for use with a pre-set save file")]
        Bronze5,
        [Description("Colo 1 Wave 6"), ToolTip("Splits upon killing all three Sturdy Fools on the raised platforms in wave 6\nRecommended for use with a pre-set save file")]
        Bronze6,
        [Description("Colo 1 Wave 7"), ToolTip("Splits upon killing both Primal Aspids and Sharp Baldurs in wave 7\nRecommended for use with a pre-set save file")]
        Bronze7,
        [Description("Colo 1 Wave 8a"), ToolTip("Splits upon killing all four Vengeflies in wave 8\nRecommended for use with a pre-set save file")]
        Bronze8a,
        [Description("Colo 1 Wave 8b"), ToolTip("Splits upon killing the Vengefly King in wave 8\nRecommended for use with a pre-set save file")]
        Bronze8b,
        [Description("Colo 1 Wave 9"), ToolTip("Splits upon killing the Sharp Baldur after the Primal Aspid at the end of wave 9\nRecommended for use with a pre-set save file")]
        Bronze9,
        [Description("Colo 1 Wave 10"), ToolTip("Splits upon killing the third Sharp Baldur in the low ceiling section in wave 10\nRecommended for use with a pre-set save file")]
        Bronze10,
        [Description("Colo 1 Wave 11a"), ToolTip("Splits upon killing the first pair of Volatile Gruzzers in wave 11\nRecommended for use with a pre-set save file")]
        Bronze11a,
        [Description("Colo 1 Wave 11b"), ToolTip("Splits upon killing the final group of Volatile Gruzzers at the end of wave 11\nRecommended for use with a pre-set save file")]
        Bronze11b,
        [Description("Colo 1 End"), ToolTip("Splits upon killing the pair Gruz Mothers at the end of Trial of the Warrior\nRecommended for use with a pre-set save file")]
        BronzeEnd,

        [Description("Colo 2 Wave 1"), ToolTip("Splits upon completing wave 1\nRecommended for use with a pre-set save file")]
        Silver1,
        [Description("Colo 2 Wave 2"), ToolTip("Splits upon completing wave 2\nRecommended for use with a pre-set save file")]
        Silver2,
        [Description("Colo 2 Wave 3"), ToolTip("Splits upon completing wave 3\nRecommended for use with a pre-set save file")]
        Silver3,
        [Description("Colo 2 Wave 4"), ToolTip("Splits upon completing wave 4\nRecommended for use with a pre-set save file")]
        Silver4,
        [Description("Colo 2 Wave 5"), ToolTip("Splits upon completing wave 5\nRecommended for use with a pre-set save file")]
        Silver5,
        [Description("Colo 2 Wave 6"), ToolTip("Splits upon the death of the 3 Belflies after the Heavy Fool\nRecommended for use with a pre-set save file")]
        Silver6,
        [Description("Colo 2 Wave 7"), ToolTip("Splits on the death of the single Belfly\nRecommended for use with a pre-set save file")]
        Silver7,
        [Description("Colo 2 Wave 8"), ToolTip("Splits upon killing the first Great Hopper\nRecommended for use with a pre-set save file")]
        Silver8,
        [Description("Colo 2 Wave 9"), ToolTip("Splits upon killing the second Great Hopper\nRecommended for use with a pre-set save file")]
        Silver9,
        [Description("Colo 2 Wave 10"), ToolTip("Splits upon killing the Mimic\nRecommended for use with a pre-set save file")]
        Silver10,
        [Description("Colo 2 Wave 11"), ToolTip("Splits upon completing wave 11\nRecommended for use with a pre-set save file")]
        Silver11,
        [Description("Colo 2 Wave 12"), ToolTip("Splits upon completing wave 12\nRecommended for use with a pre-set save file")]
        Silver12,
        [Description("Colo 2 Wave 13"), ToolTip("Splits upon completing wave 13\nRecommended for use with a pre-set save file")]
        Silver13,
        [Description("Colo 2 Wave 14"), ToolTip("Splits upon completing wave 14\nRecommended for use with a pre-set save file")]
        Silver14,
        [Description("Colo 2 Wave 15"), ToolTip("Splits upon completing wave 15\nRecommended for use with a pre-set save file")]
        Silver15,
        [Description("Colo 2 Wave 16"), ToolTip("Splits upon completing wave 16\nRecommended for use with a pre-set save file")]
        Silver16,
        [Description("Colo 2 End"), ToolTip("Splits upon killing both Oblobbles at the end of Trial of the Conqueror\nRecommended for use with a pre-set save file")]
        SilverEnd,

        [Description("Colo 3 Wave 1"), ToolTip("Splits upon completing wave 1\nRecommended for use with a pre-set save file")]
        Gold1,
        [Description("Colo 3 Wave 3"), ToolTip("Splits upon completing waves 2 and 3\nRecommended for use with a pre-set save file")]
        Gold3,
        [Description("Colo 3 Wave 4"), ToolTip("Splits upon completing wave 4\nRecommended for use with a pre-set save file")]
        Gold4,
        [Description("Colo 3 Wave 5"), ToolTip("Splits upon killing the first wave of 3 Loodles\nRecommended for use with a pre-set save file")]
        Gold5,
        [Description("Colo 3 Wave 6"), ToolTip("Splits upon killing the set of 5 Loodles\nRecommended for use with a pre-set save file")]
        Gold6,
        [Description("Colo 3 Wave 7"), ToolTip("Splits upon killing the second wave of 3 Loodles\nRecommended for use with a pre-set save file")]
        Gold7,
        [Description("Colo 3 Wave 8a"), ToolTip("Splits upon completing the first half of wave 8, before the garpedes\nRecommended for use with a pre-set save file")]
        Gold8a,
        [Description("Colo 3 Wave 8b"), ToolTip("Splits upon completing wave 8\nRecommended for use with a pre-set save file")]
        Gold8,
        [Description("Colo 3 Wave 9a"), ToolTip("Splits upon killing the fools and mantises in wave 9\nRecommended for use with a pre-set save file")]
        Gold9a,
        [Description("Colo 3 Wave 9b"), ToolTip("Splits upon killing the Soul Warrior in wave 9\nRecommended for use with a pre-set save file")]
        Gold9b,
        [Description("Colo 3 Wave 10"), ToolTip("Splits upon completing wave 10\nRecommended for use with a pre-set save file")]
        Gold10,
        [Description("Colo 3 Wave 11"), ToolTip("Splits upon completing wave 11\nRecommended for use with a pre-set save file")]
        Gold11,
        [Description("Colo 3 Wave 12a"), ToolTip("Splits upon killing second set of 2 Lesser Mawleks and Winged Fool\nRecommended for use with a pre-set save file")]
        Gold12a,
        [Description("Colo 3 Wave 12b"), ToolTip("Splits upon killing the Brooding Mawlek\nRecommended for use with a pre-set save file")]
        Gold12b,
        [Description("Colo 3 Wave 14a"), ToolTip("Splits upon killing the Squits, Petras and Primal Aspids in wave 14\nRecommended for use with a pre-set save file")]
        Gold14a,
        [Description("Colo 3 Wave 14b"), ToolTip("Splits upon killing the Winged Fools and Battle Obbles in wave 14\nRecommended for use with a pre-set save file")]
        Gold14b,
        [Description("Colo 3 Wave 15"), ToolTip("Splits upon killing both Squits in wave 15\nRecommended for use with a pre-set save file")]
        Gold15,
        [Description("Colo 3 Wave 16"), ToolTip("Splits upon the death of all 14 Death Loodles in wave 16\nRecommended for use with a pre-set save file")]
        Gold16,
        [Description("Colo 3 Wave 17a"), ToolTip("Splits upon killing the first two phases of fools and mantises in wave 17\nRecommended for use with a pre-set save file")]
        Gold17a,
        [Description("Colo 3 Wave 17b"), ToolTip("Splits upon killing the fools, Volt Twister and Soul Twister in wave 17\nRecommended for use with a pre-set save file")]
        Gold17b,
        [Description("Colo 3 Wave 17c"), ToolTip("Splits upon killing all the regular enemies in wave 17\nRecommended for use with a pre-set save file")]
        Gold17c,
        [Description("Colo 3 End"), ToolTip("Splits upon killing God Tamer\nRecommended for use with a pre-set save file")]
        GoldEnd,

        [Description("Any Transition (Transition)"), ToolTip("Splits when the knight enters a transition (only one will split per transition)")]
        AnyTransition,
        [Description("Transition excluding Save State (Transition)"), ToolTip("Splits when the knight enters a transition (excludes save states and Sly's basement)")]
        TransitionAfterSaveState,
        [Description("Transition excluding discontinuities (Transition)"), ToolTip("Splits when the knight enters a transition (excludes discontinuities including save states, deaths, and dreamgates)")]
        TransitionExcludingDiscontinuities,
        [Description("Manual Split (Misc)"), ToolTip("Never splits. Use this when you need to manually split while using ordered splits")]
        ManualSplit,
        [Description("Ghost Coins Incremented (Event)"), ToolTip("Splits when the ghostCoins PlayerData is updated. Unused by unmodded game, intended for use with mods.")]
        OnGhostCoinsIncremented,

    }
    public class ToolTipAttribute : Attribute {
        public string ToolTip { get; set; }
        public ToolTipAttribute(string text) {
            ToolTip = text;
        }
    }
}
